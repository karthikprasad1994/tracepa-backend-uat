using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsProductDetail
{
    public int? MpdPkid { get; set; }

    public int? MpdMcpPkid { get; set; }

    public int? MpdProduct { get; set; }

    public DateTime? MpdDateofDemoDetailsShared { get; set; }

    public string? MpdDetailsSent { get; set; }

    public DateTime? MpdCreatedOn { get; set; }
}
