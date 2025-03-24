using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("TRACe_CompanyBranchDetails")]
public partial class TraceCompanyBranchDetail
{
    [Column("Company_Branch_Id")]
    public int? CompanyBranchId { get; set; }

    [Column("Company_Branch_CompanyID")]
    public int? CompanyBranchCompanyId { get; set; }

    [Column("Company_Branch_Name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompanyBranchName { get; set; }

    [Column("Company_Branch_Address")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyBranchAddress { get; set; }

    [Column("Company_Branch_Contact_Person")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CompanyBranchContactPerson { get; set; }

    [Column("Company_Branch_Contact_MobileNo")]
    [StringLength(15)]
    [Unicode(false)]
    public string? CompanyBranchContactMobileNo { get; set; }

    [Column("Company_Branch_Contact_LandLineNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CompanyBranchContactLandLineNo { get; set; }

    [Column("Company_Branch_Contact_Email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompanyBranchContactEmail { get; set; }

    [Column("Company_Branch_Designation")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyBranchDesignation { get; set; }

    [Column("Company_Branch_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CompanyBranchDelFlag { get; set; }

    [Column("Company_Branch_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CompanyBranchStatus { get; set; }

    [Column("Company_Branch_CRON", TypeName = "datetime")]
    public DateTime? CompanyBranchCron { get; set; }

    [Column("Company_Branch_CRBY")]
    public int? CompanyBranchCrby { get; set; }

    [Column("Company_Branch_UpdatedOn", TypeName = "datetime")]
    public DateTime? CompanyBranchUpdatedOn { get; set; }

    [Column("Company_Branch_UpdatedBy")]
    public int? CompanyBranchUpdatedBy { get; set; }

    [Column("Company_Branch_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CompanyBranchIpaddress { get; set; }

    [Column("Company_Branch_CompID")]
    public int? CompanyBranchCompId { get; set; }
}
