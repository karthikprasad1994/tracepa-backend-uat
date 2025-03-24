using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_Mod_Operations")]
public partial class SadModOperation
{
    [Column("OP_PKID")]
    public int? OpPkid { get; set; }

    [Column("OP_ModuleID")]
    public int? OpModuleId { get; set; }

    [Column("OP_OperationCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OpOperationCode { get; set; }

    [Column("OP_OperationName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OpOperationName { get; set; }

    [Column("OP_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OpStatus { get; set; }

    [Column("OP_CompID")]
    public int? OpCompId { get; set; }
}
