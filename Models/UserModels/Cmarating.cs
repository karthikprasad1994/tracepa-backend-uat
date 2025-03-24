using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMARating")]
public partial class Cmarating
{
    [Column("CMAR_ID")]
    public int? CmarId { get; set; }

    [Column("CMAR_YearID")]
    public int? CmarYearId { get; set; }

    [Column("CMAR_StartValue")]
    public float? CmarStartValue { get; set; }

    [Column("CMAR_EndValue")]
    public float? CmarEndValue { get; set; }

    [Column("CMAR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmarDesc { get; set; }

    [Column("CMAR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmarName { get; set; }

    [Column("CMAR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmarColor { get; set; }

    [Column("CMAR_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmarFlag { get; set; }

    [Column("CMAR_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CmarStatus { get; set; }

    [Column("CMAR_CrBy")]
    public int? CmarCrBy { get; set; }

    [Column("CMAR_CrOn", TypeName = "datetime")]
    public DateTime? CmarCrOn { get; set; }

    [Column("CMAR_UpdatedBy")]
    public int? CmarUpdatedBy { get; set; }

    [Column("CMAR_UpdatedOn", TypeName = "datetime")]
    public DateTime? CmarUpdatedOn { get; set; }

    [Column("CMAR_ApprovedBy")]
    public int? CmarApprovedBy { get; set; }

    [Column("CMAR_ApprovedOn", TypeName = "datetime")]
    public DateTime? CmarApprovedOn { get; set; }

    [Column("CMAR_RecallBy")]
    public int? CmarRecallBy { get; set; }

    [Column("CMAR_RecallOn", TypeName = "datetime")]
    public DateTime? CmarRecallOn { get; set; }

    [Column("CMAR_DeletedBy")]
    public int? CmarDeletedBy { get; set; }

    [Column("CMAR_DeletedOn", TypeName = "datetime")]
    public DateTime? CmarDeletedOn { get; set; }

    [Column("CMAR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmarIpaddress { get; set; }

    [Column("CMAR_CompId")]
    public int? CmarCompId { get; set; }
}
