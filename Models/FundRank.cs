using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundRank
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? RankLastUpdate { get; set; }

    public float? RankOf12Month { get; set; }

    public float? RankOf24Month { get; set; }

    public float? RankOf36Month { get; set; }

    public float? RankOf48Month { get; set; }

    public float? RankOf60Month { get; set; }

    public virtual Fund? Fund { get; set; }
}
