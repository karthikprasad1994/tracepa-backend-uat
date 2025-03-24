using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_TrailBalance_Upload_Details")]
public partial class AccTrailBalanceUploadDetail
{
    [Column("ATBUD_ID")]
    public int AtbudId { get; set; }

    [Column("ATBUD_Masid")]
    public int? AtbudMasid { get; set; }

    [Column("ATBUD_CODE")]
    [StringLength(50)]
    [Unicode(false)]
    public string AtbudCode { get; set; } = null!;

    [Column("ATBUD_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AtbudDescription { get; set; }

    [Column("ATBUD_CustId")]
    public int? AtbudCustId { get; set; }

    [Column("ATBUD_SChedule_Type")]
    public int? AtbudScheduleType { get; set; }

    [Column("ATBUD_Company_Type")]
    public int? AtbudCompanyType { get; set; }

    [Column("ATBUD_Headingid")]
    public int? AtbudHeadingid { get; set; }

    [Column("ATBUD_Subheading")]
    public int? AtbudSubheading { get; set; }

    [Column("ATBUD_itemid")]
    public int? AtbudItemid { get; set; }

    [Column("ATBUD_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AtbudDelflg { get; set; }

    [Column("ATBUD_CRON", TypeName = "datetime")]
    public DateTime? AtbudCron { get; set; }

    [Column("ATBUD_CRBY")]
    public int? AtbudCrby { get; set; }

    [Column("ATBUD_APPROVEDBY")]
    public int? AtbudApprovedby { get; set; }

    [Column("ATBUD_APPROVEDON", TypeName = "datetime")]
    public DateTime? AtbudApprovedon { get; set; }

    [Column("ATBUD_STATUS")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AtbudStatus { get; set; }

    [Column("ATBUD_UPDATEDBY")]
    public int? AtbudUpdatedby { get; set; }

    [Column("ATBUD_UPDATEDON", TypeName = "datetime")]
    public DateTime? AtbudUpdatedon { get; set; }

    [Column("ATBUD_DELETEDBY")]
    public int? AtbudDeletedby { get; set; }

    [Column("ATBUD_DELETEDON", TypeName = "datetime")]
    public DateTime? AtbudDeletedon { get; set; }

    [Column("ATBUD_RECALLBY")]
    public int? AtbudRecallby { get; set; }

    [Column("ATBUD_RECALLON", TypeName = "datetime")]
    public DateTime? AtbudRecallon { get; set; }

    [Column("ATBUD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AtbudIpaddress { get; set; }

    [Column("ATBUD_CompId")]
    public int? AtbudCompId { get; set; }

    [Column("ATBUD_YEARId")]
    public int? AtbudYearid { get; set; }

    [Column("ATBUD_Progress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AtbudProgress { get; set; }

    [Column("ATBUD_SubItemId")]
    public int? AtbudSubItemId { get; set; }

    [Column("Atbud_Branchnameid")]
    public int? AtbudBranchnameid { get; set; }

    [Column("ATBUD_QuarterId")]
    public int? AtbudQuarterId { get; set; }
}
