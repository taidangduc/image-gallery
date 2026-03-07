using Application.FileEntries.MessageBusEvents;
using Azure.Storage.Queues.Models;
using Domain.Entities;
using Domain.Infrastructure.Messaging;
using Domain.Repositories;
using Infrastructure.Imaging;
using Infrastructure.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AzureFunctions;

public class FunctionQueueTrigger
{
    private readonly ILogger<FunctionQueueTrigger> _logger;
    private readonly ImageProcessingService _imageProcessingService;
    private readonly IRepository<FileEntryImage, Guid> _fileEntryImageRepository;
    private readonly IFileStorageManager _fileManager;

    public FunctionQueueTrigger(
        ILogger<FunctionQueueTrigger> logger,
        ImageProcessingService imageProcessingService,
        IFileStorageManager fileManager,
        IRepository<FileEntryImage, Guid> fileEntryImageRepository)
    {
        _logger = logger;
        _imageProcessingService = imageProcessingService;
        _fileManager = fileManager;
        _fileEntryImageRepository = fileEntryImageRepository;
    }

    [Function(nameof(FunctionQueueTrigger))]
    public async Task Run([QueueTrigger("image-processing-queue", Connection = "AzureQueueConnectionString")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);

        var data = JsonSerializer.Deserialize<Message<FileCreatedEvent>>(message.MessageText);
        var fileEntry = data.Data.FileEntry;

        await ProcessMessageAsync(fileEntry);
    }

    public async Task ProcessMessageAsync(FileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        if (fileEntry == null || fileEntry == default)
        {
            return;
        }

        if (string.IsNullOrEmpty(fileEntry.FileLocation))
        {
            return;
        }

        if (fileEntry.Deleted)
        {
            return;
        }

        var fileExtension = Path.GetExtension(fileEntry.FileName).ToLowerInvariant();

        if (fileExtension == ".jpg" || fileExtension == ".png")
        {
            var fileEntryImage = _fileEntryImageRepository.GetQueryableSet().FirstOrDefault(x => x.FileEntryId == fileEntry.Id);

            if (fileEntryImage == null)
            {
                fileEntryImage = new FileEntryImage
                {
                    ImageLocation = $"thumbnails/{DateTime.Now:yyyy/MM/dd}/{fileEntry.Id}.{fileExtension}",
                    FileEntryId = fileEntry.Id,
                };

                try
                {
                    var bytes = await _fileManager.ReadAsync(fileEntry.ToModel());

                    string contentType = GetMediaType(fileExtension);

                    if (bytes == null)
                    {
                        _logger.LogError("Failed to read file for FileEntry ID: {fileEntryId}", fileEntry.Id);
                        return;
                    }

                    var fileStream = await _imageProcessingService.ResizeAsync(bytes);

                    var newFile = new FileEntryModel
                    {
                        Id = fileEntry.Id,
                        FileName = fileEntry.FileName,
                        FileLocation = fileEntryImage.ImageLocation
                    };

                    await _fileManager.CreateAsync(newFile, fileStream, contentType);

                    await _fileEntryImageRepository.AddAsync(fileEntryImage);
                    await _fileEntryImageRepository.UnitOfWork.SaveChangesAsync();

                    // delete azure queue message after processing
                    // Note: Azure Functions automatically deletes the message from the queue if the function executes successfully.
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing image for FileEntry ID: {fileEntryId}", fileEntry.Id);
                    throw;
                }
            }
        }
    }

    private static string GetMediaType(string fileExtension)
    {
        return fileExtension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream",
        };
    }
}

public class FileEntryModel : IFileEntry
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileLocation { get; set; }
}

public static class FileEntryExtensions
{
    public static FileEntryModel ToModel(this FileEntry fileEntry)
    {
        if (fileEntry == null)
        {
            return null;
        }

        return new FileEntryModel
        {
            Id = fileEntry.Id,
            FileName = fileEntry.FileName,
            FileLocation = fileEntry.FileLocation
        };
    }
}

