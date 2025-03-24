using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("TRACe_Color_Master")]
public partial class TraceColorMaster
{
    [Column("TC_ID")]
    public int TcId { get; set; }

    [Column("TC_Color_Name")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TcColorName { get; set; }

    [Column("TC_Color_HEX")]
    [StringLength(10)]
    [Unicode(false)]
    public string? TcColorHex { get; set; }

    [Column("TC_KeyCode", TypeName = "decimal(5, 0)")]
    public decimal? TcKeyCode { get; set; }

    [Column("TC_AccessCode")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TcAccessCode { get; set; }

    [Column("TC_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TcStatus { get; set; }

    [Column("TC_CompID")]
    public int? TcCompId { get; set; }
}
