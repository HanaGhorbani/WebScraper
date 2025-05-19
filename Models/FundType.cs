using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class FundType
{
    public int Id { get; set; }

    public int FundType1 { get; set; }

    public string Name { get; set; } = null!;

    public int IsActive { get; set; }

    public virtual ICollection<AverageReturn> AverageReturns { get; set; } = new List<AverageReturn>();

    public virtual ICollection<Fund> Funds { get; set; } = new List<Fund>();
}
