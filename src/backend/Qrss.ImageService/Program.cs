using Qrss.Core.Domain;
using System.Diagnostics;
using System.Text;

const int RUN_MINUTE_ONES_DIGIT = 2;

int lastRunMinute = -1;
int checks = 0;
int runs = 0;

while (true)
{
    int thisMinute = DateTime.UtcNow.Minute;
    int minuteOnes = thisMinute % 10;

    // TODO: replace this with improved analytics
    Console.Write($"{DateTime.UtcNow} minute: {minuteOnes} checks:{checks++} runs:{runs}\r");
    Thread.Sleep(1000);

    bool isRunMinute = minuteOnes == RUN_MINUTE_ONES_DIGIT;
    bool runExecutedPreviously = thisMinute == lastRunMinute;
    bool timeForNextRun = isRunMinute && !runExecutedPreviously;
    bool isFirstRun = runs == 0;

    if (isFirstRun || timeForNextRun)
    {
        lastRunMinute = thisMinute;
        runs += 1;
        DownloadNewImages();
        Console.WriteLine();
    }
}

static void DownloadNewImages()
{
    DownloadNewImagesAsync().GetAwaiter().GetResult();
}

static async Task DownloadNewImagesAsync()
{
    string dataFolder = Path.Combine(AppContext.BaseDirectory, "data");
    if (Directory.Exists(dataFolder))
    {
        Directory.CreateDirectory(dataFolder);
    }
    Console.WriteLine($"APP FOLDER: {dataFolder}");

    Logger logger = new();

    logger.Log($"Downloading CSV file...");
    const string GrabbersCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/refs/heads/master/grabbers.csv";
    IGrabberInfoDB grabberDatabase = await Qrss.Core.GrabberInfoDatabases.CsvDB.FromCsvUrl(GrabbersCsvUrl);
    logger.Log($"Identified {grabberDatabase.ReadAll().Count()} grabbers");

    logger.Log($"Connecting to image database...");
    string imageFolder = Path.Combine(dataFolder, "./grabs");
    IGrabImageManager imageManager = new Qrss.Core.GrabImageManagers.FlatFolder(imageFolder);
    logger.Log($"Located {imageManager.ImageCount} existing images");

    logger.Log($"Downloading new images...");
    await imageManager.DownloadImagesAsync(grabberDatabase);

    logger.Log($"Deleting old images...");
    await imageManager.DeleteOldImagesAsync(TimeSpan.FromHours(1));

    logger.Log("DONE!");

    string logFolder = Path.Combine(dataFolder, "./logs");
    logger.SaveAs(logFolder);
}

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