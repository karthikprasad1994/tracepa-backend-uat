using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditExecutiveSummary
{
    public int? AesPkid { get; set; }

    public int? AesYearId { get; set; }

    public int? AesAuditCode { get; set; }

    public int? AesCustId { get; set; }

    public int? AesFunctionId { get; set; }

    public DateTime? AesIssuanceDate { get; set; }

    public int? AesAuditRatingId { get; set; }

    public string? AesIntroduction { get; set; }

    public string? AesBusinessOverview { get; set; }

    public string? AesAuditScope { get; set; }

    public string? AesAuditScopeOut { get; set; }

    public string? AesKeyAuditObservation { get; set; }

    public DateTime? AesAuditPeriodStartDate { get; set; }

    public DateTime? AesAuditPeriodEndDate { get; set; }

    public DateTime? AesActualPeriodStartDate { get; set; }

    public DateTime? AesActualPeriodEndDate { get; set; }

    public string? AesAuditRating { get; set; }

    public string? AesAuditRemarks { get; set; }

    public string? AesComment { get; set; }

    public string? AesStatus { get; set; }

    public int? AesAttchId { get; set; }

    public int? AesCrBy { get; set; }

    public DateTime? AesCrOn { get; set; }

    public int? AesUpdatedBy { get; set; }

    public DateTime? AesUpdatedOn { get; set; }

    public int? AesRevieweddBy { get; set; }

    public DateTime? AesReviewedOn { get; set; }

    public int? AesSubmittedBy { get; set; }

    public DateTime? AesSubmittedOn { get; set; }

    public string? AesIpaddress { get; set; }

    public int? AesCompId { get; set; }

    public int? AesPgedetailId { get; set; }
}
