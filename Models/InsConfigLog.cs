using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class InsConfigLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public string? ConfConString { get; set; }

    public string? NconfConString { get; set; }

    public string? ConfRdbms { get; set; }

    public string? NconfRdbms { get; set; }

    public string? ConfIpaddress { get; set; }

    public string? NconfIpaddress { get; set; }

    public int? ConfPort { get; set; }

    public int? NconfPort { get; set; }

    public string? ConfFrom { get; set; }

    public string? NconfFrom { get; set; }

    public string? ConfHh { get; set; }

    public string? NconfHh { get; set; }

    public string? ConfMm { get; set; }

    public string? NconfMm { get; set; }

    public string? ConfAmPm { get; set; }

    public string? NconfAmPm { get; set; }

    public string? ConfSenderId { get; set; }

    public string? NconfSenderId { get; set; }

    public DateTime? ConfRunDate { get; set; }

    public string? ConfInsIpaddress { get; set; }

    public string? ConfCompId { get; set; }
}
