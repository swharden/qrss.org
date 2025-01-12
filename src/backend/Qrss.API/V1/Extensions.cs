namespace Qrss.API.V1;

public static class Extensions
{
    public static void MapApiVersion1(this WebApplication? app)
    {
        app?.MapGet("/v1", () => Results.Ok(Directory.GetFiles(Core.ApplicationPaths.GrabImageFolder).Select(GrabInfo.FromPath)));
    }
}
