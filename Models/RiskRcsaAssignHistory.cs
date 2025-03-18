using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskRcsaAssignHistory
{
    public int? RcsaahPkid { get; set; }

    public int? RcsaahRcsaapkid { get; set; }

    public string? RcsaahComments { get; set; }

    public int? RcsaahUserId { get; set; }

    public DateTime? RcsaahDate { get; set; }

    public string? RcsaahStatus { get; set; }

    public string? RcsaahIpaddress { get; set; }

    public int? RcsaahCompId { get; set; }
}
