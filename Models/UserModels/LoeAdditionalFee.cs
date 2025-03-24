using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("LOE_AdditionalFees")]
public partial class LoeAdditionalFee
{
    [Column("LAF_ID")]
    public int? LafId { get; set; }

    [Column("LAF_LOEID")]
    public int? LafLoeid { get; set; }

    [Column("LAF_OtherExpensesID")]
    public int? LafOtherExpensesId { get; set; }

    [Column("LAF_Charges")]
    public int? LafCharges { get; set; }

    [Column("LAF_CODE")]
    [StringLength(5)]
    [Unicode(false)]
    public string? LafCode { get; set; }

    [Column("LAF_OtherExpensesName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? LafOtherExpensesName { get; set; }

    [Column("LAF_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LafDelflag { get; set; }

    [Column("LAF_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? LafStatus { get; set; }

    [Column("LAF_CrBy")]
    public int? LafCrBy { get; set; }

    [Column("LAF_CrOn", TypeName = "datetime")]
    public DateTime? LafCrOn { get; set; }

    [Column("LAF_UpdatedBy")]
    public int? LafUpdatedBy { get; set; }

    [Column("LAF_UpdatedOn", TypeName = "datetime")]
    public DateTime? LafUpdatedOn { get; set; }

    [Column("LAF_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? LafIpaddress { get; set; }

    [Column("LAF_CompID")]
    public int? LafCompId { get; set; }
}
