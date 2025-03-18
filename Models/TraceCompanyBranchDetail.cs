using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class TraceCompanyBranchDetail
{
    public int? CompanyBranchId { get; set; }

    public int? CompanyBranchCompanyId { get; set; }

    public string? CompanyBranchName { get; set; }

    public string? CompanyBranchAddress { get; set; }

    public string? CompanyBranchContactPerson { get; set; }

    public string? CompanyBranchContactMobileNo { get; set; }

    public string? CompanyBranchContactLandLineNo { get; set; }

    public string? CompanyBranchContactEmail { get; set; }

    public string? CompanyBranchDesignation { get; set; }

    public string? CompanyBranchDelFlag { get; set; }

    public string? CompanyBranchStatus { get; set; }

    public DateTime? CompanyBranchCron { get; set; }

    public int? CompanyBranchCrby { get; set; }

    public DateTime? CompanyBranchUpdatedOn { get; set; }

    public int? CompanyBranchUpdatedBy { get; set; }

    public string? CompanyBranchIpaddress { get; set; }

    public int? CompanyBranchCompId { get; set; }
}
