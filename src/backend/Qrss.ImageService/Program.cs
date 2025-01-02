Console.WriteLine($"Connecting to image database...");
Qrss.Core.ImageDatabases.ImageFolderDB imageDB = new("./image-db");
Console.WriteLine($"Located {imageDB.Count} known unique grabs");

Console.WriteLine($"Downloading CSV file...");
const string GrabbersCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/refs/heads/master/grabbers.csv";
Qrss.Core.GrabberInfoDatabases.CsvDB grabberDB = await Qrss.Core.GrabberInfoDatabases.CsvDB.FromCsvUrl(GrabbersCsvUrl);
Console.WriteLine($"Identified {grabberDB.ReadAll().Count()} grabbers");

Console.WriteLine($"Checking grabber URLs for new images...");
await imageDB.DownloadImagesAsync(grabberDB);

Console.WriteLine($"Deleting old images...");
imageDB.DeleteOldImages(TimeSpan.FromHours(1));

Console.WriteLine("DONE!");
Console.ReadLine();