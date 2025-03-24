using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_ExecutiveSummary")]
public partial class AuditExecutiveSummary
{
    [Column("AES_PKID")]
    public int? AesPkid { get; set; }

    [Column("AES_YearID")]
    public int? AesYearId { get; set; }

    [Column("AES_AuditCode")]
    public int? AesAuditCode { get; set; }

    [Column("AES_CustID")]
    public int? AesCustId { get; set; }

    [Column("AES_FunctionID")]
    public int? AesFunctionId { get; set; }

    [Column("AES_IssuanceDate", TypeName = "datetime")]
    public DateTime? AesIssuanceDate { get; set; }

    [Column("AES_AuditRatingID")]
    public int? AesAuditRatingId { get; set; }

    [Column("AES_Introduction")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesIntroduction { get; set; }

    [Column("AES_BusinessOverview")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesBusinessOverview { get; set; }

    [Column("AES_AuditScope")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesAuditScope { get; set; }

    [Column("AES_AuditScopeOut")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesAuditScopeOut { get; set; }

    [Column("AES_KeyAuditObservation")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesKeyAuditObservation { get; set; }

    [Column("AES_AuditPeriodStartDate", TypeName = "datetime")]
    public DateTime? AesAuditPeriodStartDate { get; set; }

    [Column("AES_AuditPeriodEndDate", TypeName = "datetime")]
    public DateTime? AesAuditPeriodEndDate { get; set; }

    [Column("AES_ActualPeriodStartDate", TypeName = "datetime")]
    public DateTime? AesActualPeriodStartDate { get; set; }

    [Column("AES_ActualPeriodEndDate", TypeName = "datetime")]
    public DateTime? AesActualPeriodEndDate { get; set; }

    [Column("AES_AuditRating")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesAuditRating { get; set; }

    [Column("AES_AuditRemarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesAuditRemarks { get; set; }

    [Column("AES_Comment")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AesComment { get; set; }

    [Column("AES_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AesStatus { get; set; }

    [Column("AES_AttchID")]
    public int? AesAttchId { get; set; }

    [Column("AES_CrBy")]
    public int? AesCrBy { get; set; }

    [Column("AES_CrOn", TypeName = "datetime")]
    public DateTime? AesCrOn { get; set; }

    [Column("AES_UpdatedBy")]
    public int? AesUpdatedBy { get; set; }

    [Column("AES_UpdatedOn", TypeName = "datetime")]
    public DateTime? AesUpdatedOn { get; set; }

    [Column("AES_RevieweddBy")]
    public int? AesRevieweddBy { get; set; }

    [Column("AES_ReviewedOn", TypeName = "datetime")]
    public DateTime? AesReviewedOn { get; set; }

    [Column("AES_SubmittedBy")]
    public int? AesSubmittedBy { get; set; }

    [Column("AES_SubmittedOn", TypeName = "datetime")]
    public DateTime? AesSubmittedOn { get; set; }

    [Column("AES_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AesIpaddress { get; set; }

    [Column("AES_CompID")]
    public int? AesCompId { get; set; }

    [Column("AES_PGEDetailId")]
    public int? AesPgedetailId { get; set; }
}
