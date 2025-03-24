using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_FixedAssetMaster")]
public partial class AccFixedAssetMaster
{
    [Column("AFAM_ID")]
    public int? AfamId { get; set; }

    [Column("AFAM_AssetType")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamAssetType { get; set; }

    [Column("AFAM_AssetCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamAssetCode { get; set; }

    [Column("AFAM_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamDescription { get; set; }

    [Column("AFAM_ItemCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamItemCode { get; set; }

    [Column("AFAM_ItemDescription")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamItemDescription { get; set; }

    [Column("AFAM_CommissionDate", TypeName = "datetime")]
    public DateTime? AfamCommissionDate { get; set; }

    [Column("AFAM_PurchaseDate", TypeName = "datetime")]
    public DateTime? AfamPurchaseDate { get; set; }

    [Column("AFAM_Quantity")]
    public int? AfamQuantity { get; set; }

    [Column("AFAM_Unit")]
    public int? AfamUnit { get; set; }

    [Column("AFAM_AssetAge", TypeName = "money")]
    public decimal? AfamAssetAge { get; set; }

    [Column("AFAM_PurchaseAmount", TypeName = "money")]
    public decimal? AfamPurchaseAmount { get; set; }

    [Column("AFAM_PolicyNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamPolicyNo { get; set; }

    [Column("AFAM_Amount", TypeName = "money")]
    public decimal? AfamAmount { get; set; }

    [Column("AFAM_BrokerName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamBrokerName { get; set; }

    [Column("AFAM_CompanyName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamCompanyName { get; set; }

    [Column("AFAM_Date", TypeName = "datetime")]
    public DateTime? AfamDate { get; set; }

    [Column("AFAM_ToDate", TypeName = "datetime")]
    public DateTime? AfamToDate { get; set; }

    [Column("AFAM_Location")]
    public int? AfamLocation { get; set; }

    [Column("AFAM_Division")]
    public int? AfamDivision { get; set; }

    [Column("AFAM_Department")]
    public int? AfamDepartment { get; set; }

    [Column("AFAM_Bay")]
    public int? AfamBay { get; set; }

    [Column("AFAM_EmployeeName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamEmployeeName { get; set; }

    [Column("AFAM_EmployeeCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamEmployeeCode { get; set; }

    [Column("AFAM_Code")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamCode { get; set; }

    [Column("AFAM_SuplierName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamSuplierName { get; set; }

    [Column("AFAM_ContactPerson")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamContactPerson { get; set; }

    [Column("AFAM_Address")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamAddress { get; set; }

    [Column("AFAM_Phone")]
    [StringLength(15)]
    [Unicode(false)]
    public string? AfamPhone { get; set; }

    [Column("AFAM_Fax")]
    [StringLength(10)]
    [Unicode(false)]
    public string? AfamFax { get; set; }

    [Column("AFAM_EmailID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamEmailId { get; set; }

    [Column("AFAM_Website")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamWebsite { get; set; }

    [Column("AFAM_CreatedBy")]
    public int? AfamCreatedBy { get; set; }

    [Column("AFAM_CreatedOn", TypeName = "datetime")]
    public DateTime? AfamCreatedOn { get; set; }

    [Column("AFAM_UpdatedBy")]
    public int? AfamUpdatedBy { get; set; }

    [Column("AFAM_UpdatedOn", TypeName = "datetime")]
    public DateTime? AfamUpdatedOn { get; set; }

    [Column("AFAM_DelFlag")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamDelFlag { get; set; }

    [Column("AFAM_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamStatus { get; set; }

    [Column("AFAM_YearID")]
    public int? AfamYearId { get; set; }

    [Column("AFAM_CompID")]
    public int? AfamCompId { get; set; }

    [Column("AFAM_Opeartion")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AfamOpeartion { get; set; }

    [Column("AFAM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AfamIpaddress { get; set; }

    [Column("AFAM_WrntyDesc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamWrntyDesc { get; set; }

    [Column("AFAM_ContactPrsn")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamContactPrsn { get; set; }

    [Column("AFAM_AMCFrmDate", TypeName = "datetime")]
    public DateTime? AfamAmcfrmDate { get; set; }

    [Column("AFAM_AMCTo", TypeName = "datetime")]
    public DateTime? AfamAmcto { get; set; }

    [Column("AFAM_Contprsn")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamContprsn { get; set; }

    [Column("AFAM_PhoneNo")]
    [StringLength(15)]
    [Unicode(false)]
    public string? AfamPhoneNo { get; set; }

    [Column("AFAM_AMCCompanyName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamAmccompanyName { get; set; }

    [Column("AFAM_AssetDeletion")]
    public int? AfamAssetDeletion { get; set; }

    [Column("AFAM_DlnDate", TypeName = "datetime")]
    public DateTime? AfamDlnDate { get; set; }

    [Column("AFAM_DateOfDeletion", TypeName = "datetime")]
    public DateTime? AfamDateOfDeletion { get; set; }

    [Column("AFAM_Value", TypeName = "money")]
    public decimal? AfamValue { get; set; }

    [Column("AFAM_Remark")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamRemark { get; set; }

    [Column("AFAM_EMPCode")]
    [StringLength(10)]
    [Unicode(false)]
    public string? AfamEmpcode { get; set; }

    [Column("AFAM_LToWhom")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfamLtoWhom { get; set; }

    [Column("AFAM_LAmount", TypeName = "money")]
    public decimal? AfamLamount { get; set; }

    [Column("AFAM_LAggriNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfamLaggriNo { get; set; }

    [Column("AFAM_LDate", TypeName = "datetime")]
    public DateTime? AfamLdate { get; set; }

    [Column("AFAM_LCurrencyType")]
    public int? AfamLcurrencyType { get; set; }

    [Column("AFAM_LExchDate", TypeName = "datetime")]
    public DateTime? AfamLexchDate { get; set; }

    [Column("AFAM_CustId")]
    public int? AfamCustId { get; set; }

    [Column("AFAM_TRStatus")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamTrstatus { get; set; }

    [Column("AFAM_TRYear")]
    public int? AfamTryear { get; set; }

    [Column("AFAM_TRAssetType")]
    public int? AfamTrassetType { get; set; }

    [Column("AFAM_TrAssetAge", TypeName = "money")]
    public decimal? AfamTrAssetAge { get; set; }

    [Column("AFAM_TrUpdatedBy")]
    public int? AfamTrUpdatedBy { get; set; }

    [Column("AFAM_Attribute1")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamAttribute1 { get; set; }

    [Column("AFAM_Attribute2")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamAttribute2 { get; set; }

    [Column("AFAM_Attribute3")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfamAttribute3 { get; set; }
}
