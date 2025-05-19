using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class ProfitPerUnit
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? ProfitDate { get; set; }

    public long? ProfitValue { get; set; }

    public virtual Fund? Fund { get; set; }
}
