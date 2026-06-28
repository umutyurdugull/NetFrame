using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFrame.Extensions;
using NetFrame.Services;
using System;
using System.Threading.Tasks;

namespace NetFrame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning);
            });

            string zosmfUrl = "https://204.90.115.200:10443";
            string username = "Z88116";
            string password = "GRB53ONI";

            services.AddZosmf(options =>
            {
                options.BaseUrl = zosmfUrl;
                options.Username = username;
                options.Password = password;
                options.AllowInsecureConnections = true;
            });

            var serviceProvider = services.BuildServiceProvider();
            var datasetService = serviceProvider.GetRequiredService<IDatasetService>();

            try
            {
                try
                {
                    Console.WriteLine("Reading ZXP.PUBLIC.JCL(CHKSQL)...");
                    string content = await datasetService.RetrieveDatasetContentAsync("ZXP.PUBLIC.JCL", "CHKSQL");
                    Console.WriteLine("Content of CHKSQL:\n" + content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error CHKSQL: " + ex.Message);
                }
                Console.WriteLine();

                try
                {
                    Console.WriteLine("Reading ZXP.PUBLIC.JCL(DB2BND)...");
                    string content = await datasetService.RetrieveDatasetContentAsync("ZXP.PUBLIC.JCL", "DB2BND");
                    Console.WriteLine("Content of DB2BND:\n" + content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error DB2BND: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Global Error: " + ex.Message);
            }
        }
    }
}
