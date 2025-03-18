using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccScheduleSubHeading
{
    public int AsshId { get; set; }

    public string? AsshName { get; set; }

    public string? AsshDelflg { get; set; }

    public DateTime? AsshCron { get; set; }

    public int? AsshCrby { get; set; }

    public int? AsshApprovedby { get; set; }

    public DateTime? AsshApprovedon { get; set; }

    public string? AsshStatus { get; set; }

    public int? AsshUpdatedby { get; set; }

    public DateTime? AsshUpdatedon { get; set; }

    public int? AsshDeletedby { get; set; }

    public DateTime? AsshDeletedon { get; set; }

    public int? AsshRecallby { get; set; }

    public DateTime? AsshRecallon { get; set; }

    public string? AsshIpaddress { get; set; }

    public int? AsshCompId { get; set; }

    public int? AsshYearid { get; set; }

    public int? AsshHeadingId { get; set; }

    public string? AsshCode { get; set; }

    public int? AsshNotes { get; set; }

    public int? AsshScheduletype { get; set; }

    public int? AsshOrgtype { get; set; }
}
