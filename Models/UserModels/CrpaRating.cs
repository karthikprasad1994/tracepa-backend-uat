using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_Rating")]
public partial class CrpaRating
{
    [Column("CRAT_PKID")]
    public int? CratPkid { get; set; }

    [Column("CRAT_YearID")]
    public int? CratYearId { get; set; }

    [Column("CRAT_AuditID")]
    public int? CratAuditId { get; set; }

    [Column("CRAT_StartValue")]
    public float? CratStartValue { get; set; }

    [Column("CRAT_EndValue")]
    public float? CratEndValue { get; set; }

    [Column("CRAT_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CratDesc { get; set; }

    [Column("CRAT_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CratName { get; set; }

    [Column("CRAT_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CratColor { get; set; }

    [Column("CRAT_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CratFlag { get; set; }

    [Column("CRAT_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CratStatus { get; set; }

    [Column("CRAT_CrBy")]
    public int? CratCrBy { get; set; }

    [Column("CRAT_CrOn", TypeName = "datetime")]
    public DateTime? CratCrOn { get; set; }

    [Column("CRAT_UpdatedBy")]
    public int? CratUpdatedBy { get; set; }

    [Column("CRAT_UpdatedOn", TypeName = "datetime")]
    public DateTime? CratUpdatedOn { get; set; }

    [Column("CRAT_ApprovedBy")]
    public int? CratApprovedBy { get; set; }

    [Column("CRAT_ApprovedOn", TypeName = "datetime")]
    public DateTime? CratApprovedOn { get; set; }

    [Column("CRAT_RecallBy")]
    public int? CratRecallBy { get; set; }

    [Column("CRAT_RecallOn", TypeName = "datetime")]
    public DateTime? CratRecallOn { get; set; }

    [Column("CRAT_DeletedBy")]
    public int? CratDeletedBy { get; set; }

    [Column("CRAT_DeletedOn", TypeName = "datetime")]
    public DateTime? CratDeletedOn { get; set; }

    [Column("CRAT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CratIpaddress { get; set; }

    [Column("CRAT_CompId")]
    public int? CratCompId { get; set; }
}
