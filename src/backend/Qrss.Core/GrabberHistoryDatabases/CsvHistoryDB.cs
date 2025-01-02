using Qrss.Core.Domain;
using System.Text;

namespace Qrss.Core.GrabberHistoryDatabases;

public class CsvHistoryDB : IGrabberHistoryDB
{
    private record UniqueImage(string GrabberID, string Hash, DateTime DateTime)
    {
        public string IdWithHash => $"{GrabberID}-{Hash}";
        public string ToCsvLine()
        {
            return $"{GrabberID},{Hash},{DateTime:o}";
        }

        public static UniqueImage? FromCsvLine(string line)
        {
            if (line.StartsWith("#"))
                return null;

            string[] parts = line.Split(',');
            if (parts.Length != 3)
                return null;

            try
            {
                DateTime dateTime = DateTime.Parse(parts[2]).ToUniversalTime();
                return new UniqueImage(parts[0], parts[1], dateTime);
            }
            catch
            {
                return null;
            }
        }
    }

    public int ImageCount => Images.Count;
    private readonly List<UniqueImage> Images = [];
    private readonly HashSet<string> SeenHashes = [];
    private readonly Dictionary<string, int> HashCount = [];
    private readonly int MaxGrabHistory;

    public CsvHistoryDB(string csvText, int maxGrabHistory = 5)
    {
        MaxGrabHistory = maxGrabHistory;
        csvText.Split("\n")
            .Select(UniqueImage.FromCsvLine)
            .Where(x => x is not null)
            .Cast<UniqueImage>()
            .ToList()
            .ForEach(AddGrab);
    }

    public void PurgeOldestRecords()
    {

    }

    public string GetCsvText()
    {
        StringBuilder sb = new();
        sb.AppendLine($"# QRSS Grab History Database - Version {Domain.Version.ShortString}");
        sb.AppendLine($"# Grabber ID, hash, UTC timestamp");
        Images.ForEach(x => sb.AppendLine(x.ToCsvLine()));
        return sb.ToString();
    }

    private void AddGrab(UniqueImage image)
    {
        if (SeenHashes.Contains(image.IdWithHash))
            return;

        Images.Add(image);

        if (!HashCount.ContainsKey(image.GrabberID))
            HashCount[image.GrabberID] = 0;
        HashCount[image.GrabberID]++;

        SeenHashes.Add(image.IdWithHash);

        while (HashCount[image.GrabberID] > MaxGrabHistory)
        {
            DeleteOldestGrab(image.GrabberID);
        }
    }

    private void DeleteOldestGrab(string grabberID)
    {
        UniqueImage? oldestImage = null;
        foreach (UniqueImage image in Images.Where(x => x.GrabberID == grabberID))
        {
            if (oldestImage is null || image.DateTime < oldestImage.DateTime)
            {
                oldestImage = image;
            }
        }

        if (oldestImage is not null)
        {
            Images.Remove(oldestImage);
            SeenHashes.Remove(oldestImage.Hash);
            HashCount[oldestImage.GrabberID]--;
        }
    }

    public void AddGrab(string grabberID, byte[] bytes)
    {
        string[] hashParts = System.Security.Cryptography.MD5.HashData(bytes)
            .Select(x => x.ToString("x2"))
            .ToArray();
        string hash = string.Join("", hashParts);

        UniqueImage image = new(grabberID, hash, DateTime.UtcNow);
        AddGrab(image);
    }

    public string[] GetGrabberIDs()
    {
        return Images.Select(x => x.GrabberID).Distinct().ToArray();
    }

    public string[] GetHashes(string grabberID)
    {
        List<string> hashes = [];
        foreach (var image in Images)
        {
            if (image.GrabberID == grabberID)
                hashes.Add(image.Hash);
        }
        return hashes.ToArray();
    }

    public DateTime[] GetDates(string grabberID)
    {
        List<DateTime> dates = [];
        foreach (var image in Images)
        {
            if (image.GrabberID == grabberID)
                dates.Add(image.DateTime);
        }
        return dates.ToArray();
    }
}
