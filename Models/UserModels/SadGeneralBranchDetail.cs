using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_General_BranchDetails")]
public partial class SadGeneralBranchDetail
{
    [Column("Branch_ID")]
    public int? BranchId { get; set; }

    [Column("Branch_Name")]
    [StringLength(250)]
    [Unicode(false)]
    public string? BranchName { get; set; }

    [Column("Branch_Address")]
    [StringLength(500)]
    [Unicode(false)]
    public string? BranchAddress { get; set; }

    [Column("Branch_City")]
    [StringLength(500)]
    [Unicode(false)]
    public string? BranchCity { get; set; }

    [Column("Branch_State")]
    [StringLength(500)]
    [Unicode(false)]
    public string? BranchState { get; set; }

    [Column("Branch_Country")]
    [StringLength(500)]
    [Unicode(false)]
    public string? BranchCountry { get; set; }

    [Column("Branch_PinCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BranchPinCode { get; set; }

    [Column("Branch_Establishment_Date")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BranchEstablishmentDate { get; set; }

    [Column("Branch_EmailID")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BranchEmailId { get; set; }

    [Column("Branch_ContactPerson")]
    [StringLength(20)]
    [Unicode(false)]
    public string? BranchContactPerson { get; set; }

    [Column("Branch_MobileNo")]
    [StringLength(20)]
    [Unicode(false)]
    public string? BranchMobileNo { get; set; }

    [Column("Branch_ContactEmailID")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BranchContactEmailId { get; set; }

    [Column("Branch_TelephoneNo")]
    [StringLength(30)]
    [Unicode(false)]
    public string? BranchTelephoneNo { get; set; }

    [Column("Branch_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? BranchStatus { get; set; }

    [Column("Branch_CrBy")]
    public int? BranchCrBy { get; set; }

    [Column("Branch_CrOn", TypeName = "datetime")]
    public DateTime? BranchCrOn { get; set; }
}
