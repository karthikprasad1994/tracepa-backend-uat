using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_ScheduleTemplates1")]
public partial class AccScheduleTemplates1
{
    [Column("AST_ID")]
    public int AstId { get; set; }

    [Column("AST_HeadingID")]
    public int? AstHeadingId { get; set; }

    [Column("AST_SubHeadingID")]
    public int? AstSubHeadingId { get; set; }

    [Column("AST_ItemID")]
    public int? AstItemId { get; set; }

    [Column("AST_SubItemID")]
    public int? AstSubItemId { get; set; }

    [Column("AST_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AstDelflg { get; set; }

    [Column("AST_CRON", TypeName = "datetime")]
    public DateTime? AstCron { get; set; }

    [Column("AST_CRBY")]
    public int? AstCrby { get; set; }

    [Column("AST_APPROVEDBY")]
    public int? AstApprovedby { get; set; }

    [Column("AST_APPROVEDON", TypeName = "datetime")]
    public DateTime? AstApprovedon { get; set; }

    [Column("AST_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AstStatus { get; set; }

    [Column("AST_UPDATEDBY")]
    public int? AstUpdatedby { get; set; }

    [Column("AST_UPDATEDON", TypeName = "datetime")]
    public DateTime? AstUpdatedon { get; set; }

    [Column("AST_DELETEDBY")]
    public int? AstDeletedby { get; set; }

    [Column("AST_DELETEDON", TypeName = "datetime")]
    public DateTime? AstDeletedon { get; set; }

    [Column("AST_RECALLBY")]
    public int? AstRecallby { get; set; }

    [Column("AST_RECALLON", TypeName = "datetime")]
    public DateTime? AstRecallon { get; set; }

    [Column("AST_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AstIpaddress { get; set; }

    [Column("AST_CompId")]
    public int? AstCompId { get; set; }

    [Column("AST_YEARId")]
    public int? AstYearid { get; set; }

    [Column("AST_Schedule_type")]
    public int? AstScheduleType { get; set; }

    [Column("AST_Companytype")]
    public int? AstCompanytype { get; set; }

    [Column("AST_Company_limit")]
    public int? AstCompanyLimit { get; set; }

    [Column("AST_AccHeadId")]
    public int? AstAccHeadId { get; set; }

    [Column("AST_Notes")]
    public int? AstNotes { get; set; }
}
