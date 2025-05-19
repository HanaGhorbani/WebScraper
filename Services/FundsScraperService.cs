using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Web_Scraper.Models;

namespace Web_Scraper.Services
{
    public class FundsScraperService : BaseScraperService
    {
        public FundsScraperService(SanayContext context) : base(context)
        {
        }

        public async Task ScrapeFundsAsync()
        {
            Console.WriteLine("Starting fund scraping...");

            var fundCompareData = await GetApiDataAsync("/api/v1/fund/fundcompare");
            var fundItems = fundCompareData["items"]?.AsArray();
            if (fundItems == null)
            {
                Console.WriteLine("No funds found in fundcompare API.");
                return;
            }

            foreach (var fundItem in fundItems)
            {
                string regNo = fundItem["regNo"]?.ToString();
                if (string.IsNullOrEmpty(regNo)) continue;

                await ProcessFundAsync(fundItem);
            }

            Console.WriteLine("Fund scraping completed.");
        }

        private async Task ProcessFundAsync(JsonNode fundItem)
        {
            string regNo = fundItem["regNo"]?.ToString();

            var fundData = await GetApiDataAsync($"/api/v1/fund/getfund?regno={regNo}");
            var fundDetail = fundData["item"];
            if (fundDetail == null)
            {
                Console.WriteLine($"No detailed data for fund: {regNo}");
                return;
            }

            int? fundTypeValue = fundDetail["fundType"] != null ? fundDetail["fundType"].GetValue<int>() : null;

            var fund = new Fund
            {
                RegNo = int.Parse(regNo),
                Name = fundDetail["name"]?.ToString(),
                SmallSymbolName = fundItem["smallSymbolName"]?.ToString(),
                FundTypeId = await _context.FundTypes
                    .Where(ft => ft.FundType1 == fundTypeValue)
                    .Select(ft => ft.Id)
                    .FirstOrDefaultAsync(),
                InitiationDate = fundDetail["initiationDate"]?.ToString(),
                Manager = fundDetail["manager"]?.ToString(),
                Auditor = fundDetail["auditor"]?.ToString(),
                Custodian = fundDetail["custodian"]?.ToString(),
                Guarantor = fundDetail["guarantor"]?.ToString(),
                WebsiteAddress = fundDetail["websiteAddress"]?.AsArray()?.FirstOrDefault()?.ToString(),
                IsCompleted = fundDetail["isCompleted"]?.GetValue<bool>() == true ? 1 : 0,
                InsCode = fundDetail["insCode"] != null && decimal.TryParse(fundDetail["insCode"].ToString(), out var insCode) ? insCode : 0,
                ManagerSeoRegisterNo = fundDetail["managerSeoRegisterNo"]?.ToString(),
                GuarantorSeoRegisterNo = fundDetail["guarantorSeoRegisterNo"]?.ToString(),
                FundPublisher = fundDetail["fundPublisher"]?.GetValue<int>(),
                TypeOfInvest = fundDetail["typeOfInvest"]?.ToString(),
                ArticlesOfAssociationLink = fundDetail["articlesOfAssociationLink"]?.ToString(),
                ProspectusLink = fundDetail["prosoectusLink"]?.ToString(),
                SeoRegisterDate = fundDetail["seoRegisterDate"]?.ToString(),
                RegistrationNumber = fundDetail["registrationNumber"]?.ToString(),
                RegisterDate = fundDetail["registerDate"]?.ToString(),
                NationalId = fundDetail["nationalId"]?.ToString(),
                InvestmentManager = fundDetail["investmentManager"]?.ToString(),
                ExecutiveManager = fundDetail["executiveManager"]?.ToString(),
                MarketMaker = fundDetail["marketMaker"]?.ToString()
            };

            var existingFund = await _context.Funds.FirstOrDefaultAsync(f => f.RegNo == fund.RegNo);
            if (existingFund != null)
            {
                Console.WriteLine($"Fund With regNo {regNo} already exists , skipping update.");
                return;
            }
            
            _context.Funds.Add(fund);

            //if (existingFund == null)
            //{
            //    _context.Funds.Add(fund);
            //}
            //else
            //{
            //    existingFund.RegNo = fund.RegNo;
            //    existingFund.Name = fund.Name;
            //    existingFund.SmallSymbolName = fund.SmallSymbolName;
            //    existingFund.FundTypeId = fund.FundTypeId;
            //    existingFund.InitiationDate = fund.InitiationDate;
            //    existingFund.Manager = fund.Manager;
            //    existingFund.Auditor = fund.Auditor;
            //    existingFund.Custodian = fund.Custodian;
            //    existingFund.Guarantor = fund.Guarantor;
            //    existingFund.WebsiteAddress = fund.WebsiteAddress;
            //    existingFund.IsCompleted = fund.IsCompleted;
            //    existingFund.InsCode = fund.InsCode;
            //    existingFund.ManagerSeoRegisterNo = fund.ManagerSeoRegisterNo;
            //    existingFund.GuarantorSeoRegisterNo = fund.GuarantorSeoRegisterNo;
            //    existingFund.FundPublisher = fund.FundPublisher;
            //    existingFund.TypeOfInvest = fund.TypeOfInvest;
            //    existingFund.ArticlesOfAssociationLink = fund.ArticlesOfAssociationLink;
            //    existingFund.ProspectusLink = fund.ProspectusLink;
            //    existingFund.SeoRegisterDate = fund.SeoRegisterDate;
            //    existingFund.RegistrationNumber = fund.RegistrationNumber;
            //    existingFund.RegisterDate = fund.RegisterDate;
            //    existingFund.NationalId = fund.NationalId;
            //    existingFund.InvestmentManager = fund.InvestmentManager;
            //    existingFund.ExecutiveManager = fund.ExecutiveManager;
            //    existingFund.MarketMaker = fund.MarketMaker;
            //}
            await _context.SaveChangesAsync();

            await ProcessFundMetricsAsync(regNo, fundDetail);
            await ProcessFundInvestmentAsync(regNo, fundDetail);
            await ProcessFundRanksAsync(regNo, fundDetail);
            await ProcessMutualFundLicensesAsync(regNo, fundDetail);
            await ProcessFundRiskAsync(regNo, fundDetail);
            await ProcessFundUnitsAsync(regNo, fundDetail);
            await ProcessFundMonitoringAsync(regNo, fundDetail);
            await ProcessFundCompositionAsync(regNo, regNo);
            await ProcessNetAssetsAsync(regNo, regNo);
            await ProcessNAVComparisonAsync(regNo, regNo);
            await ProcessFundEfficiencyAsync(regNo, regNo);
            await ProcessProfitPerUnitAsync(regNo, regNo);

            Console.WriteLine($"Processed fund: {regNo}");
        }

