using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_APM_Assignment_Details")]
public partial class AuditApmAssignmentDetail
{
    [Column("AAPM_ID")]
    public int? AapmId { get; set; }

    [Column("AAPM_YearID")]
    public int? AapmYearId { get; set; }

    [Column("AAPM_AuditCodeID")]
    public int? AapmAuditCodeId { get; set; }

    [Column("AAPM_CustID")]
    public int? AapmCustId { get; set; }

    [Column("AAPM_FunctionID")]
    public int? AapmFunctionId { get; set; }

    [Column("AAPM_AuditTaskID")]
    public int? AapmAuditTaskId { get; set; }

    [Column("AAPM_AuditTaskType")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AapmAuditTaskType { get; set; }

    [Column("AAPM_PStartDate", TypeName = "datetime")]
    public DateTime? AapmPstartDate { get; set; }

    [Column("AAPM_PEndDate", TypeName = "datetime")]
    public DateTime? AapmPendDate { get; set; }

    [Column("AAPM_Resource")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AapmResource { get; set; }

    [Column("AAPM_CrBy")]
    public int? AapmCrBy { get; set; }

    [Column("AAPM_CrOn", TypeName = "datetime")]
    public DateTime? AapmCrOn { get; set; }

    [Column("AAPM_UpdatedBy")]
    public int? AapmUpdatedBy { get; set; }

    [Column("AAPM_UpdatedOn", TypeName = "datetime")]
    public DateTime? AapmUpdatedOn { get; set; }

    [Column("AAPM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AapmIpaddress { get; set; }

    [Column("AAPM_CompID")]
    public int? AapmCompId { get; set; }
}
