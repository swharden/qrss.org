namespace DevTestApi.Endpoints;

public static class DevTestEndpoints
{
    static int Counter = 0;

    record TestResponse(int Count, long ID);

    public static void MapDevTestEndpoints(this WebApplication app)
    {
        app.MapGet("/devtest", () => new TestResponse(Counter++, DateTime.UtcNow.Ticks))
            .WithName("DevTest")
            .WithOpenApi();
    }
}
