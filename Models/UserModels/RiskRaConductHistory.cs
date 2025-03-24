using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RA_Conduct_History")]
public partial class RiskRaConductHistory
{
    [Column("RAAH_PKID")]
    public int? RaahPkid { get; set; }

    [Column("RAAH_RAAPKID")]
    public int? RaahRaapkid { get; set; }

    [Column("RAAH_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RaahComments { get; set; }

    [Column("RAAH_UserID")]
    public int? RaahUserId { get; set; }

    [Column("RAAH_Date", TypeName = "datetime")]
    public DateTime? RaahDate { get; set; }

    [Column("RAAH_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RaahStatus { get; set; }

    [Column("RAAH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RaahIpaddress { get; set; }

    [Column("RAAH_CompID")]
    public int? RaahCompId { get; set; }
}
