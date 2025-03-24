using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sample_selection")]
public partial class SampleSelection
{
    [Column("SS_PKID")]
    public int? SsPkid { get; set; }

    [Column("SS_AuditCodeID")]
    public int? SsAuditCodeId { get; set; }

    [Column("SS_AttachID")]
    public int? SsAttachId { get; set; }

    [Column("SS_CompID")]
    public int? SsCompId { get; set; }

    [Column("SS_CheckPointID")]
    public int? SsCheckPointId { get; set; }
}
