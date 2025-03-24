using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Compliance_Checklist")]
public partial class ComplianceChecklist
{
    [Column("CRCD_PkID")]
    public int? CrcdPkId { get; set; }

    [Column("CRCD_MasID")]
    public int? CrcdMasId { get; set; }

    [Column("CRCD_YearID")]
    public int? CrcdYearId { get; set; }

    [Column("CRCD_SubFunID")]
    public int? CrcdSubFunId { get; set; }

    [Column("CRCD_PID")]
    public int? CrcdPid { get; set; }

    [Column("CRCD_SubPID")]
    public int? CrcdSubPid { get; set; }

    [Column("CRCD_RiskID")]
    public int? CrcdRiskId { get; set; }

    [Column("CRCD_ControlID")]
    public int? CrcdControlId { get; set; }

    [Column("CRCD_CheckID")]
    public int? CrcdCheckId { get; set; }

    [Column("CRCD_AttchID")]
    public int? CrcdAttchId { get; set; }

    [Column("CRCD_Operation")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CrcdOperation { get; set; }

    [Column("CRCD_IPAddress")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CrcdIpaddress { get; set; }

    [Column("CRCD_SunFunc")]
    [Unicode(false)]
    public string? CrcdSunFunc { get; set; }

    [Column("CRCD_Process")]
    [Unicode(false)]
    public string? CrcdProcess { get; set; }

    [Column("CRCD_SunProcess")]
    [Unicode(false)]
    public string? CrcdSunProcess { get; set; }

    [Column("CRCD_Risk")]
    [Unicode(false)]
    public string? CrcdRisk { get; set; }

    [Column("CRCD_Control")]
    [Unicode(false)]
    public string? CrcdControl { get; set; }

    [Column("CRCD_CheckDesc")]
    [Unicode(false)]
    public string? CrcdCheckDesc { get; set; }

    [Column("CRCD_InherentRiskID")]
    public int? CrcdInherentRiskId { get; set; }

    [Column("CRCD_CertID")]
    public int? CrcdCertId { get; set; }

    [Column("CRCD_RiskRemarks")]
    [Unicode(false)]
    public string? CrcdRiskRemarks { get; set; }

    [Column("CRCD_CheckRemarks")]
    [Unicode(false)]
    public string? CrcdCheckRemarks { get; set; }

    [Column("CRCD_CompID")]
    public int? CrcdCompId { get; set; }
}
