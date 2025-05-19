using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class MutualFundLicense
{
    public int Id { get; set; }

    public int? FundId { get; set; }

    public int? IsExpired { get; set; }

    public string? StartDate { get; set; }

    public string? ExpireDate { get; set; }

    public string? LicenseNo { get; set; }

    public int? LicenseStatusId { get; set; }

    public string? LicenseStatusDescription { get; set; }

    public int? LicenseTypeId { get; set; }

    public int? NewLicenseTypeId { get; set; }

    public virtual Fund? Fund { get; set; }
}
