
using ZEN.Controller;
using ZEN.Controller.Extensions;
using ZEN.Infrastructure.Mysql.Persistence;

DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

if (!bool.Parse(Environment.GetEnvironmentVariable("DB_LOGGING") ?? "True"))
{
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
}
builder.WebHost.UseUrls("http://0.0.0.0:5005");
builder.Services.ApplyDInjectionService(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
    });

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (await dbContext.Database.CanConnectAsync())
    {
        Console.WriteLine("Connected to Neon DB successfully.");
    }
    else
    {
        Console.WriteLine("Failed to connect to Neon DB.");
    }
}
await app.ApplyWebBuilder();
app.Run();

