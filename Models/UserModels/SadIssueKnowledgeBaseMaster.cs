using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_IssueKnowledgeBase_Master")]
public partial class SadIssueKnowledgeBaseMaster
{
    [Column("IKB_ID")]
    public int? IkbId { get; set; }

    [Column("IKB_IssueHeading")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? IkbIssueHeading { get; set; }

    [Column("IKB_IssueDetails")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? IkbIssueDetails { get; set; }

    [Column("IKB_IssueRatingID")]
    public int? IkbIssueRatingId { get; set; }

    [Column("IKB_DelFlag")]
    [StringLength(25)]
    [Unicode(false)]
    public string? IkbDelFlag { get; set; }

    [Column("IKB_CrBy")]
    public int? IkbCrBy { get; set; }

    [Column("IKB_CrOn", TypeName = "datetime")]
    public DateTime? IkbCrOn { get; set; }

    [Column("IKB_UpdatedBy")]
    public int? IkbUpdatedBy { get; set; }

    [Column("IKB_UpdatedOn", TypeName = "datetime")]
    public DateTime? IkbUpdatedOn { get; set; }

    [Column("IKB_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? IkbStatus { get; set; }

    [Column("IKB_ApprovedBy")]
    public int? IkbApprovedBy { get; set; }

    [Column("IKB_ApprovedOn", TypeName = "datetime")]
    public DateTime? IkbApprovedOn { get; set; }

    [Column("IKB_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? IkbIpaddress { get; set; }

    [Column("IKB_CompID")]
    public int? IkbCompId { get; set; }
}
