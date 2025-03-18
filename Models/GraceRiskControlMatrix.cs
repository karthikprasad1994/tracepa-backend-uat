using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class GraceRiskControlMatrix
{
    public int? GgPkid { get; set; }

    public int? GgRisk { get; set; }

    public int? GgControls { get; set; }

    public int? GgRiskControlScore { get; set; }

    public int? GgCompId { get; set; }
}
