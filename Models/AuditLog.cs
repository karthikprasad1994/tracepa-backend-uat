using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditLog
{
    public long? AdtKeyid { get; set; }

    public int? AdtUserid { get; set; }

    public DateTime? AdtLogin { get; set; }

    public DateTime? AdtLogout { get; set; }

    public int? AdtCompId { get; set; }
}
