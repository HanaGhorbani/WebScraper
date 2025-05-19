//using Microsoft.EntityFrameworkCore;
//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.Json;
//using System.Text.Json.Nodes;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Web_Scraper.Models;

//namespace Web_Scraper.Services
//{
//    public class ScraperService
//    {
//        private readonly SanayContext _context;
//        private readonly RestClient _client;

//        public ScraperService(SanayContext context)
//        {
//            _context = context;
//            var options = new RestClientOptions("https://fund.fipiran.ir")
//            {
//                MaxTimeout = -1,
//                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36",
//                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
//            };
//            _client = new RestClient(options);
//        }

//        public async Task ScrapeFundsAsync()
//        {
//            Console.WriteLine("Starting fund scraping...");

//            // Scrape Fund Types
//            await ScrapeFundTypesAsync();

//            // Scrape Average Returns
//            await ScrapeAverageReturnsAsync();

//            // Scrape Funds (reg_no and small_symbol_name from fundcompare)
//            var fundCompareData = await GetApiDataAsync("/api/v1/fund/fundcompare");
//            var fundItems = fundCompareData["items"]?.AsArray();
//            if (fundItems == null)
//            {
//                Console.WriteLine("No funds found in fundcompare API.");
//                return;
//            }

//            foreach (var fundItem in fundItems)
//            {
//                string regNo = fundItem["regNo"]?.ToString();
//                if (string.IsNullOrEmpty(regNo)) continue;

//                // Process each fund
//                await ProcessFundAsync(fundItem);
//            }

//            Console.WriteLine("Fund scraping completed.");
//        }

//        private async Task ScrapeFundTypesAsync()
//        {
//            Console.WriteLine("Scraping fund types...");
//            var fundTypeData = await GetApiDataAsync("/api/v1/fund/fundtype");
//            var fundTypes = fundTypeData["items"]?.AsArray();
//            if (fundTypes == null) return;

//            foreach (var ft in fundTypes)
//            {
//                var fundType = new FundType
//                {
//                    FundType1 = ft["fundType"]?.GetValue<int>() ?? 0,
//                    Name = ft["name"]?.ToString(),
//                    IsActive = ft["isActive"]?.GetValue<bool>() == true ? 1 : 0
//                };

//                var existing = await _context.FundTypes
//                    .FirstOrDefaultAsync(f => f.FundType1 == fundType.FundType1);
//                if (existing == null)
//                {
//                    _context.FundTypes.Add(fundType);
//                }
//                else
//                {
//                    existing.Name = fundType.Name;
//                    existing.IsActive = fundType.IsActive;
//                }
//            }
//            await _context.SaveChangesAsync();
//            Console.WriteLine("Fund types saved.");
//        }

//        private async Task ScrapeAverageReturnsAsync()
//        {
//            Console.WriteLine("Scraping average returns...");
//            var averageReturnsData = await GetApiDataAsync("/api/v1/fund/averagereturns");
//            var returnsItems = averageReturnsData?.AsArray();
//            if (returnsItems == null)
//            {
//                Console.WriteLine("No data found in averagereturns API.");
//                return;
//            }

//            foreach (var item in returnsItems)
//            {
//                int? fundTypeId = item["fundTypeId"]?.GetValue<int>();
//                if (fundTypeId == null) continue;

//                // Find the corresponding FundType ID
//                var fundType = await _context.FundTypes
//                    .FirstOrDefaultAsync(ft => ft.FundType1 == fundTypeId);
//                if (fundType == null)
//                {
//                    Console.WriteLine($"FundType with fundTypeId {fundTypeId} not found in FundTypes table.");
//                    continue;
//                }

//                var averageReturn = new AverageReturn
//                {
//                    FundTypeId = fundType.Id,
//                    NetAsset = item["netAsset"]?.GetValue<decimal>(),
//                    Stock = item["stock"]?.GetValue<float>(),
//                    Bond = item["bond"]?.GetValue<float>(),
//                    Cash = item["cash"]?.GetValue<float>(),
//                    Deposit = item["deposit"]?.GetValue<float>(),
//                    DailyEfficiency = item["dailyEfficiency"]?.GetValue<float>(),
//                    WeeklyEfficiency = item["weeklyEfficiency"]?.GetValue<float>(),
//                    MonthlyEfficiency = item["monthlyEfficiency"]?.GetValue<float>(),
//                    QuarterlyEfficiency = item["quarterlyEfficiency"]?.GetValue<float>(),
//                    SixMonthEfficiency = item["sixMonthEfficiency"]?.GetValue<float>(),
//                    AnnualEfficiency = item["annualEfficiency"]?.GetValue<float>(),
//                    Efficiency = item["efficiency"]?.GetValue<float>()
//                };

//                var existing = await _context.AverageReturns
//                    .FirstOrDefaultAsync(ar => ar.FundTypeId == fundType.Id);
//                if (existing == null)
//                {
//                    _context.AverageReturns.Add(averageReturn);
//                }
//                else
//                {
//                    existing.FundTypeId = averageReturn.FundTypeId;
//                    existing.NetAsset = averageReturn.NetAsset;
//                    existing.Stock = averageReturn.Stock;
//                    existing.Bond = averageReturn.Bond;
//                    existing.Cash = averageReturn.Cash;
//                    existing.Deposit = averageReturn.Deposit;
//                    existing.DailyEfficiency = averageReturn.DailyEfficiency;
//                    existing.WeeklyEfficiency = averageReturn.WeeklyEfficiency;
//                    existing.MonthlyEfficiency = averageReturn.MonthlyEfficiency;
//                    existing.QuarterlyEfficiency = averageReturn.QuarterlyEfficiency;
//                    existing.SixMonthEfficiency = averageReturn.SixMonthEfficiency;
//                    existing.AnnualEfficiency = averageReturn.AnnualEfficiency;
//                    existing.Efficiency = averageReturn.Efficiency;
//                }
//            }
//            await _context.SaveChangesAsync();
//            Console.WriteLine("Average returns saved.");
//        }

