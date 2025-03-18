using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaSubProcess
{
    public int CaspId { get; set; }

    public string? CaspCode { get; set; }

    public string? CaspSubprocessname { get; set; }

    public int? CaspPoints { get; set; }

    public int? CaspSectionid { get; set; }

    public int? CaspSubsectionid { get; set; }

    public int? CaspProcessid { get; set; }

    public string? CaspDesc { get; set; }

    public string? CaspDelflg { get; set; }

    public DateTime? CaspCron { get; set; }

    public int? CaspCrby { get; set; }

    public int? CaspApprovedby { get; set; }

    public DateTime? CaspApprovedon { get; set; }

    public string? CaspStatus { get; set; }

    public int? CaspUpdatedby { get; set; }

    public DateTime? CaspUpdatedon { get; set; }

    public int? CaspDeletedby { get; set; }

    public DateTime? CaspDeletedon { get; set; }

    public int? CaspRecallby { get; set; }

    public DateTime? CaspRecallon { get; set; }

    public string? CaspIpaddress { get; set; }

    public int? CaspCompId { get; set; }

    public int? CaspYearid { get; set; }
}
