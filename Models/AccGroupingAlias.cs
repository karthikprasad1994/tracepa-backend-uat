using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccGroupingAlias
{
    public int? AgaId { get; set; }

    public string? AgaDescription { get; set; }

    public int? AgaGlid { get; set; }

    public string? AgaGldesc { get; set; }

    public int? AgaGrpLevel { get; set; }

    public int? AgaScheduletype { get; set; }

    public int? AgaOrgtype { get; set; }

    public int? AgaCompid { get; set; }

    public string? AgaStatus { get; set; }

    public int? AgaCreatedby { get; set; }

    public DateTime? AgaCreatedOn { get; set; }

    public string? AgaIpaddress { get; set; }
}
