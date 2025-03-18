using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUsrOrGrpPermissionLog
{
    public long LogPkid { get; set; }

    public DateTime? LogDate { get; set; }

    public string? LogOperation { get; set; }

    public int? LogUserId { get; set; }

    public int? PermPkid { get; set; }

    public string? PermPtype { get; set; }

    public string? NPermPtype { get; set; }

    public int? PermUsrOrgrpId { get; set; }

    public int? NPermUsrOrgrpId { get; set; }

    public int? PermModuleId { get; set; }

    public int? NPermModuleId { get; set; }

    public int? PermOpPkid { get; set; }

    public int? NPermOpPkid { get; set; }

    public string? PermCompId { get; set; }

    public string? PermIpaddress { get; set; }
}
