using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFrame.Extensions;
using NetFrame.Services;
using System;
using System.Threading.Tasks;

namespace NetFrame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var baseUrl = Environment.GetEnvironmentVariable("ZOSMF_BASE_URL")?.Trim()?.Trim('"', '\'');
            var username = Environment.GetEnvironmentVariable("ZOSMF_USERNAME")?.Trim()?.Trim('"', '\'');
            var password = Environment.GetEnvironmentVariable("ZOSMF_PASSWORD")?.Trim()?.Trim('"', '\'');

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Missing ZOSMF configuration. Set ZOSMF_BASE_URL, ZOSMF_USERNAME, and ZOSMF_PASSWORD environment variables.");
                return;
            }

            if (!baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = "https://" + baseUrl;
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

            Console.WriteLine("NetFrame z/OSMF Client Application Started.");
            Console.WriteLine("-------------------------------------------");

            try
            {
                var storageService = serviceProvider.GetRequiredService<IStorageManagementService>();
                Console.WriteLine("\n[1] Fetching Storage Groups (Type: POOL)...");
                var storageGroups = await storageService.ListStorageGroupsAsync(type: "POOL");
                if (storageGroups != null && storageGroups.Count > 0)
                {
                    foreach (var sg in storageGroups)
                    {
                        Console.WriteLine($"    -> {sg.StorageGroupName} | Used: {sg.SpaceUsed} GB / Total: {sg.TotalSpace} GB");
                    }
                }
                else
                {
                    Console.WriteLine("    No storage groups found or returned empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IStorageManagementService error: {ex.Message}");
            }

            try
            {
                var sysplexService = serviceProvider.GetRequiredService<ISysplexManagementService>();
                Console.WriteLine("\n[2] Fetching CFRM Policies...");
                var policyList = await sysplexService.ListCfrmPoliciesAsync();
                if (policyList != null && policyList.Items != null)
                {
                    foreach (var policy in policyList.Items)
                    {
                        Console.WriteLine($"    -> Policy: {policy.Name} (Defined: {policy.Defined})");
                    }

                    if (policyList.ActivePolicy != null)
                    {
                        Console.WriteLine($"\n    *** ACTIVE POLICY: {policyList.ActivePolicy.Name} ***");
                    }
                }
                else
                {
                    Console.WriteLine("    No CFRM policies found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ISysplexManagementService error: {ex.Message}");
            }

            try
            {
                var wlmPoolingService = serviceProvider.GetRequiredService<IWlmResourcePoolingService>();
                Console.WriteLine("\n[3] WLM Resource Pooling Example...");
                var wlmRequest = new NetFrame.Models.WlmResourcePooling.PrimeWrpRequest
                {
                    CloudInfo = new NetFrame.Models.WlmResourcePooling.CloudInfo
                    {
                        DomainName = "DOMAIN1",
                        TenantName = "Joey",
                        TemplateName = "CICSBasic"
                    },
                    WrpData = new NetFrame.Models.WlmResourcePooling.WrpData
                    {
                        WrpName = "WRP1",
                        ReportClassName = "Joey00",
                        ServiceLevelAgreements = new System.Collections.Generic.List<NetFrame.Models.WlmResourcePooling.ServiceLevelAgreement>
                        {
                            new NetFrame.Models.WlmResourcePooling.ServiceLevelAgreement { SlaName = "GOLD" }
                        }
                    }
                };
                var wlmResponse = await wlmPoolingService.PrimeWlmResourcePoolAsync(wlmRequest);
                Console.WriteLine($"WLM Pool primed. ID: {wlmResponse.WrpId}, Status: {wlmResponse.Status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IWlmResourcePoolingService error: {ex.Message}");
            }

            try
            {
                var jobService = serviceProvider.GetRequiredService<IJobService>();
                Console.WriteLine("\n[4] Fetching Jobs for current user...");
                var jobsList = await jobService.ListJobsAsync(owner: username, prefix: "*", maxJobs: "5");
                
                if (jobsList != null && jobsList.Count > 0)
                {
                    foreach (var job in jobsList)
                    {
                        Console.WriteLine($"    -> Job: {job.JobName} | ID: {job.JobId} | Status: {job.Status}");
                    }

                    var firstJob = jobsList[0];
                    if (!string.IsNullOrEmpty(firstJob.JobName) && !string.IsNullOrEmpty(firstJob.JobId))
                    {
                        Console.WriteLine($"\n    Fetching files for Job: {firstJob.JobName} ({firstJob.JobId})...");
                        var filesList = await jobService.ListJobFilesAsync(firstJob.JobName, firstJob.JobId);
                        
                        foreach (var file in filesList)
                        {
                            Console.WriteLine($"      -> Spool File ID: {file.Id} | DDName: {file.DdName} | Records: {file.RecordCount}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("    No jobs found for the specified criteria.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IJobService error: {ex.Message}");
            }

            try
            {
                var catalogService = serviceProvider.GetRequiredService<IManagementServicesCatalogService>();
                Console.WriteLine("\n[5] Fetching z/OS Management Services Catalog Categories...");
                var categories = await catalogService.ListCategoriesAsync();
                
                if (categories != null && categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        Console.WriteLine($"    -> Category: {category.CcName} | ID: {category.ObjectId}");
                    }

                    Console.WriteLine("\n    Fetching catalog services...");
                    var servicesList = await catalogService.ListCatalogServicesAsync(summary: true);
                    foreach (var svc in servicesList)
                    {
                        Console.WriteLine($"      -> Service: {svc.CsName} | State: {svc.CsState} | Category: {svc.CsCategoryName}");
                    }
                }
                else
                {
                    Console.WriteLine("    No catalog categories found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IManagementServicesCatalogService error: {ex.Message}");
            }

            try
            {
                var datasetService = serviceProvider.GetRequiredService<IDatasetService>();
                Console.WriteLine("\n[6] Dataset Service Example...");
                var datasetList = await datasetService.ListDatasetsAsync("SYS1.*");
                if (datasetList != null && datasetList.Count > 0)
                {
                    foreach (var ds in datasetList)
                    {
                        Console.WriteLine($"    -> Dataset: {ds}");
                    }
                }
                else
                {
                    Console.WriteLine("    No datasets found for the specified criteria.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IDatasetService error: {ex.Message}");
            }

            try
            {
                var systemService = serviceProvider.GetRequiredService<ISystemService>();
                Console.WriteLine("\n[7] System Service Example...");
                var systemInfo = await systemService.GetInfoAsync();
                Console.WriteLine($"    -> z/OSMF Version: {systemInfo?.ZosmfVersion} | z/OS Version: {systemInfo?.ZosVersion}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ISystemService error: {ex.Message}");
            }

            try
            {
                var appLinkingService = serviceProvider.GetRequiredService<IAppLinkingService>();
                Console.WriteLine("\n[8] App Linking Service Example...");
                var registerRequest = new NetFrame.Models.RegisterEventTypeRequest
                {
                    Id = "MY.COMPANY.VIEW_STATUS",
                    DisplayName = "View My App Status",
                    Owner = "MYAPP",
                    Parameters = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "appId", "ID of the application" }
                    }
                };
                var response = await appLinkingService.RegisterEventTypeAsync(registerRequest);
                if (response.Error == null)
                {
                    Console.WriteLine("    -> Event type registered successfully!");
                }
                else
                {
                    Console.WriteLine($"    -> Registration failed: {response.Error.MsgText}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IAppLinkingService error: {ex.Message}");
            }

            try
            {
                var gatewayService = serviceProvider.GetRequiredService<IExternalGatewayService>();
                Console.WriteLine("\n[9] External Gateway Service Example...");
                var gatewayRequest = new NetFrame.Models.ExternalGatewayRequest
                {
                    Target = "appServer1",
                    ResourcePath = "/testApp",
                    Wrapped = "Y"
                };
                var gatewayResponse = await gatewayService.GetDataAsync(gatewayRequest);
                if (gatewayResponse.SystemsOutput?.Error == null)
                {
                    Console.WriteLine("    -> Data retrieved via gateway successfully!");
                }
                else
                {
                    Console.WriteLine($"    -> Gateway request failed: {gatewayResponse.SystemsOutput?.Error?.MsgText}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IExternalGatewayService error: {ex.Message}");
            }

            try
            {
                var provisioningService = serviceProvider.GetRequiredService<ICloudProvisioningService>();
                Console.WriteLine("\n[10] Cloud Provisioning Service Example...");
                var obtainIpRequest = new NetFrame.Models.CloudProvisioning.ResourcePoolRequest<NetFrame.Models.CloudProvisioning.ObtainIpParams>
                {
                    TemplateName = "CICSBasic",
                    TenantId = "IZU$0AA",
                    NetworkParams = new NetFrame.Models.CloudProvisioning.ObtainIpParams
                    {
                        Name = "CICSA IP",
                        UsageType = "internal",
                        IpAddr = "any4",
                        JobName = "WLP001"
                    }
                };
                var ipResponse = await provisioningService.ObtainIpAddressAsync(obtainIpRequest);
                Console.WriteLine($"    -> Successfully obtained IP: {ipResponse.IpAddr} (ID: {ipResponse.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ICloudProvisioningService error: {ex.Message}");
            }

            try
            {
                var resourceMgmtService = serviceProvider.GetRequiredService<IResourceManagementService>();
                Console.WriteLine("\n[11] Resource Management Service Example...");
                var domains = await resourceMgmtService.ListDomainsAsync();
                if (domains.DomainList != null)
                {
                    foreach (var domain in domains.DomainList)
                    {
                        Console.WriteLine($"    -> Domain: {domain.DomainName} (ID: {domain.DomainId}, State: {domain.DomainState})");
                    }
                }
                else
                {
                    Console.WriteLine("    -> No domains found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IResourceManagementService error: {ex.Message}");
            }

            try
            {
                var templateService = serviceProvider.GetRequiredService<ISoftwareTemplateService>();
                Console.WriteLine("\n[12] Software Template Service Example...");
                var publishedTemplates = await templateService.ListPublishedTemplatesAsync();
                if (publishedTemplates.PscList != null)
                {
                    foreach (var template in publishedTemplates.PscList)
                    {
                        Console.WriteLine($"    -> Template: {template.Name} (v{template.Version}): {template.Description}");
                    }
                }
                else
                {
                    Console.WriteLine("    -> No published software templates found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ISoftwareTemplateService error: {ex.Message}");
            }

            try
            {
                var instanceService = serviceProvider.GetRequiredService<ISoftwareInstanceService>();
                Console.WriteLine("\n[13] Software Instance Service Example...");
                var instances = await instanceService.ListInstancesAsync();
                if (instances.SccList != null)
                {
                    foreach (var instance in instances.SccList)
                    {
                        Console.WriteLine($"    -> Instance: {instance.ExternalName} (Type: {instance.Type}, State: {instance.State})");
                    }
                }
                else
                {
                    Console.WriteLine("    -> No provisioned software instances found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ISoftwareInstanceService error: {ex.Message}");
            }

            try
            {
                var ssinService = serviceProvider.GetRequiredService<ISsinService>();
                Console.WriteLine("\n[14] SSIN Service Example...");
                var response = await ssinService.ListSsinAsync();
                if (response != null && response.SsinList != null)
                {
                    foreach(var ssin in response.SsinList)
                    {
                        Console.WriteLine($"    -> SSIN: {ssin.Ssin}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ISsinService error: {ex.Message}");
            }

            try
            {
                var swMgmtService = serviceProvider.GetRequiredService<ISoftwareManagementService>();
                Console.WriteLine("\n[15] Software Management Service Example...");
                var swiList = await swMgmtService.ListSoftwareInstancesAsync();
                if (swiList != null && swiList.Count > 0)
                {
                    foreach (var swi in swiList)
                    {
                        Console.WriteLine($"    -> SWI: {swi.Name} on {swi.System} (UUID: {swi.Uuid})");
                    }
                }
                else
                {
                    Console.WriteLine("    -> No defined software instances found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ISoftwareManagementService error: {ex.Message}");
            }

            try
            {
                var workflowService = serviceProvider.GetRequiredService<IWorkflowService>();
                Console.WriteLine("\n[16] Workflow Service Example...");
                var workflows = await workflowService.ListWorkflowsAsync(owner: username);
                if (workflows != null && workflows.Workflows != null && workflows.Workflows.Count > 0)
                {
                    foreach (var workflow in workflows.Workflows)
                    {
                        Console.WriteLine($"    -> Workflow: {workflow.WorkflowName} (Key: {workflow.WorkflowKey}, Status: {workflow.StatusName})");
                    }
                }
                else
                {
                    Console.WriteLine("    -> No workflows found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] IWorkflowService error: {ex.Message}");
            }

            Console.WriteLine("\nExecution completed.");
        }
    }
}
