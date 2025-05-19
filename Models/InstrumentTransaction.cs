using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class InstrumentTransaction
{
    public int Id { get; set; }

    public int FundId { get; set; }

    public decimal? InsCode { get; set; }

    public string? TransactionDate { get; set; }

    public decimal? NumberOfTransactions { get; set; }

    public decimal? NumberOfVolume { get; set; }

    public decimal? TransactionValue { get; set; }

    public decimal? ClosingPrice { get; set; }

    public int? AdjPriceForward { get; set; }

    public int? AdjPriceBackward { get; set; }

    public decimal? LastTransaction { get; set; }

    public decimal? ChangePrice { get; set; }

    public decimal? PriceMin { get; set; }

    public decimal? PriceMax { get; set; }

    public decimal? PriceFirst { get; set; }

    public decimal? PriceYesterday { get; set; }

    public int? PriceYesterdayBackward { get; set; }

    public int? LastStatus { get; set; }

    public int? Heven { get; set; }

    public virtual Fund Fund { get; set; } = null!;
}
