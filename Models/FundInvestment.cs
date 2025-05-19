using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundInvestment
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public int? InsInvNo { get; set; }

    public float? InsInvPercent { get; set; }

    public int? RetInvNo { get; set; }

    public float? RetInvPercent { get; set; }

    public float? LegalPercent { get; set; }

    public float? NaturalPercent { get; set; }

    public long? UnitsRedDay { get; set; }

    public long? UnitsRedFromFirst { get; set; }

    public long? UnitsSubDay { get; set; }

    public long? UnitsSubFromFirst { get; set; }

    public virtual Fund? Fund { get; set; }
}
