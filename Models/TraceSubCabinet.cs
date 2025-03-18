using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class TraceSubCabinet
{
    public int? TscPkid { get; set; }

    public int? TscCabinetId { get; set; }

    public string? TscName { get; set; }

    public string? TscRemarks { get; set; }

    public string? TscDecs { get; set; }

    public string? TscStatus { get; set; }

    public DateTime? TscCrOn { get; set; }

    public int? TscCrBy { get; set; }

    public string? TscIpaddress { get; set; }

    public int? TscCompId { get; set; }
}
