using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFrame.Extensions;
using NetFrame.Models;
using NetFrame.Services;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
var systemService = serviceProvider.GetRequiredService<ISystemService>();
var datasetService = serviceProvider.GetRequiredService<IDatasetService>();
var ussService = serviceProvider.GetRequiredService<IUSSSService>(); 


string jobToSubmit = "//'ZXP.PUBLIC.JCL(CHKJCL1)'";


    string finalResponse = await jobService.SubmitJobAndWaitAsync(jobToSubmit);

    if (!string.IsNullOrEmpty(finalResponse))
    {
        Console.WriteLine("final job response:");
        Console.WriteLine(finalResponse);
    }
    else
    {
        Console.WriteLine("no result returned from job service.");
    }

    
    string targetDataset = "Z88116.CBL";
    string targetMember = "HELLO";


    
    string contentToWrite2 = "       IDENTIFICATION DIVISION.\r\n       PROGRAM-ID. HELLO.\r\n       PROCEDURE DIVISION.\r\n           DISPLAY 'HELLO TURKEY!'.\r\n           GOBACK.\r\n";

    var writeOptions = new WriteContentOptions
    {
        ContentType = "text/plain",
        DataType = "text"
    };




    Console.WriteLine($"writing content to {targetDataset}({targetMember})...");

    string eTag = await datasetService.WriteDatasetContentAsync(
        datasetName: targetDataset,
        memberName: targetMember,
        content: contentToWrite2,
        volser: null,
        options: writeOptions
    );

    Console.WriteLine("write operation successful!");
    if (!string.IsNullOrEmpty(eTag))
    {
        Console.WriteLine($"returned etag: {eTag}");
    }





/*
 set ZOSMF_BASE_URL=https://204.90.115.200:10443
set ZOSMF_USERNAME=z88116
set ZOSMF_PASSWORD=GRB53ONI
dotnet run* 
 
 
 */


string newDatasetName = "Z88116.NET";
var createOptions = new CreateDatasetOptions();
string contentToWrite = "       IDENTIFICATION DIVISION.\r\n       PROGRAM-ID. HELLO.\r\n       PROCEDURE DIVISION.\r\n           DISPLAY 'HELLO TURKEY!'.\r\n           GOBACK.\r\n";
var createDatasetRequest = new CreateDatasetRequest
{
    Dsorg = "PO",     // pds (partitioned data set) icin po, sequential icin ps
    Recfm = "FB",     // fixed block
    Lrecl = 80,       // record length
    Blksize = 3200,   // block size
    Alcunit = "TRK",  // allocation unit (tracks)
    Primary = 10,     // primary space
    Secondary = 5,    // secondary space
    Dirblk = 10       // directory blocks (pds icin zorunlu)
};


//await datasetService.CreateDatasetAsync(newDatasetName, createDatasetRequest, createOptions, default);

//string createWritten = await datasetService.WriteDatasetContentAsync(
//        datasetName: targetDataset,
//        memberName: targetMember,
//        content: contentToWrite2,
//        volser: null,
//        options: writeOptions
//    );  

//zaten olustugu icin bi daha run edersem program patlayacak o yüzden hiç gereği yok.
