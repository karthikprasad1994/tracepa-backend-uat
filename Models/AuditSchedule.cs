using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditSchedule
{
    public int? AudId { get; set; }

    public string? AudCode { get; set; }

    public int? AudYearId { get; set; }

    public int? AudMonthId { get; set; }

    public int? AudSectionId { get; set; }

    public int? AudKitchenId { get; set; }

    public string? AudAuditorIds { get; set; }

    public DateTime? AudFromDate { get; set; }

    public DateTime? AudToDate { get; set; }

    public int? AudIntmail { get; set; }

    public int? AudFirstmail { get; set; }

    public int? AudSecondMail { get; set; }

    public int? AudCrBy { get; set; }

    public DateTime? AudCrOn { get; set; }

    public int? AudUpdatedBy { get; set; }

    public DateTime? AudUpdateOn { get; set; }

    public int? AudApprovedBy { get; set; }

    public DateTime? AudApprovedOn { get; set; }

    public string? AudStatus { get; set; }

    public int? AudCompId { get; set; }

    public string? AudOperation { get; set; }

    public string? AudIpaddress { get; set; }
}
