using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_AuditSummary_MRDetails")]
public partial class StandardAuditAuditSummaryMrdetail
{
    [Column("SAMR_PKID")]
    public int? SamrPkid { get; set; }

    [Column("SAMR_SA_PKID")]
    public int? SamrSaPkid { get; set; }

    [Column("SAMR_CustID")]
    public int? SamrCustId { get; set; }

    [Column("SAMR_YearID")]
    public int? SamrYearId { get; set; }

    [Column("SAMR_MRID")]
    public int? SamrMrid { get; set; }

    [Column("SAMR_RequestedDate", TypeName = "datetime")]
    public DateTime? SamrRequestedDate { get; set; }

    [Column("SAMR_RequestedByPerson")]
    [StringLength(200)]
    [Unicode(false)]
    public string? SamrRequestedByPerson { get; set; }

    [Column("SAMR_RequestedRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? SamrRequestedRemarks { get; set; }

    [Column("SAMR_DueDateReceiveDocs", TypeName = "datetime")]
    public DateTime? SamrDueDateReceiveDocs { get; set; }

    [Column("SAMR_EmailIds")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SamrEmailIds { get; set; }

    [Column("SAMR_ResponsesReceivedDate", TypeName = "datetime")]
    public DateTime? SamrResponsesReceivedDate { get; set; }

    [Column("SAMR_ResponsesDetails")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SamrResponsesDetails { get; set; }

    [Column("SAMR_ResponsesRemarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SamrResponsesRemarks { get; set; }

    [Column("SAMR_AttachID")]
    public int? SamrAttachId { get; set; }

    [Column("SAMR_CrBy")]
    public int? SamrCrBy { get; set; }

    [Column("SAMR_CrOn", TypeName = "datetime")]
    public DateTime? SamrCrOn { get; set; }

    [Column("SAMR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SamrIpaddress { get; set; }

    [Column("SAMR_CompID")]
    public int? SamrCompId { get; set; }
}
