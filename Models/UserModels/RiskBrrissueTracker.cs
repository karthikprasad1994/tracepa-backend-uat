using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_BRRIssueTracker")]
public partial class RiskBrrissueTracker
{
    [Column("BBRIT_PKID")]
    public int? BbritPkid { get; set; }

    [Column("BBRIT_BRRDPKID")]
    public int? BbritBrrdpkid { get; set; }

    [Column("BBRIT_RCMID")]
    public int? BbritRcmid { get; set; }

    [Column("BBRIT_CustID")]
    public int? BbritCustId { get; set; }

    [Column("BBRIT_AsgNo")]
    public int? BbritAsgNo { get; set; }

    [Column("BBRIT_BranchId")]
    public int? BbritBranchId { get; set; }

    [Column("BBRIT_FunctionID")]
    public int? BbritFunctionId { get; set; }

    [Column("BBRIT_AreaID")]
    public int? BbritAreaId { get; set; }

    [Column("BBRIT_CheckPointID")]
    public int? BbritCheckPointId { get; set; }

    [Column("BBRIT_FinancialYear")]
    public int? BbritFinancialYear { get; set; }

    [Column("BBRIT_IssueHeading")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BbritIssueHeading { get; set; }

    [Column("BBRIT_IssueDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BbritIssueDesc { get; set; }

    [Column("BBRIT_ActionPlan")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? BbritActionPlan { get; set; }

    [Column("BBRIT_TargetDate", TypeName = "datetime")]
    public DateTime? BbritTargetDate { get; set; }

    [Column("BBRIT_OpenCloseStatus")]
    public int? BbritOpenCloseStatus { get; set; }

    [Column("BBRIT_Responsible")]
    public int? BbritResponsible { get; set; }

    [Column("BBRIT_Remaks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BbritRemaks { get; set; }

    [Column("BBRIT_AttchID")]
    public int? BbritAttchId { get; set; }

    [Column("BBRIT_CrBy")]
    public int? BbritCrBy { get; set; }

    [Column("BBRIT_CrOn", TypeName = "datetime")]
    public DateTime? BbritCrOn { get; set; }

    [Column("BBRIT_UpdatedBy")]
    public int? BbritUpdatedBy { get; set; }

    [Column("BBRIT_UpdatedOn", TypeName = "datetime")]
    public DateTime? BbritUpdatedOn { get; set; }

    [Column("BBRIT_SubmittedBy")]
    public int? BbritSubmittedBy { get; set; }

    [Column("BBRIT_SubmittedOn", TypeName = "datetime")]
    public DateTime? BbritSubmittedOn { get; set; }

    [Column("BBRIT_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? BbritStatus { get; set; }

    [Column("BBRIT_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? BbritDelFlag { get; set; }

    [Column("BBRIT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BbritIpaddress { get; set; }

    [Column("BBRIT_CompID")]
    public int? BbritCompId { get; set; }
}
