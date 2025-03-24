using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_BRRChecklist_Mas")]
public partial class RiskBrrchecklistMa
{
    [Column("BRR_PKID")]
    public int? BrrPkid { get; set; }

    [Column("BRR_CustID")]
    public int? BrrCustId { get; set; }

    [Column("BRR_AsgID")]
    public int? BrrAsgId { get; set; }

    [Column("BRR_BranchId")]
    public int? BrrBranchId { get; set; }

    [Column("BRR_YearID")]
    public int? BrrYearId { get; set; }

    [Column("BRR_ASDate", TypeName = "datetime")]
    public DateTime? BrrAsdate { get; set; }

    [Column("BRR_AEDate", TypeName = "datetime")]
    public DateTime? BrrAedate { get; set; }

    [Column("BRR_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BrrStatus { get; set; }

    [Column("BRR_Flag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? BrrFlag { get; set; }

    [Column("BRR_Title")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? BrrTitle { get; set; }

    [Column("BRR_Remarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? BrrRemarks { get; set; }

    [Column("BRR_AttachID")]
    public int? BrrAttachId { get; set; }

    [Column("BRR_CrBy")]
    public int? BrrCrBy { get; set; }

    [Column("BRR_CrOn", TypeName = "datetime")]
    public DateTime? BrrCrOn { get; set; }

    [Column("BRR_UpdatedBy")]
    public int? BrrUpdatedBy { get; set; }

    [Column("BRR_UpdatedOn", TypeName = "datetime")]
    public DateTime? BrrUpdatedOn { get; set; }

    [Column("BRR_SubmittedBy")]
    public int? BrrSubmittedBy { get; set; }

    [Column("BRR_SubmittedOn", TypeName = "datetime")]
    public DateTime? BrrSubmittedOn { get; set; }

    [Column("BRR_ApprovedBy")]
    public int? BrrApprovedBy { get; set; }

    [Column("BRR_ApprovedOn", TypeName = "datetime")]
    public DateTime? BrrApprovedOn { get; set; }

    [Column("BRR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BrrIpaddress { get; set; }

    [Column("BRR_CompID")]
    public int? BrrCompId { get; set; }

    [Column("BRR_PGEDetailId")]
    public int? BrrPgedetailId { get; set; }
}
