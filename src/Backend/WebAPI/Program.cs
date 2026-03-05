using Application;
using Domain.Infrastructure.Messaging;
using Infrastructure.Storage;
using Persistence;
using System.Reflection;
using WebAPI.ConfigurationOptions;

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

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        await serviceProvider.MigrateAsync();
    }
}

app.MapControllers();

app.Run();