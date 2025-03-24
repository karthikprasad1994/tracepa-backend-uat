using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ajax1")]
public partial class Ajax1
{
    [Column("id")]
    public int Id { get; set; }

    [Column("engine")]
    [StringLength(255)]
    [Unicode(false)]
    public string Engine { get; set; } = null!;

    [Column("browser")]
    [StringLength(255)]
    [Unicode(false)]
    public string Browser { get; set; } = null!;

    [Column("platform1")]
    [StringLength(255)]
    [Unicode(false)]
    public string Platform1 { get; set; } = null!;

    [Column("version1")]
    public double Version1 { get; set; }

    [Column("grade")]
    [StringLength(20)]
    [Unicode(false)]
    public string Grade { get; set; } = null!;

    [Column("marketshare", TypeName = "decimal(18, 2)")]
    public decimal Marketshare { get; set; }

    [Column("released", TypeName = "datetime")]
    public DateTime Released { get; set; }
}
