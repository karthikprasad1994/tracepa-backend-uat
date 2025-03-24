using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UsersInOtherDept")]
public partial class SadUsersInOtherDept
{
    [Column("SUO_PKID")]
    public int? SuoPkid { get; set; }

    [Column("SUO_UserID")]
    public int? SuoUserId { get; set; }

    [Column("SUO_DeptID")]
    public int? SuoDeptId { get; set; }

    [Column("SUO_IsDeptHead")]
    public int? SuoIsDeptHead { get; set; }

    [Column("SUO_CreatedBy")]
    public int? SuoCreatedBy { get; set; }

    [Column("SUO_CreatedOn", TypeName = "datetime")]
    public DateTime? SuoCreatedOn { get; set; }

    [Column("SUO_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SuoIpaddress { get; set; }

    [Column("SUO_CompID")]
    public int? SuoCompId { get; set; }
}
