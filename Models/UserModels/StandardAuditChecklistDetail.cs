using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_Checklist_Details")]
public partial class StandardAuditChecklistDetail
{
    [Column("SACD_ID")]
    public int? SacdId { get; set; }

    [Column("SACD_CustId")]
    public int? SacdCustId { get; set; }

    [Column("SACD_AuditId")]
    public int? SacdAuditId { get; set; }

    [Column("SACD_AuditType")]
    public int? SacdAuditType { get; set; }

    [Column("SACD_Heading")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? SacdHeading { get; set; }

    [Column("SACD_CheckpointId")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SacdCheckpointId { get; set; }

    [Column("SACD_EmpId")]
    public int? SacdEmpId { get; set; }

    [Column("SACD_WorkType")]
    public int? SacdWorkType { get; set; }

    [Column("SACD_HrPrDay")]
    [StringLength(10)]
    [Unicode(false)]
    public string? SacdHrPrDay { get; set; }

    [Column("SACD_StartDate", TypeName = "datetime")]
    public DateTime? SacdStartDate { get; set; }

    [Column("SACD_EndDate", TypeName = "datetime")]
    public DateTime? SacdEndDate { get; set; }

    [Column("SACD_TotalHr")]
    [StringLength(10)]
    [Unicode(false)]
    public string? SacdTotalHr { get; set; }

    [Column("SACD_CRON", TypeName = "datetime")]
    public DateTime? SacdCron { get; set; }

    [Column("SACD_CRBY")]
    public int? SacdCrby { get; set; }

    [Column("SACD_UPDATEDBY")]
    public int? SacdUpdatedby { get; set; }

    [Column("SACD_UPDATEDON", TypeName = "datetime")]
    public DateTime? SacdUpdatedon { get; set; }

    [Column("SACD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SacdIpaddress { get; set; }

    [Column("SACD_CompId")]
    public int? SacdCompId { get; set; }
}
