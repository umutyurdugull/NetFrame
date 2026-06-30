using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetFrame.Models;
using NetFrame.Services;
using System;
using System.Net.Http;

namespace NetFrame.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZosmf(this IServiceCollection services, Action<ZosmfConfig> configureOptions)
        {
            services.Configure(configureOptions);

            // Register delegating handlers
            services.AddTransient<ZosmfAuthHandler>();
            services.AddTransient<ZosmfHeaderHandler>();
            services.AddTransient<ZosmfErrorHandler>();

            // Register Db2 Token cache
            services.AddSingleton<IDb2TokenStore, Db2TokenStore>();

            // Register all services using the generic helper
            services.AddZosmfServiceClient<IDatasetService, DatasetService>();
            services.AddZosmfServiceClient<IJobService, JobService>();
            services.AddZosmfServiceClient<ISystemService, SystemService>();
            services.AddZosmfServiceClient<IAppLinkingService, AppLinkingService>();
            services.AddZosmfServiceClient<IExternalGatewayService, ExternalGatewayService>();
            services.AddZosmfServiceClient<ICloudProvisioningService, CloudProvisioningService>();
            services.AddZosmfServiceClient<IResourceManagementService, ResourceManagementService>();
            services.AddZosmfServiceClient<ISoftwareTemplateService, SoftwareTemplateService>();
            services.AddZosmfServiceClient<ISoftwareInstanceService, SoftwareInstanceService>();
            services.AddZosmfServiceClient<ISsinService, SsinService>();
            services.AddZosmfServiceClient<ISoftwareManagementService, SoftwareManagementService>();
            services.AddZosmfServiceClient<IStorageManagementService, StorageManagementService>();
            services.AddZosmfServiceClient<ISysplexManagementService, SysplexManagementService>();
            services.AddZosmfServiceClient<IWlmResourcePoolingService, WlmResourcePoolingService>();
            services.AddZosmfServiceClient<IManagementServicesCatalogService, ManagementServicesCatalogService>();
            services.AddZosmfServiceClient<IWorkflowService, WorkflowService>();
            services.AddZosmfServiceClient<ITsoService, TsoService>();
            services.AddZosmfServiceClient<IUSSSService, USSService>();
            services.AddZosmfServiceClient<IConsoleService, ConsoleService>();
            services.AddZosmfServiceClient<IRmfMeteringService, RmfMeteringService>();

            // Register Db2 REST service configuration & service separately as it has dynamic connection rules
            services.AddHttpClient<IDb2RestService, Db2RestService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<Db2Config>>().Value;
                if (!string.IsNullOrWhiteSpace(config.BaseUrl))
                {
                    client.BaseAddress = new Uri(EnsureAbsoluteUriScheme(config.BaseUrl));
                }
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<Db2Config>>().Value;
                var handler = new HttpClientHandler();
                if (config.AllowInsecureConnections)
                {
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }
                return handler;
            });

            return services;
        }

        private static IHttpClientBuilder AddZosmfServiceClient<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var builder = services.AddHttpClient<TInterface, TImplementation>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                if (string.IsNullOrWhiteSpace(config.BaseUrl))
                {
                    throw new InvalidOperationException("ZosmfConfig.BaseUrl is required.");
                }

                client.BaseAddress = new Uri(EnsureAbsoluteUriScheme(config.BaseUrl));
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
            })
            .AddHttpMessageHandler<ZosmfAuthHandler>()
            .AddHttpMessageHandler<ZosmfHeaderHandler>()
            .AddHttpMessageHandler<ZosmfErrorHandler>()
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ZosmfConfig>>().Value;
                return CreateHandler(config, sp.GetRequiredService<ILogger<HttpClientHandler>>());
            });

            builder.AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);
            });

            return builder;
        }

        private static HttpClientHandler CreateHandler(ZosmfConfig config, ILogger logger)
        {
            var handler = new HttpClientHandler();
            if (config.AllowInsecureConnections)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (errors == System.Net.Security.SslPolicyErrors.None)
                    {
                        return true;
                    }

                    if (!string.IsNullOrEmpty(config.TrustedCertificateThumbprint))
                    {
                        if (cert == null) return false;
                        string thumbprint = cert.Thumbprint.Replace(":", "").Replace(" ", "").ToUpper();
                        string configThumbprint = config.TrustedCertificateThumbprint.Replace(":", "").Replace(" ", "").ToUpper();
                        if (thumbprint == configThumbprint)
                        {
                            return true;
                        }

                        logger.LogWarning("TLS validation failed. Thumbprint '{Thumbprint}' did not match expected config value '{Expected}'. Rejecting connection.", thumbprint, configThumbprint);
                        return false;
                    }

                    logger.LogWarning("Insecure TLS bypass active. Acceptable certificate validation failed for: {Subject}", cert?.Subject);
                    return true;
                };
            }
            return handler;
        }

        private static string EnsureAbsoluteUriScheme(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return "https://" + url;
            }
            return url;
        }
    }
}
