using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtDocumentType
{
    public int? DotDoctypeid { get; set; }

    public string? DotDocname { get; set; }

    public string? DotNote { get; set; }

    public int? DotPgroup { get; set; }

    public int? DotCrby { get; set; }

    public DateTime? DotCron { get; set; }

    public string? DotStatus { get; set; }

    public string? DotOperation { get; set; }

    public decimal? DotOperationby { get; set; }

    public int? DotIsGlobal { get; set; }

    public string? DotDelFlag { get; set; }

    public int? DotUpdatedby { get; set; }

    public DateTime? DotUpdatedon { get; set; }

    public int? DotRecallby { get; set; }

    public DateTime? DotRecallon { get; set; }

    public int? DotDeletedby { get; set; }

    public DateTime? DotDeletedon { get; set; }

    public int? DotApprovedby { get; set; }

    public DateTime? DotApprovedon { get; set; }

    public int? DotCompId { get; set; }

    public string? DotIpaddress { get; set; }
}
