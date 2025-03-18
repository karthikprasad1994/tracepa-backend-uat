using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ChartOfAccount
{
    public int? GlId { get; set; }

    public string? GlGlcode { get; set; }

    public int? GlParent { get; set; }

    public string? GlDesc { get; set; }

    public int? GlHead { get; set; }

    public string? GlRemarks { get; set; }

    public string? GlReason { get; set; }

    public int? GlSubglexist { get; set; }

    public string? GlDelflag { get; set; }

    public string? GlBranchCode { get; set; }

    public int? GlAccHead { get; set; }

    public string? GlReasonCreation { get; set; }

    public DateTime? GlEffectiveDate { get; set; }

    public short? GlCrBy { get; set; }

    public DateTime? GlCrDate { get; set; }

    public short? GlDelBy { get; set; }

    public DateTime? GlDelDate { get; set; }

    public short? GlSortOrder { get; set; }

    public int? GlCompId { get; set; }

    public string? GlTrialBalanceCode { get; set; }

    public long? GlBalAmt { get; set; }

    public string? GlBalType { get; set; }

    public short? GlAppBy { get; set; }

    public DateTime? GlAppOn { get; set; }

    public string? GlStatus { get; set; }

    public int? GlTds { get; set; }

    public string? GlAccType { get; set; }

    public int? GlLinkInv { get; set; }

    public int? GlInvItem { get; set; }

    public decimal? GlOdlimit { get; set; }

    public int? GlFring { get; set; }

    public int? GlOrderby { get; set; }

    public int? GlUpdatedBy { get; set; }

    public DateTime? GlUpdatedOn { get; set; }

    public string? GlOperation { get; set; }

    public string? GlIpaddress { get; set; }

    public int? GlOrgTypeId { get; set; }

    public int? GlCustId { get; set; }
}
