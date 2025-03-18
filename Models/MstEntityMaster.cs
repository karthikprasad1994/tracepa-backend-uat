using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstEntityMaster
{
    public int EntId { get; set; }

    public string EntCode { get; set; } = null!;

    public string? EntEntityname { get; set; }

    public string EntDelflg { get; set; } = null!;

    public int? EntOrder { get; set; }

    public DateTime? EntCron { get; set; }

    public int? EntCrby { get; set; }

    public int? EntApprovedby { get; set; }

    public DateTime? EntApprovedon { get; set; }

    public string? EntRrpstatus { get; set; }

    public string? EntBranch { get; set; }

    public int? EntModule { get; set; }

    public int? EntOrgId { get; set; }

    public int? EntKri { get; set; }

    public int? EntFunOwnerId { get; set; }

    public string? EntStatus { get; set; }

    public int? EntUpdatedby { get; set; }

    public DateTime? EntUpdatedon { get; set; }

    public int? EntDeletedby { get; set; }

    public DateTime? EntDeletedon { get; set; }

    public int? EntRecallby { get; set; }

    public DateTime? EntRecallon { get; set; }

    public string? EntIpaddress { get; set; }

    public int? EntCompId { get; set; }

    public string? EntDesc { get; set; }

    public int? EntFunManagerId { get; set; }

    public int? EntFunSpocid { get; set; }
}
