using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class Upload
{
    public int Pkid { get; set; }

    public string? BranchDepartmentClusterZone { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchDeptClusterZoneName { get; set; }

    public string? EmpCodeLoginName { get; set; }

    public string? EmployeeFullName { get; set; }

    public string? Designation { get; set; }

    public string? Role { get; set; }

    public string? Mail { get; set; }

    public string? AuditNo { get; set; }

    public string? YearId { get; set; }

    public string? CompId { get; set; }

    public string? Ipaddress { get; set; }
}
