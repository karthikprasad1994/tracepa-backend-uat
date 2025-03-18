using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadCurrencyMaster
{
    public int CurId { get; set; }

    public string? CurCode { get; set; }

    public string? CurCountryName { get; set; }

    public string? CurStatus { get; set; }

    public int? CurCompId { get; set; }
}
