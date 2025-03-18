using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CompanyLogoSetting
{
    public int ClsId { get; set; }

    public byte[]? ClsBigdata { get; set; }

    public double? ClsSize { get; set; }

    public string? ClsFileName { get; set; }

    public string? ClsExtn { get; set; }

    public int? ClsCompId { get; set; }

    public string? ClsStatus { get; set; }
}
