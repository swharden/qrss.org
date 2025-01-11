﻿using Qrss.Core.Domain;
using System.Diagnostics;
using System.Text;

string appFolder = args.FirstOrDefault() ?? Path.Combine(AppContext.BaseDirectory, "app");
if (Directory.Exists(appFolder))
{
    Directory.CreateDirectory(appFolder);
}
Console.WriteLine($"APP FOLDER: {appFolder}");

Logger logger = new();

logger.Log($"Downloading CSV file...");
const string GrabbersCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/refs/heads/master/grabbers.csv";
IGrabberInfoDB grabberDatabase = await Qrss.Core.GrabberInfoDatabases.CsvDB.FromCsvUrl(GrabbersCsvUrl);
logger.Log($"Identified {grabberDatabase.ReadAll().Count()} grabbers");

logger.Log($"Connecting to image database...");
string imageFolder = Path.Combine(appFolder, "./image-db");
IGrabImageManager imageManager = new Qrss.Core.GrabImageManagers.FlatFolder(imageFolder);
logger.Log($"Located {imageManager.ImageCount} existing images");

logger.Log($"Downloading new images...");
await imageManager.DownloadImagesAsync(grabberDatabase);

logger.Log($"Deleting old images...");
await imageManager.DeleteOldImagesAsync(TimeSpan.FromHours(1));

logger.Log("DONE!");

string logFolder = Path.Combine(appFolder, "./logs");
logger.SaveAs(logFolder);

class Logger()
{
    StringBuilder SB = new();
    Stopwatch SW = Stopwatch.StartNew();

    public void Log(string msg)
    {
        string timestamp = DateTime.Now.ToString();
        string line = $"[{SW.Elapsed.TotalSeconds:N2}] {msg}";
        Console.WriteLine(line);
        SB.AppendLine(line);
    }

    public void SaveAs(string folder)
    {
        folder = Path.GetFullPath(folder);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string filename = $"log-{DateTime.UtcNow.Ticks}.txt";
        string filePath = Path.Combine(folder, filename);
        File.WriteAllText(filePath, SB.ToString());
        Console.WriteLine($"Wrote: {filePath}");
    }
}