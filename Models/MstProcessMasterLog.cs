using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstProcessMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? PmId { get; set; }

    public int? PmEntId { get; set; }

    public int? NPmEntId { get; set; }

    public int? PmSemId { get; set; }

    public int? NPmSemId { get; set; }

    public string? PmCode { get; set; }

    public string? NPmCode { get; set; }

    public string? PmName { get; set; }

    public string? NPmName { get; set; }

    public string? PmDesc { get; set; }

    public string? NPmDesc { get; set; }

    public string? PmIpaddress { get; set; }

    public int? PmCompId { get; set; }
}
