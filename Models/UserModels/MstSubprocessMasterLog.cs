using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_SUBPROCESS_MASTER_Log")]
public partial class MstSubprocessMasterLog
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

    [Column("SPM_ID")]
    public int? SpmId { get; set; }

    [Column("SPM_ENT_ID")]
    public int? SpmEntId { get; set; }

    [Column("nSPM_ENT_ID")]
    public int? NSpmEntId { get; set; }

    [Column("SPM_SEM_ID")]
    public int? SpmSemId { get; set; }

    [Column("nSPM_SEM_ID")]
    public int? NSpmSemId { get; set; }

    [Column("SPM_PM_ID")]
    public int? SpmPmId { get; set; }

    [Column("nSPM_PM_ID")]
    public int? NSpmPmId { get; set; }

    [Column("SPM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SpmCode { get; set; }

    [Column("nSPM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NSpmCode { get; set; }

    [Column("SPM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SpmName { get; set; }

    [Column("nSPM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? NSpmName { get; set; }

    [Column("SPM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SpmDesc { get; set; }

    [Column("nSPM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NSpmDesc { get; set; }

    [Column("SPM_IsKey")]
    public int? SpmIsKey { get; set; }

    [Column("nSPM_IsKey")]
    public int? NSpmIsKey { get; set; }

    [Column("SPM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SpmIpaddress { get; set; }

    [Column("SPM_CompID")]
    public int? SpmCompId { get; set; }
}
