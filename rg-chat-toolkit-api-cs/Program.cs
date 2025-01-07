
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Embedding;
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
            builder.Services.AddSwaggerGen();

            // RG
            builder.Services.AddSingleton<IRGEmbeddingCache, RGCache>();
            builder.Services.AddSingleton<EmbeddingBase, RGEmbedding>();

            var app = builder.Build();

            var embeddingCache = app.Services.GetRequiredService<IRGEmbeddingCache>();
            var embeddingModel = app.Services.GetRequiredService<EmbeddingBase>();
            RG.Instance = new RG(embeddingCache, embeddingModel);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseAuthorization();

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

            app.MapControllers();

            app.Run();
        }
    }
}
