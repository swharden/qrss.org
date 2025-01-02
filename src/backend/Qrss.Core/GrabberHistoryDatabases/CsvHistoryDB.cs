using Qrss.Core.Domain;

namespace Qrss.Core.GrabberHistoryDatabases;

public class CsvHistoryDB : IGrabberHistoryDB
{
    private record UniqueImage(string GrabberID, string Hash, DateTime DateTime)
    {
        public string IdWithHash => $"{GrabberID}-{Hash}";
        public string ToCsvLine()
        {
            return $"{GrabberID},{Hash},{DateTime.Ticks}";
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
                long ticks = long.Parse(parts[2]);
                DateTime dateTime = new(ticks);
                return new UniqueImage(parts[0], parts[1], dateTime);
            }
            catch
            {
                return null;
            }
        }
    }

    public int ImageCount => Images.Count;
    private readonly List<UniqueImage> Images;
    private readonly HashSet<string> SeenHashes = [];

    public CsvHistoryDB(string csvText)
    {
        Images = csvText.Split("\n")
            .Select(UniqueImage.FromCsvLine)
            .Where(x => x is not null)
            .Cast<UniqueImage>()
            .ToList();

        foreach (var image in Images)
        {
            SeenHashes.Add(image.IdWithHash);
        }
    }

    public string GetCsvText()
    {
        return string.Join("\n", Images.Select(x => x.ToCsvLine()));
    }

    public void AddGrab(string grabberID, byte[] bytes)
    {
        string[] hashParts = System.Security.Cryptography.MD5.HashData(bytes)
            .Select(x => x.ToString("x2"))
            .ToArray();
        string hash = string.Join("", hashParts);

        UniqueImage image = new(grabberID, hash, DateTime.UtcNow);

        if (SeenHashes.Contains(image.IdWithHash))
            return;

        Images.Add(image);
        SeenHashes.Add(image.IdWithHash);
    }
}
