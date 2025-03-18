using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadOrgStructure
{
    public int OrgNode { get; set; }

    public string? OrgCode { get; set; }

    public string? OrgName { get; set; }

    public int? OrgParent { get; set; }

    public int? OrgUserid { get; set; }

    public string? OrgType { get; set; }

    public string? OrgDelFlag { get; set; }

    public string? OrgNote { get; set; }

    public short? OrgAppStrength { get; set; }

    public int? OrgAppBy { get; set; }

    public DateTime? OrgAppOn { get; set; }

    public int? OrgCreatedBy { get; set; }

    public DateTime? OrgCreatedOn { get; set; }

    public string? OrgStatus { get; set; }

    public string? OrgLevelCode { get; set; }

    public string? OrgCust { get; set; }

    public string? OrgCust1 { get; set; }

    public int? OrgCompId { get; set; }

    public string? OrgIpaddress { get; set; }

    public int? OrgUpdatedBy { get; set; }

    public DateTime? OrgUpdatedOn { get; set; }

    public int? OrgDeletedBy { get; set; }

    public DateTime? OrgDeletedOn { get; set; }

    public int? OrgRecalledBy { get; set; }

    public DateTime? OrgRecalledOn { get; set; }

    public string? OrgSalesUnitCode { get; set; }

    public string? OrgBranchCode { get; set; }
}
