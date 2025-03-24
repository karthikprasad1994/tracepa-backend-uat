using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_PROCESS_MASTER_Log")]
public partial class MstProcessMasterLog
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

    [Column("PM_ID")]
    public int? PmId { get; set; }

    [Column("PM_ENT_ID")]
    public int? PmEntId { get; set; }

    [Column("nPM_ENT_ID")]
    public int? NPmEntId { get; set; }

    [Column("PM_SEM_ID")]
    public int? PmSemId { get; set; }

    [Column("nPM_SEM_ID")]
    public int? NPmSemId { get; set; }

    [Column("PM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PmCode { get; set; }

    [Column("nPM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NPmCode { get; set; }

    [Column("PM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? PmName { get; set; }

    [Column("nPM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? NPmName { get; set; }

    [Column("PM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? PmDesc { get; set; }

    [Column("nPM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NPmDesc { get; set; }

    [Column("PM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? PmIpaddress { get; set; }

    [Column("PM_CompID")]
    public int? PmCompId { get; set; }
}
