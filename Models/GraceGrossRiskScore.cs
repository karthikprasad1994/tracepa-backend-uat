using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class GraceGrossRiskScore
{
    public int? GgPkid { get; set; }

    public int? GgImpact { get; set; }

    public int? GgLikelihood { get; set; }

    public int? GgRiskScore { get; set; }

    public int? GgCompId { get; set; }
}
