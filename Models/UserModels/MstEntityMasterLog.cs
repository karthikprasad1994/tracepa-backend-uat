using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_Entity_Master_log")]
public partial class MstEntityMasterLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(30)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("ENT_ID")]
    public int? EntId { get; set; }

    [Column("ENT_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? EntCode { get; set; }

    [Column("nENT_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NEntCode { get; set; }

    [Column("ENT_ENTITYNAME")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? EntEntityname { get; set; }

    [Column("nENT_ENTITYNAME")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? NEntEntityname { get; set; }

    [Column("Ent_Branch")]
    [StringLength(4)]
    [Unicode(false)]
    public string? EntBranch { get; set; }

    [Column("nEnt_Branch")]
    [StringLength(4)]
    [Unicode(false)]
    public string? NEntBranch { get; set; }

    [Column("ENT_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? EntDesc { get; set; }

    [Column("nENT_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NEntDesc { get; set; }

    [Column("ENT_FunOwnerID")]
    public int? EntFunOwnerId { get; set; }

    [Column("nENT_FunOwnerID")]
    public int? NEntFunOwnerId { get; set; }

    [Column("ENT_CompId")]
    public int? EntCompId { get; set; }

    [Column("ENT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? EntIpaddress { get; set; }

    [Column("Ent_FunManagerID")]
    public int? EntFunManagerId { get; set; }

    [Column("Ent_FunSPOCID")]
    public int? EntFunSpocid { get; set; }

    [Column("nEnt_FunManagerID")]
    public int? NEntFunManagerId { get; set; }

    [Column("nEnt_FunSPOCID")]
    public int? NEntFunSpocid { get; set; }
}
