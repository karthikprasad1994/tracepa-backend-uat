using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Cust_fontstyle")]
public partial class CustFontstyle
{
    [Column("CF_ID")]
    public int CfId { get; set; }

    [Column("CF_CustId")]
    public int? CfCustId { get; set; }

    [Column("CF_name")]
    [Unicode(false)]
    public string? CfName { get; set; }

    [Column("CF_YEARId")]
    public int? CfYearid { get; set; }

    [Column("CF_CompId")]
    public int? CfCompId { get; set; }

    [Column("CF_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CfStatus { get; set; }

    [Column("CF_DELFLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CfDelflag { get; set; }

    [Column("CF_CRON", TypeName = "datetime")]
    public DateTime? CfCron { get; set; }

    [Column("CF_CRBY")]
    public int? CfCrby { get; set; }

    [Column("CF_UPDATEDBY")]
    public int? CfUpdatedby { get; set; }

    [Column("CF_UPDATEDON", TypeName = "datetime")]
    public DateTime? CfUpdatedon { get; set; }

    [Column("CF_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CfIpaddress { get; set; }
}
