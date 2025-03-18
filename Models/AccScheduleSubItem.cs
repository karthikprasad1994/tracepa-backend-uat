using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccScheduleSubItem
{
    public int AssiId { get; set; }

    public string? AssiName { get; set; }

    public string? AssiDelflg { get; set; }

    public DateTime? AssiCron { get; set; }

    public int? AssiCrby { get; set; }

    public int? AssiApprovedby { get; set; }

    public DateTime? AssiApprovedon { get; set; }

    public string? AssiStatus { get; set; }

    public int? AssiUpdatedby { get; set; }

    public DateTime? AssiUpdatedon { get; set; }

    public int? AssiDeletedby { get; set; }

    public DateTime? AssiDeletedon { get; set; }

    public int? AssiRecallby { get; set; }

    public DateTime? AssiRecallon { get; set; }

    public string? AssiIpaddress { get; set; }

    public int? AssiCompId { get; set; }

    public int? AssiYearid { get; set; }

    public int? AssiHeadingId { get; set; }

    public int? AssiSubHeadingId { get; set; }

    public int? AssiItemsId { get; set; }

    public string? AssiCode { get; set; }

    public int? AssiScheduletype { get; set; }

    public int? AssiOrgtype { get; set; }
}
