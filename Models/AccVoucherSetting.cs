using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccVoucherSetting
{
    public int AvsId { get; set; }

    public int? AvsTransType { get; set; }

    public string? AvsPrefix { get; set; }

    public int? AvsSntotal { get; set; }

    public int? AvsCompId { get; set; }

    public int? AvsCreatedBy { get; set; }

    public DateTime? AvsCreatedOn { get; set; }

    public int? AvsUpdatedBy { get; set; }

    public DateTime? AvsUpdatedOn { get; set; }

    public string? AvsOperation { get; set; }

    public string? AvsIpaddress { get; set; }
}
