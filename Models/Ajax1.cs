using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class Ajax1
{
    public int Id { get; set; }

    public string Engine { get; set; } = null!;

    public string Browser { get; set; } = null!;

    public string Platform1 { get; set; } = null!;

    public double Version1 { get; set; }

    public string Grade { get; set; } = null!;

    public decimal Marketshare { get; set; }

    public DateTime Released { get; set; }
}
