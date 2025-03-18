using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadConfigSettingsLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? SadConfigId { get; set; }

    public string? SadConfigKey { get; set; }

    public string? NsadConfigKey { get; set; }

    public string? SadConfigValue { get; set; }

    public string? NsadConfigValue { get; set; }

    public DateTime? SadRunDate { get; set; }

    public string? SadCompId { get; set; }

    public string? SadConfigIpaddress { get; set; }
}
