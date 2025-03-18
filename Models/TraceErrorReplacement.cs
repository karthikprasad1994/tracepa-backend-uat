using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class TraceErrorReplacement
{
    public int TerPkid { get; set; }

    public string? TerRunTimeError { get; set; }

    public string? TerErrorReplacemnet { get; set; }

    public string? TerStatus { get; set; }
}