        private async Task ProcessFundMetricsAsync(string regNo, JsonNode fundDetail)
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

            var metrics = new FundMetric
            {
                FundId = fund.Id,
                Date = fundDetail["date"]?.ToString(),
                FundSize = fundDetail["fundSize"]?.GetValue<decimal>(),
                DailyEfficiency = fundDetail["dailyEfficiency"]?.GetValue<float>(),
                WeeklyEfficiency = fundDetail["weeklyEfficiency"]?.GetValue<float>(),
                MonthlyEfficiency = fundDetail["monthlyEfficiency"]?.GetValue<float>(),
                QuarterlyEfficiency = fundDetail["quarterlyEfficiency"]?.GetValue<float>(),
                SixMonthEfficiency = fundDetail["sixMonthEfficiency"]?.GetValue<float>(),
                AnnualEfficiency = fundDetail["annualEfficiency"]?.GetValue<float>(),
                Efficiency = fundDetail["efficiency"]?.GetValue<float>(),
                StatisticalNav = fundDetail["statisticalNav"]?.GetValue<decimal>(),
                CancelNav = fundDetail["cancelNav"]?.GetValue<decimal>(),
                IssueNav = fundDetail["issueNav"]?.GetValue<decimal>(),
                NetAsset = fundDetail["netAsset"]?.GetValue<decimal>(),
                InvestedUnits = fundDetail["investedUnits"]?.GetValue<decimal>(),
                DividendIntervalPeriod = fundDetail["dividendIntervalPeriod"]?.GetValue<int>(),
                EstimatedEarningRate = fundDetail["estimatedEarningRate"]?.GetValue<float>(),
                GuaranteedEarningRate = fundDetail["guaranteedEarningRate"]?.GetValue<float>(),
                LastModificationTime = fundDetail["lastModificationTime"]?.ToString()
            };

