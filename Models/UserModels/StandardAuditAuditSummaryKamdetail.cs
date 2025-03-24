using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_AuditSummary_KAMDetails")]
public partial class StandardAuditAuditSummaryKamdetail
{
    [Column("SAKAMD_PKID")]
    public int? SakamdPkid { get; set; }

    [Column("SAKAM_SAIFCD_PKID")]
    public int? SakamSaifcdPkid { get; set; }

    [Column("SAKAM_DescriptionOrReasonForSelectionAsKAM")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SakamDescriptionOrReasonForSelectionAsKam { get; set; }

    [Column("SAKAM_AuditProcedureUndertakenToAddressTheKAM")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SakamAuditProcedureUndertakenToAddressTheKam { get; set; }

    [Column("SAKAM_AttachID")]
    public int? SakamAttachId { get; set; }

    [Column("SAKAM_CrBy")]
    public int? SakamCrBy { get; set; }

    [Column("SAKAM_CrOn", TypeName = "datetime")]
    public DateTime? SakamCrOn { get; set; }
}
