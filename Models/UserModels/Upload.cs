using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("upload")]
public partial class Upload
{
    [Column("PKID")]
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

    [Column("YearID")]
    public string? YearId { get; set; }

    [Column("CompID")]
    public string? CompId { get; set; }

    [Column("IPAddress")]
    public string? Ipaddress { get; set; }
}
