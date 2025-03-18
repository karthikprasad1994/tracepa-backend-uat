using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAssignmentSchedule
{
    public int? AasId { get; set; }

    public string? AasAssignmentNo { get; set; }

    public int? AasCustId { get; set; }

    public int? AasPartnerId { get; set; }

    public int? AasYearId { get; set; }

    public int? AasMonthId { get; set; }

    public int? AasTaskId { get; set; }

    public int? AasStatus { get; set; }

    public int? AasAttachId { get; set; }

    public int? AasCrBy { get; set; }

    public DateTime? AasCrOn { get; set; }

    public int? AasUpdatedBy { get; set; }

    public DateTime? AasUpdatedOn { get; set; }

    public string? AasIpaddress { get; set; }

    public int? AasCompId { get; set; }

    public int? AasAdvancePartialBilling { get; set; }

    public int? AasBillingType { get; set; }

    public string? AasAssessmentYearId { get; set; }

    public int? AasIsComplianceAsg { get; set; }

    public string? AasFolderPath { get; set; }
}
