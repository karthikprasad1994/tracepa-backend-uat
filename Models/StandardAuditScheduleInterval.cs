using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditScheduleInterval
{
    public int? SaiId { get; set; }

    public int? SaiSaId { get; set; }

    public int? SaiIntervalId { get; set; }

    public int? SaiIntervalSubId { get; set; }

    public DateTime? SaiStartDate { get; set; }

    public DateTime? SaiEndDate { get; set; }

    public int? SaiCrBy { get; set; }

    public DateTime? SaiCrOn { get; set; }

    public string? SaiIpaddress { get; set; }

    public int? SaiCompId { get; set; }
}
