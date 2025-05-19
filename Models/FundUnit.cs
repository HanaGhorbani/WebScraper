using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundUnit
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public string? Date { get; set; }

    public decimal? BaseUnitsSubscriptionNav { get; set; }

    public decimal? BaseUnitsCancelNav { get; set; }

    public decimal? BaseUnitsTotalNetAssetValue { get; set; }

    public decimal? BaseTotalUnit { get; set; }

    public decimal? BaseUnitsTotalSubscription { get; set; }

    public decimal? BaseUnitsTotalCancel { get; set; }

    public decimal? SuperUnitsSubscriptionNav { get; set; }

    public decimal? SuperUnitsCancelNav { get; set; }

    public decimal? SuperUnitsTotalNetAssetValue { get; set; }

    public decimal? SuperTotalUnit { get; set; }

    public decimal? SuperUnitsTotalSubscription { get; set; }

    public decimal? SuperUnitsTotalCancel { get; set; }

    public virtual Fund? Fund { get; set; }
}
