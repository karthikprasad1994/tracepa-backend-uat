using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstInherentRiskMaster
{
    public int? MimId { get; set; }

    public string? MimName { get; set; }

    public string? MimDesc { get; set; }

    public string? MimColor { get; set; }

    public int? MimFromScore { get; set; }

    public int? MimToScore { get; set; }

    public string? MimFrequency { get; set; }

    public int? MimCrBy { get; set; }

    public DateTime? MimCrOn { get; set; }

    public string? MimIpaddress { get; set; }

    public int? MimCompId { get; set; }
}
