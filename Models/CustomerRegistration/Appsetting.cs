using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class Appsetting
{
    public int PkId { get; set; }

    public string? AccessCode { get; set; }

    public string? ConnectionPath { get; set; }
}
