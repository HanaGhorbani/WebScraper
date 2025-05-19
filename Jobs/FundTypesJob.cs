using Quartz;
using System;
using System.Threading.Tasks;
using Web_Scraper.Services;

namespace Web_Scraper.Jobs
{
    public class FundTypesJob : IJob
    {
        private readonly FundTypesScraperService _scraperService;

        public FundTypesJob(FundTypesScraperService scraperService)
        {
            _scraperService = scraperService ?? throw new ArgumentNullException(nameof(scraperService));
            Console.WriteLine("FundTypesJob: Initialized with FundTypesScraperService.");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"FundTypesJob: Starting execution at {DateTime.Now:yyyy-MM-dd HH:mm:ss}...");
            try
            {
                await _scraperService.ScrapeFundTypesAsync();
                await _scraperService.ScrapeAverageReturnsAsync();
                Console.WriteLine("FundTypesJob: Execution completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FundTypesJob: Error during execution: {ex.Message}");
            }
        }
    }
}