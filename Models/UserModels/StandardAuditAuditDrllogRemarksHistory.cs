using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_Audit_DRLLog_RemarksHistory")]
public partial class StandardAuditAuditDrllogRemarksHistory
{
    [Column("SAR_ID")]
    public int? SarId { get; set; }

    [Column("SAR_SA_ID")]
    public int? SarSaId { get; set; }

    [Column("SAR_SAC_ID")]
    public int? SarSacId { get; set; }

    [Column("SAR_RemarksType")]
    [StringLength(10)]
    [Unicode(false)]
    public string? SarRemarksType { get; set; }

    [Column("SAR_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? SarRemarks { get; set; }

    [Column("SAR_RemarksBy")]
    public int? SarRemarksBy { get; set; }

    [Column("SAR_Date", TypeName = "datetime")]
    public DateTime? SarDate { get; set; }

    [Column("SAR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SarIpaddress { get; set; }

    [Column("SAR_CompID")]
    public int? SarCompId { get; set; }

    [Column("SAR_IsIssueRaised")]
    public int? SarIsIssueRaised { get; set; }

    [Column("SAR_EmailIds")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SarEmailIds { get; set; }

    [Column("SAR_CheckPointIDs")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SarCheckPointIds { get; set; }

    [Column("SAR_TimlinetoResOn")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SarTimlinetoResOn { get; set; }

    [Column("sar_Yearid")]
    public int? SarYearid { get; set; }

    [Column("SAR_MASid")]
    public int? SarMasid { get; set; }
}
