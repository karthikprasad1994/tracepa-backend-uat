using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadGrplvlMember
{
    public short? GldGrpLvlId { get; set; }

    public short? GldUserId { get; set; }

    public DateTime? GldCrDate { get; set; }

    public DateTime? GldFromDate { get; set; }

    public DateTime? GldToDate { get; set; }

    public short? GldCrBy { get; set; }

    public short? GldGrpLvlPosn { get; set; }
}
