using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class Fund
{
    public int Id { get; set; }

    public int RegNo { get; set; }

    public string Name { get; set; } = null!;

    public int? FundTypeId { get; set; }

    public string? InitiationDate { get; set; }

    public string? Manager { get; set; }

    public string? Auditor { get; set; }

    public string? Custodian { get; set; }

    public string? Guarantor { get; set; }

    public string? WebsiteAddress { get; set; }

    public int? IsCompleted { get; set; }

    public string? SmallSymbolName { get; set; }

    public decimal InsCode { get; set; }

    public string? ManagerSeoRegisterNo { get; set; }

    public string? GuarantorSeoRegisterNo { get; set; }

    public int? FundPublisher { get; set; }

    public string? TypeOfInvest { get; set; }

    public string? ArticlesOfAssociationLink { get; set; }

    public string? ProspectusLink { get; set; }

    public string? SeoRegisterDate { get; set; }

    public string? RegistrationNumber { get; set; }

    public string? RegisterDate { get; set; }

    public string? NationalId { get; set; }

    public string? InvestmentManager { get; set; }

    public string? ExecutiveManager { get; set; }

    public string? MarketMaker { get; set; }

    public virtual ICollection<FundComposition> FundCompositions { get; set; } = new List<FundComposition>();

    public virtual ICollection<FundEfficiency> FundEfficiencies { get; set; } = new List<FundEfficiency>();

    public virtual ICollection<FundInvestment> FundInvestments { get; set; } = new List<FundInvestment>();

    public virtual ICollection<FundMetric> FundMetrics { get; set; } = new List<FundMetric>();

    public virtual ICollection<FundMonitoring> FundMonitorings { get; set; } = new List<FundMonitoring>();

    public virtual ICollection<FundRank> FundRanks { get; set; } = new List<FundRank>();

    public virtual ICollection<FundRisk> FundRisks { get; set; } = new List<FundRisk>();

    public virtual FundType? FundType { get; set; }

    public virtual ICollection<FundUnit> FundUnits { get; set; } = new List<FundUnit>();

    public virtual ICollection<InstrumentBestLimit> InstrumentBestLimits { get; set; } = new List<InstrumentBestLimit>();

    public virtual ICollection<InstrumentClientType> InstrumentClientTypes { get; set; } = new List<InstrumentClientType>();

    public virtual ICollection<InstrumentTransaction> InstrumentTransactions { get; set; } = new List<InstrumentTransaction>();

    public virtual ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();

    public virtual ICollection<MutualFundLicense> MutualFundLicenses { get; set; } = new List<MutualFundLicense>();

    public virtual ICollection<NavComparison> NavComparisons { get; set; } = new List<NavComparison>();

    public virtual ICollection<NetAsset> NetAssets { get; set; } = new List<NetAsset>();

    public virtual ICollection<ProfitPerUnit> ProfitPerUnits { get; set; } = new List<ProfitPerUnit>();
}
