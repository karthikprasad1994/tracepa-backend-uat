using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditLogDetail
{
    public int AldId { get; set; }

    public int? AldMasid { get; set; }

    public int? AldUserId { get; set; }

    public string AldModuleName { get; set; } = null!;

    public int? AldModuleTime { get; set; }

    public int? AldTotalIdleTime { get; set; }

    public int? AldScreenTotalTime { get; set; }

    public int? AldCompId { get; set; }
}
