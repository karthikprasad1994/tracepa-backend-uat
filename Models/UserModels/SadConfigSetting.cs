using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_Config_Settings")]
public partial class SadConfigSetting
{
    [Column("SAD_Config_ID")]
    public int? SadConfigId { get; set; }

    [Column("SAD_Config_Key")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SadConfigKey { get; set; }

    [Column("SAD_Config_Value")]
    [StringLength(250)]
    [Unicode(false)]
    public string? SadConfigValue { get; set; }

    [Column("SAD_UpdatedBy")]
    public int? SadUpdatedBy { get; set; }

    [Column("SAD_UpdatedOn", TypeName = "datetime")]
    public DateTime? SadUpdatedOn { get; set; }

    [Column("SAD_Config_Operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? SadConfigOperation { get; set; }

    [Column("SAD_Config_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SadConfigIpaddress { get; set; }

    [Column("SAD_CompID")]
    public int? SadCompId { get; set; }
}
