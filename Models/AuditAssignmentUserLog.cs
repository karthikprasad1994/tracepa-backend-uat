using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAssignmentUserLog
{
    public int AaulId { get; set; }

    public int? AaulAdtKeyid { get; set; }

    public int? AaulUserId { get; set; }

    public DateTime? AaulDate { get; set; }

    public int? AaulAasId { get; set; }

    public int? AaulAasStatus { get; set; }

    public int? AaulCompId { get; set; }
}
