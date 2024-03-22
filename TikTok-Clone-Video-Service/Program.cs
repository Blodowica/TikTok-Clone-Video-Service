
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;
using TikTok_Clone_User_Service.Services;
using TikTok_Clone_Video_Service.Controllers;
using TikTok_Clone_Video_Service.DatabaseContext;
using TikTok_Clone_Video_Service.Services;

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
            builder.Services.AddScoped<IRabbitMQConsumerService, RabbitMQConsumerService>();
            builder.Services.AddScoped<IRabbitMQPublisherService, RabbitMQPublisherService>();
            builder.Services.AddDbContext<VideoDatabaseContext>();

            

            //Db context

            using (var client = new VideoDatabaseContext())
            {
                client.Database.EnsureCreated();
            }


            //Cloudindary
            var account =( new Account(
              builder.Configuration["CloudinarySettings:CloudName"],
              builder.Configuration["CloudinarySettings:ApiKey"],
              builder.Configuration["CloudinarySettings:ApiSecret"]));

            var cloudinary = new Cloudinary(account);


            builder.Services.AddSingleton(cloudinary);




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
