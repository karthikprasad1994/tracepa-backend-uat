using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadOrgStructureLog
{
    public int LogPkid { get; set; }

    public DateTime? LogDate { get; set; }

    public string? LogOperation { get; set; }

    public int? LogUserId { get; set; }

    public int? OrgNode { get; set; }

    public string? OrgCode { get; set; }

    public string? NorgCode { get; set; }

    public string? OrgName { get; set; }

    public string? NorgName { get; set; }

    public int? OrgParent { get; set; }

    public int? NorgParent { get; set; }

    public int? OrgUserid { get; set; }

    public int? NorgUserid { get; set; }

    public string? OrgType { get; set; }

    public string? NorgType { get; set; }

    public string? OrgDelFlag { get; set; }

    public string? NorgDelFlag { get; set; }

    public string? OrgEmail { get; set; }

    public string? NorgEmail { get; set; }

    public string? OrgNote { get; set; }

    public string? NorgNote { get; set; }

    public long? OrgAppStrength { get; set; }

    public long? NorgAppStrength { get; set; }

    public string? OrgStatus { get; set; }

    public string? NorgStatus { get; set; }

    public string? OrgLevelCode { get; set; }

    public string? NOrgLevelCode { get; set; }

    public string? OrgCust { get; set; }

    public string? NorgCust { get; set; }

    public string? OrgCompId { get; set; }

    public string? OrgIpaddress { get; set; }

    public string? OrgSalesUnitCode { get; set; }

    public string? NOrgSalesUnitCode { get; set; }

    public string? OrgBranchCode { get; set; }

    public string? NOrgBranchCode { get; set; }
}
