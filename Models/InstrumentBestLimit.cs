using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class InstrumentBestLimit
{
    public int Id { get; set; }

    public int FundId { get; set; }

    public decimal? InsCode { get; set; }

    public int RowNumber { get; set; }

    public long? DemandVolume { get; set; }

    public int? NumberRequests { get; set; }

    public int? DemandPrice { get; set; }

    public int? SupplyPrice { get; set; }

    public int? NumberSupply { get; set; }

    public long? SupplyVolume { get; set; }

    public virtual Fund Fund { get; set; } = null!;
}
