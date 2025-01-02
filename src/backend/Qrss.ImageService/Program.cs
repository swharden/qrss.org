using Qrss.Core.Domain;

Console.WriteLine($"Downloading CSV file...");
const string GrabbersCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/refs/heads/master/grabbers.csv";
IGrabberInfoDB grabberDatabase = await Qrss.Core.GrabberInfoDatabases.CsvDB.FromCsvUrl(GrabbersCsvUrl);
Console.WriteLine($"Identified {grabberDatabase.ReadAll().Count()} grabbers");

Console.WriteLine($"Connecting to image database...");
IGrabImageManager imageManager = new Qrss.Core.GrabImageManagers.FlatFolder("./image-db");
Console.WriteLine($"Located {imageManager.ImageCount} existing images");

Console.WriteLine($"Downloading new images...");
await imageManager.DownloadImagesAsync(grabberDatabase);

Console.WriteLine($"Deleting old images...");
await imageManager.DeleteOldImagesAsync(TimeSpan.FromHours(1));

Console.WriteLine("DONE!");
Console.ReadLine();