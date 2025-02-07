
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Embedding;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace rg_chat_toolkit_api_cs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                    });
            });

            // RG
            builder.Services.AddSingleton<IRGEmbeddingCache, RGCache>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Headers.ContainsKey("TenantID"))
                            {
                                context.HttpContext.Items["TenantID"] = context.Request.Headers["TenantID"].ToString();
                            }
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            bool isTenantIDSpecified = context.HttpContext.Items.ContainsKey("TenantID");
                            bool isTenantIDValid = false;
                            if (isTenantIDSpecified)
                            {
                                var tenantId = context.HttpContext.Items["TenantID"]?.ToString();
                                Guid tenantIdGuid;
                                if (Guid.TryParse(tenantId, out tenantIdGuid))
                                {
                                    try
                                    {
                                        LookupValidIssuer(tenantIdGuid);
                                        isTenantIDValid = true;
                                    }
                                    catch (SecurityTokenInvalidIssuerException)
                                    {
                                        isTenantIDValid = false;
                                    }
                                }
                            }

                            const string MESSAGE_TENANT_ID_MISSING = "Authentication: Missing TenantID.";
                            const string MESSAGE_TENANT_ID_INVALID = "Authentication: Invalid TenantID.";

                            var message = isTenantIDSpecified ? (isTenantIDValid ? MESSAGE_TENANT_ID_INVALID : MESSAGE_TENANT_ID_MISSING) : MESSAGE_TENANT_ID_MISSING;

                            // Skip the default logic.
                            context.HandleResponse();

                            // Return a custom response.
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            //context.Response.ContentType = "application/json";
                            //var result = JsonConvert.SerializeObject(new
                            //{
                            //    StatusCode = context.Response.StatusCode,
                            //    Message = message
                            //});
                            return context.Response.WriteAsync(message);
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                        {
                            var httpContext = new HttpContextAccessor().HttpContext;
                            if (httpContext != null && httpContext.Request.Headers.ContainsKey("TenantID"))
                            {
                                var tenantId = httpContext.Request.Headers["TenantID"].ToString();
                                Guid tenantIdGuid;
                                if (Guid.TryParse(tenantId, out tenantIdGuid))
                                {
                                    parameters.ValidIssuer = LookupValidIssuer(tenantIdGuid);
                                }
                            }

                            var json = new WebClient().DownloadString(parameters.ValidIssuer + "/.well-known/jwks.json");
                            var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                            return (IEnumerable<SecurityKey>)keys;
                        },

                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,

                        // Dynamically validate issuer using TenantID
                        IssuerValidator = (issuer, securityToken, parameters) =>
                        {
                            var httpContext = new HttpContextAccessor().HttpContext;
                            if (httpContext != null && httpContext.Items.ContainsKey("TenantID"))
                            {
                                var tenantId = httpContext.Items["TenantID"]?.ToString();
                                Guid tenantIdGuid;
                                if (!Guid.TryParse(tenantId, out tenantIdGuid))
                                {
                                    throw new SecurityTokenInvalidIssuerException("Authentication: Malformed TenantID");
                                }

                                var expectedIssuer = LookupValidIssuer(tenantIdGuid); // Dynamically get issuer
                                if (issuer != expectedIssuer)
                                {
                                    throw new SecurityTokenInvalidIssuerException($"Invalid Issuer for Tenant {tenantId}");
                                }
                            }
                            else
                            {
                                throw new SecurityTokenInvalidIssuerException("Authentication: Missing TenantID");
                            }
                            return issuer; // Return the validated issuer
                        }
                    };
                });
            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = Text.Plain;
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    // Check for ApplicationException
                    if (exceptionHandlerPathFeature?.Error is ApplicationException)
                    {
                        await context.Response.WriteAsync("Error: ");
                        await context.Response.WriteAsync(exceptionHandlerPathFeature?.Error?.Message ?? "(Unknown error)");
                    }
                    else
                    {
                        await context.Response.WriteAsync("An error has occurred.");
#if DEBUG
                        await context.Response.WriteAsync(exceptionHandlerPathFeature?.Error?.Message ?? "(Unknown error)");
#endif
                    }
                });
            });

            // CORS allow all origins and methods and headers
            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static string LookupValidIssuer(Guid? tenantId)
        {
            // 902544DA-67E6-4FA8-A346-D1FAA8B27A08
            // -> https://cognito-idp.us-east-2.amazonaws.com/us-east-2_CrGQj2ghJ/.well-known/jwks.json

            Guid TENANTID_TILLEY = new Guid("902544DA-67E6-4FA8-A346-D1FAA8B27A08");
            if (tenantId == TENANTID_TILLEY)
            {
                return "https://cognito-idp.us-east-2.amazonaws.com/us-east-2_CrGQj2ghJ";
            }
            else
            {
                throw new SecurityTokenInvalidIssuerException("Unknown TenantID");
            }
        }
    }
}
