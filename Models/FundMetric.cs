using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundMetric
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public decimal? FundSize { get; set; }

    public float? DailyEfficiency { get; set; }

    public float? WeeklyEfficiency { get; set; }

    public float? MonthlyEfficiency { get; set; }

    public float? QuarterlyEfficiency { get; set; }

    public float? SixMonthEfficiency { get; set; }

    public float? AnnualEfficiency { get; set; }

    public float? Efficiency { get; set; }

    public decimal? StatisticalNav { get; set; }

    public decimal? CancelNav { get; set; }

    public decimal? IssueNav { get; set; }

    public decimal? NetAsset { get; set; }

    public decimal? InvestedUnits { get; set; }

    public int? DividendIntervalPeriod { get; set; }

    public float? EstimatedEarningRate { get; set; }

    public float? GuaranteedEarningRate { get; set; }

    public string? LastModificationTime { get; set; }

    public virtual Fund? Fund { get; set; }
}
