using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_Password_Setting_Log")]
public partial class MstPasswordSettingLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("MPS_PKID")]
    public int? MpsPkid { get; set; }

    [Column("MPS_MinimumChar")]
    public int? MpsMinimumChar { get; set; }

    [Column("nMPS_MinimumChar")]
    public int? NMpsMinimumChar { get; set; }

    [Column("MPS_MaximumChar")]
    public int? MpsMaximumChar { get; set; }

    [Column("nMPS_MaximumChar")]
    public int? NMpsMaximumChar { get; set; }

    [Column("MSP_RecoveryAttempts")]
    public int? MspRecoveryAttempts { get; set; }

    [Column("nMSP_RecoveryAttempts")]
    public int? NMspRecoveryAttempts { get; set; }

    [Column("MPS_UnSuccessfulAttempts")]
    public int? MpsUnSuccessfulAttempts { get; set; }

    [Column("nMPS_UnSuccessfulAttempts")]
    public int? NMpsUnSuccessfulAttempts { get; set; }

    [Column("MPS_PasswordExpiryDays")]
    public int? MpsPasswordExpiryDays { get; set; }

    [Column("nMPS_PasswordExpiryDays")]
    public int? NMpsPasswordExpiryDays { get; set; }

    [Column("MPS_NotLoginDays")]
    public int? MpsNotLoginDays { get; set; }

    [Column("nMPS_NotLoginDays")]
    public int? NMpsNotLoginDays { get; set; }

    [Column("MPS_Password_Contains")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MpsPasswordContains { get; set; }

    [Column("nMPS_Password_Contains")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NMpsPasswordContains { get; set; }

    [Column("MPS_PasswordExpiryAlertDays")]
    public int? MpsPasswordExpiryAlertDays { get; set; }

    [Column("nMPS_PasswordExpiryAlertDays")]
    public int? NMpsPasswordExpiryAlertDays { get; set; }

    [Column("MPS_RunDate", TypeName = "datetime")]
    public DateTime? MpsRunDate { get; set; }

    [Column("MPS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MpsIpaddress { get; set; }

    [Column("MPS_CompID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MpsCompId { get; set; }
}
