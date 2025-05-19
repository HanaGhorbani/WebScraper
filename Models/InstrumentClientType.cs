using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class InstrumentClientType
{
    public int Id { get; set; }

    public int FundId { get; set; }

    public decimal? InsCode { get; set; }

    public int? NumberIndividualsBuyers { get; set; }

    public int? NumberNonIndividualBuyers { get; set; }

    public long? SumIndividualBuyVolume { get; set; }

    public long? SumNonIndividualBuyVolume { get; set; }

    public int? NumberIndividualsSellers { get; set; }

    public int? NumberNonIndividualSellers { get; set; }

    public long? SumIndividualSellVolume { get; set; }

    public long? SumNonIndividualSellVolume { get; set; }

    public virtual Fund Fund { get; set; } = null!;
}
