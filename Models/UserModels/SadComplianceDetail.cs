using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_Compliance_Details")]
public partial class SadComplianceDetail
{
    [Column("Comp_Id")]
    public int? CompId { get; set; }

    [Column("Comp_CustID")]
    public int? CompCustId { get; set; }

    [Column("Comp_Task")]
    public int? CompTask { get; set; }

    [Column("Comp_Frequency")]
    public int? CompFrequency { get; set; }

    [Column("Comp_LoginName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CompLoginName { get; set; }

    [Column("Comp_Password")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CompPassword { get; set; }

    [Column("Comp_Email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompEmail { get; set; }

    [Column("Comp_MobileNo")]
    [StringLength(15)]
    [Unicode(false)]
    public string? CompMobileNo { get; set; }

    [Column("Comp_Accountdetails")]
    public int? CompAccountdetails { get; set; }

    [Column("Comp_AadhaarAuthen")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompAadhaarAuthen { get; set; }

    [Column("Comp_GSTIN")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CompGstin { get; set; }

    [Column("Comp_Remarks")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CompRemarks { get; set; }

    [Column("Comp_CRON", TypeName = "datetime")]
    public DateTime? CompCron { get; set; }

    [Column("Comp_CRBY")]
    public int? CompCrby { get; set; }

    [Column("Comp_UpdatedOn", TypeName = "datetime")]
    public DateTime? CompUpdatedOn { get; set; }

    [Column("Comp_UpdatedBy")]
    public int? CompUpdatedBy { get; set; }

    [Column("Comp_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CompDelFlag { get; set; }

    [Column("Comp_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CompStatus { get; set; }

    [Column("Comp_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CompIpaddress { get; set; }

    [Column("Comp_CompID")]
    public int? CompCompId { get; set; }
}
