using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("sad_org_structure")]
public partial class SadOrgStructure
{
    [Column("org_node")]
    public int OrgNode { get; set; }

    [Column("org_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? OrgCode { get; set; }

    [Column("org_name")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? OrgName { get; set; }

    [Column("org_parent")]
    public int? OrgParent { get; set; }

    [Column("org_userid")]
    public int? OrgUserid { get; set; }

    [Column("org_Type")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OrgType { get; set; }

    [Column("org_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OrgDelFlag { get; set; }

    [Column("org_Note")]
    [StringLength(255)]
    [Unicode(false)]
    public string? OrgNote { get; set; }

    [Column("org_AppStrength")]
    public short? OrgAppStrength { get; set; }

    [Column("org_AppBy")]
    public int? OrgAppBy { get; set; }

    [Column("org_AppOn", TypeName = "datetime")]
    public DateTime? OrgAppOn { get; set; }

    [Column("org_CreatedBy")]
    public int? OrgCreatedBy { get; set; }

    [Column("org_CreatedOn", TypeName = "datetime")]
    public DateTime? OrgCreatedOn { get; set; }

    [Column("org_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? OrgStatus { get; set; }

    [Column("Org_levelCode")]
    [StringLength(10)]
    [Unicode(false)]
    public string? OrgLevelCode { get; set; }

    [Column("org_cust")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OrgCust { get; set; }

    [Column("Org_Cust1")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OrgCust1 { get; set; }

    [Column("Org_CompID")]
    public int? OrgCompId { get; set; }

    [Column("Org_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? OrgIpaddress { get; set; }

    [Column("Org_UpdatedBy")]
    public int? OrgUpdatedBy { get; set; }

    [Column("Org_UpdatedOn", TypeName = "datetime")]
    public DateTime? OrgUpdatedOn { get; set; }

    [Column("Org_DeletedBy")]
    public int? OrgDeletedBy { get; set; }

    [Column("Org_DeletedOn", TypeName = "datetime")]
    public DateTime? OrgDeletedOn { get; set; }

    [Column("Org_RecalledBy")]
    public int? OrgRecalledBy { get; set; }

    [Column("org_RecalledOn", TypeName = "datetime")]
    public DateTime? OrgRecalledOn { get; set; }

    [Column("Org_SalesUnitCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string? OrgSalesUnitCode { get; set; }

    [Column("Org_BranchCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string? OrgBranchCode { get; set; }
}
