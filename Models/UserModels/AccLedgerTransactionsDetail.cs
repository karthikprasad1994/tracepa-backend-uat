using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_LedgerTransactions_Details")]
public partial class AccLedgerTransactionsDetail
{
    [Column("AJTB_ID")]
    public int? AjtbId { get; set; }

    [Column("AJTB_CustId")]
    public int? AjtbCustId { get; set; }

    [Column("AJTB_Deschead")]
    public int? AjtbDeschead { get; set; }

    [Column("AJTB_Desc")]
    public int? AjtbDesc { get; set; }

    [Column("AJTB_Debit", TypeName = "money")]
    public decimal? AjtbDebit { get; set; }

    [Column("AJTB_Credit", TypeName = "money")]
    public decimal? AjtbCredit { get; set; }

    [Column("AJTB_CreatedBy")]
    public int? AjtbCreatedBy { get; set; }

    [Column("AJTB_CreatedOn", TypeName = "datetime")]
    public DateTime? AjtbCreatedOn { get; set; }

    [Column("AJTB_UpdatedBy")]
    public int? AjtbUpdatedBy { get; set; }

    [Column("AJTB_UpdatedOn", TypeName = "datetime")]
    public DateTime? AjtbUpdatedOn { get; set; }

    [Column("AJTB_ApprovedBy")]
    public int? AjtbApprovedBy { get; set; }

    [Column("AJTB_ApprovedOn", TypeName = "datetime")]
    public DateTime? AjtbApprovedOn { get; set; }

    [Column("AJTB_Deletedby")]
    public int? AjtbDeletedby { get; set; }

    [Column("AJTB_DeletedOn", TypeName = "datetime")]
    public DateTime? AjtbDeletedOn { get; set; }

    [Column("AJTB_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AjtbStatus { get; set; }

    [Column("AJTB_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AjtbIpaddress { get; set; }

    [Column("AJTB_CompID")]
    public int? AjtbCompId { get; set; }

    [Column("AJTB_YearID")]
    public int? AjtbYearId { get; set; }

    [Column("AJTB_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AjtbOperation { get; set; }

    [Column("AJTB_TranscNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AjtbTranscNo { get; set; }

    [Column("AJTB_ScheduleTypeid")]
    public int? AjtbScheduleTypeid { get; set; }

    [Column("Ajtb_Masid")]
    public int? AjtbMasid { get; set; }

    [Column("AJTB_BillType")]
    public int? AjtbBillType { get; set; }

    [Column("AJTB_DescName")]
    [Unicode(false)]
    public string? AjtbDescName { get; set; }

    public int? SeqReferenceNum { get; set; }

    [Column("AJTB_BranchId")]
    public int? AjtbBranchId { get; set; }

    [Column("AJTB_QuarterId")]
    public int? AjtbQuarterId { get; set; }
}
