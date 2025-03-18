using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstRiskLibraryLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? MrlPkid { get; set; }

    public string? MrlRiskName { get; set; }

    public string? NMrlRiskName { get; set; }

    public string? MrlRiskDesc { get; set; }

    public string? NMrlRiskDesc { get; set; }

    public string? MrlCode { get; set; }

    public string? NMrlCode { get; set; }

    public int? MrlRiskTypeId { get; set; }

    public int? NMrlRiskTypeId { get; set; }

    public int? MrlIsKey { get; set; }

    public int? NMrlIsKey { get; set; }

    public string? MrlIpaddress { get; set; }

    public int? MrlCompId { get; set; }
}
