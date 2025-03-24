using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_CUST_Accounting_Template")]
public partial class SadCustAccountingTemplate
{
    [Column("Cust_PKID")]
    public int? CustPkid { get; set; }

    [Column("Cust_ID")]
    public int? CustId { get; set; }

    [Column("Cust_Desc")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustDesc { get; set; }

    [Column("Cust_Value")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustValue { get; set; }

    [Column("Cust_LocationId")]
    public int? CustLocationId { get; set; }

    [Column("Cust_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CustDelflag { get; set; }

    [Column("Cust_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CustStatus { get; set; }

    [Column("Cust_AttchID")]
    public int? CustAttchId { get; set; }

    [Column("Cust_CrBy")]
    public int? CustCrBy { get; set; }

    [Column("Cust_CrOn", TypeName = "datetime")]
    public DateTime? CustCrOn { get; set; }

    [Column("Cust_UpdatedBy")]
    public int? CustUpdatedBy { get; set; }

    [Column("Cust_UpdatedOn", TypeName = "datetime")]
    public DateTime? CustUpdatedOn { get; set; }

    [Column("Cust_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CustIpaddress { get; set; }

    [Column("Cust_Compid")]
    public int? CustCompid { get; set; }
}
