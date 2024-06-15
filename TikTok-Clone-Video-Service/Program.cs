
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Security.Principal;
using TikTok_Clone_User_Service.Services;
using TikTok_Clone_Video_Service.Controllers;
using TikTok_Clone_Video_Service.DatabaseContext;
using TikTok_Clone_Video_Service.Interfaces;
using TikTok_Clone_Video_Service.SignalR;

namespace TikTok_Clone_Video_Service
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
            builder.Services.AddLogging();
            builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();
            builder.Services.AddScoped<IUploadVideoService, UploadVideoService>();



            builder.Services.AddDbContext<VideoDatabaseContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("videoDatabase")));

            builder.Services.AddCors();
            builder.Services.AddSignalR();

            //Cloudindary
            var account =( new Account(
              builder.Configuration["CloudinarySettings:CloudName"],
              builder.Configuration["CloudinarySettings:ApiKey"],
              builder.Configuration["CloudinarySettings:ApiSecret"]));

            var cloudinary = new Cloudinary(account);


            builder.Services.AddSingleton(cloudinary);

            //configure and start rabbitmq service
            ConfigureRabbitMQ(builder);

                var app = builder.Build();

            var corsURL = builder.Configuration["AllowedCors"];           
            app.UseCors(builder =>
            {
                builder.WithOrigins(corsURL)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
                app.UseSwagger();
                app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            
            app.MapHub<CommentHub>("/commentHub");

            app.Run();
        }
        private static void ConfigureRabbitMQ(WebApplicationBuilder builder)
        {
            try
            {
                var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQConfiguration");
                var connectionFactory = new ConnectionFactory
                {
                    HostName = rabbitMQConfig["Hostname"],
                    Port = Convert.ToInt32(rabbitMQConfig["Port"]),
                    UserName = rabbitMQConfig["Username"],
                    Password = rabbitMQConfig["Password"],
                    VirtualHost = rabbitMQConfig["Virtualhost"]
                };

                // Register RabbitMQ services

                builder.Services.AddSingleton(connectionFactory);
                builder.Services.AddHostedService(provider =>
                    new RabbitMQConsumer(connectionFactory, provider.GetRequiredService<IServiceProvider>(), "video_exchange", "upload_video_queue"));
            }
            catch (Exception ex)
            {
                // Handle RabbitMQ configuration exception
                Console.WriteLine($"Failed to configure RabbitMQ: {ex.Message}");
                throw;
            }
        }
    }

    
}
