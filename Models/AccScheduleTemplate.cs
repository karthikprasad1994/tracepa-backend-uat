using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccScheduleTemplate
{
    public int AstId { get; set; }

    public int? AstHeadingId { get; set; }

    public int? AstSubHeadingId { get; set; }

    public int? AstItemId { get; set; }

    public int? AstSubItemId { get; set; }

    public string? AstDelflg { get; set; }

    public DateTime? AstCron { get; set; }

    public int? AstCrby { get; set; }

    public int? AstApprovedby { get; set; }

    public DateTime? AstApprovedon { get; set; }

    public string? AstStatus { get; set; }

    public int? AstUpdatedby { get; set; }

    public DateTime? AstUpdatedon { get; set; }

    public int? AstDeletedby { get; set; }

    public DateTime? AstDeletedon { get; set; }

    public int? AstRecallby { get; set; }

    public DateTime? AstRecallon { get; set; }

    public string? AstIpaddress { get; set; }

    public int? AstCompId { get; set; }

    public int? AstYearid { get; set; }

    public int? AstScheduleType { get; set; }

    public int? AstCompanytype { get; set; }

    public int? AstCompanyLimit { get; set; }

    public int? AstAccHeadId { get; set; }

    public int? AstNotes { get; set; }
}
