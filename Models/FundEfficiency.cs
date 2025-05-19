using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundEfficiency
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public float? DailyEfficiency { get; set; }

    public float? WeeklyEfficiency { get; set; }

    public float? MonthlyEfficiency { get; set; }

    public float? QuarterlyEfficiency { get; set; }

    public float? SixMonthEfficiency { get; set; }

    public float? AnnualEfficiency { get; set; }

    public virtual Fund? Fund { get; set; }
}
