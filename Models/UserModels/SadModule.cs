using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_Module")]
public partial class SadModule
{
    [Column("Mod_ID")]
    public int? ModId { get; set; }

    [Column("Mod_Code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ModCode { get; set; }

    [Column("Mod_Description")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ModDescription { get; set; }

    [Column("Mod_Notes")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ModNotes { get; set; }

    [Column("Mod_Parent")]
    public int? ModParent { get; set; }

    [Column("Mod_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? ModDelflag { get; set; }

    [Column("Mod_NavFunc")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ModNavFunc { get; set; }

    [Column("Mod_CompID")]
    public int? ModCompId { get; set; }

    [Column("Mod_Buttons")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ModButtons { get; set; }
}
