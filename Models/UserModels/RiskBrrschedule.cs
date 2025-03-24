using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_BRRSchedule")]
public partial class RiskBrrschedule
{
    [Column("BRRS_PKID")]
    public int? BrrsPkid { get; set; }

    [Column("BRRS_CustID")]
    public int? BrrsCustId { get; set; }

    [Column("BRRS_AsgNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? BrrsAsgNo { get; set; }

    [Column("BRRS_FinancialYear")]
    public int? BrrsFinancialYear { get; set; }

    [Column("BRRS_ScheduleMonth")]
    public int? BrrsScheduleMonth { get; set; }

    [Column("BRRS_ZoneID")]
    public int? BrrsZoneId { get; set; }

    [Column("BRRS_RegionID")]
    public int? BrrsRegionId { get; set; }

    [Column("BRRS_BranchID")]
    public int? BrrsBranchId { get; set; }

    [Column("BRRS_ZonalMgrID")]
    public int? BrrsZonalMgrId { get; set; }

    [Column("BRRS_BranchMgrID")]
    public int? BrrsBranchMgrId { get; set; }

    [Column("BRRS_ReviewerTypeID")]
    public int? BrrsReviewerTypeId { get; set; }

    [Column("BRRS_EmployeeID")]
    public int? BrrsEmployeeId { get; set; }

    [Column("BRRS_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BrrsRemarks { get; set; }

    [Column("BRRS_AttchID")]
    public int? BrrsAttchId { get; set; }

    [Column("BRRS_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BrrsStatus { get; set; }

    [Column("BRRS_CrBy")]
    public int? BrrsCrBy { get; set; }

    [Column("BRRS_CrOn", TypeName = "datetime")]
    public DateTime? BrrsCrOn { get; set; }

    [Column("BRRS_UpdatedBy")]
    public int? BrrsUpdatedBy { get; set; }

    [Column("BRRS_UpdatedOn", TypeName = "datetime")]
    public DateTime? BrrsUpdatedOn { get; set; }

    [Column("BRRS_SubmittedBy")]
    public int? BrrsSubmittedBy { get; set; }

    [Column("BRRS_SubmittedOn", TypeName = "datetime")]
    public DateTime? BrrsSubmittedOn { get; set; }

    [Column("BRRS_ApprovedBy")]
    public int? BrrsApprovedBy { get; set; }

    [Column("BRRS_ApprovedOn", TypeName = "datetime")]
    public DateTime? BrrsApprovedOn { get; set; }

    [Column("BRRS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BrrsIpaddress { get; set; }

    [Column("BRRS_CompID")]
    public int? BrrsCompId { get; set; }
}
