using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class TraceCabinet
{
    public int? TcPkid { get; set; }

    public string? TcName { get; set; }

    public string? TcRemarks { get; set; }

    public string? TcStatus { get; set; }

    public DateTime? TcCrOn { get; set; }

    public int? TcCrBy { get; set; }

    public string? TcIpaddress { get; set; }

    public int? TcCompId { get; set; }
}
