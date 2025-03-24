using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("chart_of_Accounts")]
public partial class ChartOfAccount
{
    [Column("gl_id")]
    public int? GlId { get; set; }

    [Column("gl_glcode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? GlGlcode { get; set; }

    [Column("gl_parent")]
    public int? GlParent { get; set; }

    [Column("gl_desc")]
    [StringLength(200)]
    [Unicode(false)]
    public string? GlDesc { get; set; }

    [Column("gl_head")]
    public int? GlHead { get; set; }

    [Column("gl_remarks")]
    [StringLength(150)]
    [Unicode(false)]
    public string? GlRemarks { get; set; }

    [Column("gl_reason")]
    [StringLength(150)]
    [Unicode(false)]
    public string? GlReason { get; set; }

    [Column("gl_subglexist")]
    public int? GlSubglexist { get; set; }

    [Column("gl_delflag")]
    [StringLength(50)]
    [Unicode(false)]
    public string? GlDelflag { get; set; }

    [Column("gl_BranchCode")]
    [StringLength(10)]
    [Unicode(false)]
    public string? GlBranchCode { get; set; }

    [Column("gl_AccHead")]
    public int? GlAccHead { get; set; }

    [Column("gl_reason_Creation")]
    [StringLength(150)]
    [Unicode(false)]
    public string? GlReasonCreation { get; set; }

    [Column("gl_effective_date", TypeName = "smalldatetime")]
    public DateTime? GlEffectiveDate { get; set; }

    [Column("gl_CrBy")]
    public short? GlCrBy { get; set; }

    [Column("gl_CrDate", TypeName = "smalldatetime")]
    public DateTime? GlCrDate { get; set; }

    [Column("gl_DelBy")]
    public short? GlDelBy { get; set; }

    [Column("gl_DelDate", TypeName = "smalldatetime")]
    public DateTime? GlDelDate { get; set; }

    [Column("gl_SortOrder")]
    public short? GlSortOrder { get; set; }

    [Column("gl_CompId")]
    public int? GlCompId { get; set; }

    [Column("gl_TrialBalanceCode")]
    [StringLength(20)]
    [Unicode(false)]
    public string? GlTrialBalanceCode { get; set; }

    [Column("gl_BalAmt")]
    public long? GlBalAmt { get; set; }

    [Column("gl_BalType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? GlBalType { get; set; }

    [Column("gl_AppBy")]
    public short? GlAppBy { get; set; }

    [Column("gl_AppOn", TypeName = "datetime")]
    public DateTime? GlAppOn { get; set; }

    [Column("gl_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? GlStatus { get; set; }

    [Column("gl_TDS")]
    public int? GlTds { get; set; }

    [Column("gl_AccType")]
    [StringLength(10)]
    [Unicode(false)]
    public string? GlAccType { get; set; }

    [Column("gl_LinkInv")]
    public int? GlLinkInv { get; set; }

    [Column("gl_InvItem")]
    public int? GlInvItem { get; set; }

    [Column("gl_ODLimit", TypeName = "money")]
    public decimal? GlOdlimit { get; set; }

    [Column("gl_fring")]
    public int? GlFring { get; set; }

    [Column("gl_orderby")]
    public int? GlOrderby { get; set; }

    [Column("gl_UpdatedBy")]
    public int? GlUpdatedBy { get; set; }

    [Column("gl_UpdatedOn", TypeName = "datetime")]
    public DateTime? GlUpdatedOn { get; set; }

    [Column("gl_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? GlOperation { get; set; }

    [Column("gl_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? GlIpaddress { get; set; }

    [Column("gl_OrgTypeID")]
    public int? GlOrgTypeId { get; set; }

    [Column("gl_CustID")]
    public int? GlCustId { get; set; }
}