            var existing = await _context.FundMetrics
                .FirstOrDefaultAsync(m => m.FundId == fund.Id);
            if (existing == null)
            {
                _context.FundMetrics.Add(metrics);
            }
            else
            {
                existing.FundId = metrics.FundId;
                existing.Date = metrics.Date;
                existing.FundSize = metrics.FundSize;
                existing.DailyEfficiency = metrics.DailyEfficiency;
                existing.WeeklyEfficiency = metrics.WeeklyEfficiency;
                existing.MonthlyEfficiency = metrics.MonthlyEfficiency;
                existing.QuarterlyEfficiency = metrics.QuarterlyEfficiency;
                existing.SixMonthEfficiency = metrics.SixMonthEfficiency;
                existing.AnnualEfficiency = metrics.AnnualEfficiency;
                existing.Efficiency = metrics.Efficiency;
                existing.StatisticalNav = metrics.StatisticalNav;
                existing.CancelNav = metrics.CancelNav;
                existing.IssueNav = metrics.IssueNav;
                existing.NetAsset = metrics.NetAsset;
                existing.InvestedUnits = metrics.InvestedUnits;
                existing.DividendIntervalPeriod = metrics.DividendIntervalPeriod;
                existing.EstimatedEarningRate = metrics.EstimatedEarningRate;
                existing.GuaranteedEarningRate = metrics.GuaranteedEarningRate;
                existing.LastModificationTime = metrics.LastModificationTime;
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessFundInvestmentAsync(string regNo, JsonNode fundDetail)
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

            var investment = new FundInvestment
            {
                FundId = fund.Id,
                Date = fundDetail["date"]?.ToString(),
                InsInvNo = fundDetail["insInvNo"]?.GetValue<int>(),
                InsInvPercent = fundDetail["insInvPercent"]?.GetValue<float>(),
                RetInvNo = fundDetail["retInvNo"]?.GetValue<int>(),
                RetInvPercent = fundDetail["retInvPercent"]?.GetValue<float>(),
                LegalPercent = fundDetail["legalPercent"]?.GetValue<float>(),
                NaturalPercent = fundDetail["naturalPercent"]?.GetValue<float>(),
                UnitsRedDay = fundDetail["unitsRedDAY"]?.GetValue<long>(),
                UnitsRedFromFirst = fundDetail["unitsRedFromFirst"]?.GetValue<long>(),
                UnitsSubDay = fundDetail["unitsSubDAY"]?.GetValue<long>(),
                UnitsSubFromFirst = fundDetail["unitsSubFromFirst"]?.GetValue<long>()
            };

            var existing = await _context.FundInvestments
                .FirstOrDefaultAsync(i => i.FundId == fund.Id);
            if (existing == null)
            {
                _context.FundInvestments.Add(investment);
            }
            else
            {
                existing.FundId = investment.FundId;
                existing.Date = investment.Date;
                existing.InsInvNo = investment.InsInvNo;
                existing.InsInvPercent = investment.InsInvPercent;
                existing.RetInvNo = investment.RetInvNo;
                existing.RetInvPercent = investment.RetInvPercent;
                existing.LegalPercent = investment.LegalPercent;
                existing.NaturalPercent = investment.NaturalPercent;
                existing.UnitsRedDay = investment.UnitsRedDay;
                existing.UnitsRedFromFirst = investment.UnitsRedFromFirst;
                existing.UnitsSubDay = investment.UnitsSubDay;
                existing.UnitsSubFromFirst = investment.UnitsSubFromFirst;
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessFundRanksAsync(string regNo, JsonNode fundDetail)
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

            var ranks = new FundRank
            {
                FundId = fund.Id,
                RankLastUpdate = fundDetail["rankLastUpdate"]?.ToString(),
                RankOf12Month = fundDetail["rankOf12Month"]?.GetValue<float>(),
                RankOf24Month = fundDetail["rankOf24Month"]?.GetValue<float>(),
                RankOf36Month = fundDetail["rankOf36Month"]?.GetValue<float>(),
                RankOf48Month = fundDetail["rankOf48Month"]?.GetValue<float>(),
                RankOf60Month = fundDetail["rankOf60Month"]?.GetValue<float>()
            };

            var existing = await _context.FundRanks
                .FirstOrDefaultAsync(r => r.FundId == fund.Id);
            if (existing == null)
            {
                _context.FundRanks.Add(ranks);
            }
            else
            {
                existing.FundId = ranks.FundId;
                existing.RankLastUpdate = ranks.RankLastUpdate;
                existing.RankOf12Month = ranks.RankOf12Month;
                existing.RankOf24Month = ranks.RankOf24Month;
                existing.RankOf36Month = ranks.RankOf36Month;
                existing.RankOf48Month = ranks.RankOf48Month;
                existing.RankOf60Month = ranks.RankOf60Month;
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessMutualFundLicensesAsync(string regNo, JsonNode fundDetail)
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

            var licenses = fundDetail["mutualFundLicenses"]?.AsArray();
            if (licenses == null) return;

            foreach (var license in licenses)
            {
                var mutualLicense = new MutualFundLicense
                {
                    FundId = fund.Id,
                    IsExpired = license["isExpired"]?.GetValue<bool>() == true ? 1 : 0,
                    StartDate = license["startDate"]?.ToString(),
                    ExpireDate = license["expireDate"]?.ToString(),
                    LicenseNo = license["licenseNo"]?.ToString(),
                    LicenseStatusId = license["licenseStatusId"]?.GetValue<int>(),
                    LicenseStatusDescription = license["licenseStatusDescription"]?.ToString(),
                    LicenseTypeId = license["licenseTypeId"]?.GetValue<int>(),
                    NewLicenseTypeId = license["newLicenseTypeId"]?.GetValue<int>()
                };

                var existing = await _context.MutualFundLicenses
                    .FirstOrDefaultAsync(l => l.FundId == fund.Id);
                if (existing == null)
                {
                    _context.MutualFundLicenses.Add(mutualLicense);
                }
                else
                {
                    existing.FundId = mutualLicense.FundId;
                    existing.IsExpired = mutualLicense.IsExpired;
                    existing.StartDate = mutualLicense.StartDate;
                    existing.ExpireDate = mutualLicense.ExpireDate;
                    existing.LicenseNo = mutualLicense.LicenseNo;
                    existing.LicenseStatusId = mutualLicense.LicenseStatusId;
                    existing.LicenseStatusDescription = mutualLicense.LicenseStatusDescription;
                    existing.LicenseTypeId = mutualLicense.LicenseTypeId;
                    existing.NewLicenseTypeId = mutualLicense.NewLicenseTypeId;
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessFundRiskAsync(string regNo, JsonNode fundDetail)
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

            var risk = new FundRisk
            {
                FundId = fund.Id,
                Date = fundDetail["date"]?.ToString(),
                Beta = fundDetail["beta"]?.GetValue<float>(),
                Alpha = fundDetail["alpha"]?.GetValue<float>()
            };

            var existing = await _context.FundRisks
                .FirstOrDefaultAsync(r => r.FundId == fund.Id);
            if (existing == null)
            {
                _context.FundRisks.Add(risk);
            }
            else
            {
                existing.FundId = risk.FundId;
                existing.Date = risk.Date;
                existing.Beta = risk.Beta;
                existing.Alpha = risk.Alpha;
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessFundUnitsAsync(string regNo, JsonNode fundDetail)
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

            var units = new FundUnit
            {
                FundId = fund.Id,
                Date = fundDetail["date"]?.ToString(),
                BaseUnitsSubscriptionNav = fundDetail["baseUnitsSubscriptionNAV"]?.GetValue<decimal>(),
                BaseUnitsCancelNav = fundDetail["baseUnitsCancelNAV"]?.GetValue<decimal>(),
                BaseUnitsTotalNetAssetValue = fundDetail["baseUnitsTotalNetAssetValue"]?.GetValue<decimal>(),
                BaseTotalUnit = fundDetail["baseTotalUnit"]?.GetValue<decimal>(),
                BaseUnitsTotalSubscription = fundDetail["baseUnitsTotalSubscription"]?.GetValue<decimal>(),
                BaseUnitsTotalCancel = fundDetail["baseUnitsTotalCancel"]?.GetValue<decimal>(),
                SuperUnitsSubscriptionNav = fundDetail["superUnitsSubscriptionNAV"]?.GetValue<decimal>(),
                SuperUnitsCancelNav = fundDetail["superUnitsCancelNAV"]?.GetValue<decimal>(),
                SuperUnitsTotalNetAssetValue = fundDetail["superUnitsTotalNetAssetValue"]?.GetValue<decimal>(),
                SuperTotalUnit = fundDetail["superTotalUnit"]?.GetValue<decimal>(),
                SuperUnitsTotalSubscription = fundDetail["superUnitsTotalSubscription"]?.GetValue<decimal>(),
                SuperUnitsTotalCancel = fundDetail["superUnitsTotalCancel"]?.GetValue<decimal>()
            };

            var existing = await _context.FundUnits
                .FirstOrDefaultAsync(u => u.FundId == fund.Id);
            if (existing == null)
            {
                _context.FundUnits.Add(units);
            }
            else
            {
                existing.FundId = units.FundId;
                existing.Date = units.Date;
                existing.BaseUnitsSubscriptionNav = units.BaseUnitsSubscriptionNav;
                existing.BaseUnitsCancelNav = units.BaseUnitsCancelNav;
                existing.BaseUnitsTotalNetAssetValue = units.BaseUnitsTotalNetAssetValue;
                existing.BaseTotalUnit = units.BaseTotalUnit;
                existing.BaseUnitsTotalSubscription = units.BaseUnitsTotalSubscription;
                existing.BaseUnitsTotalCancel = units.BaseUnitsTotalCancel;
                existing.SuperUnitsSubscriptionNav = units.SuperUnitsSubscriptionNav;
                existing.SuperUnitsCancelNav = units.SuperUnitsCancelNav;
                existing.SuperUnitsTotalNetAssetValue = units.SuperUnitsTotalNetAssetValue;
                existing.SuperTotalUnit = units.SuperTotalUnit;
                existing.SuperUnitsTotalSubscription = units.SuperUnitsTotalSubscription;
                existing.SuperUnitsTotalCancel = units.SuperUnitsTotalCancel;
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessFundMonitoringAsync(string regNo, JsonNode fundDetail)
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

            var monitoring = new FundMonitoring
            {
                FundId = fund.Id,
                FundWatch = fundDetail["fundWatch"]?.ToString()
            };

            var existing = await _context.FundMonitorings
                .FirstOrDefaultAsync(m => m.FundId == fund.Id);
            if (existing == null)
            {
                _context.FundMonitorings.Add(monitoring);
            }
            else
            {
                existing.FundId = monitoring.FundId;
                existing.FundWatch = monitoring.FundWatch;
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessFundCompositionAsync(string regNo, string regNoForApi)
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

            var data = await GetApiDataAsync($"/api/v1/chart/portfoliochart?regno={regNoForApi}&showALL=true");
            if (data == null)
            {
                Console.WriteLine($"No data returned from portfoliochart API for reg_no: {regNo}");
                return;
            }

            JsonArray items;
            if (data is JsonArray array)
            {
                items = array;
            }
            else if (data is JsonObject)
            {
                var obj = (JsonObject)data;
                if (obj["items"] is JsonArray nestedArray)
                {
                    items = nestedArray;
                }
                else
                {
                    items = new JsonArray(obj);
                }
            }
            else
            {
                Console.WriteLine($"Unexpected data format for portfoliochart API: {data.ToJsonString()}");
                return;
            }

            foreach (var item in items)
            {
                var comp = new FundComposition
                {
                    FundId = fund.Id,
                    Date = item["date"]?.ToString(),
                    FiveBest = item["fiveBest"]?.GetValue<float>(),
                    Stock = item["stock"]?.GetValue<float>(),
                    Bond = item["bond"]?.GetValue<float>(),
                    Other = item["other"]?.GetValue<float>(),
                    Cash = item["cash"]?.GetValue<float>(),
                    Deposit = item["deposit"]?.GetValue<float>(),
                    FundUnit = item["fundUnit"]?.GetValue<float>(),
                    Commodity = item["commodity"]?.GetValue<float>()
                };

                var existing = await _context.FundCompositions
                    .FirstOrDefaultAsync(c => c.FundId == fund.Id);
                if (existing == null)
                {
                    _context.FundCompositions.Add(comp);
                }
                else
                {
                    existing.FundId = comp.FundId;
                    existing.Date = comp.Date;
                    existing.FiveBest = comp.FiveBest;
                    existing.Stock = comp.Stock;
                    existing.Bond = comp.Bond;
                    existing.Other = comp.Other;
                    existing.Cash = comp.Cash;
                    existing.Deposit = comp.Deposit;
                    existing.FundUnit = comp.FundUnit;
                    existing.Commodity = comp.Commodity;
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessNetAssetsAsync(string regNo, string regNoForApi)
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

            var data = await GetApiDataAsync($"/api/v1/chart/getfundnetassetchart?regno={regNoForApi}&showAll=true");
            if (data == null)
            {
                Console.WriteLine($"No data returned from getfundnetassestchart API for reg_no: {regNo}");
                return;
            }

            JsonArray items;
            if (data is JsonArray array)
            {
                items = array;
            }
            else if (data is JsonObject)
            {
                var obj = (JsonObject)data;
                if (obj["items"] is JsonArray nestedArray)
                {
                    items = nestedArray;
                }
                else
                {
                    items = new JsonArray(obj);
                }
            }
            else
            {
                Console.WriteLine($"Unexpected data format for getfundnetassestchart API: {data.ToJsonString()}");
                return;
            }

            foreach (var item in items)
            {
                var netAsset = new NetAsset
                {
                    FundId = fund.Id,
                    Date = item["date"]?.ToString(),
                    NetAsset1 = item["netAsset"]?.GetValue<decimal>(),
                    UnitsSubDay = item["unitsSubDAY"]?.GetValue<decimal>(),
                    UnitsRedDay = item["unitsRedDAY"]?.GetValue<decimal>()
                };

                var existing = await _context.NetAssets
                    .FirstOrDefaultAsync(n => n.FundId == fund.Id);
                if (existing == null)
                {
                    _context.NetAssets.Add(netAsset);
                }
                else
                {
                    existing.FundId = netAsset.FundId;
                    existing.Date = netAsset.Date;
                    existing.NetAsset1 = netAsset.NetAsset1;
                    existing.UnitsSubDay = netAsset.UnitsSubDay;
                    existing.UnitsRedDay = netAsset.UnitsRedDay;
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessNAVComparisonAsync(string regNo, string regNoForApi)
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

            var data = await GetApiDataAsync($"/api/v1/chart/getfundchart?regno={regNoForApi}&showALL=true");
            if (data == null)
            {
                Console.WriteLine($"No data returned from getfundchart API for reg_no: {regNo}");
                return;
            }

            JsonArray items;
            if (data is JsonArray array)
            {
                items = array;
            }
            else if (data is JsonObject)
            {
                var obj = (JsonObject)data;
                if (obj["items"] is JsonArray nestedArray)
                {
                    items = nestedArray;
                }
                else
                {
                    items = new JsonArray(obj);
                }
            }
            else
            {
                Console.WriteLine($"Unexpected data format for getfundchart API: {data.ToJsonString()}");
                return;
            }

            foreach (var item in items)
            {
                var navComp = new NavComparison
                {
                    FundId = fund.Id,
                    Date = item["date"]?.ToString(),
                    IssueNav = item["issueNav"]?.GetValue<decimal>(),
                    CancelNav = item["cancelNav"]?.GetValue<decimal>(),
                    StatisticalNav = item["statisticalNav"]?.GetValue<decimal>()
                };

                var existing = await _context.NavComparisons
                    .FirstOrDefaultAsync(n => n.FundId == fund.Id);
                if (existing == null)
                {
                    _context.NavComparisons.Add(navComp);
                }
                else
                {
                    existing.FundId = navComp.FundId;
                    existing.Date = navComp.Date;
                    existing.IssueNav = navComp.IssueNav;
                    existing.CancelNav = navComp.CancelNav;
                    existing.StatisticalNav = navComp.StatisticalNav;
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessFundEfficiencyAsync(string regNo, string regNoForApi)
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

            var data = await GetApiDataAsync($"/api/v1/chart/fundefficiencychart?regno={regNoForApi}&showALL=true");
            if (data == null)
            {
                Console.WriteLine($"No data returned from fundefficiencychart API for reg_no: {regNo}");
                return;
            }

            JsonArray items;
            if (data is JsonArray array)
            {
                items = array;
            }
            else if (data is JsonObject)
            {
                var obj = (JsonObject)data;
                if (obj["items"] is JsonArray nestedArray)
                {
                    items = nestedArray;
                }
                else
                {
                    items = new JsonArray(obj);
                }
            }
            else
            {
                Console.WriteLine($"Unexpected data format for fundefficiencychart API: {data.ToJsonString()}");
                return;
            }

            foreach (var item in items)
            {
                var eff = new FundEfficiency
                {
                    FundId = fund.Id,
                    Date = item["date"]?.ToString(),
                    DailyEfficiency = item["dailyEfficiency"]?.GetValue<float>(),
                    WeeklyEfficiency = item["weeklyEfficiency"]?.GetValue<float>(),
                    MonthlyEfficiency = item["monthlyEfficiency"]?.GetValue<float>(),
                    QuarterlyEfficiency = item["quarterlyEfficiency"]?.GetValue<float>(),
                    SixMonthEfficiency = item["sixMonthEfficiency"]?.GetValue<float>(),
                    AnnualEfficiency = item["annualEfficiency"]?.GetValue<float>()
                };

                var existing = await _context.FundEfficiencies
                    .FirstOrDefaultAsync(e => e.FundId == fund.Id);
                if (existing == null)
                {
                    _context.FundEfficiencies.Add(eff);
                }
                else
                {
                    existing.FundId = eff.FundId;
                    existing.Date = eff.Date;
                    existing.DailyEfficiency = eff.DailyEfficiency;
                    existing.WeeklyEfficiency = eff.WeeklyEfficiency;
                    existing.MonthlyEfficiency = eff.MonthlyEfficiency;
                    existing.QuarterlyEfficiency = eff.QuarterlyEfficiency;
                    existing.SixMonthEfficiency = eff.SixMonthEfficiency;
                    existing.AnnualEfficiency = eff.AnnualEfficiency;
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task ProcessProfitPerUnitAsync(string regNo, string regNoForApi)
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

            var data = await GetApiDataAsync($"/api/v1/chart/fundprofitschart?regno={regNoForApi}&showALL=true");
            if (data == null)
            {
                Console.WriteLine($"No data returned from fundprofitschart API for reg_no: {regNo}");
                return;
            }

            JsonArray items;
            if (data is JsonArray array)
            {
                items = array;
            }
            else if (data is JsonObject)
            {
                var obj = (JsonObject)data;
                if (obj["items"] is JsonArray nestedArray)
                {
                    items = nestedArray;
                }
                else
                {
                    items = new JsonArray(obj);
                }
            }
            else
            {
                Console.WriteLine($"Unexpected data format for fundprofitschart API: {data.ToJsonString()}");
                return;
            }

            foreach (var item in items)
            {
                var profitUnit = new ProfitPerUnit
                {
                    FundId = fund.Id,
                    ProfitDate = item["profitDate"]?.ToString(),
                    ProfitValue = item["profitValue"]?.GetValue<long>()
                };

                var existing = await _context.ProfitPerUnits
                    .FirstOrDefaultAsync(p => p.FundId == fund.Id);
                if (existing == null)
                {
                    _context.ProfitPerUnits.Add(profitUnit);
                }
                else
                {
                    existing.FundId = profitUnit.FundId;
                    existing.ProfitDate = profitUnit.ProfitDate;
                    existing.ProfitValue = profitUnit.ProfitValue;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}