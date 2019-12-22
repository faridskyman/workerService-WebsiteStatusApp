using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

/// <summary>
/// tutorial from 
/// title: Worker Services in .NET Core 3.0 - The New Way to Create Services
/// channel: IAmTimCorey
/// url: https://www.youtube.com/watch?v=PzrTiz_NRKA
/// video date: Sep 9, 2019
/// description: In .NET Core 3.0, Microsoft has added a template called a Worker Service. This template isn't just for creating Windows Services, but that is a big benefit. In this video, we will look at how to create a worker service, how to configure it, and how to deploy it to a machine.
/// next: https://medium.com/swlh/creating-a-worker-service-in-asp-net-core-3-0-6af5dc780c80
/// </summary>
namespace WebsiteStatusApp
{
    public class Program
    {
        public static void Main(string[] args)
        {


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"c:\temp\workerservice\logfile.txt")
                .CreateLogger();

            try
            {
                Log.Information("starting service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "a problem starting service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    
                    //getting config from appsettings
                    WorkerOptions options = configuration.GetSection("MyWebsite").Get<WorkerOptions>();
                    
                    //by doing this, this is DI, passing this 'param' over. (so we have 2 param injected)
                    services.AddSingleton(options);
                    services.AddHostedService<Worker>();
                })
            .UseSerilog();
    }
}
