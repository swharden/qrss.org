namespace Qrss.Core;

public static class ApplicationPaths
{
    public static string GrabImageFolder { get; } = GetGrabImageFolder();
    public static string LogFolder { get; } = GetLogFolder();

    static string GetAppFolder()
    {
        string systemAppFolder = Path.GetFullPath("/app");
        if (Directory.Exists(systemAppFolder))
        {
            return systemAppFolder;
        }

        string repoAppFolder = GetRepoAppFolder();
        if (Directory.Exists(repoAppFolder))
        {
            return repoAppFolder;
        }

        throw new DirectoryNotFoundException("Folder named 'app' not found");
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

    /// <summary>
    /// Call this when running in a dev environment from the GitHub repo.
    /// It will return the path to REPO/src/volumes or will throw.
    /// </summary>
    static string GetRepoAppFolder()
    {
        string? path = Path.GetFullPath("./");

        while (path is not null)
        {
            string licenseFilePath = Path.Combine(path, "LICENSE");
            if (File.Exists(licenseFilePath))
            {
                string volumesFolder = Path.Combine(path, "src/volumes/app");
                if (Directory.Exists(volumesFolder))
                {
                    return Path.GetFullPath(volumesFolder);
                }
                else
                {
                    throw new DirectoryNotFoundException($"repository found but it does not contain {volumesFolder}");
                }
            }
            path = Path.GetDirectoryName(path);
        }

        throw new DirectoryNotFoundException("repository not found");
    }
}
