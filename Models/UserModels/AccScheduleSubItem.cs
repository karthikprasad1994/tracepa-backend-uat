using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_ScheduleSubItems")]
public partial class AccScheduleSubItem
{
    [Column("ASSI_ID")]
    public int AssiId { get; set; }

    [Column("ASSI_Name")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AssiName { get; set; }

    [Column("ASSI_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AssiDelflg { get; set; }

    [Column("ASSI_CRON", TypeName = "datetime")]
    public DateTime? AssiCron { get; set; }

    [Column("ASSI_CRBY")]
    public int? AssiCrby { get; set; }

    [Column("ASSI_APPROVEDBY")]
    public int? AssiApprovedby { get; set; }

    [Column("ASSI_APPROVEDON", TypeName = "datetime")]
    public DateTime? AssiApprovedon { get; set; }

    [Column("ASSI_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AssiStatus { get; set; }

    [Column("ASSI_UPDATEDBY")]
    public int? AssiUpdatedby { get; set; }

    [Column("ASSI_UPDATEDON", TypeName = "datetime")]
    public DateTime? AssiUpdatedon { get; set; }

    [Column("ASSI_DELETEDBY")]
    public int? AssiDeletedby { get; set; }

    [Column("ASSI_DELETEDON", TypeName = "datetime")]
    public DateTime? AssiDeletedon { get; set; }

    [Column("ASSI_RECALLBY")]
    public int? AssiRecallby { get; set; }

    [Column("ASSI_RECALLON", TypeName = "datetime")]
    public DateTime? AssiRecallon { get; set; }

    [Column("ASSI_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AssiIpaddress { get; set; }

    [Column("ASSI_CompId")]
    public int? AssiCompId { get; set; }

    [Column("ASSI_YEARId")]
    public int? AssiYearid { get; set; }

    [Column("ASSI_HeadingID")]
    public int? AssiHeadingId { get; set; }

    [Column("ASSI_SubHeadingID")]
    public int? AssiSubHeadingId { get; set; }

    [Column("ASSI_ItemsID")]
    public int? AssiItemsId { get; set; }

    [Column("ASSI_Code")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AssiCode { get; set; }

    [Column("Assi_scheduletype")]
    public int? AssiScheduletype { get; set; }

    [Column("Assi_Orgtype")]
    public int? AssiOrgtype { get; set; }
}