//        private async Task ProcessFundAsync(JsonNode fundItem)
//        {
//            string regNo = fundItem["regNo"]?.ToString();

//            // Fetch detailed fund data
//            var fundData = await GetApiDataAsync($"/api/v1/fund/getfund?regno={regNo}");
//            var fundDetail = fundData["item"];
//            if (fundDetail == null)
//            {
//                Console.WriteLine($"No detailed data for fund: {regNo}");
//                return;
//            }

//            int? fundTypeValue = fundDetail["fundType"] != null ? fundDetail["fundType"].GetValue<int>() : null;

//            // Map fund data
//            var fund = new Fund
//            {
//                RegNo = int.Parse(regNo),
//                Name = fundDetail["name"]?.ToString(),
//                SmallSymbolName = fundItem["smallSymbolName"]?.ToString(),
//                FundTypeId = await _context.FundTypes
//                    .Where(ft => ft.FundType1 == fundTypeValue)
//                    .Select(ft => ft.Id)
//                    .FirstOrDefaultAsync(),
//                InitiationDate = fundDetail["initiationDate"]?.ToString(),
//                Manager = fundDetail["manager"]?.ToString(),
//                Auditor = fundDetail["auditor"]?.ToString(),
//                Custodian = fundDetail["custodian"]?.ToString(),
//                Guarantor = fundDetail["guarantor"]?.ToString(),
//                WebsiteAddress = fundDetail["websiteAddress"]?.AsArray()?.FirstOrDefault()?.ToString(),
//                IsCompleted = fundDetail["isCompleted"]?.GetValue<bool>() == true ? 1 : 0,
//                InsCode = fundDetail["insCode"] != null && decimal.TryParse(fundDetail["insCode"].ToString(), out var insCode) ? insCode : 0,
//                ManagerSeoRegisterNo = fundDetail["managerSeoRegisterNo"]?.ToString(),
//                GuarantorSeoRegisterNo = fundDetail["guarantorSeoRegisterNo"]?.ToString(),
//                FundPublisher = fundDetail["fundPublisher"]?.GetValue<int>(),
//                TypeOfInvest = fundDetail["typeOfInvest"]?.ToString(),
//                ArticlesOfAssociationLink = fundDetail["articlesOfAssociationLink"]?.ToString(),
//                ProspectusLink = fundDetail["prosoectusLink"]?.ToString(),
//                SeoRegisterDate = fundDetail["seoRegisterDate"]?.ToString(),
//                RegistrationNumber = fundDetail["registrationNumber"]?.ToString(),
//                RegisterDate = fundDetail["registerDate"]?.ToString(),
//                NationalId = fundDetail["nationalId"]?.ToString(),
//                InvestmentManager = fundDetail["investmentManager"]?.ToString(),
//                ExecutiveManager = fundDetail["executiveManager"]?.ToString(),
//                MarketMaker = fundDetail["marketMaker"]?.ToString()
//            };

//            var existingFund = await _context.Funds.FirstOrDefaultAsync(f => f.RegNo == fund.RegNo);
//            if (existingFund == null)
//            {
//                _context.Funds.Add(fund);
//            }
//            else
//            {
//                existingFund.RegNo = fund.RegNo;
//                existingFund.Name = fund.Name;
//                existingFund.SmallSymbolName = fund.SmallSymbolName;
//                existingFund.FundTypeId = fund.FundTypeId;
//                existingFund.InitiationDate = fund.InitiationDate;
//                existingFund.Manager = fund.Manager;
//                existingFund.Auditor = fund.Auditor;
//                existingFund.Custodian = fund.Custodian;
//                existingFund.Guarantor = fund.Guarantor;
//                existingFund.WebsiteAddress = fund.WebsiteAddress;
//                existingFund.IsCompleted = fund.IsCompleted;
//                existingFund.InsCode = fund.InsCode;
//                existingFund.ManagerSeoRegisterNo = fund.ManagerSeoRegisterNo;
//                existingFund.GuarantorSeoRegisterNo = fund.GuarantorSeoRegisterNo;
//                existingFund.FundPublisher = fund.FundPublisher;
//                existingFund.TypeOfInvest = fund.TypeOfInvest;
//                existingFund.ArticlesOfAssociationLink = fund.ArticlesOfAssociationLink;
//                existingFund.ProspectusLink = fund.ProspectusLink;
//                existingFund.SeoRegisterDate = fund.SeoRegisterDate;
//                existingFund.RegistrationNumber = fund.RegistrationNumber;
//                existingFund.RegisterDate = fund.RegisterDate;
//                existingFund.NationalId = fund.NationalId;
//                existingFund.InvestmentManager = fund.InvestmentManager;
//                existingFund.ExecutiveManager = fund.ExecutiveManager;
//                existingFund.MarketMaker = fund.MarketMaker;
//            }
//            await _context.SaveChangesAsync();

//            // Process related tables
//            await ProcessFundMetricsAsync(regNo, fundDetail);
//            await ProcessFundInvestmentAsync(regNo, fundDetail);
//            await ProcessFundRanksAsync(regNo, fundDetail);
//            await ProcessMutualFundLicensesAsync(regNo, fundDetail);
//            await ProcessFundRiskAsync(regNo, fundDetail);
//            await ProcessFundUnitsAsync(regNo, fundDetail);
//            await ProcessFundMonitoringAsync(regNo, fundDetail);
//            await ProcessFundCompositionAsync(regNo, regNo);
//            await ProcessNetAssetsAsync(regNo, regNo);
//            await ProcessNAVComparisonAsync(regNo, regNo);
//            await ProcessFundEfficiencyAsync(regNo, regNo);
//            await ProcessProfitPerUnitAsync(regNo, regNo);
//            await ProcessInstrumentAsync(regNo);
//            await ProcessInstrumentBestLimitsAsync(regNo);
//            await ProcessInstrumentClientTypesAsync(regNo);
//            await ProcessInstrumentTransactionsAsync(regNo);

//            Console.WriteLine($"Processed fund: {regNo}");
//        }

