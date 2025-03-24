using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_Entity_UsrMap")]
public partial class MstEntityUsrMap
{
    [Column("MEUM_PKID")]
    public int? MeumPkid { get; set; }

    [Column("MEUM_EntityID")]
    public int? MeumEntityId { get; set; }

    [Column("MEUM_UsrID")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? MeumUsrId { get; set; }

    [Column("MEUM_CompID")]
    public int? MeumCompId { get; set; }
}
