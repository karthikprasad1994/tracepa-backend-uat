using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Table("Acc_Cashflow")]
public partial class AccCashflow
{
    [Key]
    [Column("ACF_pkid")]
    public int AcfPkid { get; set; }

    [Column("ACF_Description")]
    [StringLength(5000)]
    [Unicode(false)]
    public string AcfDescription { get; set; } = null!;

    [Column("ACF_Custid")]
    public int? AcfCustid { get; set; }

    [Column("ACF_Current_Amount", TypeName = "money")]
    public decimal? AcfCurrentAmount { get; set; }

    [Column("ACF_Prev_Amount", TypeName = "money")]
    public decimal? AcfPrevAmount { get; set; }

    [Column("ACF_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AcfStatus { get; set; }

    [Column("ACF_Crby")]
    public int? AcfCrby { get; set; }

    [Column("ACF_Cron", TypeName = "datetime")]
    public DateTime? AcfCron { get; set; }

    [Column("ACF_Updatedby")]
    public int? AcfUpdatedby { get; set; }

    [Column("ACF_Updated_On", TypeName = "datetime")]
    public DateTime? AcfUpdatedOn { get; set; }

    [Column("ACF_Compid")]
    public int? AcfCompid { get; set; }

    [Column("ACF_Ipaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AcfIpaddress { get; set; }

    [Column("ACF_Catagary")]
    public int? AcfCatagary { get; set; }

    [Column("ACF_Branchid")]
    public int? AcfBranchid { get; set; }

    [Column("ACF_yearid")]
    public int? AcfYearid { get; set; }
}
