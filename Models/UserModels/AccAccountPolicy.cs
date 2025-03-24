using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Table("Acc_Account_policies")]
public partial class AccAccountPolicy
{
    [Key]
    [Column("ACP_pkid")]
    public int AcpPkid { get; set; }

    [Column("ACP_Custid")]
    public int? AcpCustid { get; set; }

    [Column("ACP_Rpttype")]
    public int? AcpRpttype { get; set; }

    [Column("ACP_Headingid")]
    public int? AcpHeadingid { get; set; }

    [Column("ACP_Description")]
    [StringLength(5000)]
    [Unicode(false)]
    public string AcpDescription { get; set; } = null!;

    [Column("ACp_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AcpStatus { get; set; }

    [Column("ACP_Crby")]
    public int? AcpCrby { get; set; }

    [Column("ACP_Cron", TypeName = "datetime")]
    public DateTime? AcpCron { get; set; }

    [Column("ACP_Updatedby")]
    public int? AcpUpdatedby { get; set; }

    [Column("ACP_Updated_On", TypeName = "datetime")]
    public DateTime? AcpUpdatedOn { get; set; }

    [Column("ACP_Compid")]
    public int? AcpCompid { get; set; }

    [Column("ACP_Ipaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AcpIpaddress { get; set; }

    [Column("ACP_Catagary")]
    public int? AcpCatagary { get; set; }

    [Column("ACP_Branchid")]
    public int? AcpBranchid { get; set; }

    [Column("ACP_Rpttypename")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AcpRpttypename { get; set; }

    [Column("ACP_Heading")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AcpHeading { get; set; }

    [Column("ACP_Yearid")]
    public int? AcpYearid { get; set; }
}
