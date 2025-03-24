using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_Org_Structure_Log")]
public partial class SadOrgStructureLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_Operation")]
    [StringLength(50)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("org_node")]
    public int? OrgNode { get; set; }

    [Column("org_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? OrgCode { get; set; }

    [Column("norg_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NorgCode { get; set; }

    [Column("org_name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? OrgName { get; set; }

    [Column("norg_name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NorgName { get; set; }

    [Column("org_parent")]
    public int? OrgParent { get; set; }

    [Column("norg_parent")]
    public int? NorgParent { get; set; }

    [Column("org_userid")]
    public int? OrgUserid { get; set; }

    [Column("norg_userid")]
    public int? NorgUserid { get; set; }

    [Column("org_Type")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OrgType { get; set; }

    [Column("norg_Type")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NorgType { get; set; }

    [Column("org_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OrgDelFlag { get; set; }

    [Column("norg_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NorgDelFlag { get; set; }

    [Column("org_Email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OrgEmail { get; set; }

    [Column("norg_Email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NorgEmail { get; set; }

    [Column("org_Note")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? OrgNote { get; set; }

    [Column("norg_Note")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NorgNote { get; set; }

    [Column("org_AppStrength")]
    public long? OrgAppStrength { get; set; }

    [Column("norg_AppStrength")]
    public long? NorgAppStrength { get; set; }

    [Column("org_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? OrgStatus { get; set; }

    [Column("norg_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NorgStatus { get; set; }

    [Column("Org_levelCode")]
    [StringLength(10)]
    [Unicode(false)]
    public string? OrgLevelCode { get; set; }

    [Column("nOrg_levelCode")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NOrgLevelCode { get; set; }

    [Column("org_cust")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OrgCust { get; set; }

    [Column("norg_cust")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NorgCust { get; set; }

    [Column("Org_CompId")]
    [StringLength(300)]
    [Unicode(false)]
    public string? OrgCompId { get; set; }

    [Column("Org_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? OrgIpaddress { get; set; }

    [Column("Org_SalesUnitCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string? OrgSalesUnitCode { get; set; }

    [Column("nOrg_SalesUnitCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NOrgSalesUnitCode { get; set; }

    [Column("Org_BranchCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string? OrgBranchCode { get; set; }

    [Column("nOrg_BranchCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NOrgBranchCode { get; set; }
}
