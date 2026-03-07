using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Imaging;

// ref: https://docs.sixlabors.com/articles/imagesharp/resize.html
public class ImageProcessingService
{
    public async Task<Stream> ResizeAsync(byte[] inputBytes, int width = 300, int height = 300, CancellationToken cancellationToken = default)
    {
        using var image = Image.Load(inputBytes);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(width, height)
        }));

        var outputStream = new MemoryStream();

        await image.SaveAsJpegAsync(outputStream, new JpegEncoder
        {
            Quality = 90
        }, cancellationToken);

        outputStream.Position = 0;

        return outputStream;
    }
}
