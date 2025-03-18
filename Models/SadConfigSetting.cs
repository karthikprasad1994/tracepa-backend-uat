using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadConfigSetting
{
    public int? SadConfigId { get; set; }

    public string? SadConfigKey { get; set; }

    public string? SadConfigValue { get; set; }

    public int? SadUpdatedBy { get; set; }

    public DateTime? SadUpdatedOn { get; set; }

    public string? SadConfigOperation { get; set; }

    public string? SadConfigIpaddress { get; set; }

    public int? SadCompId { get; set; }
}
