using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UsrOrGrp_Permission")]
public partial class SadUsrOrGrpPermission
{
    [Column("Perm_PKID")]
    public int? PermPkid { get; set; }

    [Column("Perm_PType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PermPtype { get; set; }

    [Column("Perm_UsrORGrpID")]
    public int? PermUsrOrgrpId { get; set; }

    [Column("Perm_ModuleID")]
    public int? PermModuleId { get; set; }

    [Column("Perm_OpPKID")]
    public int? PermOpPkid { get; set; }

    [Column("Perm_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PermStatus { get; set; }

    [Column("Perm_Crby")]
    public int? PermCrby { get; set; }

    [Column("Perm_Cron", TypeName = "datetime")]
    public DateTime? PermCron { get; set; }

    [Column("Perm_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PermOperation { get; set; }

    [Column("Perm_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? PermIpaddress { get; set; }

    [Column("Perm_CompID")]
    public int? PermCompId { get; set; }
}
