using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_Config_Settings_Log")]
public partial class SadConfigSettingsLog
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

    [Column("sad_config_ID")]
    public int? SadConfigId { get; set; }

    [Column("sad_Config_Key")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SadConfigKey { get; set; }

    [Column("nsad_Config_Key")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NsadConfigKey { get; set; }

    [Column("sad_Config_Value")]
    [StringLength(250)]
    [Unicode(false)]
    public string? SadConfigValue { get; set; }

    [Column("nsad_Config_Value")]
    [StringLength(250)]
    [Unicode(false)]
    public string? NsadConfigValue { get; set; }

    [Column("SAD_RunDate", TypeName = "datetime")]
    public DateTime? SadRunDate { get; set; }

    [Column("SAD_CompID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SadCompId { get; set; }

    [Column("sad_Config_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SadConfigIpaddress { get; set; }
}
