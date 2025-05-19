using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundRisk
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public float? Beta { get; set; }

    public float? Alpha { get; set; }

    public virtual Fund? Fund { get; set; }
}
