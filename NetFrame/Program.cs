using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFrame.Extensions;
using NetFrame.Services;
using System;
using System.Threading.Tasks;
var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var baseUrl = Environment.GetEnvironmentVariable("ZOSMF_BASE_URL");
var username = Environment.GetEnvironmentVariable("ZOSMF_USERNAME");
var password = Environment.GetEnvironmentVariable("ZOSMF_PASSWORD");

if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("Missing ZOSMF configuration. Set ZOSMF_BASE_URL, ZOSMF_USERNAME, and ZOSMF_PASSWORD environment variables.");
    return;
}

services.AddZosmf(options =>
{
    options.BaseUrl = baseUrl;
    options.Username = username;
    options.Password = password;
    options.AllowInsecureConnections = true; 
    options.PollingIntervalSeconds = 5;
    options.MaxPollingAttempts = 20;
});

var serviceProvider = services.BuildServiceProvider();

var jobService = serviceProvider.GetRequiredService<IJobService>();

string jobToSubmit = "//'ZXP.PUBLIC.JCL(CHKJCL1)'";

try
{
    Console.WriteLine($"Submitting job: {jobToSubmit}...");
    string finalResponse = await jobService.SubmitJobAndWaitAsync(jobToSubmit);
    
    if (!string.IsNullOrEmpty(finalResponse))
    {
        Console.WriteLine("Final Job Response:");
        Console.WriteLine(finalResponse);
    }
    else
    {
        Console.WriteLine("No result returned from job service.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }
}
