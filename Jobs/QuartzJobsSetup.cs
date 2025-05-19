using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;
using Web_Scraper.Services;

namespace Web_Scraper.Jobs
{
    public static class QuartzJobsSetup
    {
        public static async Task ConfigureQuartz(IServiceCollection services, IConfiguration configuration)
        {
            Console.WriteLine("QuartzJobsSetup: Starting Quartz configuration...");
            services.AddQuartz(q =>
            {
                Console.WriteLine("QuartzJobsSetup: Configuring Quartz scheduler...");
                q.SchedulerId = "WebScraperScheduler";
                q.UseMicrosoftDependencyInjectionJobFactory();

                var fundTypesJobKey = new JobKey("FundTypesJob");
                q.AddJob<FundTypesJob>(opts => opts.WithIdentity(fundTypesJobKey));
                Console.WriteLine("QuartzJobsSetup: FundTypesJob registered.");

                var fundsJobKey = new JobKey("FundsJob");
                q.AddJob<FundsJob>(opts => opts.WithIdentity(fundsJobKey));
                Console.WriteLine("QuartzJobsSetup: FundsJob registered.");

                var instrumentsJobKey = new JobKey("InstrumentsJob");
                q.AddJob<InstrumentsJob>(opts => opts.WithIdentity(instrumentsJobKey));
                Console.WriteLine("QuartzJobsSetup: InstrumentsJob registered.");

                q.AddTrigger(opts => opts
                    .ForJob(fundTypesJobKey)
                    .WithIdentity("FundTypesTrigger")
                    .WithCronSchedule(configuration.GetSection("Quartz:FundTypesSchedule").Value ?? "0 0 1 * * ?"));
                Console.WriteLine("QuartzJobsSetup: FundTypesTrigger configured.");

                q.AddTrigger(opts => opts
                    .ForJob(fundsJobKey)
                    .WithIdentity("FundsTrigger")
                    .WithCronSchedule(configuration.GetSection("Quartz:FundsSchedule").Value ?? "0 5 1 * * ?"));
                Console.WriteLine("QuartzJobsSetup: FundsTrigger configured.");

                q.AddTrigger(opts => opts
                    .ForJob(instrumentsJobKey)
                    .WithIdentity("InstrumentsTrigger")
                    .WithCronSchedule(configuration.GetSection("Quartz:InstrumentsSchedule").Value ?? "0 10 1 * * ?"));
                Console.WriteLine("QuartzJobsSetup: InstrumentsTrigger configured.");
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            Console.WriteLine("QuartzJobsSetup: QuartzHostedService registered.");

            services.AddScoped<FundTypesScraperService>();
            services.AddScoped<FundsScraperService>();
            services.AddScoped<InstrumentsScraperService>();
            Console.WriteLine("QuartzJobsSetup: Scraper services registered.");

            Console.WriteLine("QuartzJobsSetup: Quartz configuration completed.");
        }
    }
}