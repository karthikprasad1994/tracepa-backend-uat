using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UsrOrGrp_Permission_Log")]
public partial class SadUsrOrGrpPermissionLog
{
    [Column("Log_PKID")]
    public long LogPkid { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("Perm_PKID")]
    public int? PermPkid { get; set; }

    [Column("Perm_PType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PermPtype { get; set; }

    [Column("nPerm_PType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NPermPtype { get; set; }

    [Column("Perm_UsrORGrpID")]
    public int? PermUsrOrgrpId { get; set; }

    [Column("nPerm_UsrORGrpID")]
    public int? NPermUsrOrgrpId { get; set; }

    [Column("Perm_ModuleID")]
    public int? PermModuleId { get; set; }

    [Column("nPerm_ModuleID")]
    public int? NPermModuleId { get; set; }

    [Column("Perm_OpPKID")]
    public int? PermOpPkid { get; set; }

    [Column("nPerm_OpPKID")]
    public int? NPermOpPkid { get; set; }

    [Column("Perm_CompID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? PermCompId { get; set; }

    [Column("Perm_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? PermIpaddress { get; set; }
}
