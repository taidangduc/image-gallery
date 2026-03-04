//ref: https://github.com/dotnet/aspire/tree/main/src/Aspire.Hosting.Azure.Functions

var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sqlserver");
var sqldb = sqlserver.AddDatabase("DefaultConnection");

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var blob = storage.AddBlobs("blob");
var queue = storage.AddQueues("queue");

builder.AddProject<Projects.WebAPI>("webapi")
    .WaitFor(sqldb)
    .WithReference(sqldb)
    .WithReference(blob)
    .WithReference(queue, "QueueTriggerConnection");

builder.AddAzureFunctionsProject<Projects.AzureFunctions>("azurefunctions")
    .WithReference(blob)
    .WithReference(queue, "QueueTriggerConnection");

builder.Build().Run();
