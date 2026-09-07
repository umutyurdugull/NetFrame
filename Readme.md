# DotNetFrame (NetFrame)

[![NuGet Version](https://img.shields.io/nuget/v/DotNetFrame.svg)](https://www.nuget.org/packages/DotNetFrame/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![Documentation](https://img.shields.io/badge/Docs-Vercel-black.svg)](https://netframedocs.vercel.app)

**DotNetFrame** is a high-performance, thread-safe, and resilient .NET 10 SDK that wraps IBM's z/OS Management Facility (z/OSMF) REST APIs. It empowers modern .NET applications to interact seamlessly and securely with enterprise z/OS mainframe environments—from core batch jobs and datasets to cloud provisioning, resource pooling, and sysplex management.

Built for enterprise workloads, NetFrame incorporates Microsoft standard HTTP resilience pipelines (Polly), thread-safe JWT token caching, strict TLS thumbprint verification, and native OpenTelemetry distributed tracing.

---

## Documentation

Full documentation, architecture blueprints, API reference, and module guides are available at:
 **[https://netframe.vercel.app](https://netframedocs.vercel.app)**


---

## Features

### 1. Core Mainframe Operations
* **Datasets Management (`IDatasetService`):** List, read, write, allocate, and delete Sequential and Partitioned (PDS/PDSE) datasets and members.
* **Job & JES Control (`IJobService`):** Submit JCL jobs, monitor execution progress with exponential backoff & jitter polling, hold, release, cancel, purge jobs, and retrieve spool output files.
* **JCL Builder Utility (`JclBuilder`):** Construct syntactically validated 80-column punch-card JCL scripts using a fluent, strongly typed builder API.
* **UNIX System Services (`IUSSSService`):** Navigate USS directory hierarchies, stream file contents, write UNIX files, and perform recursive directory deletions.
* **TSO Command Execution (`ITsoService`):** Execute stateless TSO command payloads inside dedicated CEA servlet sessions.
* **Operator Console Service (`IConsoleService`):** Issue operator console commands (`MVS Console`) and retrieve synchronous/asynchronous multi-line message buffers.
* **DB2 REST Client (`IDb2RestService`):** Execute SQL queries and call DB2 REST services with automated JWT token acquisition and thread-safe caching (`IDb2TokenStore`).
* **z/OS Workflows (`IWorkflowService`):** Instantiate, start, monitor, cancel, and archive multi-step configuration workflows defined in z/OSMF XML.

### 2. Cloud Provisioning & Resource Pooling
* **Cloud Provisioning (`ICloudProvisioningService`):** Dynamically allocate and release network resources (IPv4/IPv6 addresses, TCP ports, SNA application names), manage WLM classification rules, query storage dataset attributes, and manage LPAR resource pool entries.
* **Software Templates (`ISoftwareTemplateService`):** Manage private catalog (`scc`) and published self-service catalog (`psc`) templates, test templates, fetch prompt variables, and track version histories.
* **Provisioned Software Instances (`ISoftwareInstanceService`):** Manage provisioned software instance lifecycles, invoke lifecycle actions, query action responses, handle workflow resumes/retries, and update runtime variables.
* **WLM Resource Pooling (`IWlmResourcePoolingService`):** Prime WLM resource pools, construct dynamic WLM service definitions, and decommission resource pools.

### 3. Governance, Storage & Sysplex
* **Resource Management (`IResourceManagementService`):** Manage cloud Domains and Tenants, configure tenant CPU and Memory capping, bind Solution IDs, enable/disable metering, and manage consumer group permissions.
* **Management Services Catalog (`IManagementServicesCatalogService`):** Browse service catalog categories, submit service requests, modify pending submissions, trigger actions, and retrieve target systems and job statements.
* **SMP/E Software Management (`ISoftwareManagementService`):** Query SMP/E software instances, retrieve installed products/features, perform asynchronous dataset listing, and generate portable deployment exports.
* **Storage Management (`IStorageManagementService`):** Inspect DFSMS Storage Groups (from active configuration or SCDS), examine volume definitions and allocations, and list SMS Data Classes.
* **Sysplex Management (`ISysplexManagementService`):** Query CFRM (Coupling Facility Resource Management) active and stored policies, inspect coupling facility structures, and evaluate sysplex node topology.

### 4. Integration & Diagnostics
* **System Information (`ISystemService`):** Retrieve z/OSMF host system details, version metadata, and active plugin inventory.
* **RMF Metering Service (`IRmfMeteringService`):** Retrieve real-time CPU consumption and CPC resource utilization data via IBM Resource Measurement Facility.
* **External REST Gateway (`IExternalGatewayService`):** Securely proxy arbitrary HTTP GET/POST/PUT/DELETE requests to mainframe internal subsystem endpoints.
* **Application Linking & SSIN (`IAppLinkingService`, `ISsinService`):** Register z/OSMF event types, event handlers, and query eligible tasks; manage Subsystem Interface (SSIN) registries and variable names.

---

## Installation

Install the package via the NuGet Package Manager:

```bash
dotnet add package DotNetFrame
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package DotNetFrame
```

---

## Getting Started

### 1. Configuration and Service Registration

Register all NetFrame services in your application's `IServiceCollection` (typically in `Program.cs`):

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetFrame.Extensions;
using NetFrame.Models;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // Register all z/OSMF SDK Services with resilient HTTP pipelines
    services.AddZosmf(options =>
    {
        options.BaseUrl = "https://your-mainframe-ip:10443";
        options.Username = "HERC01";
        options.Password = "Mypassword";
        options.AllowInsecureConnections = true; // Set true for self-signed mainframe certificates
        // options.TrustedCertificateThumbprint = "XX:XX:XX..."; // Optional: pin exact SSL certificate thumbprint
        options.TimeoutSeconds = 90;
        options.PollingIntervalSeconds = 3;
        options.MaxPollingAttempts = 30;
    });

    // Optional: Register DB2 REST configurations separately
    services.Configure<Db2Config>(options =>
    {
        options.BaseUrl = "https://your-mainframe-ip:50050";
        options.Username = "DB2USER";
        options.Password = "Db2Password";
        options.DatabaseName = "DSN10";
        options.AllowInsecureConnections = true;
    });
});

var host = builder.Build();
```

---

## Code Examples

### 1. Dataset Management (List, Read, Write, Allocate)

```csharp
using NetFrame.Services;
using NetFrame.Models;

var datasetService = host.Services.GetRequiredService<IDatasetService>();

// 1. List datasets matching a HLQ pattern
var datasets = await datasetService.ListDatasetsAsync("HERC01.*");
foreach (var ds in datasets)
{
    Console.WriteLine($"Found Dataset: {ds}");
}

// 2. Allocate a new Partitioned Dataset (PDS)
await datasetService.CreateDatasetAsync("HERC01.PROD.JCL", new CreateDatasetRequest
{
    DirectoryBlocks = 10,
    DatasetOrganization = "PO",
    RecordFormat = "FB",
    RecordLength = 80,
    BlockSize = 3120,
    PrimarySpace = 5,
    SecondarySpace = 2,
    SpaceUnits = "CYL"
});

// 3. Write a JCL member to the PDS
await datasetService.WriteDatasetContentAsync(
    datasetName: "HERC01.PROD.JCL",
    memberName: "BACKUP01",
    content: "//BACKUP01 JOB (123),'DAILY BACKUP',CLASS=A,MSGCLASS=X\n//STEP1 EXEC PGM=IEFBR14\n",
    options: new WriteContentOptions { DataType = ZosmfDataType.Text }
);

// 4. Retrieve member content
string memberContent = await datasetService.RetrieveDatasetContentAsync("HERC01.PROD.JCL", "BACKUP01");
Console.WriteLine($"Content:\n{memberContent}");
```

### 2. JCL Builder & Job Submission (Direct & Polling)

```csharp
using NetFrame.Services;
using NetFrame.Models;

var jobService = host.Services.GetRequiredService<IJobService>();

// Programmatically build 80-column validated JCL
string jcl = new JclBuilder()
    .AddJobCard("NETFRAME", "(1001)", "ADMIN", "A", "X", "1,1")
    .AddExecStep("STEP1", "IEFBR14")
    .AddDdStatement("NEWDSN", "HERC01.SCRATCH.DATA", "NEW,CATLG,DELETE", space: "(CYL,(1,1))")
    .Build();

// Method A: Submit directly and receive strongly-typed ZosJob
Console.WriteLine("Submitting JCL directly...");
ZosJob job = await jobService.SubmitJobAsync(jcl);
Console.WriteLine($"Submitted: {job.JobName} (ID: {job.JobId}, Status: {job.Status})");

// Query real-time status
ZosJob? status = await jobService.GetJobAsync(job.JobName, job.JobId);
Console.WriteLine($"Current Status: {status?.Status}");

// Wait for job completion
ZosJob completedJob = await jobService.WaitForJobCompletionAsync(job.JobName, job.JobId, maxWaitSeconds: 60);
Console.WriteLine($"Job Completed with Return Code: {completedJob.ReturnCode}");

// Method B: Or submit and auto-wait with exponential backoff & jitter polling
string feedbackJson = await jobService.SubmitJobAndWaitAsync(new JobSubmissionOptions
{
    JclContent = jcl,
    IntrdrMode = "text"
});
Console.WriteLine($"Job Completed: {feedbackJson}");
```

### 3. UNIX System Services (USS)

```csharp
using NetFrame.Services;

var ussService = host.Services.GetRequiredService<IUSSSService>();

// List files in a USS directory
var dirResponse = await ussService.ListDirectoryAsync("/u/herc01");
foreach (var file in dirResponse.Items)
{
    Console.WriteLine($"File: {file.Name}, Size: {file.Size} bytes, Mode: {file.Mode}");
}

// Write content to a USS file
await ussService.WriteFileAsync("/u/herc01/config.json", "{\"env\": \"production\"}");

// Read content back
string content = await ussService.ReadFileAsync("/u/herc01/config.json");
Console.WriteLine($"USS Content: {content}");
```

### 4. Operator Console & TSO Commands

```csharp
using NetFrame.Services;

// Execute Operator Console command
var consoleService = host.Services.GetRequiredService<IConsoleService>();
string consoleOutput = await consoleService.IssueCommandAsync("D A,L");
Console.WriteLine($"Console Response:\n{consoleOutput}");

// Execute TSO command
var tsoService = host.Services.GetRequiredService<ITsoService>();
var tsoResponse = await tsoService.IssueCommandAsync("TIME");
foreach (var msg in tsoResponse.TsoResponseMessages)
{
    Console.WriteLine($"TSO: {msg.Data}");
}
```

### 5. DB2 REST Gateway Queries

```csharp
using NetFrame.Services;

var db2Service = host.Services.GetRequiredService<IDb2RestService>();

// Execute SQL via DB2 REST Gateway with auto-cached JWT token
string jsonResult = await db2Service.ExecuteSqlAsync("SELECT CURRENT TIMESTAMP FROM SYSIBM.SYSDUMMY1");
Console.WriteLine($"DB2 Result: {jsonResult}");
```

### 6. z/OS Workflows

```csharp
using NetFrame.Services;
using NetFrame.Models.Workflow;

var workflowService = host.Services.GetRequiredService<IWorkflowService>();

// Create a new workflow instance from XML definition file
var createResponse = await workflowService.CreateWorkflowAsync(new CreateWorkflowRequest
{
    WorkflowName = "PROVISION_ENV_01",
    WorkflowDefinitionFile = "/usr/lpp/zosmf/samples/workflow_sample.xml",
    System = "SYS1",
    Owner = "HERC01",
    AssignToOwner = true
});

// Start workflow execution
await workflowService.StartWorkflowAsync(createResponse.WorkflowKey, new StartWorkflowRequest
{
    PerformSubsequent = true
});

// Check status
var props = await workflowService.GetWorkflowPropertiesAsync(createResponse.WorkflowKey);
Console.WriteLine($"Workflow Status: {props.StatusName}");
```

### 7. Cloud Provisioning & Resource Pools

```csharp
using NetFrame.Services;
using NetFrame.Models.CloudProvisioning;

var cloudService = host.Services.GetRequiredService<ICloudProvisioningService>();

// Dynamically obtain an IP address from the network resource pool
var ipResponse = await cloudService.ObtainIpAddressAsync(new ResourcePoolRequest<ObtainIpParams>
{
    TemplateName = "DB2_TEMPLATE",
    TenantId = "TENANT_DEV",
    NetworkParams = new ObtainIpParams
    {
        PoolName = "DYNAMIC_IP_POOL",
        System = "SYS1"
    }
});
Console.WriteLine($"Allocated IP: {ipResponse.IpAddress} (ID: {ipResponse.IpId})");

// Release IP address when decommissioned
await cloudService.ReleaseIpAddressAsync(new ResourcePoolRequest<ReleaseIpParams>
{
    TenantId = "TENANT_DEV",
    NetworkParams = new ReleaseIpParams { IpId = ipResponse.IpId }
});
```

### 8. Cloud Software Templates & Instances

```csharp
using NetFrame.Services;
using NetFrame.Models.Provisioning;

var templateService = host.Services.GetRequiredService<ISoftwareTemplateService>();
var instanceService = host.Services.GetRequiredService<ISoftwareInstanceService>();

// Run a published software template to provision an environment
var runResponse = await templateService.RunTemplateAsync("CICS_PROD_TEMPLATE", new RunTemplateRequest
{
    System = "SYS1",
    TenantId = "TENANT_PROD",
    PromptVariables = new List<PromptVariable>
    {
        new() { Name = "APPLID", Value = "CICSTST1" },
        new() { Name = "PORT", Value = "5000" }
    }
});

// Perform a lifecycle action (e.g. stop/start/deprovision) on the instance
await instanceService.PerformActionAsync(runResponse.InstanceId, "stop");
```

### 9. Resource Management (Domains, Tenants, Capping)

```csharp
using NetFrame.Services;
using NetFrame.Models.ResourceManagement;

var resourceMgmt = host.Services.GetRequiredService<IResourceManagementService>();

// List cloud tenants
var tenants = await resourceMgmt.ListTenantsAsync();
foreach (var tenant in tenants.TenantList ?? new())
{
    Console.WriteLine($"Tenant: {tenant.TenantName}, ID: {tenant.TenantId}");
}

// Assign CPU capping properties to enforce workload boundaries
await resourceMgmt.AssignCpuCappingPropertiesAsync("TENANT_DEV", new AssignCpuCappingRequest
{
    CappingLimit = 250,
    CappingType = "MSU"
});
```

### 10. Management Services Catalog

```csharp
using NetFrame.Services;
using NetFrame.Models.ManagementServicesCatalog;

var catalogService = host.Services.GetRequiredService<IManagementServicesCatalogService>();

// List available self-service catalog offerings
var services = await catalogService.ListCatalogServicesAsync(state: "published");
foreach (var s in services)
{
    Console.WriteLine($"Catalog Service: {s.CsName} ({s.ObjectId})");
}

// Submit a service execution request
string submissionId = await catalogService.CreateServiceSubmissionAsync(new CreateServiceSubmissionRequest
{
    CsObjectId = services[0].ObjectId,
    SubmissionName = "REQUEST_STORAGE_EXPANSION"
});
Console.WriteLine($"Submission ID: {submissionId}");
```

### 11. SMP/E Software Management

```csharp
using NetFrame.Services;

var swMgmt = host.Services.GetRequiredService<ISoftwareManagementService>();

// List SMP/E managed software instances across sysplex
var swInstances = await swMgmt.ListSoftwareInstancesAsync();
foreach (var inst in swInstances)
{
    Console.WriteLine($"Software Instance: {inst.SwiName} on {inst.SystemNickname}");
}

// Start async dataset inventory calculation
string statusUrl = await swMgmt.StartListDataSetsAsync(swInstances[0].Uuid);
var status = await swMgmt.GetListDataSetsStatusAsync(statusUrl);
Console.WriteLine($"Dataset List Status: {status.Status}");
```

### 12. Storage & Sysplex Management

```csharp
using NetFrame.Services;

// Storage Management (DFSMS)
var storageService = host.Services.GetRequiredService<IStorageManagementService>();
var storageGroups = await storageService.ListStorageGroupsAsync(type: "POOL");
foreach (var sg in storageGroups)
{
    Console.WriteLine($"Storage Group: {sg.StorageGroupName}, Type: {sg.StorageGroupType}");
}

// Sysplex Management (CFRM)
var sysplexService = host.Services.GetRequiredService<ISysplexManagementService>();
var cfrm = await sysplexService.ListCfrmPoliciesAsync();
Console.WriteLine($"Active CFRM Policy: {cfrm.ActivePolicy?.PolicyName}");
```

### 13. System Information & RMF Metering

```csharp
using NetFrame.Services;

// Retrieve z/OSMF version and topology
var systemService = host.Services.GetRequiredService<ISystemService>();
var info = await systemService.GetInfoAsync();
Console.WriteLine($"z/OSMF Version: {info.ZosmfVersion}, Host: {info.ZosmfHost}");

// Collect RMF CPC metering metrics
var rmfService = host.Services.GetRequiredService<IRmfMeteringService>();
string meterData = await rmfService.GetMeterDataAsync();
Console.WriteLine($"RMF Metrics:\n{meterData}");
```

---

## Resilience & Network Pipelines

Every service client in NetFrame is pre-configured with Microsoft's `Microsoft.Extensions.Http.Resilience` pipeline:

* **Attempt Timeout:** 30 seconds per individual HTTP request attempt.
* **Total Timeout:** 90 seconds overall execution ceiling.
* **Circuit Breaker:** Automatic sampling over 60-second windows to isolate failing mainframe connections and prevent thread pool starvation.
* **Automatic Retries:** Exponential backoff with jitter for transient HTTP 5xx failures and socket disconnects.
* **Credential Redaction:** Automatic redaction in `ZosmfErrorHandler` prevents plaintext mainframe passwords or RACF credentials from appearing in application logs.

---

## Telemetry and Tracing

`DotNetFrame` natively supports OpenTelemetry distributed tracing using `ActivitySource`. Listen to source `NetFrame.Sdk`:

```csharp
using System.Diagnostics;

var listener = new ActivityListener
{
    ShouldListenTo = source => source.Name == "NetFrame.Sdk",
    Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
    ActivityStarted = activity => Console.WriteLine($"[Trace] Started: {activity.DisplayName}"),
    ActivityStopped = activity => Console.WriteLine($"[Trace] Stopped: {activity.DisplayName} ({activity.Duration})")
};

ActivitySource.AddActivityListener(listener);
```

---

## License

This project is licensed under the [MIT License](LICENSE).
