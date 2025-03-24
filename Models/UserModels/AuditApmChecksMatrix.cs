using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_APM_ChecksMatrix")]
public partial class AuditApmChecksMatrix
{
    [Column("APMCM_PKID")]
    public int? ApmcmPkid { get; set; }

    [Column("APMCM_APMPKID")]
    public int? ApmcmApmpkid { get; set; }

    [Column("APMCM_YearID")]
    public int? ApmcmYearId { get; set; }

    [Column("APMCM_CustID")]
    public int? ApmcmCustId { get; set; }

    [Column("APMCM_FunctionID")]
    public int? ApmcmFunctionId { get; set; }

    [Column("APMCM_SubFunctionID")]
    public int? ApmcmSubFunctionId { get; set; }

    [Column("APMCM_ProcessID")]
    public int? ApmcmProcessId { get; set; }

    [Column("APMCM_SubProcessID")]
    public int? ApmcmSubProcessId { get; set; }

    [Column("APMCM_RiskID")]
    public int? ApmcmRiskId { get; set; }

    [Column("APMCM_ControlID")]
    public int? ApmcmControlId { get; set; }

    [Column("APMCM_ChecksID")]
    public int? ApmcmChecksId { get; set; }

    [Column("APMCM_MMMID")]
    public int? ApmcmMmmid { get; set; }

    [Column("APMCM_Status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ApmcmStatus { get; set; }

    [Column("APMCM_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ApmcmIpaddress { get; set; }

    [Column("APMCM_CompID")]
    public int? ApmcmCompId { get; set; }
}