//        private async Task ProcessFundMetricsAsync(string regNo, JsonNode fundDetail)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var metrics = new FundMetric
//            {
//                FundId = fund.Id,
//                Date = fundDetail["date"]?.ToString(),
//                FundSize = fundDetail["fundSize"]?.GetValue<decimal>(),
//                DailyEfficiency = fundDetail["dailyEfficiency"]?.GetValue<float>(),
//                WeeklyEfficiency = fundDetail["weeklyEfficiency"]?.GetValue<float>(),
//                MonthlyEfficiency = fundDetail["monthlyEfficiency"]?.GetValue<float>(),
//                QuarterlyEfficiency = fundDetail["quarterlyEfficiency"]?.GetValue<float>(),
//                SixMonthEfficiency = fundDetail["sixMonthEfficiency"]?.GetValue<float>(),
//                AnnualEfficiency = fundDetail["annualEfficiency"]?.GetValue<float>(),
//                Efficiency = fundDetail["efficiency"]?.GetValue<float>(),
//                StatisticalNav = fundDetail["statisticalNav"]?.GetValue<decimal>(),
//                CancelNav = fundDetail["cancelNav"]?.GetValue<decimal>(),
//                IssueNav = fundDetail["issueNav"]?.GetValue<decimal>(),
//                NetAsset = fundDetail["netAsset"]?.GetValue<decimal>(),
//                InvestedUnits = fundDetail["investedUnits"]?.GetValue<decimal>(),
//                DividendIntervalPeriod = fundDetail["dividendIntervalPeriod"]?.GetValue<int>(),
//                EstimatedEarningRate = fundDetail["estimatedEarningRate"]?.GetValue<float>(),
//                GuaranteedEarningRate = fundDetail["guaranteedEarningRate"]?.GetValue<float>(),
//                LastModificationTime = fundDetail["lastModificationTime"]?.ToString()
//            };

//            var existing = await _context.FundMetrics
//                .FirstOrDefaultAsync(m => m.FundId == fund.Id);
//            if (existing == null)
//            {
//                _context.FundMetrics.Add(metrics);
//            }
//            else
//            {
//                existing.FundId = metrics.FundId;
//                existing.Date = metrics.Date;
//                existing.FundSize = metrics.FundSize;
//                existing.DailyEfficiency = metrics.DailyEfficiency;
//                existing.WeeklyEfficiency = metrics.WeeklyEfficiency;
//                existing.MonthlyEfficiency = metrics.MonthlyEfficiency;
//                existing.QuarterlyEfficiency = metrics.QuarterlyEfficiency;
//                existing.SixMonthEfficiency = metrics.SixMonthEfficiency;
//                existing.AnnualEfficiency = metrics.AnnualEfficiency;
//                existing.Efficiency = metrics.Efficiency;
//                existing.StatisticalNav = metrics.StatisticalNav;
//                existing.CancelNav = metrics.CancelNav;
//                existing.IssueNav = metrics.IssueNav;
//                existing.NetAsset = metrics.NetAsset;
//                existing.InvestedUnits = metrics.InvestedUnits;
//                existing.DividendIntervalPeriod = metrics.DividendIntervalPeriod;
//                existing.EstimatedEarningRate = metrics.EstimatedEarningRate;
//                existing.GuaranteedEarningRate = metrics.GuaranteedEarningRate;
//                existing.LastModificationTime = metrics.LastModificationTime;
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessFundInvestmentAsync(string regNo, JsonNode fundDetail)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var investment = new FundInvestment
//            {
//                FundId = fund.Id,
//                Date = fundDetail["date"]?.ToString(),
//                InsInvNo = fundDetail["insInvNo"]?.GetValue<int>(),
//                InsInvPercent = fundDetail["insInvPercent"]?.GetValue<float>(),
//                RetInvNo = fundDetail["retInvNo"]?.GetValue<int>(),
//                RetInvPercent = fundDetail["retInvPercent"]?.GetValue<float>(),
//                LegalPercent = fundDetail["legalPercent"]?.GetValue<float>(),
//                NaturalPercent = fundDetail["naturalPercent"]?.GetValue<float>(),
//                UnitsRedDay = fundDetail["unitsRedDAY"]?.GetValue<long>(),
//                UnitsRedFromFirst = fundDetail["unitsRedFromFirst"]?.GetValue<long>(),
//                UnitsSubDay = fundDetail["unitsSubDAY"]?.GetValue<long>(),
//                UnitsSubFromFirst = fundDetail["unitsSubFromFirst"]?.GetValue<long>()
//            };

//            var existing = await _context.FundInvestments
//                .FirstOrDefaultAsync(i => i.FundId == fund.Id);
//            if (existing == null)
//            {
//                _context.FundInvestments.Add(investment);
//            }
//            else
//            {
//                existing.FundId = investment.FundId;
//                existing.Date = investment.Date;
//                existing.InsInvNo = investment.InsInvNo;
//                existing.InsInvPercent = investment.InsInvPercent;
//                existing.RetInvNo = investment.RetInvNo;
//                existing.RetInvPercent = investment.RetInvPercent;
//                existing.LegalPercent = investment.LegalPercent;
//                existing.NaturalPercent = investment.NaturalPercent;
//                existing.UnitsRedDay = investment.UnitsRedDay;
//                existing.UnitsRedFromFirst = investment.UnitsRedFromFirst;
//                existing.UnitsSubDay = investment.UnitsSubDay;
//                existing.UnitsSubFromFirst = investment.UnitsSubFromFirst;
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessFundRanksAsync(string regNo, JsonNode fundDetail)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var ranks = new FundRank
//            {
//                FundId = fund.Id,
//                RankLastUpdate = fundDetail["rankLastUpdate"]?.ToString(),
//                RankOf12Month = fundDetail["rankOf12Month"]?.GetValue<float>(),
//                RankOf24Month = fundDetail["rankOf24Month"]?.GetValue<float>(),
//                RankOf36Month = fundDetail["rankOf36Month"]?.GetValue<float>(),
//                RankOf48Month = fundDetail["rankOf48Month"]?.GetValue<float>(),
//                RankOf60Month = fundDetail["rankOf60Month"]?.GetValue<float>()
//            };

