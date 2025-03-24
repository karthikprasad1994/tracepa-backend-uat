using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditAssignment_EmpSubTask")]
public partial class AuditAssignmentEmpSubTask
{
    [Column("AAEST_ID")]
    public int? AaestId { get; set; }

    [Column("AAEST_AAS_ID")]
    public int? AaestAasId { get; set; }

    [Column("AAEST_AAST_ID")]
    public int? AaestAastId { get; set; }

    [Column("AAEST_WorkStatusID")]
    public int? AaestWorkStatusId { get; set; }

    [Column("AAEST_Comments")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AaestComments { get; set; }

    [Column("AAEST_AttachID")]
    public int? AaestAttachId { get; set; }

    [Column("AAEST_CrBy")]
    public int? AaestCrBy { get; set; }

    [Column("AAEST_CrOn", TypeName = "datetime")]
    public DateTime? AaestCrOn { get; set; }

    [Column("AAEST_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AaestIpaddress { get; set; }

    [Column("AAEST_CompID")]
    public int? AaestCompId { get; set; }
}
