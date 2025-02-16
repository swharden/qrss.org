﻿using Qrss.Core.Domain;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Qrss.Core.GrabImageManagers;

/// <summary>
/// This database of grabber images is maintained as a collection of image files in a folder.
/// The image filename contains the grabber ID, its own hash, and the date it was acquired.
/// </summary>
public class FlatFolder : IGrabImageManager
{
    // the filename stores the dates of the last N unique images for each grabber ID
    private readonly List<string> Filenames = [];
    public int ImageCount => Filenames.Count;

    private const string FILE_PREFIX = "GRAB";

    private readonly string FolderPath;

    public FlatFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        FolderPath = Path.GetFullPath(folderPath);
        Filenames.AddRange(GetImageFilenames());
    }

    private string[] GetImageFilenames()
    {
        List<string> filenames = [];
        foreach (string fullPath in Directory.GetFiles(FolderPath))
        {
            string filename = Path.GetFileName(fullPath);
            if (filename.StartsWith(FILE_PREFIX))
            {
                filenames.Add(filename);
            }
        }
        return [.. filenames];
    }

    private Dictionary<string, List<string>> GetFilenamesByGrabber()
    {
        Dictionary<string, List<string>> filenamesByGrabber = [];

        foreach (string filename in GetImageFilenames())
        {
            string grabberID = GetGrabberIdFromFilename(filename);
            if (filenamesByGrabber.TryGetValue(grabberID, out List<string>? value))
            {
                value.Add(filename);
            }
            else
            {
                filenamesByGrabber[grabberID] = [filename];
            }
        }

        return filenamesByGrabber;
    }

    private bool IsHashInDatabase(string id, string hash)
    {
        foreach (string filename in Filenames.Where(x => x.Contains($"-{id}-")))
        {
            if (hash == GetHashFromFilename(filename))
                return true;
        }
        return false;
    }

    private static string GetHashFromFilename(string filename)
    {
        return filename.Split("-")[4].Split(".")[0];
    }

    private static string GetGrabberIdFromFilename(string filename)
    {
        return filename.Split("-")[1];
    }

    private static DateTime GetDateTimeFromFilename(string filename)
    {
        int year = int.Parse(filename.Split("-")[2][0..4]);
        int month = int.Parse(filename.Split("-")[2][4..6]);
        int day = int.Parse(filename.Split("-")[2][6..8]);
        int hour = int.Parse(filename.Split("-")[3][0..2]);
        int minute = int.Parse(filename.Split("-")[3][2..4]);
        int second = int.Parse(filename.Split("-")[3][4..6]);
        return new DateTime(year, month, day, hour, minute, second);
    }

    private static TimeSpan GetAgeFromFilename(string filename)
    {
        return DateTime.UtcNow - GetDateTimeFromFilename(filename);
    }

    private static string GetFilename(string id, string hash, string originalFilename, DateTime dt)
    {
        string dateCode = $"{dt.Year:0000}{dt.Month:00}{dt.Day:00}";
        string timeCode = $"{dt.Hour:00}{dt.Minute:00}{dt.Second:00}";
        string[] parts = [FILE_PREFIX, id, dateCode, timeCode, hash];
        return string.Join("-", parts) + Path.GetExtension(originalFilename);
    }

    private static string GetHashForFilename(byte[] bytes, int length = 8)
    {
        string[] parts = System.Security.Cryptography.MD5.HashData(bytes)
            .Select(x => x.ToString("x2"))
            .ToArray();

        string fullHash = string.Join("", parts);

        return fullHash[..length];
    }

    public async Task DownloadImagesAsync(IGrabberInfoDB grabberDB, int maxCount = 10, int threads = 10)
    {
        var grabbers = grabberDB.ReadAll().Take(maxCount);

        ParallelOptions options = new() { MaxDegreeOfParallelism = threads };

        int bytesDownloaded = 0;
        int imageCount = 0;

        Stopwatch sw = Stopwatch.StartNew();

        await Parallel.ForEachAsync(grabbers, async (grabber, token) =>
        {
            try
            {
                byte[] bytes = await DownloadAsync(grabber, token);
                bytesDownloaded += bytes.Length;
                imageCount += 1;
                //Console.WriteLine($"[{grabber.ID}] downloaded {bytes.Length / 1e6:N2} MB");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{grabber.ID}] download error: {ex.Message}");
            }
        });

        Console.WriteLine($"Downloaded: {imageCount} images ({bytesDownloaded / 1e6:N2} MB) in {sw.Elapsed.TotalSeconds:N2} sec");
    }

    private async Task<byte[]> DownloadAsync(GrabberInfo grabber, CancellationToken token)
    {
        HttpClientHandler handler = new()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        };

        using HttpClient client = new(handler);
        using HttpResponseMessage response = await client.GetAsync(grabber.ImageUrl, token);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"request failed: ({(int)response.StatusCode}) {response.ReasonPhrase}");

        using HttpContent content = response.Content;
        byte[] bytes = await content.ReadAsByteArrayAsync(token);

        string hash = GetHashForFilename(bytes);

        if (IsHashInDatabase(grabber.ID, hash))
            return bytes;

        string originalFilename = Path.GetFileName(grabber.ImageUrl);
        string newFilename = GetFilename(grabber.ID, hash, originalFilename, DateTime.UtcNow);
        string saveAs = Path.Combine(FolderPath, newFilename);
        await File.WriteAllBytesAsync(saveAs, bytes, token);
        Filenames.Add(newFilename);

        return bytes;
    }

    private async Task DeleteImageAsync(string filename)
    {
        string path = Path.Combine(FolderPath, filename);
        File.Delete(path);
        await Task.CompletedTask;
    }

    public async Task DeleteOldImagesAsync(TimeSpan maxAge)
    {
        foreach ((_, List<string> filenames) in GetFilenamesByGrabber())
        {
            filenames.Sort();
            string newestFile = filenames.Last();

            foreach (string filename in filenames)
            {
                TimeSpan age = GetAgeFromFilename(newestFile);

                if (filename == newestFile)
                    continue;

                TimeSpan imageAge = GetAgeFromFilename(filename);
                if (imageAge <= maxAge)
                    continue;

                await DeleteImageAsync(filename);
            }
        }
    }
}