//            var existing = await _context.FundRanks
//                .FirstOrDefaultAsync(r => r.FundId == fund.Id);
//            if (existing == null)
//            {
//                _context.FundRanks.Add(ranks);
//            }
//            else
//            {
//                existing.FundId = ranks.FundId;
//                existing.RankLastUpdate = ranks.RankLastUpdate;
//                existing.RankOf12Month = ranks.RankOf12Month;
//                existing.RankOf24Month = ranks.RankOf24Month;
//                existing.RankOf36Month = ranks.RankOf36Month;
//                existing.RankOf48Month = ranks.RankOf48Month;
//                existing.RankOf60Month = ranks.RankOf60Month;
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessMutualFundLicensesAsync(string regNo, JsonNode fundDetail)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var licenses = fundDetail["mutualFundLicenses"]?.AsArray();
//            if (licenses == null) return;

//            foreach (var license in licenses)
//            {
//                var mutualLicense = new MutualFundLicense
//                {
//                    FundId = fund.Id,
//                    IsExpired = license["isExpired"]?.GetValue<bool>() == true ? 1 : 0,
//                    StartDate = license["startDate"]?.ToString(),
//                    ExpireDate = license["expireDate"]?.ToString(),
//                    LicenseNo = license["licenseNo"]?.ToString(),
//                    LicenseStatusId = license["licenseStatusId"]?.GetValue<int>(),
//                    LicenseStatusDescription = license["licenseStatusDescription"]?.ToString(),
//                    LicenseTypeId = license["licenseTypeId"]?.GetValue<int>(),
//                    NewLicenseTypeId = license["newLicenseTypeId"]?.GetValue<int>()
//                };

//                var existing = await _context.MutualFundLicenses
//                    .FirstOrDefaultAsync(l => l.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.MutualFundLicenses.Add(mutualLicense);
//                }
//                else
//                {
//                    existing.FundId = mutualLicense.FundId;
//                    existing.IsExpired = mutualLicense.IsExpired;
//                    existing.StartDate = mutualLicense.StartDate;
//                    existing.ExpireDate = mutualLicense.ExpireDate;
//                    existing.LicenseNo = mutualLicense.LicenseNo;
//                    existing.LicenseStatusId = mutualLicense.LicenseStatusId;
//                    existing.LicenseStatusDescription = mutualLicense.LicenseStatusDescription;
//                    existing.LicenseTypeId = mutualLicense.LicenseTypeId;
//                    existing.NewLicenseTypeId = mutualLicense.NewLicenseTypeId;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessFundRiskAsync(string regNo, JsonNode fundDetail)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var risk = new FundRisk
//            {
//                FundId = fund.Id,
//                Date = fundDetail["date"]?.ToString(),
//                Beta = fundDetail["beta"]?.GetValue<float>(),
//                Alpha = fundDetail["alpha"]?.GetValue<float>()
//            };

//            var existing = await _context.FundRisks
//                .FirstOrDefaultAsync(r => r.FundId == fund.Id);
//            if (existing == null)
//            {
//                _context.FundRisks.Add(risk);
//            }
//            else
//            {
//                existing.FundId = risk.FundId;
//                existing.Date = risk.Date;
//                existing.Beta = risk.Beta;
//                existing.Alpha = risk.Alpha;
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessFundUnitsAsync(string regNo, JsonNode fundDetail)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var units = new FundUnit
//            {
//                FundId = fund.Id,
//                Date = fundDetail["date"]?.ToString(),
//                BaseUnitsSubscriptionNav = fundDetail["baseUnitsSubscriptionNAV"]?.GetValue<decimal>(),
//                BaseUnitsCancelNav = fundDetail["baseUnitsCancelNAV"]?.GetValue<decimal>(),
//                BaseUnitsTotalNetAssetValue = fundDetail["baseUnitsTotalNetAssetValue"]?.GetValue<decimal>(),
//                BaseTotalUnit = fundDetail["baseTotalUnit"]?.GetValue<decimal>(),
//                BaseUnitsTotalSubscription = fundDetail["baseUnitsTotalSubscription"]?.GetValue<decimal>(),
//                BaseUnitsTotalCancel = fundDetail["baseUnitsTotalCancel"]?.GetValue<decimal>(),
//                SuperUnitsSubscriptionNav = fundDetail["superUnitsSubscriptionNAV"]?.GetValue<decimal>(),
//                SuperUnitsCancelNav = fundDetail["superUnitsCancelNAV"]?.GetValue<decimal>(),
//                SuperUnitsTotalNetAssetValue = fundDetail["superUnitsTotalNetAssetValue"]?.GetValue<decimal>(),
//                SuperTotalUnit = fundDetail["superTotalUnit"]?.GetValue<decimal>(),
//                SuperUnitsTotalSubscription = fundDetail["superUnitsTotalSubscription"]?.GetValue<decimal>(),
//                SuperUnitsTotalCancel = fundDetail["superUnitsTotalCancel"]?.GetValue<decimal>()
//            };

//            var existing = await _context.FundUnits
//                .FirstOrDefaultAsync(u => u.FundId == fund.Id);
//            if (existing == null)
//            {
//                _context.FundUnits.Add(units);
//            }
//            else
//            {
//                existing.FundId = units.FundId;
//                existing.Date = units.Date;
//                existing.BaseUnitsSubscriptionNav = units.BaseUnitsSubscriptionNav;
//                existing.BaseUnitsCancelNav = units.BaseUnitsCancelNav;
//                existing.BaseUnitsTotalNetAssetValue = units.BaseUnitsTotalNetAssetValue;
//                existing.BaseTotalUnit = units.BaseTotalUnit;
//                existing.BaseUnitsTotalSubscription = units.BaseUnitsTotalSubscription;
//                existing.BaseUnitsTotalCancel = units.BaseUnitsTotalCancel;
//                existing.SuperUnitsSubscriptionNav = units.SuperUnitsSubscriptionNav;
//                existing.SuperUnitsCancelNav = units.SuperUnitsCancelNav;
//                existing.SuperUnitsTotalNetAssetValue = units.SuperUnitsTotalNetAssetValue;
//                existing.SuperTotalUnit = units.SuperTotalUnit;
//                existing.SuperUnitsTotalSubscription = units.SuperUnitsTotalSubscription;
//                existing.SuperUnitsTotalCancel = units.SuperUnitsTotalCancel;
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessFundMonitoringAsync(string regNo, JsonNode fundDetail)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var monitoring = new FundMonitoring
//            {
//                FundId = fund.Id,
//                FundWatch = fundDetail["fundWatch"]?.ToString()
//            };

