using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstChecksMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? ChkId { get; set; }

    public int? ChkControlId { get; set; }

    public int? NChkControlId { get; set; }

    public string? ChkCheckName { get; set; }

    public string? NChkCheckName { get; set; }

    public string? ChkCheckDesc { get; set; }

    public string? NChkCheckDesc { get; set; }

    public int? ChkCatId { get; set; }

    public int? NChkCatId { get; set; }

    public int? ChkIsKey { get; set; }

    public int? NChkIsKey { get; set; }

    public string? ChkIpaddress { get; set; }

    public int? ChkCompId { get; set; }
}
