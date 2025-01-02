using Qrss.Core.Domain;

namespace Qrss.Core.GrabberInfoDatabases;

public class MemoryDB : IGrabberInfoDB
{
    readonly List<GrabberInfo> Grabbers = [];

    public void Create(GrabberInfo info)
    {
        // add a number to the end of the title to make it unique
        string originalTitle = info.Title;
        int duplicateCounter = 2;
        while (Read(info.ID) is not null)
        {
            string newTitle = $"{originalTitle} ({duplicateCounter++})";
            info = new(info.Callsign, newTitle, info.Name, info.Location, info.ImageUrl, info.WebsiteUrl);
        }
        Grabbers.Add(info);
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public GrabberInfo? Read(string id)
    {
        return Grabbers.Where(x => x?.ID == id).FirstOrDefault();
    }

    public IEnumerable<GrabberInfo> ReadAll()
    {
        return Grabbers;
    }

    public void Update(GrabberInfo info)
    {
        GrabberInfo oldInfo = Read(info.ID) ?? throw new ArgumentException($"Grabber with ID '{info.ID}' does not exist"); ;
        Grabbers.Remove(oldInfo);
        Grabbers.Add(info);
    }
}
