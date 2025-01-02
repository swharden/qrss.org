namespace Qrss.Core.Domain;
public class GrabberInfo : IEquatable<GrabberInfo>
{
    public string ID { get; }
    public string Callsign { get; }
    public string Title { get; }
    public string Name { get; }
    public string Location { get; }
    public string ImageUrl { get; }
    public string WebsiteUrl { get; }

    public GrabberInfo(string callsign, string title, string name, string location, string imageUrl, string websiteUrl)
    {
        AssertValidImageURL(imageUrl);
        AssertValidPageURL(websiteUrl);
        ID = GetID(callsign, title);
        Callsign = callsign;
        Title = title;
        Name = name;
        Location = location;
        ImageUrl = imageUrl;
        WebsiteUrl = websiteUrl;
    }

    public static void AssertValidImageURL(string url)
    {
        // TODO: validation logic
    }

    public static void AssertValidPageURL(string url)
    {
        // TODO: validation logic
    }

    public static string GetID(string callsign, string title)
    {
        char[] cleanChars = $"{callsign}-{title}"
            .ToLowerInvariant()
            .ToCharArray()
            .Where(c => char.IsAsciiLetterLower(c) || char.IsDigit(c))
            .ToArray();

        return new string(cleanChars);
    }

    public bool Equals(GrabberInfo? other)
    {
        return other is not null &&
            ID == other.ID &&
            Callsign == other.Callsign &&
            Title == other.Title &&
            Name == other.Name &&
            Location == other.Location &&
            ImageUrl == other.ImageUrl &&
            WebsiteUrl == other.WebsiteUrl;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as GrabberInfo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Callsign, Title, Name, Location, ImageUrl, WebsiteUrl);
    }
}
