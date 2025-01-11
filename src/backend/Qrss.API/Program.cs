using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    // TODO: use core logic to store folder paths
    string dataFolder = Path.Combine(AppContext.BaseDirectory, "data");
    string grabImageFolder = Path.Combine(dataFolder, "grabs");

    if (!Directory.Exists(grabImageFolder))
    {
        return Results.Problem($"grab image folder not found: {grabImageFolder}");
    }

    StringBuilder sb = new();

    sb.AppendLine($"<div>Updated {DateTime.Now.Ticks}</div>");

    foreach (string path in Directory.GetFiles(grabImageFolder))
    {
        sb.AppendLine($"<div>{path}</div>");
    }

    string html = $"""
        <!doctype html>
        <html lang="en">
          <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
          </head>
          <body>
            <div class='container'>{sb.ToString()}</div>
          </body>
        </html>
        """;

    return Results.Content(html, "text/html");
});

app.Run();
