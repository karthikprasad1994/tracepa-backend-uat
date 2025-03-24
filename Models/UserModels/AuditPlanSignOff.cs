using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_PlanSignOff")]
public partial class AuditPlanSignOff
{
    [Column("APSO_ID")]
    public int? ApsoId { get; set; }

    [Column("APSO_YearID")]
    public int? ApsoYearId { get; set; }

    [Column("APSO_AuditCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ApsoAuditCode { get; set; }

    [Column("APSO_CustID")]
    public int? ApsoCustId { get; set; }

    [Column("APSO_FunctionID")]
    public int? ApsoFunctionId { get; set; }

    [Column("APSO_AuditReview")]
    public int? ApsoAuditReview { get; set; }

    [Column("APSO_AuditPlanStatus")]
    public int? ApsoAuditPlanStatus { get; set; }

    [Column("APSO_Remarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ApsoRemarks { get; set; }

    [Column("APSO_CrBy")]
    public int? ApsoCrBy { get; set; }

    [Column("APSO_CrOn", TypeName = "datetime")]
    public DateTime? ApsoCrOn { get; set; }

    [Column("APSO_UpdatedBy")]
    public int? ApsoUpdatedBy { get; set; }

    [Column("APSO_UpdatedOn", TypeName = "datetime")]
    public DateTime? ApsoUpdatedOn { get; set; }

    [Column("APSO_AppBy")]
    public int? ApsoAppBy { get; set; }

    [Column("APSO_AppOn", TypeName = "datetime")]
    public DateTime? ApsoAppOn { get; set; }

    [Column("APSO_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ApsoStatus { get; set; }

    [Column("APSO_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ApsoIpaddress { get; set; }

    [Column("APSO_AttachID")]
    public int? ApsoAttachId { get; set; }

    [Column("APSO_CompID")]
    public int? ApsoCompId { get; set; }

    [Column("APSO_PGEDetailId")]
    public int? ApsoPgedetailId { get; set; }
}
