using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditSchedule
{
    public int? SaId { get; set; }

    public string? SaAuditNo { get; set; }

    public int? SaCustId { get; set; }

    public int? SaYearId { get; set; }

    public int? SaAuditTypeId { get; set; }

    public string? SaPartnerId { get; set; }

    public string? SaReviewPartnerId { get; set; }

    public string? SaAdditionalSupportEmployeeId { get; set; }

    public string? SaScopeOfAudit { get; set; }

    public int? SaStatus { get; set; }

    public int? SaAttachId { get; set; }

    public int? SaCrBy { get; set; }

    public DateTime? SaCrOn { get; set; }

    public int? SaUpdatedBy { get; set; }

    public DateTime? SaUpdatedOn { get; set; }

    public string? SaIpaddress { get; set; }

    public int? SaCompId { get; set; }

    public DateTime? SaStartDate { get; set; }

    public DateTime? SaExpCompDate { get; set; }

    public DateTime? SaRptFilDate { get; set; }

    public DateTime? SaRptRvDate { get; set; }

    public DateTime? SaMrsdate { get; set; }

    public DateTime? SaAuditOpinionDate { get; set; }

    public DateTime? SaFilingDateSec { get; set; }

    public DateTime? SaMrldate { get; set; }

    public DateTime? SaFilingDatePcaob { get; set; }

    public DateTime? SaBinderCompletedDate { get; set; }

    public int? SaIntervalId { get; set; }

    public string? SaEngagementPartnerId { get; set; }

    public int? SaSignedBy { get; set; }

    public string? SaUdin { get; set; }

    public DateTime? SaUdindate { get; set; }
}
