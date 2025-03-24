using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("acc_FixedAssets_Transaction")]
public partial class AccFixedAssetsTransaction
{
    [Column("Acc_FAT_ID")]
    public int AccFatId { get; set; }

    [Column("Acc_FAT_IndType")]
    public int? AccFatIndType { get; set; }

    [Column("Acc_FAT_CustID")]
    public int? AccFatCustId { get; set; }

    [Column("Acc_FAT_ReportHeaderID")]
    public int? AccFatReportHeaderId { get; set; }

    [Column("Acc_FAT_FixedAssetsID")]
    public int? AccFatFixedAssetsId { get; set; }

    [Column("Acc_FAT_FixedAssetsSubID")]
    public int? AccFatFixedAssetsSubId { get; set; }

    [Column("Acc_FAT_AdditionID")]
    public int? AccFatAdditionId { get; set; }

    [Column("Acc_FAT_AdditionSUBID")]
    public int? AccFatAdditionSubid { get; set; }

    [Column("Acc_FAT_AccountHeadID")]
    public int? AccFatAccountHeadId { get; set; }

    [Column("Acc_FAT_ChartofAccountID")]
    public int? AccFatChartofAccountId { get; set; }

    [Column("Acc_FAT_Additon", TypeName = "money")]
    public decimal? AccFatAdditon { get; set; }

    [Column("Acc_FAT_Transfer", TypeName = "money")]
    public decimal? AccFatTransfer { get; set; }

    [Column("Acc_FAT_Reduction", TypeName = "money")]
    public decimal? AccFatReduction { get; set; }

    [Column("Acc_FAT_Sold", TypeName = "money")]
    public decimal? AccFatSold { get; set; }

    [Column("Acc_FAT_RTransfer", TypeName = "money")]
    public decimal? AccFatRtransfer { get; set; }

    [Column("Acc_FAT_RReduction", TypeName = "money")]
    public decimal? AccFatRreduction { get; set; }

    [Column("Acc_FAT_RRateoff", TypeName = "money")]
    public decimal? AccFatRrateoff { get; set; }

    [Column("Acc_FAT_ROpnBal", TypeName = "money")]
    public decimal? AccFatRopnBal { get; set; }

    [Column("Acc_FAT_DFortheYear", TypeName = "money")]
    public decimal? AccFatDfortheYear { get; set; }

    [Column("Acc_FAT_DDeduction", TypeName = "money")]
    public decimal? AccFatDdeduction { get; set; }

    [Column("Acc_FAT_DClsBal", TypeName = "money")]
    public decimal? AccFatDclsBal { get; set; }

    [Column("Acc_FAT_MOpnBal", TypeName = "money")]
    public decimal? AccFatMopnBal { get; set; }

    [Column("Acc_FAT_MClsBal", TypeName = "money")]
    public decimal? AccFatMclsBal { get; set; }
}
