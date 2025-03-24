using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_FixedAssetDeletion")]
public partial class AccFixedAssetDeletion
{
    [Column("AFAD_ID")]
    public int? AfadId { get; set; }

    [Column("AFAD_CustomerName")]
    public int? AfadCustomerName { get; set; }

    [Column("AFAD_TransNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfadTransNo { get; set; }

    [Column("AFAD_Location")]
    public int? AfadLocation { get; set; }

    [Column("AFAD_Division")]
    public int? AfadDivision { get; set; }

    [Column("AFAD_Department")]
    public int? AfadDepartment { get; set; }

    [Column("AFAD_Bay")]
    public int? AfadBay { get; set; }

    [Column("AFAD_AssetClass")]
    public int? AfadAssetClass { get; set; }

    [Column("AFAD_Asset")]
    public int? AfadAsset { get; set; }

    [Column("AFAD_AssetDeletion")]
    public int? AfadAssetDeletion { get; set; }

    [Column("AFAD_AssetDeletionType")]
    public int? AfadAssetDeletionType { get; set; }

    [Column("AFAD_DeletionDate", TypeName = "datetime")]
    public DateTime? AfadDeletionDate { get; set; }

    [Column("AFAD_Amount", TypeName = "money")]
    public decimal? AfadAmount { get; set; }

    [Column("AFAD_Quantity")]
    public int? AfadQuantity { get; set; }

    [Column("AFAD_Paymenttype")]
    public int? AfadPaymenttype { get; set; }

    [Column("AFAD_CostofTransport", TypeName = "money")]
    public decimal? AfadCostofTransport { get; set; }

    [Column("AFAD_InstallationCost", TypeName = "money")]
    public decimal? AfadInstallationCost { get; set; }

    [Column("AFAD_DateofInitiate", TypeName = "datetime")]
    public DateTime? AfadDateofInitiate { get; set; }

    [Column("AFAD_DateofReceived", TypeName = "datetime")]
    public DateTime? AfadDateofReceived { get; set; }

    [Column("AFAD_ToLocation")]
    public int? AfadToLocation { get; set; }

    [Column("AFAD_ToDivision")]
    public int? AfadToDivision { get; set; }

    [Column("AFAD_ToDepartment")]
    public int? AfadToDepartment { get; set; }

    [Column("AFAD_ToBay")]
    public int? AfadToBay { get; set; }

    [Column("AFAD_AssetDelDesc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfadAssetDelDesc { get; set; }

    [Column("AFAD_PorLStatus")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AfadPorLstatus { get; set; }

    [Column("AFAD_PorLAmount", TypeName = "money")]
    public decimal? AfadPorLamount { get; set; }

    [Column("AFAD_SalesPrice", TypeName = "money")]
    public decimal? AfadSalesPrice { get; set; }

    [Column("AFAD_DelDeprec", TypeName = "money")]
    public decimal? AfadDelDeprec { get; set; }

    [Column("AFAD_WDVValue", TypeName = "money")]
    public decimal? AfadWdvvalue { get; set; }

    [Column("AFAD_ContAssetValue", TypeName = "money")]
    public decimal? AfadContAssetValue { get; set; }

    [Column("AFAD_ContDep", TypeName = "money")]
    public decimal? AfadContDep { get; set; }

    [Column("AFAD_ContWDV", TypeName = "money")]
    public decimal? AfadContWdv { get; set; }

    [Column("AFAD_CreatedBy")]
    public int? AfadCreatedBy { get; set; }

    [Column("AFAD_CreatedOn", TypeName = "datetime")]
    public DateTime? AfadCreatedOn { get; set; }

    [Column("AFAD_ApprovedBy")]
    public int? AfadApprovedBy { get; set; }

    [Column("AFAD_ApprovedOn", TypeName = "datetime")]
    public DateTime? AfadApprovedOn { get; set; }

    [Column("AFAD_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AfadStatus { get; set; }

    [Column("AFAD_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AfadDelflag { get; set; }

    [Column("AFAD_YearID")]
    public int? AfadYearId { get; set; }

    [Column("AFAD_CompID")]
    public int? AfadCompId { get; set; }

    [Column("AFAD_Deletedby")]
    public int? AfadDeletedby { get; set; }

    [Column("AFAD_DeletedOn", TypeName = "datetime")]
    public DateTime? AfadDeletedOn { get; set; }

    [Column("AFAD_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AfadIpaddress { get; set; }

    [Column("AFAD_InsClaimedNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AfadInsClaimedNo { get; set; }

    [Column("AFAD_InsAmtClaimed", TypeName = "money")]
    public decimal? AfadInsAmtClaimed { get; set; }

    [Column("AFAD_InsClaimedDate", TypeName = "datetime")]
    public DateTime? AfadInsClaimedDate { get; set; }

    [Column("AFAD_InsAmtRecvd", TypeName = "money")]
    public decimal? AfadInsAmtRecvd { get; set; }

    [Column("AFAD_InsRefNo")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AfadInsRefNo { get; set; }

    [Column("AFAD_InsRefDate", TypeName = "datetime")]
    public DateTime? AfadInsRefDate { get; set; }
}
