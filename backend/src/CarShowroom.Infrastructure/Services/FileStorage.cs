using CarShowroom.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace CarShowroom.Infrastructure.Services;

public class FileStorage(IWebHostEnvironment environment, ILogger<FileStorage> logger) : IFileStorage
{
    private readonly string _webRoot = environment.WebRootPath
        ?? Path.Combine(environment.ContentRootPath, "wwwroot");

    public async Task<string> SaveAsync(Stream content, string extension, string folder, CancellationToken cancellationToken = default)
    {
        var cleanExt = extension.TrimStart('.').ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}.{cleanExt}";
        var relativeDir = Path.Combine("uploads", folder);
        var targetDir = Path.Combine(_webRoot, relativeDir);

        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        var fullPath = Path.Combine(targetDir, fileName);
        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream, cancellationToken);

        var webPath = $"/uploads/{folder.Replace('\\', '/')}/{fileName}";
        logger.LogInformation("File saved to disk at {FullPath}, web path: {WebPath}", fullPath, webPath);
        return webPath;
    }

    public void Delete(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return;

        try
        {
            var trimmed = relativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_webRoot, trimmed);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                logger.LogInformation("Deleted file from disk: {FullPath}", fullPath);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete file {RelativePath}", relativePath);
        }
    }
}
