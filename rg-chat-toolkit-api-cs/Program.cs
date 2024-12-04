
using Microsoft.Extensions.DependencyInjection;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_integration_abstractions.Embedding;

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
