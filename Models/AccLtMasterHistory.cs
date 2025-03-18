using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccLtMasterHistory
{
    public int? AjehPkid { get; set; }

    public int? AjehAccJeid { get; set; }

    public string? AjehComments { get; set; }

    public int? AjehUserId { get; set; }

    public DateTime? AjehDate { get; set; }

    public string? AjehStatus { get; set; }

    public string? AjehIpaddress { get; set; }

    public int? AjehCompId { get; set; }
}
