using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_Schedule")]
public partial class StandardAuditSchedule
{
    [Column("SA_ID")]
    public int? SaId { get; set; }

    [Column("SA_AuditNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SaAuditNo { get; set; }

    [Column("SA_CustID")]
    public int? SaCustId { get; set; }

    [Column("SA_YearID")]
    public int? SaYearId { get; set; }

    [Column("SA_AuditTypeID")]
    public int? SaAuditTypeId { get; set; }

    [Column("SA_PartnerID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SaPartnerId { get; set; }

    [Column("SA_ReviewPartnerID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SaReviewPartnerId { get; set; }

    [Column("SA_AdditionalSupportEmployeeID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SaAdditionalSupportEmployeeId { get; set; }

    [Column("SA_ScopeOfAudit")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? SaScopeOfAudit { get; set; }

    [Column("SA_Status")]
    public int? SaStatus { get; set; }

    [Column("SA_SignedBy")]
    public int? SaSignedBy { get; set; }

    [Column("SA_UDIN")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SaUdin { get; set; }

    [Column("SA_UDINdate", TypeName = "datetime")]
    public DateTime? SaUdindate { get; set; }

    [Column("SA_StartDate", TypeName = "datetime")]
    public DateTime? SaStartDate { get; set; }

    [Column("SA_ExpCompDate", TypeName = "datetime")]
    public DateTime? SaExpCompDate { get; set; }

    [Column("SA_RptRvDate", TypeName = "datetime")]
    public DateTime? SaRptRvDate { get; set; }

    [Column("SA_RptFilDate", TypeName = "datetime")]
    public DateTime? SaRptFilDate { get; set; }

    [Column("SA_MRSDate", TypeName = "datetime")]
    public DateTime? SaMrsdate { get; set; }

    [Column("SA_AttachID")]
    public int? SaAttachId { get; set; }

    [Column("SA_CrBy")]
    public int? SaCrBy { get; set; }

    [Column("SA_CrOn", TypeName = "datetime")]
    public DateTime? SaCrOn { get; set; }

    [Column("SA_UpdatedBy")]
    public int? SaUpdatedBy { get; set; }

    [Column("SA_UpdatedOn", TypeName = "datetime")]
    public DateTime? SaUpdatedOn { get; set; }

    [Column("SA_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SaIpaddress { get; set; }

    [Column("SA_CompID")]
    public int? SaCompId { get; set; }
}
