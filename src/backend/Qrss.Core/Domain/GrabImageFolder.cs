namespace Qrss.Core.Domain;

// TODO: use interface to allow this to be a S3 bucket or something

/// <summary>
/// This database of grabber images is maintained as a collection of image files in a folder.
/// The image filename contains the grabber ID, its own hash, and the date it was acquired.
/// </summary>
public class GrabImageFolder
{
    // the filename stores the dates of the last N unique images for each grabber ID
    private readonly List<string> Filenames = [];
    public int Count => Filenames.Count;

    private const string FILE_PREFIX = "GRAB";

    public GrabImageFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException(folderPath);

        foreach (string filename in Directory.GetFiles(folderPath))
        {
            if (filename.StartsWith(FILE_PREFIX))
            {
                Filenames.Add(filename);
            }
        }
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
        return Path.GetFileNameWithoutExtension(filename).Split("-")[4];
    }

    private static string GetFilename(string id, string hash, string originalFilename, DateTime dt)
    {
        string dateCode = $"{dt.Year:0000}{dt.Month:00}{dt.Day:00}";
        string timeCode = $"{dt.Hour:00}{dt.Minute:00}{dt.Second:00}";
        string[] parts = [FILE_PREFIX, id, dateCode, timeCode, hash];
        return string.Join("-", parts) + Path.GetExtension(originalFilename);
    }

    public string? SaveIfUnique(string id, byte[] bytes, string originalFilename)
    {
        string hash = GetHash(bytes);

        if (IsHashInDatabase(id, hash))
            return null;

        string filename = GetFilename(id, hash, originalFilename, DateTime.UtcNow);
        Filenames.Add(filename);

        // TODO: save file to disk

        // TODO: identify old files, delete them, and remove them from the DB

        return filename;
    }

    public static string GetHash(byte[] bytes)
    {
        return string.Join("", System.Security.Cryptography.MD5.HashData(bytes).Select(x => x.ToString("x2")).ToArray());
    }
}
