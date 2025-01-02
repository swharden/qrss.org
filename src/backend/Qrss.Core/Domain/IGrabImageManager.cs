namespace Qrss.Core.Domain;

/// <summary>
/// Logic for fetching grabber images, storing new ones, and deleting old ones
/// </summary>
public interface IGrabImageManager
{
    int ImageCount { get; }
    Task DownloadImagesAsync(IGrabberInfoDB grabberDB, int maxCount = 10, int threads = 10);
    Task DeleteOldImagesAsync(TimeSpan maxAge);
}
