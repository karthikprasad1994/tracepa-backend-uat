using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAssignmentEmpSubTask
{
    public int? AaestId { get; set; }

    public int? AaestAasId { get; set; }

    public int? AaestAastId { get; set; }

    public int? AaestWorkStatusId { get; set; }

    public string? AaestComments { get; set; }

    public int? AaestAttachId { get; set; }

    public int? AaestCrBy { get; set; }

    public DateTime? AaestCrOn { get; set; }

    public string? AaestIpaddress { get; set; }

    public int? AaestCompId { get; set; }
}
