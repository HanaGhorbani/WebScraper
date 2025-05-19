using System;
using System.Collections.Generic;

namespace Web_Scraper.Models;

public partial class JsonChecksum
{
    public int Id { get; set; }

    public string ApiEndpoint { get; set; } = null!;

    public string RegNo { get; set; } = null!;

    public string Checksum { get; set; } = null!;
}
