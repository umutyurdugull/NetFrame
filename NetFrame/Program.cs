using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFrame.Extensions;
using NetFrame.Models;
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
var datasetService = serviceProvider.GetRequiredService<IDatasetService>();


var case3Options = new JobSubmissionOptions
{
    LocalFilePath = "simple.jcl",
    DestinationDataset = "Z88116.JCL",
    DestinationMember = "HELLO"
};

string case3Response = await jobService.SubmitJobAndWaitAsync(case3Options);

if (!string.IsNullOrEmpty(case3Response))
{

    Console.WriteLine(case3Response);
}
