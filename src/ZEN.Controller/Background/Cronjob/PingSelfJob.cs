using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class PingSelfJob : IJob
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly ILogger<PingSelfJob> _logger;
    public PingSelfJob(ILogger<PingSelfJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            // var response = await _httpClient.GetAsync("https://www.google.com");
            var response = await _httpClient.GetAsync("https://my-cv-suxl.onrender.com/healthcheck");
            _logger.LogTrace($"[Ping] {DateTime.Now}: {(int)response.StatusCode}");
            Console.WriteLine($"[Ping Console] {DateTime.Now}: {(int)response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogTrace($"[Ping Error] {ex.Message}");
            Console.WriteLine($"[Ping Error Console] {ex.Message}");
        }
    }
}
