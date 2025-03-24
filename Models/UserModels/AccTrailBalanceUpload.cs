using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_TrailBalance_Upload")]
public partial class AccTrailBalanceUpload
{
    [Column("ATBU_ID")]
    public int AtbuId { get; set; }

    [Column("ATBU_CODE")]
    [StringLength(50)]
    [Unicode(false)]
    public string AtbuCode { get; set; } = null!;

    [Column("ATBU_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AtbuDescription { get; set; }

    [Column("ATBU_CustId")]
    public int? AtbuCustId { get; set; }

    [Column("ATBU_Branchid")]
    public int? AtbuBranchid { get; set; }

    [Column("ATBU_Opening_Debit_Amount", TypeName = "money")]
    public decimal? AtbuOpeningDebitAmount { get; set; }

    [Column("ATBU_Opening_Credit_Amount", TypeName = "money")]
    public decimal? AtbuOpeningCreditAmount { get; set; }

    [Column("ATBU_TR_Debit_Amount", TypeName = "money")]
    public decimal? AtbuTrDebitAmount { get; set; }

    [Column("ATBU_TR_Credit_Amount", TypeName = "money")]
    public decimal? AtbuTrCreditAmount { get; set; }

    [Column("ATBU_Closing_Debit_Amount", TypeName = "money")]
    public decimal? AtbuClosingDebitAmount { get; set; }

    [Column("ATBU_Closing_Credit_Amount", TypeName = "money")]
    public decimal? AtbuClosingCreditAmount { get; set; }

    [Column("ATBU_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AtbuDelflg { get; set; }

    [Column("ATBU_CRON", TypeName = "datetime")]
    public DateTime? AtbuCron { get; set; }

    [Column("ATBU_CRBY")]
    public int? AtbuCrby { get; set; }

    [Column("ATBU_APPROVEDBY")]
    public int? AtbuApprovedby { get; set; }

    [Column("ATBU_APPROVEDON", TypeName = "datetime")]
    public DateTime? AtbuApprovedon { get; set; }

    [Column("ATBU_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AtbuStatus { get; set; }

    [Column("ATBU_UPDATEDBY")]
    public int? AtbuUpdatedby { get; set; }

    [Column("ATBU_UPDATEDON", TypeName = "datetime")]
    public DateTime? AtbuUpdatedon { get; set; }

    [Column("ATBU_DELETEDBY")]
    public int? AtbuDeletedby { get; set; }

    [Column("ATBU_DELETEDON", TypeName = "datetime")]
    public DateTime? AtbuDeletedon { get; set; }

    [Column("ATBU_RECALLBY")]
    public int? AtbuRecallby { get; set; }

    [Column("ATBU_RECALLON", TypeName = "datetime")]
    public DateTime? AtbuRecallon { get; set; }

    [Column("ATBU_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AtbuIpaddress { get; set; }

    [Column("ATBU_CompId")]
    public int? AtbuCompId { get; set; }

    [Column("ATBU_YEARId")]
    public int? AtbuYearid { get; set; }

    [Column("ATBU_Closing_TotalDebit_Amount", TypeName = "money")]
    public decimal? AtbuClosingTotalDebitAmount { get; set; }

    [Column("ATBU_Closing_TotalCredit_Amount", TypeName = "money")]
    public decimal? AtbuClosingTotalCreditAmount { get; set; }

    [Column("ATBU_Progress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? AtbuProgress { get; set; }

    [Column("ATBU_QuarterId")]
    public int? AtbuQuarterId { get; set; }
}
