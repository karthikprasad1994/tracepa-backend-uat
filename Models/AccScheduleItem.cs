using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccScheduleItem
{
    public int AsiId { get; set; }

    public string? AsiName { get; set; }

    public string? AsiDelflg { get; set; }

    public DateTime? AsiCron { get; set; }

    public int? AsiCrby { get; set; }

    public int? AsiApprovedby { get; set; }

    public DateTime? AsiApprovedon { get; set; }

    public string? AsiStatus { get; set; }

    public int? AsiUpdatedby { get; set; }

    public DateTime? AsiUpdatedon { get; set; }

    public int? AsiDeletedby { get; set; }

    public DateTime? AsiDeletedon { get; set; }

    public int? AsiRecallby { get; set; }

    public DateTime? AsiRecallon { get; set; }

    public string? AsiIpaddress { get; set; }

    public int? AsiCompId { get; set; }

    public int? AsiYearid { get; set; }

    public int? AsiHeadingId { get; set; }

    public int? AsiSubHeadingId { get; set; }

    public string? AsiCode { get; set; }

    public int? AsiOrgtype { get; set; }

    public int? AsiScheduletype { get; set; }
}
