using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditApmDetail
{
    public int? ApmId { get; set; }

    public int? ApmYearId { get; set; }

    public string? ApmAuditCode { get; set; }

    public int? ApmCustId { get; set; }

    public int? ApmFunctionId { get; set; }

    public int? ApmAuditorsRoleId { get; set; }

    public string? ApmAuditTeamsId { get; set; }

    public int? ApmBranchId { get; set; }

    public string? ApmPartnersId { get; set; }

    public DateTime? ApmTstartDate { get; set; }

    public DateTime? ApmTendDate { get; set; }

    public int? ApmEstimatedEffortDays { get; set; }

    public string? ApmObjectives { get; set; }

    public string? ApmCustomerRemarks { get; set; }

    public string? ApmAuditorsRemarks { get; set; }

    public int? ApmAuditConfirm { get; set; }

    public string? ApmAuditConfirmYes { get; set; }

    public int? ApmCrBy { get; set; }

    public DateTime? ApmCrOn { get; set; }

    public int? ApmUpdatedBy { get; set; }

    public DateTime? ApmUpdatedOn { get; set; }

    public int? ApmAppBy { get; set; }

    public DateTime? ApmAppOn { get; set; }

    public string? ApmSubject { get; set; }

    public string? ApmBody { get; set; }

    public string? ApmToemail { get; set; }

    public string? ApmCcemail { get; set; }

    public string? ApmApmstatus { get; set; }

    public string? ApmApmcrstatus { get; set; }

    public string? ApmApmtastatus { get; set; }

    public string? ApmIpaddress { get; set; }

    public int? ApmAttachId { get; set; }

    public int? ApmCompId { get; set; }

    public int? ApmStatusId { get; set; }

    public int? ApmPgedetailId { get; set; }
}
