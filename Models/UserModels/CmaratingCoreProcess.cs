using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMARating_CoreProcess")]
public partial class CmaratingCoreProcess
{
    [Column("CMACR_ID")]
    public int? CmacrId { get; set; }

    [Column("CMACR_YearID")]
    public int? CmacrYearId { get; set; }

    [Column("CMACR_StartValue")]
    public float? CmacrStartValue { get; set; }

    [Column("CMACR_EndValue")]
    public float? CmacrEndValue { get; set; }

    [Column("CMACR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmacrName { get; set; }

    [Column("CMACR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmacrColor { get; set; }

    [Column("CMACR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmacrDesc { get; set; }

    [Column("CMACR_FLAG")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CmacrFlag { get; set; }

    [Column("CMACR_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CmacrStatus { get; set; }

    [Column("CMACR_CrBy")]
    public int? CmacrCrBy { get; set; }

    [Column("CMACR_CrOn", TypeName = "datetime")]
    public DateTime? CmacrCrOn { get; set; }

    [Column("CMACR_UpdatedBy")]
    public int? CmacrUpdatedBy { get; set; }

    [Column("CMACR_UpdatedOn", TypeName = "datetime")]
    public DateTime? CmacrUpdatedOn { get; set; }

    [Column("CMACR_ApprovedBy")]
    public int? CmacrApprovedBy { get; set; }

    [Column("CMACR_ApprovedOn", TypeName = "datetime")]
    public DateTime? CmacrApprovedOn { get; set; }

    [Column("CMACR_RecallBy")]
    public int? CmacrRecallBy { get; set; }

    [Column("CMACR_RecallOn", TypeName = "datetime")]
    public DateTime? CmacrRecallOn { get; set; }

    [Column("CMACR_DeletedBy")]
    public int? CmacrDeletedBy { get; set; }

    [Column("CMACR_DeletedOn", TypeName = "datetime")]
    public DateTime? CmacrDeletedOn { get; set; }

    [Column("CMACR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmacrIpaddress { get; set; }

    [Column("CMACR_CompId")]
    public int? CmacrCompId { get; set; }
}
