using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccFixedAssetsTransaction
{
    public int AccFatId { get; set; }

    public int? AccFatIndType { get; set; }

    public int? AccFatCustId { get; set; }

    public int? AccFatReportHeaderId { get; set; }

    public int? AccFatFixedAssetsId { get; set; }

    public int? AccFatFixedAssetsSubId { get; set; }

    public int? AccFatAdditionId { get; set; }

    public int? AccFatAdditionSubid { get; set; }

    public int? AccFatAccountHeadId { get; set; }

    public int? AccFatChartofAccountId { get; set; }

    public decimal? AccFatAdditon { get; set; }

    public decimal? AccFatTransfer { get; set; }

    public decimal? AccFatReduction { get; set; }

    public decimal? AccFatSold { get; set; }

    public decimal? AccFatRtransfer { get; set; }

    public decimal? AccFatRreduction { get; set; }

    public decimal? AccFatRrateoff { get; set; }

    public decimal? AccFatRopnBal { get; set; }

    public decimal? AccFatDfortheYear { get; set; }

    public decimal? AccFatDdeduction { get; set; }

    public decimal? AccFatDclsBal { get; set; }

    public decimal? AccFatMopnBal { get; set; }

    public decimal? AccFatMclsBal { get; set; }
}
