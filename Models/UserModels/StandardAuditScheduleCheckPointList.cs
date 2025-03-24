using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_ScheduleCheckPointList")]
public partial class StandardAuditScheduleCheckPointList
{
    [Column("SAC_ID")]
    public int? SacId { get; set; }

    [Column("SAC_SA_ID")]
    public int? SacSaId { get; set; }

    [Column("SAC_CheckPointID")]
    public int? SacCheckPointId { get; set; }

    [Column("SAC_Mandatory")]
    public int? SacMandatory { get; set; }

    [Column("SAC_Annexure")]
    public int? SacAnnexure { get; set; }

    [Column("SAC_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? SacRemarks { get; set; }

    [Column("SAC_ReviewerRemarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? SacReviewerRemarks { get; set; }

    [Column("SAC_Status")]
    public int? SacStatus { get; set; }

    [Column("SAC_ReviewLedgerID")]
    public int? SacReviewLedgerId { get; set; }

    [Column("SAC_DRLID")]
    public int? SacDrlid { get; set; }

    [Column("SAC_SamplingID")]
    public int? SacSamplingId { get; set; }

    [Column("SAC_TestResult")]
    public int? SacTestResult { get; set; }

    [Column("SAC_ConductedBy")]
    public int? SacConductedBy { get; set; }

    [Column("SAC_LastUpdatedOn", TypeName = "datetime")]
    public DateTime? SacLastUpdatedOn { get; set; }

    [Column("SAC_AttachID")]
    public int? SacAttachId { get; set; }

    [Column("SAC_CrBy")]
    public int? SacCrBy { get; set; }

    [Column("SAC_CrOn", TypeName = "datetime")]
    public DateTime? SacCrOn { get; set; }

    [Column("SAC_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SacIpaddress { get; set; }

    [Column("SAC_CompID")]
    public int? SacCompId { get; set; }
}
