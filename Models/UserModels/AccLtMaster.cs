using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_LT_Master")]
public partial class AccLtMaster
{
    [Column("Acc_JE_ID")]
    public int? AccJeId { get; set; }

    [Column("Acc_JE_TransactionNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AccJeTransactionNo { get; set; }

    [Column("Acc_JE_Party")]
    public int? AccJeParty { get; set; }

    [Column("Acc_JE_Location")]
    public int? AccJeLocation { get; set; }

    [Column("Acc_JE_BillType")]
    public int? AccJeBillType { get; set; }

    [Column("Acc_JE_BillNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AccJeBillNo { get; set; }

    [Column("Acc_JE_BillDate", TypeName = "datetime")]
    public DateTime? AccJeBillDate { get; set; }

    [Column("Acc_JE_BillAmount", TypeName = "money")]
    public decimal? AccJeBillAmount { get; set; }

    [Column("Acc_JE_AdvanceAmount", TypeName = "money")]
    public decimal? AccJeAdvanceAmount { get; set; }

    [Column("Acc_JE_AdvanceNaration")]
    [Unicode(false)]
    public string? AccJeAdvanceNaration { get; set; }

    [Column("Acc_JE_BalanceAmount", TypeName = "money")]
    public decimal? AccJeBalanceAmount { get; set; }

    [Column("Acc_JE_NetAmount", TypeName = "money")]
    public decimal? AccJeNetAmount { get; set; }

    [Column("Acc_JE_PaymentNarration")]
    [Unicode(false)]
    public string? AccJePaymentNarration { get; set; }

    [Column("Acc_JE_ChequeNo")]
    [Unicode(false)]
    public string? AccJeChequeNo { get; set; }

    [Column("Acc_JE_ChequeDate", TypeName = "datetime")]
    public DateTime? AccJeChequeDate { get; set; }

    [Column("Acc_JE_IFSCCode")]
    [Unicode(false)]
    public string? AccJeIfsccode { get; set; }

    [Column("Acc_JE_BankName")]
    [Unicode(false)]
    public string? AccJeBankName { get; set; }

    [Column("Acc_JE_BranchName")]
    [Unicode(false)]
    public string? AccJeBranchName { get; set; }

    [Column("Acc_JE_CreatedBy")]
    public int? AccJeCreatedBy { get; set; }

    [Column("Acc_JE_CreatedOn", TypeName = "datetime")]
    public DateTime? AccJeCreatedOn { get; set; }

    [Column("Acc_JE_ApprovedBy")]
    public int? AccJeApprovedBy { get; set; }

    [Column("Acc_JE_ApprovedOn", TypeName = "datetime")]
    public DateTime? AccJeApprovedOn { get; set; }

    [Column("Acc_JE_DeletedBy")]
    public int? AccJeDeletedBy { get; set; }

    [Column("Acc_JE_DeletedOn", TypeName = "datetime")]
    public DateTime? AccJeDeletedOn { get; set; }

    [Column("Acc_JE_RecalledBy")]
    public int? AccJeRecalledBy { get; set; }

    [Column("Acc_JE_RecalledOn", TypeName = "datetime")]
    public DateTime? AccJeRecalledOn { get; set; }

    [Column("Acc_JE_YearID")]
    public int? AccJeYearId { get; set; }

    [Column("Acc_JE_CompID")]
    public int? AccJeCompId { get; set; }

    [Column("Acc_JE_Status")]
    [StringLength(40)]
    [Unicode(false)]
    public string? AccJeStatus { get; set; }

    [Column("Acc_JE_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AccJeOperation { get; set; }

    [Column("Acc_JE_IPAddress")]
    [StringLength(200)]
    [Unicode(false)]
    public string? AccJeIpaddress { get; set; }

    [Column("Acc_JE_BillCreatedDate", TypeName = "datetime")]
    public DateTime? AccJeBillCreatedDate { get; set; }

    [Column("acc_JE_BranchId")]
    public int? AccJeBranchId { get; set; }

    [Column("Acc_JE_Comnments")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AccJeComnments { get; set; }

    [Column("Acc_JE_QuarterId")]
    public int? AccJeQuarterId { get; set; }
}
