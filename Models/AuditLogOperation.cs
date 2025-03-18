using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditLogOperation
{
    public long? AlpPkid { get; set; }

    public string? AlpUserName { get; set; }

    public int? AlpUserId { get; set; }

    public string? AlpPassword { get; set; }

    public DateTime? AlpDate { get; set; }

    public string? AlpLogType { get; set; }

    public string? AlpIpaddress { get; set; }

    public int? AlpCompId { get; set; }

    public DateTime? AlpLogOut { get; set; }
}
