using Quartz;
using System;
using System.Threading.Tasks;
using Web_Scraper.Services;

namespace Web_Scraper.Jobs
{
    public class FundsJob : IJob
    {
        private readonly FundsScraperService _scraperService;

        public FundsJob(FundsScraperService scraperService)
        {
            _scraperService = scraperService ?? throw new ArgumentNullException(nameof(scraperService));
            Console.WriteLine("FundsJob: Initialized with FundsScraperService.");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"FundsJob: Starting execution at {DateTime.Now:yyyy-MM-dd HH:mm:ss}...");
            try
            {
                await _scraperService.ScrapeFundsAsync();
                Console.WriteLine("FundsJob: Execution completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FundsJob: Error during execution: {ex.Message}");
            }
        }
    }
}