using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class NavComparison
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public decimal? IssueNav { get; set; }

    public decimal? CancelNav { get; set; }

    public decimal? StatisticalNav { get; set; }

    public virtual Fund? Fund { get; set; }
}
