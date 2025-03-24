using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_Partnership_Firms")]
public partial class AccPartnershipFirm
{
    [Column("APF_ID")]
    public int? ApfId { get; set; }

    [Column("APF_YearID")]
    public int? ApfYearId { get; set; }

    [Column("APF_Cust_ID")]
    public int? ApfCustId { get; set; }

    [Column("APF_Branch_ID")]
    public int? ApfBranchId { get; set; }

    [Column("APF_Partner_ID")]
    public int? ApfPartnerId { get; set; }

    [Column("APF_OpeningBalance", TypeName = "decimal(19, 2)")]
    public decimal? ApfOpeningBalance { get; set; }

    [Column("APF_UnsecuredLoanTreatedAsCapital", TypeName = "decimal(19, 2)")]
    public decimal? ApfUnsecuredLoanTreatedAsCapital { get; set; }

    [Column("APF_InterestOnCapital", TypeName = "decimal(19, 2)")]
    public decimal? ApfInterestOnCapital { get; set; }

    [Column("APF_PartnersSalary", TypeName = "decimal(19, 2)")]
    public decimal? ApfPartnersSalary { get; set; }

    [Column("APF_ShareOfprofit", TypeName = "decimal(19, 2)")]
    public decimal? ApfShareOfprofit { get; set; }

    [Column("APF_TransferToFixedCapital", TypeName = "decimal(19, 2)")]
    public decimal? ApfTransferToFixedCapital { get; set; }

    [Column("APF_Drawings", TypeName = "decimal(19, 2)")]
    public decimal? ApfDrawings { get; set; }

    [Column("APF_AddOthers", TypeName = "decimal(19, 2)")]
    public decimal? ApfAddOthers { get; set; }

    [Column("APF_LessOthers", TypeName = "decimal(19, 2)")]
    public decimal? ApfLessOthers { get; set; }

    [Column("APF_CrBy")]
    public int? ApfCrBy { get; set; }

    [Column("APF_CrOn", TypeName = "datetime")]
    public DateTime? ApfCrOn { get; set; }

    [Column("APF_UpdateBy")]
    public int? ApfUpdateBy { get; set; }

    [Column("APF_UpdateOn", TypeName = "datetime")]
    public DateTime? ApfUpdateOn { get; set; }

    [Column("APF_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ApfIpaddress { get; set; }

    [Column("APF_CompID")]
    public int? ApfCompId { get; set; }
}
