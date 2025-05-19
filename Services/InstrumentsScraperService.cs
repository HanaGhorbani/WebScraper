using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Web_Scraper.Models;

namespace Web_Scraper.Services
{
    public class InstrumentsScraperService : BaseScraperService
    {
        public InstrumentsScraperService(SanayContext context) : base(context)
        {
        }

        public async Task ScrapeInstrumentsAsync()
        {
            Console.WriteLine("Starting instrument scraping...");

            var insCodes = await _context.Funds
                .Where(f => f.InsCode != 0)
                .Select(f => new { f.RegNo, f.InsCode })
                .ToListAsync();

            if (!insCodes.Any())
            {
                Console.WriteLine("No non-zero ins_codes found in Funds table.");
                return;
            }

            foreach (var fund in insCodes)
            {
                string regNo = fund.RegNo.ToString();
                Console.WriteLine($"Processing instrument for reg_no: {regNo}, ins_code: {fund.InsCode}");
                await ProcessInstrumentAsync(regNo);
                await ProcessInstrumentBestLimitsAsync(regNo);
                await ProcessInstrumentClientTypesAsync(regNo);
                await ProcessInstrumentTransactionsAsync(regNo);
            }

            Console.WriteLine("Instrument scraping completed.");
        }

        private async Task ProcessInstrumentAsync(string regNo)
        {
            if (!int.TryParse(regNo, out var regNoInt))
            {
                Console.WriteLine($"Invalid reg_no format: {regNo}");
                return;
            }

            var fund = await _context.Funds
                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

            if (fund == null)
            {
                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
                return;
            }

            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
            if (data == null)
            {
                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
                return;
            }

            var item = data["item"];
            if (item == null)
            {
                Console.WriteLine($"No 'item' found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            // فرض می‌کنیم item یه شیء JSON تکیه، نه آرایه
            var instrumentData = item["instrument"];
            if (instrumentData == null)
            {
                Console.WriteLine($"No 'instrument' data found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            var instrument = new Instrument
            {
                FundId = fund.Id,
                InsCode = instrumentData["insCode"] != null && decimal.TryParse(instrumentData["insCode"].ToString(), out var insCode) ? insCode : fund.InsCode,
                SmallSymbolName = instrumentData["smallSymbolName"]?.ToString(),
                SymbolFullName = instrumentData["symbolFullName"]?.ToString(),
                IndustryGroupCode = instrumentData["industryGroupCode"] != null && int.TryParse(instrumentData["industryGroupCode"].ToString(), out var industryGroupCode) ? industryGroupCode : 0,
                IndustryGroupName = instrumentData["industryGroupName"]?.ToString(),
                IndustrySubCode = instrumentData["industrySubCode"] != null && int.TryParse(instrumentData["industrySubCode"].ToString(), out var industrySubCode) ? industrySubCode : 0,
                IndustrySubName = instrumentData["industrySubName"]?.ToString(),
                SymbolStatus = instrumentData["symbolStatus"]?.ToString() == "A" ? 1 : 0,
                Type = instrumentData["type"] != null && int.TryParse(instrumentData["type"].ToString(), out var type) ? type : 0,
                MarketCode = instrumentData["marketCode"] != null && int.TryParse(instrumentData["marketCode"].ToString(), out var marketCode) ? marketCode : 0,
                StaticThresholdMaxPrice = instrumentData["staticThresholdMaxPrice"]?.GetValue<decimal>(),
                StaticThresholdMinPrice = instrumentData["staticThresholdMinPrice"]?.GetValue<decimal>(),
                Status = instrumentData["status"] != null && int.TryParse(instrumentData["status"].ToString(), out var status) ? status : 0
            };

            var existing = await _context.Instruments
                .FirstOrDefaultAsync(i => i.FundId == fund.Id);
            if (existing == null)
            {
                _context.Instruments.Add(instrument);
            }
            else
            {
                existing.FundId = instrument.FundId;
                existing.InsCode = instrument.InsCode;
                existing.SmallSymbolName = instrument.SmallSymbolName;
                existing.SymbolFullName = instrument.SymbolFullName;
                existing.IndustryGroupCode = instrument.IndustryGroupCode;
                existing.IndustryGroupName = instrument.IndustryGroupName;
                existing.IndustrySubCode = instrument.IndustrySubCode;
                existing.IndustrySubName = instrument.IndustrySubName;
                existing.SymbolStatus = instrument.SymbolStatus;
                existing.Type = instrument.Type;
                existing.MarketCode = instrument.MarketCode;
                existing.StaticThresholdMaxPrice = instrument.StaticThresholdMaxPrice;
                existing.StaticThresholdMinPrice = instrument.StaticThresholdMinPrice;
                existing.Status = instrument.Status;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Instrument processed for reg_no: {regNo}");
        }

        private async Task ProcessInstrumentBestLimitsAsync(string regNo)
        {
            if (!int.TryParse(regNo, out var regNoInt))
            {
                Console.WriteLine($"Invalid reg_no format: {regNo}");
                return;
            }

            var fund = await _context.Funds
                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

            if (fund == null)
            {
                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
                return;
            }

            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
            if (data == null)
            {
                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
                return;
            }

            var item = data["item"];
            if (item == null)
            {
                Console.WriteLine($"No 'item' found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            var bestLimits = item["instrument5BestLimits"]?.AsArray();
            if (bestLimits == null || !bestLimits.Any())
            {
                Console.WriteLine($"No 'instrument5BestLimits' found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            foreach (var limit in bestLimits)
            {
                var bestLimit = new InstrumentBestLimit
                {
                    FundId = fund.Id,
                    InsCode = limit["insCode"] != null && decimal.TryParse(limit["insCode"]?.ToString(), out var insCode) ? insCode : fund.InsCode,
                    RowNumber = limit["rowNumber"]?.GetValue<int>() ?? 0,
                    DemandVolume = limit["demandVolume"]?.GetValue<long>(),
                    NumberRequests = limit["numberRequests"]?.GetValue<int>(),
                    DemandPrice = limit["demandPrice"]?.GetValue<int>(),
                    SupplyPrice = limit["supplyPrice"]?.GetValue<int>(),
                    NumberSupply = limit["numberSupply"]?.GetValue<int>(),
                    SupplyVolume = limit["supplyVolume"]?.GetValue<long>()
                };

                var existing = await _context.InstrumentBestLimits
                    .FirstOrDefaultAsync(bl => bl.FundId == fund.Id && bl.RowNumber == bestLimit.RowNumber);
                if (existing == null)
                {
                    _context.InstrumentBestLimits.Add(bestLimit);
                }
                else
                {
                    existing.FundId = bestLimit.FundId;
                    existing.InsCode = bestLimit.InsCode;
                    existing.RowNumber = bestLimit.RowNumber;
                    existing.DemandVolume = bestLimit.DemandVolume;
                    existing.NumberRequests = bestLimit.NumberRequests;
                    existing.DemandPrice = bestLimit.DemandPrice;
                    existing.SupplyPrice = bestLimit.SupplyPrice;
                    existing.NumberSupply = bestLimit.NumberSupply;
                    existing.SupplyVolume = bestLimit.SupplyVolume;
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"InstrumentBestLimits processed for reg_no: {regNo}");
        }

        private async Task ProcessInstrumentClientTypesAsync(string regNo)
        {
            if (!int.TryParse(regNo, out var regNoInt))
            {
                Console.WriteLine($"Invalid reg_no format: {regNo}");
                return;
            }

            var fund = await _context.Funds
                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

            if (fund == null)
            {
                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
                return;
            }

            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
            if (data == null)
            {
                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
                return;
            }

            var item = data["item"];
            if (item == null)
            {
                Console.WriteLine($"No 'item' found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            var clientTypes = item["instrumentClientTypes"]?.AsArray();
            if (clientTypes == null || !clientTypes.Any())
            {
                Console.WriteLine($"No 'instrumentClientTypes' found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            foreach (var clientType in clientTypes)
            {
                var instrumentClientType = new InstrumentClientType
                {
                    FundId = fund.Id,
                    InsCode = clientType["insCode"] != null && decimal.TryParse(clientType["insCode"]?.ToString(), out var insCode) ? insCode : fund.InsCode,
                    NumberIndividualsBuyers = clientType["numberIndividualsBuyers"]?.GetValue<int>(),
                    NumberNonIndividualBuyers = clientType["numberNonIndividualBuyers"]?.GetValue<int>(),
                    SumIndividualBuyVolume = clientType["sumIndividualBuyVolume"]?.GetValue<long>(),
                    SumNonIndividualBuyVolume = clientType["sumNonIndividualBuyVolume"]?.GetValue<long>(),
                    NumberIndividualsSellers = clientType["numberIndividualsSellers"]?.GetValue<int>(),
                    NumberNonIndividualSellers = clientType["numberNonIndividualSellers"]?.GetValue<int>(),
                    SumIndividualSellVolume = clientType["sumIndividualSellVolume"]?.GetValue<long>(),
                    SumNonIndividualSellVolume = clientType["sumNonIndividualSellVolume"]?.GetValue<long>()
                };

                var existing = await _context.InstrumentClientTypes
                    .FirstOrDefaultAsync(ct => ct.FundId == fund.Id);
                if (existing == null)
                {
                    _context.InstrumentClientTypes.Add(instrumentClientType);
                }
                else
                {
                    existing.FundId = instrumentClientType.FundId;
                    existing.InsCode = instrumentClientType.InsCode;
                    existing.NumberIndividualsBuyers = instrumentClientType.NumberIndividualsBuyers;
                    existing.NumberNonIndividualBuyers = instrumentClientType.NumberNonIndividualBuyers;
                    existing.SumIndividualBuyVolume = instrumentClientType.SumIndividualBuyVolume;
                    existing.SumNonIndividualBuyVolume = instrumentClientType.SumNonIndividualBuyVolume;
                    existing.NumberIndividualsSellers = instrumentClientType.NumberIndividualsSellers;
                    existing.NumberNonIndividualSellers = instrumentClientType.NumberNonIndividualSellers;
                    existing.SumIndividualSellVolume = instrumentClientType.SumIndividualSellVolume;
                    existing.SumNonIndividualSellVolume = instrumentClientType.SumNonIndividualSellVolume;
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"InstrumentClientTypes processed for reg_no: {regNo}");
        }

        private async Task ProcessInstrumentTransactionsAsync(string regNo)
        {
            if (!int.TryParse(regNo, out var regNoInt))
            {
                Console.WriteLine($"Invalid reg_no format: {regNo}");
                return;
            }

            var fund = await _context.Funds
                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

            if (fund == null)
            {
                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
                return;
            }

            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
            if (data == null)
            {
                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
                return;
            }

            var item = data["item"];
            if (item == null)
            {
                Console.WriteLine($"No 'item' found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            var transactionData = item["instrumentTransaction"];
            if (transactionData == null)
            {
                Console.WriteLine($"No 'instrumentTransaction' found in getinstrument API response for insCode: {fund.InsCode}");
                return;
            }

            var transaction = new InstrumentTransaction
            {
                FundId = fund.Id,
                InsCode = transactionData["insCode"] != null && decimal.TryParse(transactionData["insCode"].ToString(), out var insCode) ? insCode : fund.InsCode,
                TransactionDate = transactionData["transactionDate"]?.ToString(),
                NumberOfTransactions = transactionData["numberOfTransactions"] != null && decimal.TryParse(transactionData["numberOfTransactions"].ToString(), out var numberOfTransactions) ? numberOfTransactions : 0,
                NumberOfVolume = transactionData["numberOfVolume"] != null && decimal.TryParse(transactionData["numberOfVolume"].ToString(), out var numberOfVolume) ? numberOfVolume : 0,
                TransactionValue = transactionData["transactionValue"]?.GetValue<decimal>(),
                ClosingPrice = transactionData["closingPrice"]?.GetValue<decimal>(),
                AdjPriceForward = transactionData["adjPriceForward"] != null && int.TryParse(transactionData["adjPriceForward"].ToString(), out var adjPriceForward) ? adjPriceForward : 0,
                AdjPriceBackward = transactionData["adjPriceBackward"] != null && int.TryParse(transactionData["adjPriceBackward"].ToString(), out var adjPriceBackward) ? adjPriceBackward : 0,
                LastTransaction = transactionData["lastTransaction"]?.GetValue<decimal>(),
                ChangePrice = transactionData["changePrice"]?.GetValue<decimal>(),
                PriceMin = transactionData["priceMin"]?.GetValue<decimal>(),
                PriceMax = transactionData["priceMax"]?.GetValue<decimal>(),
                PriceFirst = transactionData["priceFirst"]?.GetValue<decimal>(),
                PriceYesterday = transactionData["priceYesterday"]?.GetValue<decimal>(),
                PriceYesterdayBackward = transactionData["priceYesterdayBackward"] != null && int.TryParse(transactionData["priceYesterdayBackward"].ToString(), out var priceYesterdayBackward) ? priceYesterdayBackward : 0,
                LastStatus = transactionData["lastStatus"] != null && int.TryParse(transactionData["lastStatus"].ToString(), out var lastStatus) ? lastStatus : 0,
                Heven = transactionData["hEven"] != null && int.TryParse(transactionData["hEven"].ToString(), out var heven) ? heven : 0
            };

            var existing = await _context.InstrumentTransactions
                .FirstOrDefaultAsync(t => t.FundId == fund.Id);
            if (existing == null)
            {
                _context.InstrumentTransactions.Add(transaction);
            }
            else
            {
                existing.FundId = transaction.FundId;
                existing.InsCode = transaction.InsCode;
                existing.TransactionDate = transaction.TransactionDate;
                existing.NumberOfTransactions = transaction.NumberOfTransactions;
                existing.NumberOfVolume = transaction.NumberOfVolume;
                existing.TransactionValue = transaction.TransactionValue;
                existing.ClosingPrice = transaction.ClosingPrice;
                existing.AdjPriceForward = transaction.AdjPriceForward;
                existing.AdjPriceBackward = transaction.AdjPriceBackward;
                existing.LastTransaction = transaction.LastTransaction;
                existing.ChangePrice = transaction.ChangePrice;
                existing.PriceMin = transaction.PriceMin;
                existing.PriceMax = transaction.PriceMax;
                existing.PriceFirst = transaction.PriceFirst;
                existing.PriceYesterday = transaction.PriceYesterday;
                existing.PriceYesterdayBackward = transaction.PriceYesterdayBackward;
                existing.LastStatus = transaction.LastStatus;
                existing.Heven = transaction.Heven;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"InstrumentTransactions processed for reg_no: {regNo}");
        }
    }
}