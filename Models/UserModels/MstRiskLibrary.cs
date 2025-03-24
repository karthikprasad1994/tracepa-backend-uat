using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_RISK_Library")]
public partial class MstRiskLibrary
{
    [Column("MRL_PKID")]
    public int? MrlPkid { get; set; }

    [Column("MRL_RiskName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MrlRiskName { get; set; }

    [Column("MRL_RiskDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? MrlRiskDesc { get; set; }

    [Column("MRL_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? MrlCode { get; set; }

    [Column("MRL_RiskTypeID")]
    public int? MrlRiskTypeId { get; set; }

    [Column("MRL_IsKey")]
    public int? MrlIsKey { get; set; }

    [Column("MRL_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MrlDelFlag { get; set; }

    [Column("MRL_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MrlStatus { get; set; }

    [Column("MRL_CrBy")]
    public int? MrlCrBy { get; set; }

    [Column("MRL_CrOn", TypeName = "datetime")]
    public DateTime? MrlCrOn { get; set; }

    [Column("MRL_UpdatedBy")]
    public int? MrlUpdatedBy { get; set; }

    [Column("MRL_UpdatedOn", TypeName = "datetime")]
    public DateTime? MrlUpdatedOn { get; set; }

    [Column("MRL_ApprovedBy")]
    public int? MrlApprovedBy { get; set; }

    [Column("MRL_ApprovedOn", TypeName = "datetime")]
    public DateTime? MrlApprovedOn { get; set; }

    [Column("MRL_DeletedBy")]
    public int? MrlDeletedBy { get; set; }

    [Column("MRL_DeletedOn", TypeName = "datetime")]
    public DateTime? MrlDeletedOn { get; set; }

    [Column("MRL_RecallBy")]
    public int? MrlRecallBy { get; set; }

    [Column("MRL_RecallOn", TypeName = "datetime")]
    public DateTime? MrlRecallOn { get; set; }

    [Column("MRL_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MrlIpaddress { get; set; }

    [Column("MRL_CompID")]
    public int? MrlCompId { get; set; }

    [Column("MRL_InherentRiskID")]
    public int? MrlInherentRiskId { get; set; }

    [Column("MRL_Module")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MrlModule { get; set; }
}
