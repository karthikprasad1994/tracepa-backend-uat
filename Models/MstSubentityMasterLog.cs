using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstSubentityMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? SemId { get; set; }

    public int? SemEntId { get; set; }

    public int? NSemEntId { get; set; }

    public string? SemCode { get; set; }

    public string? NSemCode { get; set; }

    public string? SemName { get; set; }

    public string? NSemName { get; set; }

    public string? SemDesc { get; set; }

    public string? NSemDesc { get; set; }

    public string? SemIpaddress { get; set; }

    public int? SemCompId { get; set; }
}
