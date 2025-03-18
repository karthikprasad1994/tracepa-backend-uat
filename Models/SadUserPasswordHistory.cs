using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUserPasswordHistory
{
    public decimal UspId { get; set; }

    public int? UspUserid { get; set; }

    public string? UspPassword { get; set; }

    public DateTime? UspDate { get; set; }

    public int? UspCompId { get; set; }
}
