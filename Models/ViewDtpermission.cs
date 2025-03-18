using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ViewDtpermission
{
    public int? DotDoctypeid { get; set; }

    public string? DotDocname { get; set; }

    public string? DotNote { get; set; }

    public int? DotPgroup { get; set; }

    public int? DotCrby { get; set; }

    public DateTime? DotCron { get; set; }

    public string? DotStatus { get; set; }

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
}
