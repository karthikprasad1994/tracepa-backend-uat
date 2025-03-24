using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_SignOff")]
public partial class AuditSignOff
{
    [Column("ASO_PKID")]
    public int? AsoPkid { get; set; }

    [Column("ASO_YearID")]
    public int? AsoYearId { get; set; }

    [Column("ASO_AuditCodeID")]
    public int? AsoAuditCodeId { get; set; }

    [Column("ASO_FunctionID")]
    public int? AsoFunctionId { get; set; }

    [Column("ASO_CustID")]
    public int? AsoCustId { get; set; }

    [Column("ASO_MasterID")]
    public int? AsoMasterId { get; set; }

    [Column("ASO_AuditRatingID")]
    public int? AsoAuditRatingId { get; set; }

    [Column("ASO_SignOffStatus")]
    [StringLength(5)]
    [Unicode(false)]
    public string? AsoSignOffStatus { get; set; }

    [Column("ASO_Comments")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AsoComments { get; set; }

    [Column("ASO_OverAllComments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AsoOverAllComments { get; set; }

    [Column("ASO_KeyObservation")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AsoKeyObservation { get; set; }

    [Column("ASO_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AsoStatus { get; set; }

    [Column("ASO_CrBy")]
    public int? AsoCrBy { get; set; }

    [Column("ASO_CrOn", TypeName = "datetime")]
    public DateTime? AsoCrOn { get; set; }

    [Column("ASO_UpdatedBy")]
    public int? AsoUpdatedBy { get; set; }

    [Column("ASO_UpdatedOn", TypeName = "datetime")]
    public DateTime? AsoUpdatedOn { get; set; }

    [Column("ASO_SubmittedBy")]
    public int? AsoSubmittedBy { get; set; }

    [Column("ASO_SubmittedOn", TypeName = "datetime")]
    public DateTime? AsoSubmittedOn { get; set; }

    [Column("ASO_AttachID")]
    public int? AsoAttachId { get; set; }

    [Column("ASO_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AsoIpaddress { get; set; }

    [Column("ASO_CompID")]
    public int? AsoCompId { get; set; }
}
