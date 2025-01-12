namespace Qrss.API.V1;

/// <summary>
/// Details of a single grab image obtained exclusively from its filename
/// </summary>
public class GrabInfo()
{
    public string GrabberID { get; set; } = "undefined";
    public DateTime DateTime { get; set; }
    public int AgeSeconds { get; set; }
    public string Hash { get; set; } = "undefined";
    public string Filename { get; set; } = "undefined";

    public static GrabInfo FromPath(string path)
    {
        string[] parts = Path.GetFileNameWithoutExtension(path).Split("-");

        GrabInfo info = new()
        {
            Filename = Path.GetFileName(path),
            GrabberID = parts[1],
            Hash = parts[4],
        };

        string dayCode = parts[2];
        int day = int.Parse(dayCode[6..8]);
        int month = int.Parse(dayCode[4..6]);
        int year = int.Parse(dayCode[0..4]);
        DateOnly date = new(year, month, day);

        string timeCode = parts[3];
        int hour = int.Parse(timeCode[0..2]);
        int minute = int.Parse(timeCode[2..4]);
        int second = int.Parse(timeCode[4..6]);
        TimeOnly time = new(hour, minute, second);

        info.DateTime = new(date, time);
        info.AgeSeconds = (int)(DateTime.UtcNow - info.DateTime).TotalSeconds;

        return info;
    }

    public static Dictionary<string, List<GrabInfo>> FromPaths(string[] paths)
    {
        Dictionary<string, List<GrabInfo>> byGrabber = [];

        foreach (string path in paths)
        {
            GrabInfo info = GrabInfo.FromPath(path);
            if (byGrabber.TryGetValue(info.GrabberID, out List<GrabInfo>? value))
            {
                value.Add(info);
            }
            else
            {
                byGrabber[info.GrabberID] = [info];
            }
        }

        return byGrabber;
    }
}
