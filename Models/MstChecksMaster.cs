using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstChecksMaster
{
    public int? ChkId { get; set; }

    public int? ChkControlId { get; set; }

    public string? ChkCheckName { get; set; }

    public string? ChkCheckDesc { get; set; }

    public int? ChkCatId { get; set; }

    public int? ChkIsKey { get; set; }

    public string? ChkDelFlag { get; set; }

    public string? ChkStatus { get; set; }

    public int? ChkCrBy { get; set; }

    public DateTime? ChkCrOn { get; set; }

    public int? ChkUpdatedBy { get; set; }

    public DateTime? ChkUpdatedOn { get; set; }

    public int? ChkApprovedBy { get; set; }

    public DateTime? ChkApprovedOn { get; set; }

    public int? ChkDeletedBy { get; set; }

    public DateTime? ChkDeletedOn { get; set; }

    public int? ChkRecallBy { get; set; }

    public DateTime? ChkRecallOn { get; set; }

    public string? ChkIpaddress { get; set; }

    public int? ChkCompId { get; set; }
}
