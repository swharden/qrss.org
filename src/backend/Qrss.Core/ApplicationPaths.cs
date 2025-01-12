namespace Qrss.Core;

public static class ApplicationPaths
{
    public static string GrabImageFolder { get; } = GetGrabImageFolder();
    public static string LogFolder { get; } = GetLogFolder();

    static string GetAppFolder()
    {
        string?[] foldersToCheck = [
            "/",
            GetRepoRootFolder(),
        ];

        foreach (string? folder in foldersToCheck)
        {
            if (folder is null)
                continue;

            string appFolder = Path.Combine(folder, "app");
            Console.WriteLine($"Checking app folder: {appFolder}");
            if (Directory.Exists(appFolder))
                return appFolder;
        }

        throw new DirectoryNotFoundException("Folder named 'app' could not be located. " +
            "Place it at the root of the git repo (for dev) or filesystem (for prod)");
    }

    static string GetDataFolder()
    {
        string path = Path.Combine(GetAppFolder(), "data");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    static string GetGrabImageFolder()
    {
        string path = Path.Combine(GetDataFolder(), "grabs");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    static string GetLogFolder()
    {
        string path = Path.Combine(GetDataFolder(), "logs");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    static string? GetRepoRootFolder()
    {
        string? path = Path.GetFullPath("./");

        while (path is not null)
        {
            string licenseFilePath = Path.Combine(path, "LICENSE");
            if (File.Exists(licenseFilePath))
            {
                Console.WriteLine($"Repository root: {path}");
                return path;
            }
            path = Path.GetDirectoryName(path);
        }

        return null;
    }
}
