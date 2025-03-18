using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class GenProductDetail
{
    public int? GpPkid { get; set; }

    public string? GpAccesscode { get; set; }

    public string? GpProductkey { get; set; }

    public int? GpLicensetype { get; set; }

    public int? GpLicenseuser { get; set; }

    public string? GpStatus { get; set; }
}
