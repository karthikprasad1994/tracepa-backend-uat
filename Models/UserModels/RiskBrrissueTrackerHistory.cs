using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_BRRIssueTracker_History")]
public partial class RiskBrrissueTrackerHistory
{
    [Column("BRRITH_PKID")]
    public int? BrrithPkid { get; set; }

    [Column("BRRITH_BBRITPKID")]
    public int? BrrithBbritpkid { get; set; }

    [Column("BRRITH_CustID")]
    public int? BrrithCustId { get; set; }

    [Column("BRRITH_AsgNo")]
    public int? BrrithAsgNo { get; set; }

    [Column("BRRITH_ActionPlan")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? BrrithActionPlan { get; set; }

    [Column("BRRITH_TargetDate", TypeName = "datetime")]
    public DateTime? BrrithTargetDate { get; set; }

    [Column("BRRITH_OpenCloseStatus")]
    public int? BrrithOpenCloseStatus { get; set; }

    [Column("BRRITH_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BrrithRemarks { get; set; }

    [Column("BRRITH_CrBy")]
    public int? BrrithCrBy { get; set; }

    [Column("BRRITH_CrOn", TypeName = "datetime")]
    public DateTime? BrrithCrOn { get; set; }

    [Column("BRRITH_UpdatedBy")]
    public int? BrrithUpdatedBy { get; set; }

    [Column("BRRITH_UpdatedOn", TypeName = "datetime")]
    public DateTime? BrrithUpdatedOn { get; set; }

    [Column("BRRITH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BrrithIpaddress { get; set; }

    [Column("BRRITH_CompID")]
    public int? BrrithCompId { get; set; }
}
