using Qrss.Core.Domain;
using System.Text;

namespace Qrss.Core.GrabberInfoDatabases;
public class CsvGrabberInfoDB : IGrabberInfoDB
{
    private readonly MemoryDB MemoryDB = new();

    public CsvGrabberInfoDB(string csvFilePath)
    {
        string[] lines = File.ReadAllLines(csvFilePath);
        foreach (string line in lines)
        {
            if (line.StartsWith('#'))
                continue;

            var parts = ParseCsvLine(line);
            if (parts.Count != 7)
                continue;

            //string id = parts[0];
            string callsign = parts[1];
            string title = parts[2];
            string name = parts[3];
            string location = parts[4];
            string website = parts[5];
            string file = parts[6];
            GrabberInfo info = new(callsign, title, name, location, file, website);
            Create(info);
        };
    }

    // NOTE: validation is performed in the memory database
    public void Create(GrabberInfo info) => MemoryDB.Create(info);
    public void Delete(string id) => MemoryDB.Delete(id);
    public GrabberInfo? Read(string id) => MemoryDB.Read(id);
    public IEnumerable<GrabberInfo> ReadAll() => MemoryDB.ReadAll();
    public void Update(GrabberInfo info) => MemoryDB.Update(info);

    public string GetCsvText()
    {
        StringBuilder sb = new();
        foreach (var info in ReadAll())
            sb.AppendLine(GetCsvLine(info));
        return string.Join("\n", sb.ToString());
    }

    public void SaveAs(string csvFilePath) => File.WriteAllText(csvFilePath, GetCsvText());

    private static string GetCsvLine(GrabberInfo info)
    {
        static string EncloseInQuotesIfNeeded(string str) => str.Contains(',') ? $"\"{str}\"" : str; ;

        string id = info.ID;
        string callsign = info.Callsign;
        string title = EncloseInQuotesIfNeeded(info.Title);
        string name = EncloseInQuotesIfNeeded(info.Name);
        string location = EncloseInQuotesIfNeeded(info.Location);
        string website = info.WebsiteUrl;
        string file = info.ImageUrl;
        string[] parts = [id, callsign, title, name, location, website, file];
        return string.Join(",", parts);
    }

    private static List<string> ParseCsvLine(string line)
    {
        List<string> parts = [];
        string current = string.Empty;
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (insideQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current += '"';
                        i++;
                    }
                    else
                    {
                        insideQuotes = false;
                    }
                }
                else
                {
                    current += c;
                }
            }
            else
            {
                if (c == '"')
                {
                    insideQuotes = true;
                }
                else if (c == ',')
                {
                    parts.Add(current);
                    current = string.Empty;
                }
                else
                {
                    current += c;
                }
            }
        }

        parts.Add(current);

        return parts;
    }
}
