using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_Password_Setting")]
public partial class MstPasswordSetting
{
    [Column("MPS_pkID")]
    public int? MpsPkId { get; set; }

    [Column("MPS_MinimumChar")]
    public int? MpsMinimumChar { get; set; }

    [Column("MPS_MaximumChar")]
    public int? MpsMaximumChar { get; set; }

    [Column("MPS_RecoveryAttempts")]
    public int? MpsRecoveryAttempts { get; set; }

    [Column("MPS_UnsuccessfulAttempts")]
    public int? MpsUnsuccessfulAttempts { get; set; }

    [Column("MPS_PasswordExpiryDays")]
    public int? MpsPasswordExpiryDays { get; set; }

    [Column("MPS_NotLoginDays")]
    public int? MpsNotLoginDays { get; set; }

    [Column("MPS_Password_Contains")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MpsPasswordContains { get; set; }

    [Column("MPS_PasswordExpiryAlertDays")]
    public int? MpsPasswordExpiryAlertDays { get; set; }

    [Column("MPS_UpdatedBy")]
    public int? MpsUpdatedBy { get; set; }

    [Column("MPS_UpdatedOn", TypeName = "datetime")]
    public DateTime? MpsUpdatedOn { get; set; }

    [Column("MPS_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? MpsOperation { get; set; }

    [Column("MPS_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MpsIpaddress { get; set; }

    [Column("MPS_CompID")]
    public int? MpsCompId { get; set; }
}
