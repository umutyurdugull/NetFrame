using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetFrame.Models;
using NetFrame.Services;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace NetFrame.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZosmf(this IServiceCollection services, Action<ZosmfConfig> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddHttpClient<IDatasetService, DatasetService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IJobService, JobService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ISystemService, SystemService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IAppLinkingService, AppLinkingService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IExternalGatewayService, ExternalGatewayService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ICloudProvisioningService, CloudProvisioningService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IResourceManagementService, ResourceManagementService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ISoftwareTemplateService, SoftwareTemplateService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ISoftwareInstanceService, SoftwareInstanceService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ISsinService, SsinService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ISoftwareManagementService, SoftwareManagementService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IStorageManagementService, StorageManagementService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ISysplexManagementService, SysplexManagementService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IWlmResourcePoolingService, WlmResourcePoolingService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IManagementServicesCatalogService, ManagementServicesCatalogService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IWorkflowService, WorkflowService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<ITsoService, TsoService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IUSSSService, USSService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IConsoleService, ConsoleService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            services.AddHttpClient<IRmfMeteringService, RmfMeteringService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                ConfigureHttpClient(client, config);
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config);
            });

            return services;
        }

        private static void ConfigureHttpClient(HttpClient client, ZosmfConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.BaseUrl))
            {
                throw new InvalidOperationException("ZosmfConfig.BaseUrl is required.");
            }

            client.BaseAddress = new Uri(config.BaseUrl);
            
            var authInfo = $"{config.Username}:{config.Password}";
            var byteArray = Encoding.ASCII.GetBytes(authInfo);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        private static HttpClientHandler CreateHandler(ZosmfConfig config)
        {
            var handler = new HttpClientHandler();
            if (config.AllowInsecureConnections)
            {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }
            return handler;
        }
    }
}
