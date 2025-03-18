using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstPasswordSetting
{
    public int? MpsPkId { get; set; }

    public int? MpsMinimumChar { get; set; }

    public int? MpsMaximumChar { get; set; }

    public int? MpsRecoveryAttempts { get; set; }

    public int? MpsUnsuccessfulAttempts { get; set; }

    public int? MpsPasswordExpiryDays { get; set; }

    public int? MpsNotLoginDays { get; set; }

    public string? MpsPasswordContains { get; set; }

    public int? MpsPasswordExpiryAlertDays { get; set; }

    public int? MpsUpdatedBy { get; set; }

    public DateTime? MpsUpdatedOn { get; set; }

    public string? MpsOperation { get; set; }

    public string? MpsIpaddress { get; set; }

    public int? MpsCompId { get; set; }
}
