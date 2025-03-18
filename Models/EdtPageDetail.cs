using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtPageDetail
{
    public decimal? EpdBaseid { get; set; }

    public decimal? EpdDoctype { get; set; }

    public decimal? EpdDescid { get; set; }

    public string? EpdKeyword { get; set; }

    public string? EpdValue { get; set; }

    public int? EpdCompId { get; set; }
}
