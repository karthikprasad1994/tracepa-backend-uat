using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ITReturnsFiling_Details")]
public partial class ItreturnsFilingDetail
{
    [Column("ITRFD_ID")]
    public int? ItrfdId { get; set; }

    [Column("ITRFD_ITR_ID")]
    public int? ItrfdItrId { get; set; }

    [Column("ITRFD_ITRNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ItrfdItrno { get; set; }

    [Column("ITRFD_FinancialYearID")]
    public int? ItrfdFinancialYearId { get; set; }

    [Column("ITRFD_AssessmentYearID")]
    public int? ItrfdAssessmentYearId { get; set; }

    [Column("ITRFD_ServiceChargeInINR", TypeName = "decimal(19, 2)")]
    public decimal? ItrfdServiceChargeInInr { get; set; }

    [Column("ITRFD_Status")]
    public int? ItrfdStatus { get; set; }

    [Column("ITRFD_InvoiceMail")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ItrfdInvoiceMail { get; set; }

    [Column("ITRFD_AssignTo")]
    public int? ItrfdAssignTo { get; set; }

    [Column("ITRFD_BillingEntityId")]
    public int? ItrfdBillingEntityId { get; set; }

    [Column("ITRFD_CrBy")]
    public int? ItrfdCrBy { get; set; }

    [Column("ITRFD_CrOn", TypeName = "datetime")]
    public DateTime? ItrfdCrOn { get; set; }

    [Column("ITRFD_UpdatedBy")]
    public int? ItrfdUpdatedBy { get; set; }

    [Column("ITRFD_UpdateOn", TypeName = "datetime")]
    public DateTime? ItrfdUpdateOn { get; set; }

    [Column("ITRFD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ItrfdIpaddress { get; set; }

    [Column("ITRFD_CompID")]
    public int? ItrfdCompId { get; set; }
}
