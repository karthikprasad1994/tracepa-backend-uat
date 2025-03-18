using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstSubprocessMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? SpmId { get; set; }

    public int? SpmEntId { get; set; }

    public int? NSpmEntId { get; set; }

    public int? SpmSemId { get; set; }

    public int? NSpmSemId { get; set; }

    public int? SpmPmId { get; set; }

    public int? NSpmPmId { get; set; }

    public string? SpmCode { get; set; }

    public string? NSpmCode { get; set; }

    public string? SpmName { get; set; }

    public string? NSpmName { get; set; }

    public string? SpmDesc { get; set; }

    public string? NSpmDesc { get; set; }

    public int? SpmIsKey { get; set; }

    public int? NSpmIsKey { get; set; }

    public string? SpmIpaddress { get; set; }

    public int? SpmCompId { get; set; }
}
