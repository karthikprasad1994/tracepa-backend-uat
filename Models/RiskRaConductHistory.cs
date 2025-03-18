using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskRaConductHistory
{
    public int? RaahPkid { get; set; }

    public int? RaahRaapkid { get; set; }

    public string? RaahComments { get; set; }

    public int? RaahUserId { get; set; }

    public DateTime? RaahDate { get; set; }

    public string? RaahStatus { get; set; }

    public string? RaahIpaddress { get; set; }

    public int? RaahCompId { get; set; }
}
