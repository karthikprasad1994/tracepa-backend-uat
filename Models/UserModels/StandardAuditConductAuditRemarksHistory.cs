using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_ConductAudit_RemarksHistory")]
public partial class StandardAuditConductAuditRemarksHistory
{
    [Column("SCR_ID")]
    public int? ScrId { get; set; }

    [Column("SCR_SA_ID")]
    public int? ScrSaId { get; set; }

    [Column("SCR_SAC_ID")]
    public int? ScrSacId { get; set; }

    [Column("SCR_CheckPointID")]
    public int? ScrCheckPointId { get; set; }

    [Column("SCR_RemarksType")]
    public int? ScrRemarksType { get; set; }

    [Column("SCR_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ScrRemarks { get; set; }

    [Column("SCR_RemarksBy")]
    public int? ScrRemarksBy { get; set; }

    [Column("SCR_Date", TypeName = "datetime")]
    public DateTime? ScrDate { get; set; }

    [Column("SCR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ScrIpaddress { get; set; }

    [Column("SCR_CompID")]
    public int? ScrCompId { get; set; }

    [Column("SCR_IsIssueRaised")]
    public int? ScrIsIssueRaised { get; set; }

    [Column("SCR_EmailIds")]
    [StringLength(500)]
    [Unicode(false)]
    public string? ScrEmailIds { get; set; }
}
