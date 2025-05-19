using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Web_Scraper.Jobs;
using Web_Scraper.Models;

namespace Web_Scraper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Program: Starting application...");
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Console.WriteLine("Program: Configuring services...");
                    services.AddDbContext<SanayContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));
                    Console.WriteLine("Program: SanayContext registered.");

                    QuartzJobsSetup.ConfigureQuartz(services, hostContext.Configuration).GetAwaiter().GetResult();
                    Console.WriteLine("Program: Quartz services configured.");
                });

            var host = builder.Build();
            Console.WriteLine("Program: Host built. Starting run...");
            await host.RunAsync();
            Console.WriteLine("Program: Application stopped.");
        }
    }
}