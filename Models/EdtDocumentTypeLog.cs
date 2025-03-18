using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtDocumentTypeLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? DotDoctypeid { get; set; }

    public string? DotDocname { get; set; }

    public string? NDotDocname { get; set; }

    public string? DotNote { get; set; }

    public string? NDotNote { get; set; }

    public int? DotPgroup { get; set; }

    public int? NDotPgroup { get; set; }

    public string? DotOperation { get; set; }

    public string? NdotOperation { get; set; }

    public int? DotOperationby { get; set; }

    public int? NdotOperationby { get; set; }

    public int? DotIsGlobal { get; set; }

    public int? NDotIsGlobal { get; set; }

    public string? DotDelFlag { get; set; }

    public string? NDotDelFlag { get; set; }

    public int? DotCompId { get; set; }

    public string? DotIpaddress { get; set; }
}
