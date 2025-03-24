using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_Transactions_Details")]
public partial class AccTransactionsDetail
{
    [Column("ATD_ID")]
    public int? AtdId { get; set; }

    [Column("ATD_TransactionDate", TypeName = "datetime")]
    public DateTime? AtdTransactionDate { get; set; }

    [Column("ATD_TrType")]
    public int? AtdTrType { get; set; }

    [Column("ATD_BillId")]
    public int? AtdBillId { get; set; }

    [Column("ATD_PaymentType")]
    public int? AtdPaymentType { get; set; }

    [Column("ATD_Head")]
    public int? AtdHead { get; set; }

    [Column("ATD_GL")]
    public int? AtdGl { get; set; }

    [Column("ATD_SubGL")]
    public int? AtdSubGl { get; set; }

    [Column("ATD_DbOrCr")]
    public int? AtdDbOrCr { get; set; }

    [Column("ATD_Debit", TypeName = "money")]
    public decimal? AtdDebit { get; set; }

    [Column("ATD_Credit", TypeName = "money")]
    public decimal? AtdCredit { get; set; }

    [Column("ATD_CreatedBy")]
    public int? AtdCreatedBy { get; set; }

    [Column("ATD_CreatedOn", TypeName = "datetime")]
    public DateTime? AtdCreatedOn { get; set; }

    [Column("ATD_UpdatedBy")]
    public int? AtdUpdatedBy { get; set; }

    [Column("ATD_UpdatedOn", TypeName = "datetime")]
    public DateTime? AtdUpdatedOn { get; set; }

    [Column("ATD_ApprovedBy")]
    public int? AtdApprovedBy { get; set; }

    [Column("ATD_ApprovedOn", TypeName = "datetime")]
    public DateTime? AtdApprovedOn { get; set; }

    [Column("ATD_Deletedby")]
    public int? AtdDeletedby { get; set; }

    [Column("ATD_DeletedOn", TypeName = "datetime")]
    public DateTime? AtdDeletedOn { get; set; }

    [Column("ATD_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AtdStatus { get; set; }

    [Column("ATD_YearID")]
    public int? AtdYearId { get; set; }

    [Column("ATD_CompID")]
    public int? AtdCompId { get; set; }

    [Column("ATD_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AtdOperation { get; set; }

    [Column("ATD_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AtdIpaddress { get; set; }

    [Column("ATD_CustId")]
    public int? AtdCustId { get; set; }

    [Column("ATD_OrgType")]
    public int? AtdOrgType { get; set; }
}
