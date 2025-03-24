using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_InherentRisk_Master")]
public partial class MstInherentRiskMaster
{
    [Column("MIM_ID")]
    public int? MimId { get; set; }

    [Column("MIM_Name")]
    [StringLength(500)]
    [Unicode(false)]
    public string? MimName { get; set; }

    [Column("MIM_Desc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? MimDesc { get; set; }

    [Column("MIM_Color")]
    [StringLength(500)]
    [Unicode(false)]
    public string? MimColor { get; set; }

    [Column("MIM_FromScore")]
    public int? MimFromScore { get; set; }

    [Column("MIM_ToScore")]
    public int? MimToScore { get; set; }

    [Column("MIM_Frequency")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MimFrequency { get; set; }

    [Column("MIM_CrBy")]
    public int? MimCrBy { get; set; }

    [Column("MIM_CrOn", TypeName = "datetime")]
    public DateTime? MimCrOn { get; set; }

    [Column("MIM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MimIpaddress { get; set; }

    [Column("MIM_CompID")]
    public int? MimCompId { get; set; }
}
