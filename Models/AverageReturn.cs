using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class AverageReturn
{
    public int Id { get; set; }

    public int FundTypeId { get; set; }

    public decimal? NetAsset { get; set; }

    public double? Stock { get; set; }

    public double? Bond { get; set; }

    public double? Cash { get; set; }

    public double? Deposit { get; set; }

    public double? DailyEfficiency { get; set; }

    public double? WeeklyEfficiency { get; set; }

    public double? MonthlyEfficiency { get; set; }

    public double? QuarterlyEfficiency { get; set; }

    public double? SixMonthEfficiency { get; set; }

    public double? AnnualEfficiency { get; set; }

    public double? Efficiency { get; set; }

    public virtual FundType FundType { get; set; } = null!;
}
