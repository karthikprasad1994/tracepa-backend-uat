using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstControlLibrary
{
    public int? MclPkid { get; set; }

    public string? MclControlName { get; set; }

    public string? MclControlDesc { get; set; }

    public string? MclCode { get; set; }

    public int? MclIsKey { get; set; }

    public string? MclDelFlag { get; set; }

    public string? MclStatus { get; set; }

    public int? MclCrBy { get; set; }

    public DateTime? MclCrOn { get; set; }

    public int? MclUpdatedBy { get; set; }

    public DateTime? MclUpdatedOn { get; set; }

    public int? MclApprovedBy { get; set; }

    public DateTime? MclApprovedOn { get; set; }

    public int? MclDeletedBy { get; set; }

    public DateTime? MclDeletedOn { get; set; }

    public int? MclRecallBy { get; set; }

    public DateTime? MclRecallOn { get; set; }

    public string? MclIpaddress { get; set; }

    public int? MclCompId { get; set; }
}
