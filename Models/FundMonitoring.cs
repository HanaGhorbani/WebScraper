using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundMonitoring
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? FundWatch { get; set; }

    public virtual Fund? Fund { get; set; }
}
