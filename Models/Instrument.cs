using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class Instrument
{
    public int Id { get; set; }

    public int FundId { get; set; }

    public decimal? InsCode { get; set; }

    public string? SmallSymbolName { get; set; }

    public string? SymbolFullName { get; set; }

    public int? IndustryGroupCode { get; set; }

    public string? IndustryGroupName { get; set; }

    public int? IndustrySubCode { get; set; }

    public string? IndustrySubName { get; set; }

    public int? SymbolStatus { get; set; }

    public int? Type { get; set; }

    public int? MarketCode { get; set; }

    public decimal? StaticThresholdMaxPrice { get; set; }

    public decimal? StaticThresholdMinPrice { get; set; }

    public int? Status { get; set; }

    public virtual Fund Fund { get; set; } = null!;
}
