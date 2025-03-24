using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_CUST_LOCATION")]
public partial class SadCustLocation
{
    [Column("Mas_Id")]
    public int? MasId { get; set; }

    [Column("Mas_code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MasCode { get; set; }

    [Column("Mas_Description")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MasDescription { get; set; }

    [Column("Mas_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? MasDelFlag { get; set; }

    [Column("Mas_CustID")]
    public int? MasCustId { get; set; }

    [Column("Mas_Loc_Address")]
    [StringLength(500)]
    [Unicode(false)]
    public string? MasLocAddress { get; set; }

    [Column("Mas_Contact_Person")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MasContactPerson { get; set; }

    [Column("Mas_Contact_MobileNo")]
    [StringLength(15)]
    [Unicode(false)]
    public string? MasContactMobileNo { get; set; }

    [Column("Mas_Contact_LandLineNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MasContactLandLineNo { get; set; }

    [Column("Mas_Contact_Email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MasContactEmail { get; set; }

    [Column("mas_Designation")]
    [StringLength(500)]
    [Unicode(false)]
    public string? MasDesignation { get; set; }

    [Column("Mas_CRON", TypeName = "datetime")]
    public DateTime? MasCron { get; set; }

    [Column("Mas_CRBY")]
    public int? MasCrby { get; set; }

    [Column("Mas_UpdatedOn", TypeName = "datetime")]
    public DateTime? MasUpdatedOn { get; set; }

    [Column("Mas_UpdatedBy")]
    public int? MasUpdatedBy { get; set; }

    [Column("Mas_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MasStatus { get; set; }

    [Column("Mas_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MasIpaddress { get; set; }

    [Column("Mas_CompID")]
    public int? MasCompId { get; set; }
}
