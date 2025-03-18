using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtCollateLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? CltCollateno { get; set; }

    public string? CltCollateref { get; set; }

    public string? NCltCollateref { get; set; }

    public int? CltAllow { get; set; }

    public int? NCltAllow { get; set; }

    public string? CltComment { get; set; }

    public string? NCltComment { get; set; }

    public int? CltGroup { get; set; }

    public int? NcltGroup { get; set; }

    public int? CltOperationby { get; set; }

    public int? NcltOperationby { get; set; }

    public string? CltOperation { get; set; }

    public string? NcltOperation { get; set; }

    public string? CltDelFlag { get; set; }

    public string? NCltDelFlag { get; set; }

    public int? CltCompId { get; set; }

    public string? CltIpaddress { get; set; }
}
