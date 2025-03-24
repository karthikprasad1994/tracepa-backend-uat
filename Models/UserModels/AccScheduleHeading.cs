using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_ScheduleHeading")]
public partial class AccScheduleHeading
{
    [Column("ASH_ID")]
    public int AshId { get; set; }

    [Column("ASH_Name")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AshName { get; set; }

    [Column("ASH_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AshDelflg { get; set; }

    [Column("ASH_CRON", TypeName = "datetime")]
    public DateTime? AshCron { get; set; }

    [Column("ASH_CRBY")]
    public int? AshCrby { get; set; }

    [Column("ASH_APPROVEDBY")]
    public int? AshApprovedby { get; set; }

    [Column("ASH_APPROVEDON", TypeName = "datetime")]
    public DateTime? AshApprovedon { get; set; }

    [Column("ASH_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AshStatus { get; set; }

    [Column("ASH_UPDATEDBY")]
    public int? AshUpdatedby { get; set; }

    [Column("ASH_UPDATEDON", TypeName = "datetime")]
    public DateTime? AshUpdatedon { get; set; }

    [Column("ASH_DELETEDBY")]
    public int? AshDeletedby { get; set; }

    [Column("ASH_DELETEDON", TypeName = "datetime")]
    public DateTime? AshDeletedon { get; set; }

    [Column("ASH_RECALLBY")]
    public int? AshRecallby { get; set; }

    [Column("ASH_RECALLON", TypeName = "datetime")]
    public DateTime? AshRecallon { get; set; }

    [Column("ASH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AshIpaddress { get; set; }

    [Column("ASH_CompId")]
    public int? AshCompId { get; set; }

    [Column("ASH_YEARId")]
    public int? AshYearid { get; set; }

    [Column("Ash_Total", TypeName = "money")]
    public decimal? AshTotal { get; set; }

    [Column("ASH_Code")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AshCode { get; set; }

    [Column("ASH_Notes")]
    public int? AshNotes { get; set; }

    [Column("Ash_scheduletype")]
    public int? AshScheduletype { get; set; }

    [Column("Ash_Orgtype")]
    public int? AshOrgtype { get; set; }
}
