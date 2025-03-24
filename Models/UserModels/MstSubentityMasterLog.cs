using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_SUBENTITY_MASTER_Log")]
public partial class MstSubentityMasterLog
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

    [Column("SEM_ID")]
    public int? SemId { get; set; }

    [Column("SEM_Ent_ID")]
    public int? SemEntId { get; set; }

    [Column("nSEM_Ent_ID")]
    public int? NSemEntId { get; set; }

    [Column("SEM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SemCode { get; set; }

    [Column("nSEM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NSemCode { get; set; }

    [Column("SEM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SemName { get; set; }

    [Column("nSEM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? NSemName { get; set; }

    [Column("SEM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SemDesc { get; set; }

    [Column("nSEM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NSemDesc { get; set; }

    [Column("SEM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SemIpaddress { get; set; }

    [Column("SEM_CompID")]
    public int? SemCompId { get; set; }
}
