using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtDoctypePermission
{
    public int? EdpPid { get; set; }

    public int? EdpDoctypeid { get; set; }

    public string? EdpPtype { get; set; }

    public short EdpGrpid { get; set; }

    public short EdpUsrid { get; set; }

    public byte? EdpIndex { get; set; }

    public byte? EdpSearch { get; set; }

    public byte? EdpMfyType { get; set; }

    public byte? EdpMfyDocument { get; set; }

    public byte? EdpDelDocument { get; set; }

    public byte? EdpOther { get; set; }

    public string? EdpWhen { get; set; }

    public string? EdpStatus { get; set; }

    public int? EdpCrby { get; set; }

    public DateTime? EdpCron { get; set; }

    public int? EdpUpdatedby { get; set; }

    public DateTime? EdpUpdatedon { get; set; }

    public int? EdpRecallby { get; set; }

    public DateTime? EdpRecallon { get; set; }

    public int? EdpDeletedby { get; set; }

    public DateTime? EdpDeletedon { get; set; }

    public int? EdpApprovedby { get; set; }

    public DateTime? EdpApprovedon { get; set; }

    public int? EdpCompId { get; set; }

    public string? EdpIpaddress { get; set; }
}
