using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstEntityMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? EntId { get; set; }

    public string? EntCode { get; set; }

    public string? NEntCode { get; set; }

    public string? EntEntityname { get; set; }

    public string? NEntEntityname { get; set; }

    public string? EntBranch { get; set; }

    public string? NEntBranch { get; set; }

    public string? EntDesc { get; set; }

    public string? NEntDesc { get; set; }

    public int? EntFunOwnerId { get; set; }

    public int? NEntFunOwnerId { get; set; }

    public int? EntCompId { get; set; }

    public string? EntIpaddress { get; set; }

    public int? EntFunManagerId { get; set; }

    public int? EntFunSpocid { get; set; }

    public int? NEntFunManagerId { get; set; }

    public int? NEntFunSpocid { get; set; }
}
