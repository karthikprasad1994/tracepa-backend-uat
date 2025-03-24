using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditAssignment_SubTask")]
public partial class AuditAssignmentSubTask
{
    [Column("AAST_ID")]
    public int? AastId { get; set; }

    [Column("AAST_AAS_ID")]
    public int? AastAasId { get; set; }

    [Column("AAST_SubTaskID")]
    public int? AastSubTaskId { get; set; }

    [Column("AAST_EmployeeID")]
    public int? AastEmployeeId { get; set; }

    [Column("AAST_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AastDesc { get; set; }

    [Column("AAST_FrequencyID")]
    public int? AastFrequencyId { get; set; }

    [Column("AAST_YearOrMonthID")]
    public int? AastYearOrMonthId { get; set; }

    [Column("AAST_DueDate", TypeName = "datetime")]
    public DateTime? AastDueDate { get; set; }

    [Column("AAST_ExpectedCompletionDate", TypeName = "datetime")]
    public DateTime? AastExpectedCompletionDate { get; set; }

    [Column("AAST_WorkStatusID")]
    public int? AastWorkStatusId { get; set; }

    [Column("AAST_Closed")]
    public int? AastClosed { get; set; }

    [Column("AAST_AttachID")]
    public int? AastAttachId { get; set; }

    [Column("AAST_CrBy")]
    public int? AastCrBy { get; set; }

    [Column("AAST_CrOn", TypeName = "datetime")]
    public DateTime? AastCrOn { get; set; }

    [Column("AAST_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AastIpaddress { get; set; }

    [Column("AAST_CompID")]
    public int? AastCompId { get; set; }

    [Column("AAST_AssistedByEmployeesID")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AastAssistedByEmployeesId { get; set; }

    [Column("AAST_Review")]
    public int? AastReview { get; set; }
}