//            var existing = await _context.FundMonitorings
//                .FirstOrDefaultAsync(m => m.FundId == fund.Id);
//            if (existing == null)
//            {
//                _context.FundMonitorings.Add(monitoring);
//            }
//            else
//            {
//                existing.FundId = monitoring.FundId;
//                existing.FundWatch = monitoring.FundWatch;
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessFundCompositionAsync(string regNo, string regNoForApi)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/chart/portfoliochart?regno={regNoForApi}&showALL=true");
//            if (data == null)
//            {
//                Console.WriteLine($"No data returned from portfoliochart API for reg_no: {regNo}");
//                return;
//            }

//            JsonArray items;
//            if (data is JsonArray array)
//            {
//                items = array;
//            }
//            else if (data is JsonObject)
//            {
//                var obj = (JsonObject)data;
//                if (obj["items"] is JsonArray nestedArray)
//                {
//                    items = nestedArray;
//                }
//                else
//                {
//                    items = new JsonArray(obj);
//                }
//            }
//            else
//            {
//                Console.WriteLine($"Unexpected data format for portfoliochart API: {data.ToJsonString()}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var comp = new FundComposition
//                {
//                    FundId = fund.Id,
//                    Date = item["date"]?.ToString(),
//                    FiveBest = item["fiveBest"]?.GetValue<float>(),
//                    Stock = item["stock"]?.GetValue<float>(),
//                    Bond = item["bond"]?.GetValue<float>(),
//                    Other = item["other"]?.GetValue<float>(),
//                    Cash = item["cash"]?.GetValue<float>(),
//                    Deposit = item["deposit"]?.GetValue<float>(),
//                    FundUnit = item["fundUnit"]?.GetValue<float>(),
//                    Commodity = item["commodity"]?.GetValue<float>()
//                };

//                var existing = await _context.FundCompositions
//                    .FirstOrDefaultAsync(c => c.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.FundCompositions.Add(comp);
//                }
//                else
//                {
//                    existing.FundId = comp.FundId;
//                    existing.Date = comp.Date;
//                    existing.FiveBest = comp.FiveBest;
//                    existing.Stock = comp.Stock;
//                    existing.Bond = comp.Bond;
//                    existing.Other = comp.Other;
//                    existing.Cash = comp.Cash;
//                    existing.Deposit = comp.Deposit;
//                    existing.FundUnit = comp.FundUnit;
//                    existing.Commodity = comp.Commodity;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessNetAssetsAsync(string regNo, string regNoForApi)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/chart/getfundnetassetchart?regno={regNoForApi}&showAll=true");
//            if (data == null)
//            {
//                Console.WriteLine($"No data returned from getfundnetassestchart API for reg_no: {regNo}");
//                return;
//            }

//            JsonArray items;
//            if (data is JsonArray array)
//            {
//                items = array;
//            }
//            else if (data is JsonObject)
//            {
//                var obj = (JsonObject)data;
//                if (obj["items"] is JsonArray nestedArray)
//                {
//                    items = nestedArray;
//                }
//                else
//                {
//                    items = new JsonArray(obj);
//                }
//            }
//            else
//            {
//                Console.WriteLine($"Unexpected data format for getfundnetassestchart API: {data.ToJsonString()}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var netAsset = new NetAsset
//                {
//                    FundId = fund.Id,
//                    Date = item["date"]?.ToString(),
//                    NetAsset1 = item["netAsset"]?.GetValue<decimal>(),
//                    UnitsSubDay = item["unitsSubDAY"]?.GetValue<decimal>(),
//                    UnitsRedDay = item["unitsRedDAY"]?.GetValue<decimal>()
//                };

//                var existing = await _context.NetAssets
//                    .FirstOrDefaultAsync(n => n.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.NetAssets.Add(netAsset);
//                }
//                else
//                {
//                    existing.FundId = netAsset.FundId;
//                    existing.Date = netAsset.Date;
//                    existing.NetAsset1 = netAsset.NetAsset1;
//                    existing.UnitsSubDay = netAsset.UnitsSubDay;
//                    existing.UnitsRedDay = netAsset.UnitsRedDay;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessNAVComparisonAsync(string regNo, string regNoForApi)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/chart/getfundchart?regno={regNoForApi}&showALL=true");
//            if (data == null)
//            {
//                Console.WriteLine($"No data returned from getfundchart API for reg_no: {regNo}");
//                return;
//            }

//            JsonArray items;
//            if (data is JsonArray array)
//            {
//                items = array;
//            }
//            else if (data is JsonObject)
//            {
//                var obj = (JsonObject)data;
//                if (obj["items"] is JsonArray nestedArray)
//                {
//                    items = nestedArray;
//                }
//                else
//                {
//                    items = new JsonArray(obj);
//                }
//            }
//            else
//            {
//                Console.WriteLine($"Unexpected data format for getfundchart API: {data.ToJsonString()}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var navComp = new NavComparison
//                {
//                    FundId = fund.Id,
//                    Date = item["date"]?.ToString(),
//                    IssueNav = item["issueNav"]?.GetValue<decimal>(),
//                    CancelNav = item["cancelNav"]?.GetValue<decimal>(),
//                    StatisticalNav = item["statisticalNav"]?.GetValue<decimal>()
//                };

