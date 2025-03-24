using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Fla_LeaveDetails")]
public partial class FlaLeaveDetail
{
    [Column("LPE_ID")]
    public int? LpeId { get; set; }

    [Column("LPE_EMPID")]
    public int? LpeEmpid { get; set; }

    [Column("LPE_YearID")]
    public int? LpeYearId { get; set; }

    [Column("LPE_FROMDATE", TypeName = "datetime")]
    public DateTime? LpeFromdate { get; set; }

    [Column("LPE_TODATE", TypeName = "datetime")]
    public DateTime? LpeTodate { get; set; }

    [Column("LPE_DAYS")]
    public int? LpeDays { get; set; }

    [Column("LPE_PURPOSE")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? LpePurpose { get; set; }

    [Column("LPE_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LpeDelFlag { get; set; }

    [Column("LPE_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LpeStatus { get; set; }

    [Column("LPE_CrBY")]
    public int? LpeCrBy { get; set; }

    [Column("LPE_CrOn", TypeName = "datetime")]
    public DateTime? LpeCrOn { get; set; }

    [Column("LPE_UpdatedBY")]
    public int? LpeUpdatedBy { get; set; }

    [Column("LPE_UpdatedOn", TypeName = "datetime")]
    public DateTime? LpeUpdatedOn { get; set; }

    [Column("LPE_Approve")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LpeApprove { get; set; }

    [Column("LPE_ApprovedDetails")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? LpeApprovedDetails { get; set; }

    [Column("LPE_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? LpeIpaddress { get; set; }

    [Column("LPE_CompID")]
    public int? LpeCompId { get; set; }
}
