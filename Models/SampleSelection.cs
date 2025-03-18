using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SampleSelection
{
    public int? SsPkid { get; set; }

    public int? SsAuditCodeId { get; set; }

    public int? SsAttachId { get; set; }

    public int? SsCompId { get; set; }

    public int? SsCheckPointId { get; set; }
}