//                var existing = await _context.NavComparisons
//                    .FirstOrDefaultAsync(n => n.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.NavComparisons.Add(navComp);
//                }
//                else
//                {
//                    existing.FundId = navComp.FundId;
//                    existing.Date = navComp.Date;
//                    existing.IssueNav = navComp.IssueNav;
//                    existing.CancelNav = navComp.CancelNav;
//                    existing.StatisticalNav = navComp.StatisticalNav;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessFundEfficiencyAsync(string regNo, string regNoForApi)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/chart/fundefficiencychart?regno={regNoForApi}&showALL=true");
//            if (data == null)
//            {
//                Console.WriteLine($"No data returned from fundefficiencychart API for reg_no: {regNo}");
//                return;
//            }

//            JsonArray items;
//            if (data is JsonArray array)
//            {
//                items = array;
//            }
//            else if (data is JsonObject)
//            {
//                var obj = (JsonObject)data;
//                if (obj["items"] is JsonArray nestedArray)
//                {
//                    items = nestedArray;
//                }
//                else
//                {
//                    items = new JsonArray(obj);
//                }
//            }
//            else
//            {
//                Console.WriteLine($"Unexpected data format for fundefficiencychart API: {data.ToJsonString()}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var eff = new FundEfficiency
//                {
//                    FundId = fund.Id,
//                    Date = item["date"]?.ToString(),
//                    DailyEfficiency = item["dailyEfficiency"]?.GetValue<float>(),
//                    WeeklyEfficiency = item["weeklyEfficiency"]?.GetValue<float>(),
//                    MonthlyEfficiency = item["monthlyEfficiency"]?.GetValue<float>(),
//                    QuarterlyEfficiency = item["quarterlyEfficiency"]?.GetValue<float>(),
//                    SixMonthEfficiency = item["sixMonthEfficiency"]?.GetValue<float>(),
//                    AnnualEfficiency = item["annualEfficiency"]?.GetValue<float>()
//                };

//                var existing = await _context.FundEfficiencies
//                    .FirstOrDefaultAsync(e => e.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.FundEfficiencies.Add(eff);
//                }
//                else
//                {
//                    existing.FundId = eff.FundId;
//                    existing.Date = eff.Date;
//                    existing.DailyEfficiency = eff.DailyEfficiency;
//                    existing.WeeklyEfficiency = eff.WeeklyEfficiency;
//                    existing.MonthlyEfficiency = eff.MonthlyEfficiency;
//                    existing.QuarterlyEfficiency = eff.QuarterlyEfficiency;
//                    existing.SixMonthEfficiency = eff.SixMonthEfficiency;
//                    existing.AnnualEfficiency = eff.AnnualEfficiency;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessProfitPerUnitAsync(string regNo, string regNoForApi)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/chart/fundprofitschart?regno={regNoForApi}&showALL=true");
//            if (data == null)
//            {
//                Console.WriteLine($"No data returned from fundprofitschart API for reg_no: {regNo}");
//                return;
//            }

//            JsonArray items;
//            if (data is JsonArray array)
//            {
//                items = array;
//            }
//            else if (data is JsonObject)
//            {
//                var obj = (JsonObject)data;
//                if (obj["items"] is JsonArray nestedArray)
//                {
//                    items = nestedArray;
//                }
//                else
//                {
//                    items = new JsonArray(obj);
//                }
//            }
//            else
//            {
//                Console.WriteLine($"Unexpected data format for fundprofitschart API: {data.ToJsonString()}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var profitUnit = new ProfitPerUnit
//                {
//                    FundId = fund.Id,
//                    ProfitDate = item["profitDate"]?.ToString(),
//                    ProfitValue = item["profitValue"]?.GetValue<long>()
//                };

//                var existing = await _context.ProfitPerUnits
//                    .FirstOrDefaultAsync(p => p.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.ProfitPerUnits.Add(profitUnit);
//                }
//                else
//                {
//                    existing.FundId = profitUnit.FundId;
//                    existing.ProfitDate = profitUnit.ProfitDate;
//                    existing.ProfitValue = profitUnit.ProfitValue;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessInstrumentAsync(string regNo)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
//            var items = data?["item"]?.AsArray();
//            if (items == null)
//            {
//                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var instrumentData = item["instrument"];
//                if (instrumentData == null) continue;

//                var instrument = new Instrument
//                {
//                    FundId = fund.Id,
//                    InsCode = instrumentData["insCode"] != null && decimal.TryParse(instrumentData["insCode"].ToString(), out var insCode) ? insCode : fund.InsCode,
//                    SmallSymbolName = instrumentData["smallSymbolName"]?.ToString(),
//                    SymbolFullName = instrumentData["symbolFullName"]?.ToString(),
//                    IndustryGroupCode = instrumentData["industryGroupCode"] != null && int.TryParse(instrumentData["industryGroupCode"].ToString(), out var industryGroupCode) ? industryGroupCode : 0,
//                    IndustryGroupName = instrumentData["industryGroupName"]?.ToString(),
//                    IndustrySubCode = instrumentData["industrySubCode"] != null && int.TryParse(instrumentData["industrySubCode"].ToString(), out var industrySubCode) ? industrySubCode : 0,
//                    IndustrySubName = instrumentData["industrySubName"]?.ToString(),
//                    SymbolStatus = instrumentData["symbolStatus"]?.ToString() == "A" ? 1 : 0,
//                    Type = instrumentData["type"] != null && int.TryParse(instrumentData["type"].ToString(), out var type) ? type : 0,
//                    MarketCode = instrumentData["marketCode"] != null && int.TryParse(instrumentData["marketCode"].ToString(), out var marketCode) ? marketCode : 0,
//                    StaticThresholdMaxPrice = instrumentData["staticThresholdMaxPrice"]?.GetValue<decimal>(),
//                    StaticThresholdMinPrice = instrumentData["staticThresholdMinPrice"]?.GetValue<decimal>(),
//                    Status = instrumentData["status"] != null && int.TryParse(instrumentData["status"].ToString(), out var status) ? status : 0
//                };

