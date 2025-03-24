using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RCSA_Assign_History")]
public partial class RiskRcsaAssignHistory
{
    [Column("RCSAAH_PKID")]
    public int? RcsaahPkid { get; set; }

    [Column("RCSAAH_RCSAAPKID")]
    public int? RcsaahRcsaapkid { get; set; }

    [Column("RCSAAH_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RcsaahComments { get; set; }

    [Column("RCSAAH_UserID")]
    public int? RcsaahUserId { get; set; }

    [Column("RCSAAH_Date", TypeName = "datetime")]
    public DateTime? RcsaahDate { get; set; }

    [Column("RCSAAH_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcsaahStatus { get; set; }

    [Column("RCSAAH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcsaahIpaddress { get; set; }

    [Column("RCSAAH_CompID")]
    public int? RcsaahCompId { get; set; }
}
