using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_Settings_Log")]
public partial class EdtSettingsLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("SET_ID")]
    public int? SetId { get; set; }

    [Column("SET_CODE")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SetCode { get; set; }

    [Column("nSET_CODE")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NSetCode { get; set; }

    [Column("SET_Value")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SetValue { get; set; }

    [Column("nSET_Value")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NSetValue { get; set; }

    [Column("SAD_RunDate", TypeName = "datetime")]
    public DateTime? SadRunDate { get; set; }

    [Column("SET_CompID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SetCompId { get; set; }

    [Column("SET_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SetIpaddress { get; set; }

    [Column("SET_Operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? SetOperation { get; set; }

    [Column("nSET_Operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NSetOperation { get; set; }
}
