using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("TRACe_SubCabinet")]
public partial class TraceSubCabinet
{
    [Column("TSC_PKID")]
    public int? TscPkid { get; set; }

    [Column("TSC_CabinetID")]
    public int? TscCabinetId { get; set; }

    [Column("TSC_Name")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? TscName { get; set; }

    [Column("TSC_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? TscRemarks { get; set; }

    [Column("TSC_Decs")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? TscDecs { get; set; }

    [Column("TSC_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TscStatus { get; set; }

    [Column("TSC_CrOn", TypeName = "datetime")]
    public DateTime? TscCrOn { get; set; }

    [Column("TSC_CrBy")]
    public int? TscCrBy { get; set; }

    [Column("TSC_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TscIpaddress { get; set; }

    [Column("TSC_CompID")]
    public int? TscCompId { get; set; }
}
