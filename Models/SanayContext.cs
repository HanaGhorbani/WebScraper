using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Web_Scraper.Models;

public partial class SanayContext : DbContext
{
    public SanayContext()
    {
    }

    public SanayContext(DbContextOptions<SanayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AverageReturn> AverageReturns { get; set; }

    public virtual DbSet<Fund> Funds { get; set; }

    public virtual DbSet<FundComposition> FundCompositions { get; set; }

    public virtual DbSet<FundEfficiency> FundEfficiencies { get; set; }

    public virtual DbSet<FundInvestment> FundInvestments { get; set; }

    public virtual DbSet<FundMetric> FundMetrics { get; set; }

    public virtual DbSet<FundMonitoring> FundMonitorings { get; set; }

    public virtual DbSet<FundRank> FundRanks { get; set; }

    public virtual DbSet<FundRisk> FundRisks { get; set; }

    public virtual DbSet<FundType> FundTypes { get; set; }

    public virtual DbSet<FundUnit> FundUnits { get; set; }

    public virtual DbSet<Instrument> Instruments { get; set; }

    public virtual DbSet<InstrumentBestLimit> InstrumentBestLimits { get; set; }

    public virtual DbSet<InstrumentClientType> InstrumentClientTypes { get; set; }

    public virtual DbSet<InstrumentTransaction> InstrumentTransactions { get; set; }

    public virtual DbSet<JsonChecksum> JsonChecksums { get; set; }

    public virtual DbSet<MutualFundLicense> MutualFundLicenses { get; set; }

    public virtual DbSet<NavComparison> NavComparisons { get; set; }

    public virtual DbSet<NetAsset> NetAssets { get; set; }

    public virtual DbSet<ProfitPerUnit> ProfitPerUnits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\HANA;Database=sanay;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AverageReturn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AverageR__3214EC07020EB201");

            entity.Property(e => e.NetAsset).HasColumnType("decimal(38, 4)");

            entity.HasOne(d => d.FundType).WithMany(p => p.AverageReturns)
                .HasForeignKey(d => d.FundTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AverageReturns_FundTypes");
        });

        modelBuilder.Entity<Fund>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Funds__3213E83F81FEC9A2");

            entity.HasIndex(e => e.InsCode, "IX_Funds_InsCode_NonZero")
                .IsUnique()
                .HasFilter("([Ins_code]<>(0))");

            entity.HasIndex(e => e.RegNo, "IX_Funds_reg_no").IsUnique();

            entity.Property(e => e.ArticlesOfAssociationLink)
                .HasMaxLength(500)
                .HasColumnName("articles_of_association_link");
            entity.Property(e => e.Auditor)
                .HasMaxLength(750)
                .HasColumnName("auditor");
            entity.Property(e => e.Custodian)
                .HasMaxLength(750)
                .HasColumnName("custodian");
            entity.Property(e => e.ExecutiveManager)
                .HasColumnType("text")
                .HasColumnName("executive_manager");
            entity.Property(e => e.FundPublisher).HasColumnName("fund_publisher");
            entity.Property(e => e.FundTypeId).HasColumnName("fund_type_id");
            entity.Property(e => e.Guarantor)
                .HasMaxLength(750)
                .HasColumnName("guarantor");
            entity.Property(e => e.GuarantorSeoRegisterNo)
                .HasMaxLength(100)
                .HasColumnName("guarantor_seo_register_no");
            entity.Property(e => e.InitiationDate)
                .HasColumnType("text")
                .HasColumnName("initiation_date");
            entity.Property(e => e.InsCode)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("ins_code");
            entity.Property(e => e.InvestmentManager)
                .HasMaxLength(1000)
                .HasColumnName("investment_manager");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
            entity.Property(e => e.Manager)
                .HasMaxLength(750)
                .HasColumnName("manager");
            entity.Property(e => e.ManagerSeoRegisterNo)
                .HasMaxLength(100)
                .HasColumnName("manager_seo_register_no");
            entity.Property(e => e.MarketMaker)
                .HasMaxLength(500)
                .HasColumnName("market_maker");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .HasColumnName("name");
            entity.Property(e => e.NationalId)
                .HasColumnType("text")
                .HasColumnName("national_id");
            entity.Property(e => e.ProspectusLink)
                .HasMaxLength(500)
                .HasColumnName("prospectus_link");
            entity.Property(e => e.RegNo).HasColumnName("reg_no");
            entity.Property(e => e.RegisterDate)
                .HasColumnType("text")
                .HasColumnName("register_date");
            entity.Property(e => e.RegistrationNumber)
                .HasColumnType("text")
                .HasColumnName("registration_number");
            entity.Property(e => e.SeoRegisterDate)
                .HasColumnType("text")
                .HasColumnName("seo_register_date");
            entity.Property(e => e.SmallSymbolName)
                .HasMaxLength(500)
                .HasColumnName("small_symbol_name");
            entity.Property(e => e.TypeOfInvest)
                .HasMaxLength(100)
                .HasColumnName("type_of_invest");
            entity.Property(e => e.WebsiteAddress)
                .HasMaxLength(500)
                .HasColumnName("website_address");

