using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_Changes_Inventories")]
public partial class AccChangesInventory
{
    [Column("CI_PKID")]
    public int? CiPkid { get; set; }

    [Column("CI_FinancialYear")]
    public int? CiFinancialYear { get; set; }

    [Column("CI_CustId")]
    public int? CiCustId { get; set; }

    [Column("CI_Orgtype")]
    public int? CiOrgtype { get; set; }

    [Column("CI_Head")]
    public int? CiHead { get; set; }

    [Column("CI_Group")]
    public int? CiGroup { get; set; }

    [Column("CI_Subgroup")]
    public int? CiSubgroup { get; set; }

    [Column("CI_Glid")]
    public int? CiGlid { get; set; }

    [Column("CI_SubGlid")]
    public int? CiSubGlid { get; set; }

    [Column("CI_Note")]
    public int? CiNote { get; set; }

    [Column("CI_OBValues", TypeName = "money")]
    public decimal? CiObvalues { get; set; }

    [Column("CI_CBValues", TypeName = "money")]
    public decimal? CiCbvalues { get; set; }

    [Column("CI_DATE", TypeName = "datetime")]
    public DateTime? CiDate { get; set; }

    [Column("CI_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CiStatus { get; set; }

    [Column("CI_Delflag")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CiDelflag { get; set; }

    [Column("CI_CrBy")]
    public int? CiCrBy { get; set; }

    [Column("CI_CrOn", TypeName = "datetime")]
    public DateTime? CiCrOn { get; set; }

    [Column("CI_UpdatedBy")]
    public int? CiUpdatedBy { get; set; }

    [Column("CI_UpdatedOn", TypeName = "datetime")]
    public DateTime? CiUpdatedOn { get; set; }

    [Column("CI_SavedBy")]
    public int? CiSavedBy { get; set; }

    [Column("CI_SavedOn", TypeName = "datetime")]
    public DateTime? CiSavedOn { get; set; }

    [Column("CI_Approvedby")]
    public int? CiApprovedby { get; set; }

    [Column("CI_ApprovedOn", TypeName = "datetime")]
    public DateTime? CiApprovedOn { get; set; }

    [Column("CI_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CiIpaddress { get; set; }

    [Column("CI_CompID")]
    public int? CiCompId { get; set; }
}
