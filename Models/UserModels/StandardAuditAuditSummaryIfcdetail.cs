using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_AuditSummary_IFCDetails")]
public partial class StandardAuditAuditSummaryIfcdetail
{
    [Column("SAIFCD_PKID")]
    public int? SaifcdPkid { get; set; }

    [Column("SAIFCD_SAIFC_PKID")]
    public int? SaifcdSaifcPkid { get; set; }

    [Column("SAIFCD_ColumnRowType")]
    public int? SaifcdColumnRowType { get; set; }

    [Column("SAIFCD_Column1")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SaifcdColumn1 { get; set; }

    [Column("SAIFCD_Column2")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SaifcdColumn2 { get; set; }

    [Column("SAIFCD_Column3")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SaifcdColumn3 { get; set; }

    [Column("SAIFCD_Column4")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SaifcdColumn4 { get; set; }

    [Column("SAIFCD_Column5")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SaifcdColumn5 { get; set; }

    [Column("SAIFCD_Column6")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SaifcdColumn6 { get; set; }

    [Column("SAIFCD_DateOfTesting", TypeName = "datetime")]
    public DateTime? SaifcdDateOfTesting { get; set; }

    [Column("SAIFCD_TypeOfTestingDetails")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SaifcdTypeOfTestingDetails { get; set; }

    [Column("SAIFCD_SampleSizeUsed")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SaifcdSampleSizeUsed { get; set; }

    [Column("SAIFCD_Conclusion")]
    public int? SaifcdConclusion { get; set; }

    [Column("SAIFCD_AttachID")]
    public int? SaifcdAttachId { get; set; }

    [Column("SAIFCD_CrBy")]
    public int? SaifcdCrBy { get; set; }

    [Column("SAIFCD_CrOn", TypeName = "datetime")]
    public DateTime? SaifcdCrOn { get; set; }
}
