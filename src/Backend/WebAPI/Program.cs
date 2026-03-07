using Application;
using Domain.Infrastructure.Messaging;
using Infrastructure.Storage;
using Persistence;
using System.Reflection;
using WebAPI.ConfigurationOptions;
using Microsoft.OpenApi.Models;
using Infrastructure.Messaging;
using Application.FileEntries.MessageBusEvents;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

var appSettings = new AppSettings();
configuration.Bind(appSettings);

builder.AddServiceDefaults();

services.AddOpenApi();

services.AddControllers();

services.AddPersistence(appSettings.ConnectionStrings.DefaultConnection)
        .AddApplicationServices()
        .AddStorage(appSettings.Storage);

services.AddMessageBus(Assembly.GetExecutingAssembly());
services.AddMessageBusSender<FileCreatedEvent>(appSettings.Messaging);

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "ImageGallery",
        new OpenApiInfo
        {
            Title = "ImageGallery API",
            Version = "1",
            Description = "ImageGallery API specifications",
            Contact = new OpenApiContact
            {
                Name = "taidangduc",
                Url = new Uri("https://github.com/taidangduc")
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/license/mit/")
            }
        });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/ImageGallery/swagger.json", "ImageGallery API");
    options.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        await serviceProvider.MigrateAsync();
    }
}

app.MapControllers();

app.Run();