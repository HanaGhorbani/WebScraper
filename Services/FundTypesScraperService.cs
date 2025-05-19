using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Web_Scraper.Models;

namespace Web_Scraper.Services
{
    public class FundTypesScraperService : BaseScraperService
    {
        public FundTypesScraperService(SanayContext context) : base(context)
        {
            Console.WriteLine("FundTypesScraperService: Initialized.");
        }

        public async Task ScrapeFundTypesAsync()
        {
            Console.WriteLine("FundTypesScraperService: Starting ScrapeFundTypesAsync...");
            var fundTypeData = await GetApiDataAsync("/api/v1/fund/fundtype");
            if (fundTypeData == null)
            {
                Console.WriteLine("FundTypesScraperService: No data returned from fundtype API.");
                return;
            }

            var fundTypes = fundTypeData["items"]?.AsArray();
            if (fundTypes == null || !fundTypes.Any())
            {
                Console.WriteLine("FundTypesScraperService: No items found in fundtype API response.");
                return;
            }

            Console.WriteLine($"FundTypesScraperService: Found {fundTypes.Count} fund types to process.");
            foreach (var ft in fundTypes)
            {
                Console.WriteLine($"FundTypesScraperService: Processing fund type: {ft["name"]?.ToString()}");
                var fundType = new FundType
                {
                    FundType1 = ft["fundType"]?.GetValue<int>() ?? 0,
                    Name = ft["name"]?.ToString(),
                    IsActive = ft["isActive"]?.GetValue<bool>() == true ? 1 : 0
                };

                var existing = await _context.FundTypes
                    .FirstOrDefaultAsync(f => f.FundType1 == fundType.FundType1);
                if (existing == null)
                {
                    Console.WriteLine($"FundTypesScraperService: Adding new fund type: {fundType.Name}");
                    _context.FundTypes.Add(fundType);
                }
                else
                {
                    Console.WriteLine($"FundTypesScraperService: Updating existing fund type: {fundType.Name}");
                    existing.Name = fundType.Name;
                    existing.IsActive = fundType.IsActive;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("FundTypesScraperService: Fund types saved to database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FundTypesScraperService: Error saving fund types to database: {ex.Message}");
            }
        }

        public async Task ScrapeAverageReturnsAsync()
        {
            Console.WriteLine("FundTypesScraperService: Starting ScrapeAverageReturnsAsync...");
            var averageReturnsData = await GetApiDataAsync("/api/v1/fund/averagereturns");
            if (averageReturnsData == null)
            {
                Console.WriteLine("FundTypesScraperService: No data returned from averagereturns API.");
                return;
            }

            var returnsItems = averageReturnsData.AsArray();
            if (returnsItems == null || !returnsItems.Any())
            {
                Console.WriteLine("FundTypesScraperService: No items found in averagereturns API response.");
                return;
            }

            Console.WriteLine($"FundTypesScraperService: Found {returnsItems.Count} average returns to process.");
            foreach (var item in returnsItems)
            {
                int? fundTypeId = item["fundTypeId"]?.GetValue<int>();
                if (fundTypeId == null)
                {
                    Console.WriteLine("FundTypesScraperService: Skipping item with null fundTypeId.");
                    continue;
                }

                var fundType = await _context.FundTypes
                    .FirstOrDefaultAsync(ft => ft.FundType1 == fundTypeId);
                if (fundType == null)
                {
                    Console.WriteLine($"FundTypesScraperService: FundType with fundTypeId {fundTypeId} not found.");
                    continue;
                }

                Console.WriteLine($"FundTypesScraperService: Processing average return for fundTypeId: {fundTypeId}");
                var averageReturn = new AverageReturn
                {
                    FundTypeId = fundType.Id,
                    NetAsset = item["netAsset"]?.GetValue<decimal>(),
                    Stock = item["stock"]?.GetValue<float>(),
                    Bond = item["bond"]?.GetValue<float>(),
                    Cash = item["cash"]?.GetValue<float>(),
                    Deposit = item["deposit"]?.GetValue<float>(),
                    DailyEfficiency = item["dailyEfficiency"]?.GetValue<float>(),
                    WeeklyEfficiency = item["weeklyEfficiency"]?.GetValue<float>(),
                    MonthlyEfficiency = item["monthlyEfficiency"]?.GetValue<float>(),
                    QuarterlyEfficiency = item["quarterlyEfficiency"]?.GetValue<float>(),
                    SixMonthEfficiency = item["sixMonthEfficiency"]?.GetValue<float>(),
                    AnnualEfficiency = item["annualEfficiency"]?.GetValue<float>(),
                    Efficiency = item["efficiency"]?.GetValue<float>()
                };

                var existing = await _context.AverageReturns
                    .FirstOrDefaultAsync(ar => ar.FundTypeId == fundType.Id);
                if (existing == null)
                {
                    Console.WriteLine($"FundTypesScraperService: Adding new average return for fundTypeId: {fundTypeId}");
                    _context.AverageReturns.Add(averageReturn);
                }
                else
                {
                    Console.WriteLine($"FundTypesScraperService: Updating existing average return for fundTypeId: {fundTypeId}");
                    existing.FundTypeId = averageReturn.FundTypeId;
                    existing.NetAsset = averageReturn.NetAsset;
                    existing.Stock = averageReturn.Stock;
                    existing.Bond = averageReturn.Bond;
                    existing.Cash = averageReturn.Cash;
                    existing.Deposit = averageReturn.Deposit;
                    existing.DailyEfficiency = averageReturn.DailyEfficiency;
                    existing.WeeklyEfficiency = averageReturn.WeeklyEfficiency;
                    existing.MonthlyEfficiency = averageReturn.MonthlyEfficiency;
                    existing.QuarterlyEfficiency = averageReturn.QuarterlyEfficiency;
                    existing.SixMonthEfficiency = averageReturn.SixMonthEfficiency;
                    existing.AnnualEfficiency = averageReturn.AnnualEfficiency;
                    existing.Efficiency = averageReturn.Efficiency;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("FundTypesScraperService: Average returns saved to database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FundTypesScraperService: Error saving average returns to database: {ex.Message}");
            }
        }
    }
}