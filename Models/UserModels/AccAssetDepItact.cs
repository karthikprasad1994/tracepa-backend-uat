using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_AssetDepITAct")]
public partial class AccAssetDepItact
{
    [Column("ADITAct_ID")]
    public int? AditactId { get; set; }

    [Column("ADITAct_AssetClassID")]
    public int? AditactAssetClassId { get; set; }

    [Column("ADITAct_RateofDep", TypeName = "money")]
    public decimal? AditactRateofDep { get; set; }

    [Column("ADITAct_OPBForYR", TypeName = "money")]
    public decimal? AditactOpbforYr { get; set; }

    [Column("ADITAct_DepreciationforFY", TypeName = "money")]
    public decimal? AditactDepreciationforFy { get; set; }

    [Column("ADITAct_WrittenDownValue", TypeName = "money")]
    public decimal? AditactWrittenDownValue { get; set; }

    [Column("ADITAct_BfrQtrAmount", TypeName = "money")]
    public decimal? AditactBfrQtrAmount { get; set; }

    [Column("ADITAct_BfrQtrDep", TypeName = "money")]
    public decimal? AditactBfrQtrDep { get; set; }

    [Column("ADITAct_AftQtrAmount", TypeName = "money")]
    public decimal? AditactAftQtrAmount { get; set; }

    [Column("ADITAct_AftQtrDep", TypeName = "money")]
    public decimal? AditactAftQtrDep { get; set; }

    [Column("ADITAct_DelAmount", TypeName = "money")]
    public decimal? AditactDelAmount { get; set; }

    [Column("ADITAct_CreatedBy")]
    public int? AditactCreatedBy { get; set; }

    [Column("ADITAct_CreatedOn", TypeName = "datetime")]
    public DateTime? AditactCreatedOn { get; set; }

    [Column("ADITAct_UpdatedBy")]
    public int? AditactUpdatedBy { get; set; }

    [Column("ADITAct_UpdatedOn", TypeName = "datetime")]
    public DateTime? AditactUpdatedOn { get; set; }

    [Column("ADITAct_ApprovedBy")]
    public int? AditactApprovedBy { get; set; }

    [Column("ADITAct_ApprovedOn", TypeName = "datetime")]
    public DateTime? AditactApprovedOn { get; set; }

    [Column("ADITAct_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AditactDelFlag { get; set; }

    [Column("ADITAct_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AditactStatus { get; set; }

    [Column("ADITAct_YearID")]
    public int? AditactYearId { get; set; }

    [Column("ADITAct_CompID")]
    public int? AditactCompId { get; set; }

    [Column("ADITAct_CustId")]
    public int? AditactCustId { get; set; }

    [Column("ADITAct_Opeartion")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AditactOpeartion { get; set; }

    [Column("ADITAct_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AditactIpaddress { get; set; }

    [Column("ADITAct_InitAmt", TypeName = "money")]
    public decimal? AditactInitAmt { get; set; }
}
