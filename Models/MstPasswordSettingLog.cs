using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstPasswordSettingLog
{
    public int LogPkid { get; set; }

    public DateTime? LogDate { get; set; }

    public string? LogOperation { get; set; }

    public int? LogUserId { get; set; }

    public int? MpsPkid { get; set; }

    public int? MpsMinimumChar { get; set; }

    public int? NMpsMinimumChar { get; set; }

    public int? MpsMaximumChar { get; set; }

    public int? NMpsMaximumChar { get; set; }

    public int? MspRecoveryAttempts { get; set; }

    public int? NMspRecoveryAttempts { get; set; }

    public int? MpsUnSuccessfulAttempts { get; set; }

    public int? NMpsUnSuccessfulAttempts { get; set; }

    public int? MpsPasswordExpiryDays { get; set; }

    public int? NMpsPasswordExpiryDays { get; set; }

    public int? MpsNotLoginDays { get; set; }

    public int? NMpsNotLoginDays { get; set; }

    public string? MpsPasswordContains { get; set; }

    public string? NMpsPasswordContains { get; set; }

    public int? MpsPasswordExpiryAlertDays { get; set; }

    public int? NMpsPasswordExpiryAlertDays { get; set; }

    public DateTime? MpsRunDate { get; set; }

    public string? MpsIpaddress { get; set; }

    public string? MpsCompId { get; set; }
}
