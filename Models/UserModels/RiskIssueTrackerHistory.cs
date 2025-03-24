using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_IssueTracker_History")]
public partial class RiskIssueTrackerHistory
{
    [Column("RITH_PKID")]
    public int? RithPkid { get; set; }

    [Column("RITH_RITPKID")]
    public int? RithRitpkid { get; set; }

    [Column("RITH_AsgNo")]
    public int? RithAsgNo { get; set; }

    [Column("RITH_ActionPlan")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? RithActionPlan { get; set; }

    [Column("RITH_TargetDate", TypeName = "datetime")]
    public DateTime? RithTargetDate { get; set; }

    [Column("RITH_OpenCloseStatus")]
    public int? RithOpenCloseStatus { get; set; }

    [Column("RITH_ManagerResponsible")]
    public int? RithManagerResponsible { get; set; }

    [Column("RITH_IndividualResponsible")]
    public int? RithIndividualResponsible { get; set; }

    [Column("RITH_Remaks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RithRemaks { get; set; }

    [Column("RITH_CrBy")]
    public int? RithCrBy { get; set; }

    [Column("RITH_CrOn", TypeName = "datetime")]
    public DateTime? RithCrOn { get; set; }

    [Column("RITH_UpdatedBy")]
    public int? RithUpdatedBy { get; set; }

    [Column("RITH_UpdatedOn", TypeName = "datetime")]
    public DateTime? RithUpdatedOn { get; set; }

    [Column("RITH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RithIpaddress { get; set; }

    [Column("RITH_CompID")]
    public int? RithCompId { get; set; }
}
