using Infrastructure.Storage.Azure;
using Infrastructure.Storage.Fake;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Storage;

public static class StorageCollectionExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services, StorageOptions options)
    {
        if (options.UseAzure())
        {
            services.AddAzureBlobStorage(options.Azure);
        }

        else
        {
            services.AddFakeStorageManager();
        }

        return services;
    }
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, AzureBlobOption options)
    {
        services.AddSingleton<IFileStorageManager>(new AzureBlobStorageManager(options));

        return services;
    }

     public static IServiceCollection AddFakeStorageManager(this IServiceCollection services)
    {
        services.AddSingleton<IFileStorageManager>(new FakeStorageManager());

        return services;
    }

}
