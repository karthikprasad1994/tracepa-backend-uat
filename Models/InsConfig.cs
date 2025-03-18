using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class InsConfig
{
    public string? ConfConString { get; set; }

    public string? ConfRdbms { get; set; }

    public string? ConfIpaddress { get; set; }

    public int? ConfPort { get; set; }

    public string? ConfFrom { get; set; }

    public string? ConfHh { get; set; }

    public string? ConfMm { get; set; }

    public string? ConfAmPm { get; set; }

    public string? ConfSenderId { get; set; }

    public string? ConfInsIpaddress { get; set; }

    public string? ConfStatus { get; set; }

    public int? ConfUpdatedBy { get; set; }

    public DateTime? ConfUpdatedOn { get; set; }

    public int? ConfCompId { get; set; }
}
