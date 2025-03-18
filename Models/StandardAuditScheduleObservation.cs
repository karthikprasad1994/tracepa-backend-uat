using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditScheduleObservation
{
    public int? SsoPkid { get; set; }

    public int? SsoSaId { get; set; }

    public int? SsoSacCheckPointId { get; set; }

    public string? SsoObservations { get; set; }

    public int? SsoCrBy { get; set; }

    public DateTime? SsoCrOn { get; set; }

    public string? SsoIpaddress { get; set; }

    public int? SsoCompId { get; set; }
}
