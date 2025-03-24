using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CustomerGL_Linkage_Master")]
public partial class CustomerGlLinkageMaster
{
    [Column("CLM_ID")]
    public int? ClmId { get; set; }

    [Column("CLM_Head")]
    public int? ClmHead { get; set; }

    [Column("CLM_GroupID")]
    public int? ClmGroupId { get; set; }

    [Column("CLM_SubGroupID")]
    public int? ClmSubGroupId { get; set; }

    [Column("CLM_GLLedger")]
    [Unicode(false)]
    public string? ClmGlledger { get; set; }

    [Column("CLM_CreatedBy")]
    public int? ClmCreatedBy { get; set; }

    [Column("CLM_CreatedOn", TypeName = "datetime")]
    public DateTime? ClmCreatedOn { get; set; }

    [Column("CLM_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? ClmStatus { get; set; }

    [Column("CLM_YearID")]
    public int? ClmYearId { get; set; }

    [Column("CLM_CompID")]
    public int? ClmCompId { get; set; }

    [Column("CLM_DeletedBy")]
    public int? ClmDeletedBy { get; set; }

    [Column("CLM_DeletedOn", TypeName = "datetime")]
    public DateTime? ClmDeletedOn { get; set; }

    [Column("CLM_UpdatedBy")]
    public int? ClmUpdatedBy { get; set; }

    [Column("CLM_UpdatedOn", TypeName = "datetime")]
    public DateTime? ClmUpdatedOn { get; set; }

    [Column("CLM_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? ClmOperation { get; set; }

    [Column("CLM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ClmIpaddress { get; set; }

    [Column("CLM_CustID")]
    public int? ClmCustId { get; set; }

    [Column("CLM_OrgID")]
    public int? ClmOrgId { get; set; }

    [Column("CLM_GL")]
    public int? ClmGl { get; set; }

    [Column("CLM_SubGL")]
    public int? ClmSubGl { get; set; }

    [Column("CLM_GLID")]
    public int? ClmGlid { get; set; }
}
