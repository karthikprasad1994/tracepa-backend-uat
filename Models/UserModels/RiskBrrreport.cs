using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_BRRReport")]
public partial class RiskBrrreport
{
    [Column("BRRR_Pkid")]
    public int BrrrPkid { get; set; }

    [Column("BRRR_CustID")]
    public int? BrrrCustId { get; set; }

    [Column("BRRR_AsgID")]
    public int? BrrrAsgId { get; set; }

    [Column("BRRR_BBRITID")]
    public int? BrrrBbritid { get; set; }

    [Column("BRRR_BRRDID")]
    public int? BrrrBrrdid { get; set; }

    [Column("BRRR_BranchId")]
    public int? BrrrBranchId { get; set; }

    [Column("BRRR_FunctionID")]
    public int? BrrrFunctionId { get; set; }

    [Column("BRRR_AreaID")]
    public int? BrrrAreaId { get; set; }

    [Column("BRRR_IssuAgreed")]
    public int? BrrrIssuAgreed { get; set; }

    [Column("BRRR_ActionPlanDate", TypeName = "datetime")]
    public DateTime? BrrrActionPlanDate { get; set; }

    [Column("BRRR_DisAgreedRsn")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BrrrDisAgreedRsn { get; set; }

    [Column("BRRR_IssuStatus")]
    public int? BrrrIssuStatus { get; set; }

    [Column("BRRR_ClosingDate", TypeName = "datetime")]
    public DateTime? BrrrClosingDate { get; set; }

    [Column("BRRR_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? BrrrStatus { get; set; }

    [Column("BRRR_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? BrrrDelFlag { get; set; }

    [Column("BRRR_AttachID")]
    public int? BrrrAttachId { get; set; }

    [Column("BRRR_CreatedBy")]
    [StringLength(5)]
    [Unicode(false)]
    public string? BrrrCreatedBy { get; set; }

    [Column("BRRR_CreatedOn", TypeName = "datetime")]
    public DateTime? BrrrCreatedOn { get; set; }

    [Column("BRRR_UpdatedBy")]
    [StringLength(5)]
    [Unicode(false)]
    public string? BrrrUpdatedBy { get; set; }

    [Column("BRRR_UpdatedOn", TypeName = "datetime")]
    public DateTime? BrrrUpdatedOn { get; set; }

    [Column("BRRR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BrrrIpaddress { get; set; }

    [Column("BRRR_YearID")]
    [StringLength(5)]
    [Unicode(false)]
    public string? BrrrYearId { get; set; }

    [Column("BRRR_CompID")]
    public int? BrrrCompId { get; set; }

    [Column("BRRR_PGEDetailId")]
    public int? BrrrPgedetailId { get; set; }
}
