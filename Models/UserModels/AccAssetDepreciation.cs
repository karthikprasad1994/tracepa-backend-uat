using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_AssetDepreciation")]
public partial class AccAssetDepreciation
{
    [Column("ADep_ID")]
    public int? AdepId { get; set; }

    [Column("ADep_AssetID")]
    public int? AdepAssetId { get; set; }

    [Column("ADep_Item")]
    [StringLength(250)]
    [Unicode(false)]
    public string? AdepItem { get; set; }

    [Column("ADep_RateofDep", TypeName = "money")]
    public decimal? AdepRateofDep { get; set; }

    [Column("ADep_OPBForYR", TypeName = "money")]
    public decimal? AdepOpbforYr { get; set; }

    [Column("ADep_DepreciationforFY", TypeName = "money")]
    public decimal? AdepDepreciationforFy { get; set; }

    [Column("ADep_WrittenDownValue", TypeName = "money")]
    public decimal? AdepWrittenDownValue { get; set; }

    [Column("ADep_ClosingDate", TypeName = "datetime")]
    public DateTime? AdepClosingDate { get; set; }

    [Column("ADep_CreatedBy")]
    public int? AdepCreatedBy { get; set; }

    [Column("ADep_CreatedOn", TypeName = "datetime")]
    public DateTime? AdepCreatedOn { get; set; }

    [Column("ADep_UpdatedBy")]
    public int? AdepUpdatedBy { get; set; }

    [Column("ADep_UpdatedOn", TypeName = "datetime")]
    public DateTime? AdepUpdatedOn { get; set; }

    [Column("ADep_ApprovedBy")]
    public int? AdepApprovedBy { get; set; }

    [Column("ADep_ApprovedOn", TypeName = "datetime")]
    public DateTime? AdepApprovedOn { get; set; }

    [Column("ADep_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AdepDelFlag { get; set; }

    [Column("ADep_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AdepStatus { get; set; }

    [Column("ADep_YearID")]
    public int? AdepYearId { get; set; }

    [Column("ADep_CompID")]
    public int? AdepCompId { get; set; }

    [Column("ADep_CustId")]
    public int? AdepCustId { get; set; }

    [Column("ADep_Location")]
    public int? AdepLocation { get; set; }

    [Column("ADep_Division")]
    public int? AdepDivision { get; set; }

    [Column("ADep_Department")]
    public int? AdepDepartment { get; set; }

    [Column("ADep_Bay")]
    public int? AdepBay { get; set; }

    [Column("ADep_TransType")]
    public int? AdepTransType { get; set; }

    [Column("ADep_Method")]
    public int? AdepMethod { get; set; }

    [Column("ADep_Opeartion")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AdepOpeartion { get; set; }

    [Column("ADep_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AdepIpaddress { get; set; }
}
