using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstControlLibraryLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? MclPkid { get; set; }

    public string? MclControlName { get; set; }

    public string? NMclControlName { get; set; }

    public string? MclControlDesc { get; set; }

    public string? NMclControlDesc { get; set; }

    public string? MclCode { get; set; }

    public string? NMclCode { get; set; }

    public int? MclIsKey { get; set; }

    public int? NMclIsKey { get; set; }

    public string? MclIpaddress { get; set; }

    public int? MclCompId { get; set; }
}
