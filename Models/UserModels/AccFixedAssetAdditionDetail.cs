using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_FixedAssetAdditionDetails")]
public partial class AccFixedAssetAdditionDetail
{
    [Column("FAAD_PKID")]
    public int? FaadPkid { get; set; }

    [Column("FAAD_MasID")]
    public int? FaadMasId { get; set; }

    [Column("FAAD_Location")]
    public int? FaadLocation { get; set; }

    [Column("FAAD_Division")]
    public int? FaadDivision { get; set; }

    [Column("FAAD_Department")]
    public int? FaadDepartment { get; set; }

    [Column("FAAD_Bay")]
    public int? FaadBay { get; set; }

    [Column("FAAD_Particulars")]
    [StringLength(250)]
    [Unicode(false)]
    public string? FaadParticulars { get; set; }

    [Column("FAAD_DocNo")]
    [StringLength(25)]
    [Unicode(false)]
    public string? FaadDocNo { get; set; }

    [Column("FAAD_DocDate", TypeName = "datetime")]
    public DateTime? FaadDocDate { get; set; }

    [Column("FAAD_chkCost")]
    public int? FaadChkCost { get; set; }

    [Column("FAAD_BasicCost", TypeName = "money")]
    public decimal? FaadBasicCost { get; set; }

    [Column("FAAD_TaxAmount", TypeName = "money")]
    public decimal? FaadTaxAmount { get; set; }

    [Column("FAAD_Total", TypeName = "money")]
    public decimal? FaadTotal { get; set; }

    [Column("FAAD_AssetValue", TypeName = "money")]
    public decimal? FaadAssetValue { get; set; }

    [Column("FAAD_CreatedBy")]
    public int? FaadCreatedBy { get; set; }

    [Column("FAAD_CreatedOn", TypeName = "datetime")]
    public DateTime? FaadCreatedOn { get; set; }

    [Column("FAAD_UpdatedBy")]
    public int? FaadUpdatedBy { get; set; }

    [Column("FAAD_UpdatedOn", TypeName = "datetime")]
    public DateTime? FaadUpdatedOn { get; set; }

    [Column("FAAD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? FaadIpaddress { get; set; }

    [Column("FAAD_CompID")]
    public int? FaadCompId { get; set; }

    [Column("FAAD_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? FaadStatus { get; set; }

    [Column("FAAD_AssetType")]
    public int? FaadAssetType { get; set; }

    [Column("FAAD_ItemType")]
    public int? FaadItemType { get; set; }

    [Column("FAAD_SupplierName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? FaadSupplierName { get; set; }

    [Column("FAAD_CustId")]
    public int? FaadCustId { get; set; }

    [Column("FAAD_YearID")]
    public int? FaadYearId { get; set; }

    [Column("FAAD_InitDep")]
    public int? FaadInitDep { get; set; }

    [Column("FAAD_Delflag")]
    [StringLength(20)]
    [Unicode(false)]
    public string? FaadDelflag { get; set; }

    [Column("FAAD_OtherTrType")]
    public int? FaadOtherTrType { get; set; }

    [Column("FAAD_OtherTrAmount")]
    [StringLength(100)]
    [Unicode(false)]
    public string? FaadOtherTrAmount { get; set; }
}
