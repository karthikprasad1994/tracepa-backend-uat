using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMARating_SupportProcess")]
public partial class CmaratingSupportProcess
{
    [Column("CMASR_ID")]
    public int? CmasrId { get; set; }

    [Column("CMASR_YearID")]
    public int? CmasrYearId { get; set; }

    [Column("CMASR_StartValue")]
    public float? CmasrStartValue { get; set; }

    [Column("CMASR_EndValue")]
    public float? CmasrEndValue { get; set; }

    [Column("CMASR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmasrDesc { get; set; }

    [Column("CMASR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmasrName { get; set; }

    [Column("CMASR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmasrColor { get; set; }

    [Column("CMASR_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmasrFlag { get; set; }

    [Column("CMASR_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CmasrStatus { get; set; }

    [Column("CMASR_CrBy")]
    public int? CmasrCrBy { get; set; }

    [Column("CMASR_CrOn", TypeName = "datetime")]
    public DateTime? CmasrCrOn { get; set; }

    [Column("CMASR_UpdatedBy")]
    public int? CmasrUpdatedBy { get; set; }

    [Column("CMASR_UpdatedOn", TypeName = "datetime")]
    public DateTime? CmasrUpdatedOn { get; set; }

    [Column("CMASR_ApprovedBy")]
    public int? CmasrApprovedBy { get; set; }

    [Column("CMASR_ApprovedOn", TypeName = "datetime")]
    public DateTime? CmasrApprovedOn { get; set; }

    [Column("CMASR_RecallBy")]
    public int? CmasrRecallBy { get; set; }

    [Column("CMASR_RecallOn", TypeName = "datetime")]
    public DateTime? CmasrRecallOn { get; set; }

    [Column("CMASR_DeletedBy")]
    public int? CmasrDeletedBy { get; set; }

    [Column("CMASR_DeletedOn", TypeName = "datetime")]
    public DateTime? CmasrDeletedOn { get; set; }

    [Column("CMASR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmasrIpaddress { get; set; }

    [Column("CMASR_CompId")]
    public int? CmasrCompId { get; set; }
}
