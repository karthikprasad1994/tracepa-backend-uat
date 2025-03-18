using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccScheduleHeading
{
    public int AshId { get; set; }

    public string? AshName { get; set; }

    public string? AshDelflg { get; set; }

    public DateTime? AshCron { get; set; }

    public int? AshCrby { get; set; }

    public int? AshApprovedby { get; set; }

    public DateTime? AshApprovedon { get; set; }

    public string? AshStatus { get; set; }

    public int? AshUpdatedby { get; set; }

    public DateTime? AshUpdatedon { get; set; }

    public int? AshDeletedby { get; set; }

    public DateTime? AshDeletedon { get; set; }

    public int? AshRecallby { get; set; }

    public DateTime? AshRecallon { get; set; }

    public string? AshIpaddress { get; set; }

    public int? AshCompId { get; set; }

    public int? AshYearid { get; set; }

    public decimal? AshTotal { get; set; }

    public string? AshCode { get; set; }

    public int? AshNotes { get; set; }

    public int? AshScheduletype { get; set; }

    public int? AshOrgtype { get; set; }
}
