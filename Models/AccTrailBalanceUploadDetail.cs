using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccTrailBalanceUploadDetail
{
    public int AtbudId { get; set; }

    public int? AtbudMasid { get; set; }

    public string AtbudCode { get; set; } = null!;

    public string? AtbudDescription { get; set; }

    public int? AtbudCustId { get; set; }

    public int? AtbudScheduleType { get; set; }

    public int? AtbudCompanyType { get; set; }

    public int? AtbudHeadingid { get; set; }

    public int? AtbudSubheading { get; set; }

    public int? AtbudItemid { get; set; }

    public string? AtbudDelflg { get; set; }

    public DateTime? AtbudCron { get; set; }

    public int? AtbudCrby { get; set; }

    public int? AtbudApprovedby { get; set; }

    public DateTime? AtbudApprovedon { get; set; }

    public string? AtbudStatus { get; set; }

    public int? AtbudUpdatedby { get; set; }

    public DateTime? AtbudUpdatedon { get; set; }

    public int? AtbudDeletedby { get; set; }

    public DateTime? AtbudDeletedon { get; set; }

    public int? AtbudRecallby { get; set; }

    public DateTime? AtbudRecallon { get; set; }

    public string? AtbudIpaddress { get; set; }

    public int? AtbudCompId { get; set; }

    public int? AtbudYearid { get; set; }

    public string? AtbudProgress { get; set; }

    public int? AtbudSubItemId { get; set; }

    public int? AtbudBranchnameid { get; set; }

    public int? AtbudQuarterId { get; set; }

    public virtual AccTrailBalanceUpload? AtbudMas { get; set; }
}
