using Qrss.API.Pages;
using Qrss.API.V1;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
WebApplication? app = builder.Build();

app.MapPages();
app.MapApiVersion1();

app.Run();
