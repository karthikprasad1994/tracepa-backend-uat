using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_ScheduleItems")]
public partial class AccScheduleItem
{
    [Column("ASI_ID")]
    public int AsiId { get; set; }

    [Column("ASI_Name")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AsiName { get; set; }

    [Column("ASI_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AsiDelflg { get; set; }

    [Column("ASI_CRON", TypeName = "datetime")]
    public DateTime? AsiCron { get; set; }

    [Column("ASI_CRBY")]
    public int? AsiCrby { get; set; }

    [Column("ASI_APPROVEDBY")]
    public int? AsiApprovedby { get; set; }

    [Column("ASI_APPROVEDON", TypeName = "datetime")]
    public DateTime? AsiApprovedon { get; set; }

    [Column("ASI_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AsiStatus { get; set; }

    [Column("ASI_UPDATEDBY")]
    public int? AsiUpdatedby { get; set; }

    [Column("ASI_UPDATEDON", TypeName = "datetime")]
    public DateTime? AsiUpdatedon { get; set; }

    [Column("ASI_DELETEDBY")]
    public int? AsiDeletedby { get; set; }

    [Column("ASI_DELETEDON", TypeName = "datetime")]
    public DateTime? AsiDeletedon { get; set; }

    [Column("ASI_RECALLBY")]
    public int? AsiRecallby { get; set; }

    [Column("ASI_RECALLON", TypeName = "datetime")]
    public DateTime? AsiRecallon { get; set; }

    [Column("ASI_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AsiIpaddress { get; set; }

    [Column("ASI_CompId")]
    public int? AsiCompId { get; set; }

    [Column("ASI_YEARId")]
    public int? AsiYearid { get; set; }

    [Column("ASI_HeadingID")]
    public int? AsiHeadingId { get; set; }

    [Column("ASI_SubHeadingID")]
    public int? AsiSubHeadingId { get; set; }

    [Column("ASI_Code")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AsiCode { get; set; }

    [Column("Asi_Orgtype")]
    public int? AsiOrgtype { get; set; }

    [Column("Asi_scheduletype")]
    public int? AsiScheduletype { get; set; }
}
