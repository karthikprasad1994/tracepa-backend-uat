using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAssignmentSubTask
{
    public int? AastId { get; set; }

    public int? AastAasId { get; set; }

    public int? AastSubTaskId { get; set; }

    public int? AastEmployeeId { get; set; }

    public string? AastDesc { get; set; }

    public int? AastFrequencyId { get; set; }

    public int? AastYearOrMonthId { get; set; }

    public DateTime? AastDueDate { get; set; }

    public DateTime? AastExpectedCompletionDate { get; set; }

    public int? AastWorkStatusId { get; set; }

    public int? AastClosed { get; set; }

    public int? AastAttachId { get; set; }

    public int? AastCrBy { get; set; }

    public DateTime? AastCrOn { get; set; }

    public string? AastIpaddress { get; set; }

    public int? AastCompId { get; set; }

    public string? AastAssistedByEmployeesId { get; set; }

    public int? AastReview { get; set; }
}
