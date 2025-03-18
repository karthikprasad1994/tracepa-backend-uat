using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class TraceColorMaster
{
    public int TcId { get; set; }

    public string? TcColorName { get; set; }

    public string? TcColorHex { get; set; }

    public decimal? TcKeyCode { get; set; }

    public string? TcAccessCode { get; set; }

    public string? TcStatus { get; set; }

    public int? TcCompId { get; set; }
}
