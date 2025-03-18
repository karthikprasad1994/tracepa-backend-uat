using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtCollate
{
    public decimal? CltCollateno { get; set; }

    public string? CltCollateref { get; set; }

    public decimal? CltCreator { get; set; }

    public decimal? CltAllow { get; set; }

    public DateTime? CltCreatedon { get; set; }

    public string? CltComment { get; set; }

    public int? CltGroup { get; set; }

    public string? CltOperation { get; set; }

    public int? CltOperationby { get; set; }

    public string? CltDelFlag { get; set; }

    public int? CltUpdatedby { get; set; }

    public DateTime? CltUpdatedon { get; set; }

    public int? CltRecallby { get; set; }

    public DateTime? CltRecallon { get; set; }

    public int? CltDeletedby { get; set; }

    public DateTime? CltDeletedon { get; set; }

    public int? CltApprovedby { get; set; }

    public DateTime? CltApprovedon { get; set; }

    public int? CltCompId { get; set; }

    public string? CltIpaddress { get; set; }
}
