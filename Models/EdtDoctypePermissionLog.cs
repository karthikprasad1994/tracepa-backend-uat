using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtDoctypePermissionLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? EdpPid { get; set; }

    public int? EdpDoctypeid { get; set; }

    public int? NEdpDoctypeid { get; set; }

    public string? EdpPtype { get; set; }

    public string? NEdpPtype { get; set; }

    public int? EdpGrpid { get; set; }

    public int? NEdpGrpid { get; set; }

    public int? EdpUsrid { get; set; }

    public int? NEdpUsrid { get; set; }

    public int? EdpIndex { get; set; }

    public int? NEdpIndex { get; set; }

    public int? EdpSearch { get; set; }

    public int? NEdpSearch { get; set; }

    public int? EdpMfyType { get; set; }

    public int? NEdpMfyType { get; set; }

    public int? EdpMfyDocument { get; set; }

    public int? NEdpMfyDocument { get; set; }

    public int? EdpDelDocument { get; set; }

    public int? NEdpDelDocument { get; set; }

    public int? EdpOther { get; set; }

    public int? NEdpOther { get; set; }

    public string? EdpWhen { get; set; }

    public string? NEdpWhen { get; set; }

    public int? EdpCompId { get; set; }

    public string? EdpIpaddress { get; set; }
}