            entity.HasOne(d => d.FundType).WithMany(p => p.Funds)
                .HasForeignKey(d => d.FundTypeId)
                .HasConstraintName("FK__Funds__fund_type__28ED12D1");
        });

        modelBuilder.Entity<FundComposition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Com__3213E83FFD8658AC");

            entity.ToTable("Fund_Composition");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bond).HasColumnName("bond");
            entity.Property(e => e.Cash).HasColumnName("cash");
            entity.Property(e => e.Commodity).HasColumnName("commodity");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.Deposit).HasColumnName("deposit");
            entity.Property(e => e.FiveBest).HasColumnName("five_best");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.FundUnit).HasColumnName("fund_unit");
            entity.Property(e => e.Other).HasColumnName("other");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundCompositions)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Comp__fund___2EA5EC27");
        });

        modelBuilder.Entity<FundEfficiency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Eff__3213E83FAAD4CA0D");

            entity.ToTable("Fund_Efficiency");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnnualEfficiency).HasColumnName("annual_efficiency");
            entity.Property(e => e.DailyEfficiency).HasColumnName("daily_efficiency");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.MonthlyEfficiency).HasColumnName("monthly_efficiency");
            entity.Property(e => e.QuarterlyEfficiency).HasColumnName("quarterly_efficiency");
            entity.Property(e => e.SixMonthEfficiency).HasColumnName("six_month_efficiency");
            entity.Property(e => e.WeeklyEfficiency).HasColumnName("weekly_efficiency");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundEfficiencies)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Effi__fund___4959E263");
        });

        modelBuilder.Entity<FundInvestment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Inv__3213E83FA5C57C26");

            entity.ToTable("Fund_Investment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.InsInvNo).HasColumnName("ins_inv_no");
            entity.Property(e => e.InsInvPercent).HasColumnName("ins_inv_percent");
            entity.Property(e => e.LegalPercent).HasColumnName("legal_percent");
            entity.Property(e => e.NaturalPercent).HasColumnName("natural_percent");
            entity.Property(e => e.RetInvNo).HasColumnName("ret_inv_no");
            entity.Property(e => e.RetInvPercent).HasColumnName("ret_inv_percent");
            entity.Property(e => e.UnitsRedDay).HasColumnName("units_red_day");
            entity.Property(e => e.UnitsRedFromFirst).HasColumnName("units_red_from_first");
            entity.Property(e => e.UnitsSubDay).HasColumnName("units_sub_day");
            entity.Property(e => e.UnitsSubFromFirst).HasColumnName("units_sub_from_first");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundInvestments)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Inve__fund___318258D2");
        });

        modelBuilder.Entity<FundMetric>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Met__3213E83F19AD2272");

            entity.ToTable("Fund_Metrics");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnnualEfficiency).HasColumnName("annual_efficiency");
            entity.Property(e => e.CancelNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("cancel_nav");
            entity.Property(e => e.DailyEfficiency).HasColumnName("daily_efficiency");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.DividendIntervalPeriod).HasColumnName("dividend_interval_period");
            entity.Property(e => e.Efficiency).HasColumnName("efficiency");
            entity.Property(e => e.EstimatedEarningRate).HasColumnName("estimated_earning_rate");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.FundSize)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("fund_size");
            entity.Property(e => e.GuaranteedEarningRate).HasColumnName("guaranteed_earning_rate");
            entity.Property(e => e.InvestedUnits)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("invested_units");
            entity.Property(e => e.IssueNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("issue_nav");
            entity.Property(e => e.LastModificationTime)
                .HasColumnType("text")
                .HasColumnName("last_modification_time");
            entity.Property(e => e.MonthlyEfficiency).HasColumnName("monthly_efficiency");
            entity.Property(e => e.NetAsset)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("net_asset");
            entity.Property(e => e.QuarterlyEfficiency).HasColumnName("quarterly_efficiency");
            entity.Property(e => e.SixMonthEfficiency).HasColumnName("six_month_efficiency");
            entity.Property(e => e.StatisticalNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("statistical_nav");
            entity.Property(e => e.WeeklyEfficiency).HasColumnName("weekly_efficiency");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundMetrics)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Metr__fund___2BC97F7C");
        });

        modelBuilder.Entity<FundMonitoring>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Mon__3213E83F45C0C92F");

            entity.ToTable("Fund_Monitoring");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.FundWatch)
                .HasColumnType("text")
                .HasColumnName("fund_watch");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundMonitorings)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Moni__fund___40C49C62");
        });

        modelBuilder.Entity<FundRank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Ran__3213E83FEEB0C9DC");

            entity.ToTable("Fund_Ranks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.RankLastUpdate)
                .HasColumnType("text")
                .HasColumnName("rank_last_update");
            entity.Property(e => e.RankOf12Month).HasColumnName("rank_of_12_month");
            entity.Property(e => e.RankOf24Month).HasColumnName("rank_of_24_month");
            entity.Property(e => e.RankOf36Month).HasColumnName("rank_of_36_month");
            entity.Property(e => e.RankOf48Month).HasColumnName("rank_of_48_month");
            entity.Property(e => e.RankOf60Month).HasColumnName("rank_of_60_month");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundRanks)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Rank__fund___345EC57D");
        });

        modelBuilder.Entity<FundRisk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Ris__3213E83FBD18A53E");

            entity.ToTable("Fund_Risk");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Alpha).HasColumnName("alpha");
            entity.Property(e => e.Beta).HasColumnName("beta");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.FundId).HasColumnName("fund_id");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundRisks)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Risk__fund___3B0BC30C");
        });

        modelBuilder.Entity<FundType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Typ__3213E83FA42719E4");

            entity.ToTable("Fund_Types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FundType1).HasColumnName("fund_type");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .HasColumnName("name");
        });

        modelBuilder.Entity<FundUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fund_Uni__3213E83F6F065151");

            entity.ToTable("Fund_Units");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BaseTotalUnit)
                .HasColumnType("decimal(20, 4)")
                .HasColumnName("base_total_unit");
            entity.Property(e => e.BaseUnitsCancelNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("base_units_cancel_nav");
            entity.Property(e => e.BaseUnitsSubscriptionNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("base_units_subscription_nav");
            entity.Property(e => e.BaseUnitsTotalCancel)
                .HasColumnType("decimal(20, 4)")
                .HasColumnName("base_units_total_cancel");
            entity.Property(e => e.BaseUnitsTotalNetAssetValue)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("base_units_total_net_asset_value");
            entity.Property(e => e.BaseUnitsTotalSubscription)
                .HasColumnType("decimal(20, 4)")
                .HasColumnName("base_units_total_subscription");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.SuperTotalUnit)
                .HasColumnType("decimal(20, 4)")
                .HasColumnName("super_total_unit");
            entity.Property(e => e.SuperUnitsCancelNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("super_units_cancel_nav");
            entity.Property(e => e.SuperUnitsSubscriptionNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("super_units_subscription_nav");
            entity.Property(e => e.SuperUnitsTotalCancel)
                .HasColumnType("decimal(20, 4)")
                .HasColumnName("super_units_total_cancel");
            entity.Property(e => e.SuperUnitsTotalNetAssetValue)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("super_units_total_net_asset_value");
            entity.Property(e => e.SuperUnitsTotalSubscription)
                .HasColumnType("decimal(20, 4)")
                .HasColumnName("super_units_total_subscription");

            entity.HasOne(d => d.Fund).WithMany(p => p.FundUnits)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Fund_Unit__fund___3DE82FB7");
        });

        modelBuilder.Entity<Instrument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instrume__3214EC074CBB0F1F");

            entity.Property(e => e.IndustryGroupName).HasMaxLength(500);
            entity.Property(e => e.IndustrySubName).HasMaxLength(500);
            entity.Property(e => e.InsCode).HasColumnType("decimal(38, 4)");
            entity.Property(e => e.SmallSymbolName).HasMaxLength(255);
            entity.Property(e => e.StaticThresholdMaxPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StaticThresholdMinPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SymbolFullName).HasMaxLength(500);

            entity.HasOne(d => d.Fund).WithMany(p => p.Instruments)
                .HasForeignKey(d => d.FundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Instruments_Funds");
        });

        modelBuilder.Entity<InstrumentBestLimit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instrume__3214EC077D1A9C9D");

            entity.Property(e => e.InsCode).HasColumnType("decimal(38, 4)");

            entity.HasOne(d => d.Fund).WithMany(p => p.InstrumentBestLimits)
                .HasForeignKey(d => d.FundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InstrumentBestLimits_Funds");
        });

        modelBuilder.Entity<InstrumentClientType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instrume__3214EC0744D69C0B");

            entity.Property(e => e.InsCode).HasColumnType("decimal(38, 4)");

            entity.HasOne(d => d.Fund).WithMany(p => p.InstrumentClientTypes)
                .HasForeignKey(d => d.FundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InstrumentClientTypes_Funds");
        });

        modelBuilder.Entity<InstrumentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instrume__3214EC075739D47A");

            entity.Property(e => e.ChangePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ClosingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Heven).HasColumnName("HEven");
            entity.Property(e => e.InsCode).HasColumnType("decimal(38, 4)");
            entity.Property(e => e.LastTransaction).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceFirst).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceMax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceMin).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceYesterday).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionDate).HasMaxLength(50);
            entity.Property(e => e.TransactionValue).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Fund).WithMany(p => p.InstrumentTransactions)
                .HasForeignKey(d => d.FundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InstrumentTransactions_Funds");
        });

        modelBuilder.Entity<JsonChecksum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JsonChec__3213E83FD1917B3B");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApiEndpoint).HasColumnType("text");
            entity.Property(e => e.Checksum).HasColumnType("text");
            entity.Property(e => e.RegNo).HasColumnType("text");
        });

        modelBuilder.Entity<MutualFundLicense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mutual_F__3213E83F4F8595CA");

            entity.ToTable("Mutual_Fund_Licenses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpireDate)
                .HasColumnType("text")
                .HasColumnName("expire_date");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.IsExpired).HasColumnName("is_expired");
            entity.Property(e => e.LicenseNo)
                .HasColumnType("text")
                .HasColumnName("license_no");
            entity.Property(e => e.LicenseStatusDescription)
                .HasColumnType("text")
                .HasColumnName("license_status_description");
            entity.Property(e => e.LicenseStatusId).HasColumnName("license_status_id");
            entity.Property(e => e.LicenseTypeId).HasColumnName("license_type_id");
            entity.Property(e => e.NewLicenseTypeId).HasColumnName("new_license_type_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("text")
                .HasColumnName("start_date");

            entity.HasOne(d => d.Fund).WithMany(p => p.MutualFundLicenses)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Mutual_Fu__fund___382F5661");
        });

        modelBuilder.Entity<NavComparison>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NAV_Comp__3213E83F57A95375");

            entity.ToTable("NAV_Comparison");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CancelNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("cancel_nav");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.IssueNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("issue_nav");
            entity.Property(e => e.StatisticalNav)
                .HasColumnType("decimal(38, 4)")
                .HasColumnName("statistical_nav");

            entity.HasOne(d => d.Fund).WithMany(p => p.NavComparisons)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__NAV_Compa__fund___467D75B8");
        });

        modelBuilder.Entity<NetAsset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Net_Asse__3213E83F01040856");

            entity.ToTable("Net_Assets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("text")
                .HasColumnName("date");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.NetAsset1)
                .HasColumnType("decimal(20, 4)")
                .HasColumnName("net_asset");
            entity.Property(e => e.UnitsRedDay)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("units_red_day");
            entity.Property(e => e.UnitsSubDay)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("units_sub_day");

            entity.HasOne(d => d.Fund).WithMany(p => p.NetAssets)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Net_Asset__fund___43A1090D");
        });

        modelBuilder.Entity<ProfitPerUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Profit_P__3213E83F488F8ED7");

            entity.ToTable("Profit_Per_Unit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FundId).HasColumnName("fund_id");
            entity.Property(e => e.ProfitDate)
                .HasColumnType("text")
                .HasColumnName("profit_date");
            entity.Property(e => e.ProfitValue).HasColumnName("profit_value");

            entity.HasOne(d => d.Fund).WithMany(p => p.ProfitPerUnits)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK__Profit_Pe__fund___4C364F0E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