//                var existing = await _context.Instruments
//                    .FirstOrDefaultAsync(i => i.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.Instruments.Add(instrument);
//                }
//                else
//                {
//                    existing.FundId = instrument.FundId;
//                    existing.InsCode = instrument.InsCode;
//                    existing.SmallSymbolName = instrument.SmallSymbolName;
//                    existing.SymbolFullName = instrument.SymbolFullName;
//                    existing.IndustryGroupCode = instrument.IndustryGroupCode;
//                    existing.IndustryGroupName = instrument.IndustryGroupName;
//                    existing.IndustrySubCode = instrument.IndustrySubCode;
//                    existing.IndustrySubName = instrument.IndustrySubName;
//                    existing.SymbolStatus = instrument.SymbolStatus;
//                    existing.Type = instrument.Type;
//                    existing.MarketCode = instrument.MarketCode;
//                    existing.StaticThresholdMaxPrice = instrument.StaticThresholdMaxPrice;
//                    existing.StaticThresholdMinPrice = instrument.StaticThresholdMinPrice;
//                    existing.Status = instrument.Status;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessInstrumentBestLimitsAsync(string regNo)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
//            var items = data?["item"]?.AsArray();
//            if (items == null)
//            {
//                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var bestLimits = item["instrument5BestLimits"]?.AsArray();
//                if (bestLimits == null) continue;

//                foreach (var limit in bestLimits)
//                {
//                    var bestLimit = new InstrumentBestLimit
//                    {
//                        FundId = fund.Id,
//                        InsCode = limit["insCode"] != null && decimal.TryParse(limit["insCode"]?.ToString(), out var insCode) ? insCode : fund.InsCode,
//                        RowNumber = limit["rowNumber"]?.GetValue<int>() ?? 0,
//                        DemandVolume = limit["demandVolume"]?.GetValue<long>(),
//                        NumberRequests = limit["numberRequests"]?.GetValue<int>(),
//                        DemandPrice = limit["demandPrice"]?.GetValue<int>(),
//                        SupplyPrice = limit["supplyPrice"]?.GetValue<int>(),
//                        NumberSupply = limit["numberSupply"]?.GetValue<int>(),
//                        SupplyVolume = limit["supplyVolume"]?.GetValue<long>()
//                    };

//                    var existing = await _context.InstrumentBestLimits
//                        .FirstOrDefaultAsync(bl => bl.FundId == fund.Id && bl.RowNumber == bestLimit.RowNumber);
//                    if (existing == null)
//                    {
//                        _context.InstrumentBestLimits.Add(bestLimit);
//                    }
//                    else
//                    {
//                        existing.FundId = bestLimit.FundId;
//                        existing.InsCode = bestLimit.InsCode;
//                        existing.RowNumber = bestLimit.RowNumber;
//                        existing.DemandVolume = bestLimit.DemandVolume;
//                        existing.NumberRequests = bestLimit.NumberRequests;
//                        existing.DemandPrice = bestLimit.DemandPrice;
//                        existing.SupplyPrice = bestLimit.SupplyPrice;
//                        existing.NumberSupply = bestLimit.NumberSupply;
//                        existing.SupplyVolume = bestLimit.SupplyVolume;
//                    }
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessInstrumentClientTypesAsync(string regNo)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
//            var items = data?["item"]?.AsArray();
//            if (items == null)
//            {
//                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var clientTypes = item["instrumentClientTypes"]?.AsArray();
//                if (clientTypes == null) continue;

//                foreach (var clientType in clientTypes)
//                {
//                    var instrumentClientType = new InstrumentClientType
//                    {
//                        FundId = fund.Id,
//                        InsCode = clientType["insCode"] != null && decimal.TryParse(clientType["insCode"]?.ToString(), out var insCode) ? insCode : fund.InsCode,
//                        NumberIndividualsBuyers = clientType["numberIndividualsBuyers"]?.GetValue<int>(),
//                        NumberNonIndividualBuyers = clientType["numberNonIndividualBuyers"]?.GetValue<int>(),
//                        SumIndividualBuyVolume = clientType["sumIndividualBuyVolume"]?.GetValue<long>(),
//                        SumNonIndividualBuyVolume = clientType["sumNonIndividualBuyVolume"]?.GetValue<long>(),
//                        NumberIndividualsSellers = clientType["numberIndividualsSellers"]?.GetValue<int>(),
//                        NumberNonIndividualSellers = clientType["numberNonIndividualSellers"]?.GetValue<int>(),
//                        SumIndividualSellVolume = clientType["sumIndividualSellVolume"]?.GetValue<long>(),
//                        SumNonIndividualSellVolume = clientType["sumNonIndividualSellVolume"]?.GetValue<long>()
//                    };

//                    var existing = await _context.InstrumentClientTypes
//                        .FirstOrDefaultAsync(ct => ct.FundId == fund.Id);
//                    if (existing == null)
//                    {
//                        _context.InstrumentClientTypes.Add(instrumentClientType);
//                    }
//                    else
//                    {
//                        existing.FundId = instrumentClientType.FundId;
//                        existing.InsCode = instrumentClientType.InsCode;
//                        existing.NumberIndividualsBuyers = instrumentClientType.NumberIndividualsBuyers;
//                        existing.NumberNonIndividualBuyers = instrumentClientType.NumberNonIndividualBuyers;
//                        existing.SumIndividualBuyVolume = instrumentClientType.SumIndividualBuyVolume;
//                        existing.SumNonIndividualBuyVolume = instrumentClientType.SumNonIndividualBuyVolume;
//                        existing.NumberIndividualsSellers = instrumentClientType.NumberIndividualsSellers;
//                        existing.NumberNonIndividualSellers = instrumentClientType.NumberNonIndividualSellers;
//                        existing.SumIndividualSellVolume = instrumentClientType.SumIndividualSellVolume;
//                        existing.SumNonIndividualSellVolume = instrumentClientType.SumNonIndividualSellVolume;
//                    }
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task ProcessInstrumentTransactionsAsync(string regNo)
//        {
//            if (!int.TryParse(regNo, out var regNoInt))
//            {
//                Console.WriteLine($"Invalid reg_no format: {regNo}");
//                return;
//            }

