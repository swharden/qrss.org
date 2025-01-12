using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    StringBuilder sb = new();

    sb.AppendLine($"<div>Updated {DateTime.Now.Ticks}</div>");

    foreach (string path in Directory.GetFiles(Qrss.Core.ApplicationPaths.GrabImageFolder))
    {
        string url = "data/grabs/" + Path.GetFileName(path);
        sb.AppendLine($"<a href='{url}'><img src='{url}' width='300'></a>");
    }

    string html = $"""
        <!doctype html>
        <html lang="en">
          <head>
            <base href="http://localhost/" target="_blank">
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
