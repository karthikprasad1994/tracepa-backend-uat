using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_AnnualPlan")]
public partial class AuditAnnualPlan
{
    [Column("AAP_PKID")]
    public int? AapPkid { get; set; }

    [Column("AAP_YearID")]
    public int? AapYearId { get; set; }

    [Column("AAP_MonthID")]
    public int? AapMonthId { get; set; }

    [Column("AAP_CustID")]
    public int? AapCustId { get; set; }

    [Column("AAP_FunID")]
    public int? AapFunId { get; set; }

    [Column("AAP_ResourceID")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AapResourceId { get; set; }

    [Column("AAP_Crby")]
    public int? AapCrby { get; set; }

    [Column("AAP_Cron", TypeName = "datetime")]
    public DateTime? AapCron { get; set; }

    [Column("AAP_Updatedby")]
    public int? AapUpdatedby { get; set; }

    [Column("AAP_UpdatedOn", TypeName = "datetime")]
    public DateTime? AapUpdatedOn { get; set; }

    [Column("AAP_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AapIpaddress { get; set; }

    [Column("AAP_CompID")]
    public int? AapCompId { get; set; }

    [Column("AAP_Comments")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AapComments { get; set; }
}
