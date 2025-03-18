using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstRiskColorMatrix
{
    public int? RcmRowId { get; set; }

    public int? RcmColumnId { get; set; }

    public string? RcmCategory { get; set; }

    public string? RcmColorsName { get; set; }

    public int? RcmCreatedBy { get; set; }

    public DateTime? RcmCreatedOn { get; set; }

    public int? RcmUpdatedBy { get; set; }

    public DateTime? RcmUpdatedOn { get; set; }

    public int? RcmCompId { get; set; }

    public string? RcmIpaddress { get; set; }
}
