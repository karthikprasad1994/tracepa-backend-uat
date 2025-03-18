using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadModule
{
    public int? ModId { get; set; }

    public string? ModCode { get; set; }

    public string? ModDescription { get; set; }

    public string? ModNotes { get; set; }

    public int? ModParent { get; set; }

    public string? ModDelflag { get; set; }

    public string? ModNavFunc { get; set; }

    public int? ModCompId { get; set; }

    public string? ModButtons { get; set; }
}
