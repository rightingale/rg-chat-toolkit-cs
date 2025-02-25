
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_api_cs.Data.Models;
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
                            if (context.Request.Headers.ContainsKey(AuthenticationHelper.HEADER_KEY_TENANT_ID))
                            {
                                context.HttpContext.Items[AuthenticationHelper.HEADER_KEY_TENANT_ID] = context.Request.Headers[AuthenticationHelper.HEADER_KEY_TENANT_ID].ToString();
                            }
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            string? errorMessage = null;

                            bool isTenantIDSpecified = context.HttpContext.Items.ContainsKey(AuthenticationHelper.HEADER_KEY_TENANT_ID);
                            bool isTenantIDValid = false;
                            if (isTenantIDSpecified)
                            {
                                var tenantId = context.HttpContext.Items[AuthenticationHelper.HEADER_KEY_TENANT_ID]?.ToString();
                                Guid tenantIdGuid;
                                if (Guid.TryParse(tenantId, out tenantIdGuid))
                                {
                                    try
                                    {
                                        var auths = DataMethods.JwtAuthentication_Select(tenantIdGuid);

                                        if (auths != null && auths.Count > 0)
                                        {
                                            isTenantIDValid = true;
                                        }
                                        else
                                        {
                                            errorMessage = AuthenticationHelper.MESSAGE_TENANT_ID_UNKNOWN_HEADER;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errorMessage = AuthenticationHelper.MESSAGE_TENANT_ID_UNKNOWN_HEADER + " " + ex.Message;
                                    }
                                }
                                else
                                {
                                    errorMessage = AuthenticationHelper.MESSAGE_TENANT_ID_INVALID;
                                }
                            }
                            else
                            {
                                errorMessage = AuthenticationHelper.MESSAGE_TENANT_ID_MISSING;
                            }

                            // Skip the default logic.
                            context.HandleResponse();
                            // Return a custom response.
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                            if (!isTenantIDValid)
                            {
                                return context.Response.WriteAsync(errorMessage ?? "");
                            }
                            else
                            {
                                return context.Response.WriteAsync(AuthenticationHelper.MESSAGE_TOKEN_INVALID);
                            }
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                        {
                            var httpContext = new HttpContextAccessor().HttpContext;
                            if (httpContext != null && httpContext.Request.Headers.ContainsKey(AuthenticationHelper.HEADER_KEY_TENANT_ID))
                            {
                                var tenantId = httpContext.Request.Headers[AuthenticationHelper.HEADER_KEY_TENANT_ID].ToString();
                                Guid tenantIdGuid;
                                if (Guid.TryParse(tenantId, out tenantIdGuid))
                                {
                                    var auths = DataMethods.JwtAuthentication_Select(tenantIdGuid);
                                    if (auths == null || auths.Count == 0)
                                    {
                                        throw new ApplicationException(AuthenticationHelper.MESSAGE_TENANT_ID_UNKNOWN);
                                    }

                                    parameters.ValidIssuer = auths[0].ValidIssuer;

                                    var json = new WebClient().DownloadString(auths[0].JwksUri);
                                    var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                                    return (IEnumerable<SecurityKey>)keys;
                                }
                                else
                                {
                                    throw new ApplicationException(AuthenticationHelper.MESSAGE_TENANT_ID_INVALID_REQUEST);
                                }
                            }
                            else
                            {
                                throw new ApplicationException(AuthenticationHelper.MESSAGE_TENANT_ID_MISSING_REQUEST);
                            }
                        },

                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,

                        // Dynamically validate issuer using TenantID
                        IssuerValidator = (issuer, securityToken, parameters) =>
                        {
                            var httpContext = new HttpContextAccessor().HttpContext;
                            if (httpContext != null && httpContext.Items.ContainsKey(AuthenticationHelper.HEADER_KEY_TENANT_ID))
                            {
                                var tenantId = httpContext.Items[AuthenticationHelper.HEADER_KEY_TENANT_ID]?.ToString();
                                Guid tenantIdGuid;
                                if (!Guid.TryParse(tenantId, out tenantIdGuid))
                                {
                                    throw new SecurityTokenInvalidIssuerException(AuthenticationHelper.MESSAGE_TENANT_ID_INVALID);
                                }

                                var auths = DataMethods.JwtAuthentication_Select(tenantIdGuid);
                                if (auths == null || auths.Count == 0)
                                {
                                    throw new ApplicationException(AuthenticationHelper.MESSAGE_TENANT_ID_UNKNOWN);
                                }

                                var expectedIssuer = auths[0].ValidIssuer;
                                if (issuer != expectedIssuer)
                                {
                                    throw new SecurityTokenInvalidIssuerException($"Invalid Issuer for Tenant {tenantId}");
                                }
                            }
                            else
                            {
                                throw new SecurityTokenInvalidIssuerException("Authentication: Missing " + AuthenticationHelper.HEADER_KEY_TENANT_ID);
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
                        await context.Response.WriteAsync("An unknown error has occurred.");
#if DEBUG
                        await context.Response.WriteAsync(" " + exceptionHandlerPathFeature?.Error?.Message ?? "(Unknown error)");
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
    }
}
