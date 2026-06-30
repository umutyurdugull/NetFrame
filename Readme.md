# DotNetFrame (NetFrame)

[![NuGet Version](https://img.shields.io/nuget/v/DotNetFrame.svg)](https://www.nuget.org/packages/DotNetFrame/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**DotNetFrame** is a high-performance, thread-safe, and resilient .NET 10 SDK that wraps IBM's z/OS Management Facility (z/OSMF) REST APIs. It allows .NET applications to interact seamlessly with mainframe components (Datasets, Jobs, UNIX System Services, DB2, and System Consoles).

Built for enterprise-grade workloads, it includes built-in Polly resilience pipelines, thread-safe token caching, strict TLS thumbprint verification, and leak-free connection pool handling.

---

## Features

- **Data Sets Management:** List, create, read, and write mainframe partitioned and sequential data sets.
- **Job Submissions (JCL):** Submit JCL jobs, monitor progress using exponential backoff + jitter polling, hold, release, and retrieve execution logs.
- **UNIX System Services (USS):** Full USS directory navigation and file reading/writing.
- **TSO Commands:** Execute stateless TSO command payloads directly.
- **Console Service:** Run console commands and retrieve buffer messages.
- **DB2 REST Client:** Query DB2 databases using secure, auto-refreshing Bearer token cached sessions.
- **Resilience Strategy:** Native integration of Microsoft's HTTP resilience policies (Circuit Breaker, Retries, Timeouts).
- **Modern Security Standards:** Clean basic authentication, secure local credential redaction, and strict custom certificate thumbprint validations.

---

## Installation

Install the package via the NuGet Package Manager Console:

```bash
dotnet add package DotNetFrame
```

---

## Getting Started

### 1. Configuration and Service Registration

Register the SDK services in your `IServiceCollection` (typically in `Program.cs` or `Startup.cs`):

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetFrame.Extensions;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // Register z/OSMF SDK Services
    services.AddZosmf(options =>
    {
        options.BaseUrl = "https://your-mainframe-ip:10443";
        options.Username = "HERC01";
        options.Password = "Mypassword";
        options.AllowInsecureConnections = true; // Set to true to bypass self-signed certificate warnings
        // options.TrustedCertificateThumbprint = "XX:XX:XX..."; // Optional: restrict SSL trust to a specific thumbprint
    });

    // Optional: Register DB2 REST configurations separately
    // services.Configure<Db2Config>(context.Configuration.GetSection("Db2Config"));
});

var host = builder.Build();
```

---

## Usage Examples

### 1. List and Read Data Sets

```csharp
using NetFrame.Services;

var datasetService = host.Services.GetRequiredService<IDatasetService>();

// List datasets matching a pattern
var datasets = await datasetService.ListDatasetsAsync("HERC01.*");
foreach (var dsName in datasets)
{
    Console.WriteLine($"Found Dataset: {dsName}");
}

// Write content to a Partitioned Data Set member
await datasetService.WriteDatasetContentAsync(
    datasetName: "HERC01.TEST.JCL",
    memberName: "MYJOB",
    content: "//MYJOB JOB (123),'TEST'...",
    options: new WriteContentOptions { DataType = ZosmfDataType.Text }
);

// Read content from a Data Set
string content = await datasetService.RetrieveDatasetContentAsync("HERC01.TEST.JCL", "MYJOB");
Console.WriteLine(content);
```

### 2. Submit a JCL Job and Wait for Output

```csharp
using NetFrame.Services;
using NetFrame.Models;

var jobService = host.Services.GetRequiredService<IJobService>();

var options = new JobSubmissionOptions
{
    JclContent = @"//HELLO    JOB (123),'HELLO WORLD',CLASS=A,MSGCLASS=X
//STEP1    EXEC PGM=IEFBR14
",
    IntrdrMode = "text"
};

Console.WriteLine("Submitting JCL Job...");
string jobFeedbackJson = await jobService.SubmitJobAndWaitAsync(options);
Console.WriteLine($"Job finished! Status details: {jobFeedbackJson}");
```

### 3. Run Console Commands

```csharp
using NetFrame.Services;

var consoleService = host.Services.GetRequiredService<IConsoleService>();

// Execute operator console commands
string response = await consoleService.IssueCommandAsync("D A,L");
Console.WriteLine("Console Output:");
Console.WriteLine(response);
```

### 4. Query DB2 via REST Client

```csharp
using NetFrame.Services;

var db2Service = host.Services.GetRequiredService<IDb2RestService>();

// Execute arbitrary SQL queries through the DB2 gateway
string jsonResult = await db2Service.ExecuteSqlAsync("SELECT CURRENT TIMESTAMP FROM SYSIBM.SYSDUMMY1");
Console.WriteLine(jsonResult);
```

---

## Telemetry and Tracing

`DotNetFrame` supports OpenTelemetry distributed tracing natively using `ActivitySource`. You can listen to the activity source `NetFrame.Sdk` to monitor performance metrics:

```csharp
using System.Diagnostics;

var listener = new ActivityListener
{
    ShouldListenTo = source => source.Name == "NetFrame.Sdk",
    Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
    ActivityStarted = activity => Console.WriteLine($"Tracing Started: {activity.DisplayName}"),
    ActivityStopped = activity => Console.WriteLine($"Tracing Stopped: {activity.DisplayName} - Duration: {activity.Duration}")
};

ActivitySource.AddActivityListener(listener);
```

---

## License

This project is licensed under the MIT License 
