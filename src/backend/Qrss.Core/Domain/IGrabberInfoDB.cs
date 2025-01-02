namespace Qrss.Core.Domain;
public interface IGrabberInfoDB
{
    public void Create(GrabberInfo info);
    public GrabberInfo? Read(string id);
    public IEnumerable<GrabberInfo> ReadAll();
    public void Update(GrabberInfo info);
    public void Delete(string id);
}
