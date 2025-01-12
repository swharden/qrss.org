namespace Qrss.API.Pages;

public static class Extensions
{
    public static void MapPages(this WebApplication? app)
    {
        app?.MapGet("/", () => GetHomePage());
    }

    private static IResult GetHomePage()
    {
        string html = $"""
        <!doctype html>
        <html lang="en">
          <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
          </head>
          <body>
            <div class='container my-5'>
                <h1>QRSS.org API</h1>
                <div class='my-3'>
                    <div><a href='v1'>JSON version 1</a></div>
                    <div><code>{DateTime.Now.Ticks}</code></div>
                </div>
            </div>
          </body>
        </html>
        """;

        return Results.Content(html, "text/html");
    }
}
