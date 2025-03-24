using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_GroupingAlias")]
public partial class AccGroupingAlias
{
    [Column("AGA_ID")]
    public int? AgaId { get; set; }

    [Column("AGA_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AgaDescription { get; set; }

    [Column("AGA_GLID")]
    public int? AgaGlid { get; set; }

    [Column("AGA_GLDESC")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AgaGldesc { get; set; }

    [Column("AGA_GrpLevel")]
    public int? AgaGrpLevel { get; set; }

    [Column("AGA_scheduletype")]
    public int? AgaScheduletype { get; set; }

    [Column("AGA_Orgtype")]
    public int? AgaOrgtype { get; set; }

    [Column("AGA_Compid")]
    public int? AgaCompid { get; set; }

    [Column("AGA_Status")]
    [StringLength(10)]
    [Unicode(false)]
    public string? AgaStatus { get; set; }

    [Column("AGA_Createdby")]
    public int? AgaCreatedby { get; set; }

    [Column("AGA_CreatedOn", TypeName = "datetime")]
    public DateTime? AgaCreatedOn { get; set; }

    [Column("AGA_IPaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AgaIpaddress { get; set; }
}
