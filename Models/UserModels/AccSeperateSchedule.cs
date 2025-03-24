using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_Seperate_Schedule")]
public partial class AccSeperateSchedule
{
    [Column("SS_PKID")]
    public int? SsPkid { get; set; }

    [Column("SS_FinancialYear")]
    public int? SsFinancialYear { get; set; }

    [Column("SS_CustId")]
    public int? SsCustId { get; set; }

    [Column("SS_Orgtype")]
    public int? SsOrgtype { get; set; }

    [Column("SS_Group")]
    public int? SsGroup { get; set; }

    [Column("SS_Particulars")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SsParticulars { get; set; }

    [Column("SS_Values", TypeName = "money")]
    public decimal? SsValues { get; set; }

    [Column("SS_DATE", TypeName = "datetime")]
    public DateTime? SsDate { get; set; }

    [Column("SS_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SsStatus { get; set; }

    [Column("SS_Delflag")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SsDelflag { get; set; }

    [Column("SS_CrBy")]
    public int? SsCrBy { get; set; }

    [Column("SS_CrOn", TypeName = "datetime")]
    public DateTime? SsCrOn { get; set; }

    [Column("SS_UpdatedBy")]
    public int? SsUpdatedBy { get; set; }

    [Column("SS_UpdatedOn", TypeName = "datetime")]
    public DateTime? SsUpdatedOn { get; set; }

    [Column("SS_Approvedby")]
    public int? SsApprovedby { get; set; }

    [Column("SS_ApprovedOn", TypeName = "datetime")]
    public DateTime? SsApprovedOn { get; set; }

    [Column("SS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SsIpaddress { get; set; }

    [Column("SS_CompID")]
    public int? SsCompId { get; set; }
}
