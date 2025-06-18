
using ZEN.Controller;
using ZEN.Controller.Extensions;
using ZEN.Infrastructure.Mysql.Persistence;
using Quartz;

DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // để hiển thị log ra console
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("PingSelfJob");
    q.AddJob<PingSelfJob>(opts => opts.WithIdentity(jobKey));


    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("PingSelfTrigger")
        .WithCronSchedule("0 */5 * * * ?")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

if (!bool.Parse(Environment.GetEnvironmentVariable("DB_LOGGING") ?? "True"))
{
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
}
// builder.WebHost.UseUrls("http://0.0.0.0:5005");
var port = Environment.GetEnvironmentVariable("PORT") ?? "5005";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
builder.Services.ApplyDInjectionService(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
    });

var app = builder.Build();
app.MapGet("/healthcheck", () => Results.Ok("Server is alive!"));
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

