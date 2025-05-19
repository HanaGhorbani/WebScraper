using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class NetAsset
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public decimal? NetAsset1 { get; set; }

    public decimal? UnitsSubDay { get; set; }

    public decimal? UnitsRedDay { get; set; }

    public virtual Fund? Fund { get; set; }
}
