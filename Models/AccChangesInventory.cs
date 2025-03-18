using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccChangesInventory
{
    public int? CiPkid { get; set; }

    public int? CiFinancialYear { get; set; }

    public int? CiCustId { get; set; }

    public int? CiOrgtype { get; set; }

    public int? CiHead { get; set; }

    public int? CiGroup { get; set; }

    public int? CiSubgroup { get; set; }

    public int? CiGlid { get; set; }

    public int? CiSubGlid { get; set; }

    public int? CiNote { get; set; }

    public decimal? CiObvalues { get; set; }

    public decimal? CiCbvalues { get; set; }

    public DateTime? CiDate { get; set; }

    public string? CiStatus { get; set; }

    public string? CiDelflag { get; set; }

    public int? CiCrBy { get; set; }

    public DateTime? CiCrOn { get; set; }

    public int? CiUpdatedBy { get; set; }

    public DateTime? CiUpdatedOn { get; set; }

    public int? CiSavedBy { get; set; }

    public DateTime? CiSavedOn { get; set; }

    public int? CiApprovedby { get; set; }

    public DateTime? CiApprovedOn { get; set; }

    public string? CiIpaddress { get; set; }

    public int? CiCompId { get; set; }
}
