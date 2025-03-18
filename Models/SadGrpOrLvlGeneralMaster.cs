using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadGrpOrLvlGeneralMaster
{
    public int? MasId { get; set; }

    public string? MasCode { get; set; }

    public string? MasDescription { get; set; }

    public string? MasNotes { get; set; }

    public string? MasDelFlag { get; set; }

    public string? MasStatus { get; set; }

    public int? MasClassify { get; set; }

    public int? MasCreatedby { get; set; }

    public DateTime? MasCreatedon { get; set; }

    public int? MasUpdatedby { get; set; }

    public DateTime? MasUpdatedOn { get; set; }

    public int? MasApprovedBy { get; set; }

    public DateTime? MasApprovedOn { get; set; }

    public int? MasDeletedBy { get; set; }

    public DateTime? MasDeletedOn { get; set; }

    public int? MasRecalledBy { get; set; }

    public DateTime? MasRecalledOn { get; set; }

    public string? MasIpaddress { get; set; }

    public int? MasCompId { get; set; }
}