//            var fund = await _context.Funds
//                .FirstOrDefaultAsync(f => f.RegNo == regNoInt);

//            if (fund == null)
//            {
//                Console.WriteLine($"Fund with reg_no {regNo} not found in Funds table.");
//                return;
//            }

//            var data = await GetApiDataAsync($"/api/v1/instrument/getinstrument?insCode={fund.InsCode}");
//            var items = data?["item"]?.AsArray();
//            if (items == null)
//            {
//                Console.WriteLine($"No data returned from getinstrument API for insCode: {fund.InsCode}");
//                return;
//            }

//            foreach (var item in items)
//            {
//                var transactionData = item["instrumentTransaction"];
//                if (transactionData == null) continue;

//                var transaction = new InstrumentTransaction
//                {
//                    FundId = fund.Id,
//                    InsCode = transactionData["insCode"] != null && decimal.TryParse(transactionData["insCode"].ToString(), out var insCode) ? insCode : fund.InsCode,
//                    TransactionDate = transactionData["transactionDate"]?.ToString(),
//                    NumberOfTransactions = transactionData["numberOfTransactions"] != null && decimal.TryParse(transactionData["numberOfTransactions"].ToString(), out var numberOfTransactions) ? numberOfTransactions : 0,
//                    NumberOfVolume = transactionData["numberOfVolume"] != null && decimal.TryParse(transactionData["numberOfVolume"].ToString(), out var numberOfVolume) ? numberOfVolume : 0,
//                    TransactionValue = transactionData["transactionValue"]?.GetValue<decimal>(),
//                    ClosingPrice = transactionData["closingPrice"]?.GetValue<decimal>(),
//                    AdjPriceForward = transactionData["adjPriceForward"] != null && int.TryParse(transactionData["adjPriceForward"].ToString(), out var adjPriceForward) ? adjPriceForward : 0,
//                    AdjPriceBackward = transactionData["adjPriceBackward"] != null && int.TryParse(transactionData["adjPriceBackward"].ToString(), out var adjPriceBackward) ? adjPriceBackward : 0,
//                    LastTransaction = transactionData["lastTransaction"]?.GetValue<decimal>(),
//                    ChangePrice = transactionData["changePrice"]?.GetValue<decimal>(),
//                    PriceMin = transactionData["priceMin"]?.GetValue<decimal>(),
//                    PriceMax = transactionData["priceMax"]?.GetValue<decimal>(),
//                    PriceFirst = transactionData["priceFirst"]?.GetValue<decimal>(),
//                    PriceYesterday = transactionData["priceYesterday"]?.GetValue<decimal>(),
//                    PriceYesterdayBackward = transactionData["priceYesterdayBackward"] != null && int.TryParse(transactionData["priceYesterdayBackward"].ToString(), out var priceYesterdayBackward) ? priceYesterdayBackward : 0,
//                    LastStatus = transactionData["lastStatus"] != null && int.TryParse(transactionData["lastStatus"].ToString(), out var lastStatus) ? lastStatus : 0,
//                    Heven = transactionData["hEven"] != null && int.TryParse(transactionData["hEven"].ToString(), out var heven) ? heven : 0
//                };

//                var existing = await _context.InstrumentTransactions
//                    .FirstOrDefaultAsync(t => t.FundId == fund.Id);
//                if (existing == null)
//                {
//                    _context.InstrumentTransactions.Add(transaction);
//                }
//                else
//                {
//                    existing.FundId = transaction.FundId;
//                    existing.InsCode = transaction.InsCode;
//                    existing.TransactionDate = transaction.TransactionDate;
//                    existing.NumberOfTransactions = transaction.NumberOfTransactions;
//                    existing.NumberOfVolume = transaction.NumberOfVolume;
//                    existing.TransactionValue = transaction.TransactionValue;
//                    existing.ClosingPrice = transaction.ClosingPrice;
//                    existing.AdjPriceForward = transaction.AdjPriceForward;
//                    existing.AdjPriceBackward = transaction.AdjPriceBackward;
//                    existing.LastTransaction = transaction.LastTransaction;
//                    existing.ChangePrice = transaction.ChangePrice;
//                    existing.PriceMin = transaction.PriceMin;
//                    existing.PriceMax = transaction.PriceMax;
//                    existing.PriceFirst = transaction.PriceFirst;
//                    existing.PriceYesterday = transaction.PriceYesterday;
//                    existing.PriceYesterdayBackward = transaction.PriceYesterdayBackward;
//                    existing.LastStatus = transaction.LastStatus;
//                    existing.Heven = transaction.Heven;
//                }
//            }
//            await _context.SaveChangesAsync();
//        }

//        private async Task<JsonNode> GetApiDataAsync(string endpoint)
//        {
//            var request = new RestRequest(endpoint, Method.Get);
//            request.AddHeader("Accept", "application/json, text/plain, */*");
//            request.AddHeader("Accept-Language", "en-US,en;q=0.9,fa-IR;q=0.8,fa;q=0.7");
//            request.AddHeader("Connection", "keep-alive");
//            request.AddHeader("Referer", "https://fund.fipiran.ir/mf/list");
//            request.AddHeader("Sec-Fetch-Dest", "empty");
//            request.AddHeader("Sec-Fetch-Mode", "cors");
//            request.AddHeader("Sec-Fetch-Site", "same-origin");
//            request.AddHeader("sec-ch-ua", "\"Google Chrome\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"");
//            request.AddHeader("sec-ch-ua-mobile", "?0");
//            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");

//            try
//            {
//                Console.WriteLine($"Requesting {endpoint}...");
//                var response = await _client.ExecuteAsync(request);
//                if (!response.IsSuccessful)
//                {
//                    Console.WriteLine($"Error HTTP: {response.StatusCode} - {response.ErrorMessage}");
//                    return null;
//                }

//                return JsonNode.Parse(response.Content);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error requesting {endpoint}: {ex.Message}");
//                return null;
//            }
//        }
//    }
//}