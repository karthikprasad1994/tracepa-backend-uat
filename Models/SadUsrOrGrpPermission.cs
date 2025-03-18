using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUsrOrGrpPermission
{
    public int? PermPkid { get; set; }

    public string? PermPtype { get; set; }

    public int? PermUsrOrgrpId { get; set; }

    public int? PermModuleId { get; set; }

    public int? PermOpPkid { get; set; }

    public string? PermStatus { get; set; }

    public int? PermCrby { get; set; }

    public DateTime? PermCron { get; set; }

    public string? PermIpaddress { get; set; }

    public int? PermCompId { get; set; }

    public string? PermOperation { get; set; }
}
