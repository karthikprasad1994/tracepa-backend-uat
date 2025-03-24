using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Trace_CompanyDetails")]
public partial class TraceCompanyDetail
{
    [Column("Company_ID")]
    public int? CompanyId { get; set; }

    [Column("Company_Code")]
    [StringLength(30)]
    [Unicode(false)]
    public string? CompanyCode { get; set; }

    [Column("Company_Name")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyName { get; set; }

    [Column("Company_Address")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? CompanyAddress { get; set; }

    [Column("Company_City")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyCity { get; set; }

    [Column("Company_State")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyState { get; set; }

    [Column("Company_Country")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyCountry { get; set; }

    [Column("Company_PinCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CompanyPinCode { get; set; }

    [Column("Company_EmailID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompanyEmailId { get; set; }

    [Column("Company_Establishment_Date")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyEstablishmentDate { get; set; }

    [Column("Company_ContactPerson")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyContactPerson { get; set; }

    [Column("Company_MobileNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompanyMobileNo { get; set; }

    [Column("Company_ContactEmailID")]
    [StringLength(30)]
    [Unicode(false)]
    public string? CompanyContactEmailId { get; set; }

    [Column("Company_TelephoneNo")]
    [StringLength(30)]
    [Unicode(false)]
    public string? CompanyTelephoneNo { get; set; }

    [Column("Company_WebSite")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompanyWebSite { get; set; }

    [Column("Company_ContactNo1")]
    [StringLength(30)]
    [Unicode(false)]
    public string? CompanyContactNo1 { get; set; }

    [Column("Company_ContactNo2")]
    [StringLength(30)]
    [Unicode(false)]
    public string? CompanyContactNo2 { get; set; }

    [Column("Company_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CompanyStatus { get; set; }

    [Column("Company_CrBy")]
    public int? CompanyCrBy { get; set; }

    [Column("Company_CrOn", TypeName = "datetime")]
    public DateTime? CompanyCrOn { get; set; }

    [Column("Company_UpdatedBy")]
    public int? CompanyUpdatedBy { get; set; }

    [Column("Company_UpdatedOn", TypeName = "datetime")]
    public DateTime? CompanyUpdatedOn { get; set; }

    [Column("Company_SubmittedBy")]
    public int? CompanySubmittedBy { get; set; }

    [Column("Company_SubmittedOn", TypeName = "datetime")]
    public DateTime? CompanySubmittedOn { get; set; }

    [Column("Company_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CompanyIpaddress { get; set; }

    [Column("Company_CompID")]
    public int? CompanyCompId { get; set; }

    [Column("Company_HolderName")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CompanyHolderName { get; set; }

    [Column("Company_AccountNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CompanyAccountNo { get; set; }

    [Column("Company_Bankname")]
    [StringLength(5)]
    [Unicode(false)]
    public string? CompanyBankname { get; set; }

    [Column("Company_Branch")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CompanyBranch { get; set; }

    [Column("Company_Conditions")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyConditions { get; set; }

    [Column("Company_Paymentterms")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CompanyPaymentterms { get; set; }
}
