using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_FixedAssetAdditionDel")]
public partial class AccFixedAssetAdditionDel
{
    [Column("AFAA_ID")]
    public int? AfaaId { get; set; }

    [Column("AFAA_AssetTrType")]
    public int? AfaaAssetTrType { get; set; }

    [Column("AFAA_CurrencyType")]
    public int? AfaaCurrencyType { get; set; }

    [Column("AFAA_CurrencyAmnt", TypeName = "money")]
    public decimal? AfaaCurrencyAmnt { get; set; }

    [Column("AFAA_Location")]
    public int? AfaaLocation { get; set; }

    [Column("AFAA_Division")]
    public int? AfaaDivision { get; set; }

    [Column("AFAA_Department")]
    public int? AfaaDepartment { get; set; }

    [Column("AFAA_Bay")]
    public int? AfaaBay { get; set; }

    [Column("AFAA_ActualLocn")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfaaActualLocn { get; set; }

    [Column("AFAA_SupplierName")]
    public int? AfaaSupplierName { get; set; }

    [Column("AFAA_SupplierCode")]
    public int? AfaaSupplierCode { get; set; }

    [Column("AFAA_TrType")]
    public int? AfaaTrType { get; set; }

    [Column("AFAA_AssetType")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfaaAssetType { get; set; }

    [Column("AFAA_AssetNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfaaAssetNo { get; set; }

    [Column("AFAA_AssetRefNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfaaAssetRefNo { get; set; }

    [Column("AFAA_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfaaDescription { get; set; }

    [Column("AFAA_ItemCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfaaItemCode { get; set; }

    [Column("AFAA_ItemDescription")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfaaItemDescription { get; set; }

    [Column("AFAA_Quantity")]
    public int? AfaaQuantity { get; set; }

    [Column("AFAA_CommissionDate", TypeName = "datetime")]
    public DateTime? AfaaCommissionDate { get; set; }

    [Column("AFAA_PurchaseDate", TypeName = "datetime")]
    public DateTime? AfaaPurchaseDate { get; set; }

    [Column("AFAA_AssetAge", TypeName = "money")]
    public decimal? AfaaAssetAge { get; set; }

    [Column("AFAA_AssetAmount", TypeName = "money")]
    public decimal? AfaaAssetAmount { get; set; }

    [Column("AFAA_FYAmount", TypeName = "money")]
    public decimal? AfaaFyamount { get; set; }

    [Column("AFAA_DepreAmount", TypeName = "money")]
    public decimal? AfaaDepreAmount { get; set; }

    [Column("AFAA_AssetDelID")]
    public int? AfaaAssetDelId { get; set; }

    [Column("AFAA_AssetDelDate", TypeName = "datetime")]
    public DateTime? AfaaAssetDelDate { get; set; }

    [Column("AFAA_AssetDeletionDate", TypeName = "datetime")]
    public DateTime? AfaaAssetDeletionDate { get; set; }

    [Column("AFAA_Assetvalue", TypeName = "money")]
    public decimal? AfaaAssetvalue { get; set; }

    [Column("AFAA_AssetDesc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfaaAssetDesc { get; set; }

    [Column("AFAA_CreatedBy")]
    public int? AfaaCreatedBy { get; set; }

    [Column("AFAA_CreatedOn", TypeName = "datetime")]
    public DateTime? AfaaCreatedOn { get; set; }

    [Column("AFAA_UpdatedBy")]
    public int? AfaaUpdatedBy { get; set; }

    [Column("AFAA_UpdatedOn", TypeName = "datetime")]
    public DateTime? AfaaUpdatedOn { get; set; }

    [Column("AFAA_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AfaaStatus { get; set; }

    [Column("AFAA_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AfaaDelflag { get; set; }

    [Column("AFAA_YearID")]
    public int? AfaaYearId { get; set; }

    [Column("AFAA_CompID")]
    public int? AfaaCompId { get; set; }

    [Column("AFAA_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AfaaOperation { get; set; }

    [Column("AFAA_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfaaIpaddress { get; set; }

    [Column("AFAA_AddnType")]
    [StringLength(5)]
    [Unicode(false)]
    public string? AfaaAddnType { get; set; }

    [Column("AFAA_DelnType")]
    [StringLength(5)]
    [Unicode(false)]
    public string? AfaaDelnType { get; set; }

    [Column("AFAA_Depreciation", TypeName = "money")]
    public decimal? AfaaDepreciation { get; set; }

    [Column("AFAA_AddtnDate", TypeName = "datetime")]
    public DateTime? AfaaAddtnDate { get; set; }

    [Column("AFAA_ApprovedBy")]
    public int? AfaaApprovedBy { get; set; }

    [Column("AFAA_ApprovedOn", TypeName = "datetime")]
    public DateTime? AfaaApprovedOn { get; set; }

    [Column("AFAA_ItemType")]
    public int? AfaaItemType { get; set; }

    [Column("AFAA_CustId")]
    public int? AfaaCustId { get; set; }
}
