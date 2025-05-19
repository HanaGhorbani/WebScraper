using Quartz;
using System;
using System.Threading.Tasks;
using Web_Scraper.Services;

namespace Web_Scraper.Jobs
{
    public class InstrumentsJob : IJob
    {
        private readonly InstrumentsScraperService _scraperService;

        public InstrumentsJob(InstrumentsScraperService scraperService)
        {
            _scraperService = scraperService ?? throw new ArgumentNullException(nameof(scraperService));
            Console.WriteLine("InstrumentsJob: Initialized with InstrumentsScraperService.");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"InstrumentsJob: Starting execution at {DateTime.Now:yyyy-MM-dd HH:mm:ss}...");
            try
            {
                await _scraperService.ScrapeInstrumentsAsync();
                Console.WriteLine("InstrumentsJob: Execution completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InstrumentsJob: Error during execution: {ex.Message}");
            }
        }
    }
}