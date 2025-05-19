using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundComposition
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public float? FiveBest { get; set; }

    public float? Stock { get; set; }

    public float? Bond { get; set; }

    public float? Other { get; set; }

    public float? Cash { get; set; }

    public float? Deposit { get; set; }

    public float? FundUnit { get; set; }

    public float? Commodity { get; set; }

    public virtual Fund? Fund { get; set; }
}
