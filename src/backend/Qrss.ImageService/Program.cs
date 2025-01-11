using Qrss.Core.Domain;
using System.Diagnostics;
using System.Text;

Logger logger = new();

logger.Log($"Downloading CSV file...");
const string GrabbersCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/refs/heads/master/grabbers.csv";
IGrabberInfoDB grabberDatabase = await Qrss.Core.GrabberInfoDatabases.CsvDB.FromCsvUrl(GrabbersCsvUrl);
logger.Log($"Identified {grabberDatabase.ReadAll().Count()} grabbers");

logger.Log($"Connecting to image database...");
IGrabImageManager imageManager = new Qrss.Core.GrabImageManagers.FlatFolder("./image-db");
logger.Log($"Located {imageManager.ImageCount} existing images");

logger.Log($"Downloading new images...");
await imageManager.DownloadImagesAsync(grabberDatabase);

logger.Log($"Deleting old images...");
await imageManager.DeleteOldImagesAsync(TimeSpan.FromHours(1));

logger.Log("DONE!");
logger.SaveAs($"log-{DateTime.UtcNow.Ticks}.txt");

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

    public void SaveAs(string filename)
    {
        filename = Path.GetFullPath(filename);
        File.WriteAllText(filename, SB.ToString());
        Console.WriteLine($"Wrote: {filename}");
    }
}