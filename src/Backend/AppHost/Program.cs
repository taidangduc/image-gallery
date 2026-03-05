//ref: https://github.com/dotnet/aspire/tree/main/src/Aspire.Hosting.Azure.Functions

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebAPI>("webapi");

builder.AddAzureFunctionsProject<Projects.AzureFunctions>("azurefunctions");

builder.Build().Run();