using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Compliance_Plan_history")]
public partial class CompliancePlanHistory
{
    [Column("CPH_ID")]
    public int? CphId { get; set; }

    [Column("CPH_YearID")]
    public int? CphYearId { get; set; }

    [Column("CPH_CPID")]
    public int? CphCpid { get; set; }

    [Column("CPH_CustomerID")]
    public int? CphCustomerId { get; set; }

    [Column("CPH_FunctionID")]
    public int? CphFunctionId { get; set; }

    [Column("CPH_SubFunctionID")]
    public int? CphSubFunctionId { get; set; }

    [Column("CPH_ScheduledMonthID")]
    public int? CphScheduledMonthId { get; set; }

    [Column("CPH_IsCurrentYear")]
    public int? CphIsCurrentYear { get; set; }

    [Column("CPH_status")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CphStatus { get; set; }

    [Column("CPH_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CphRemarks { get; set; }

    [Column("CPH_CreatedBy")]
    public int? CphCreatedBy { get; set; }

    [Column("CPH_CreatedOn", TypeName = "datetime")]
    public DateTime? CphCreatedOn { get; set; }

    [Column("CPH_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CphIpaddress { get; set; }

    [Column("CPH_CompID")]
    public int? CphCompId { get; set; }
}
