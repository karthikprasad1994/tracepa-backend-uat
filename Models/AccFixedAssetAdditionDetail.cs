using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccFixedAssetAdditionDetail
{
    public int? FaadPkid { get; set; }

    public int? FaadMasId { get; set; }

    public int? FaadLocation { get; set; }

    public int? FaadDivision { get; set; }

    public int? FaadDepartment { get; set; }

    public int? FaadBay { get; set; }

    public string? FaadParticulars { get; set; }

    public string? FaadDocNo { get; set; }

    public DateTime? FaadDocDate { get; set; }

    public int? FaadChkCost { get; set; }

    public decimal? FaadBasicCost { get; set; }

    public decimal? FaadTaxAmount { get; set; }

    public decimal? FaadTotal { get; set; }

    public decimal? FaadAssetValue { get; set; }

    public int? FaadCreatedBy { get; set; }

    public DateTime? FaadCreatedOn { get; set; }

    public int? FaadUpdatedBy { get; set; }

    public DateTime? FaadUpdatedOn { get; set; }

    public string? FaadIpaddress { get; set; }

    public int? FaadCompId { get; set; }

    public string? FaadStatus { get; set; }

    public int? FaadAssetType { get; set; }

    public int? FaadItemType { get; set; }

    public string? FaadSupplierName { get; set; }

    public int? FaadCustId { get; set; }

    public int? FaadYearId { get; set; }

    public int? FaadInitDep { get; set; }

    public string? FaadDelflag { get; set; }

    public int? FaadOtherTrType { get; set; }

    public string? FaadOtherTrAmount { get; set; }
}
