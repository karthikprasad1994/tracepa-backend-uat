using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CustFontstyle
{
    public int CfId { get; set; }

    public int? CfCustId { get; set; }

    public string? CfName { get; set; }

    public int? CfYearid { get; set; }

    public int? CfCompId { get; set; }

    public string? CfStatus { get; set; }

    public string? CfDelflag { get; set; }

    public DateTime? CfCron { get; set; }

    public int? CfCrby { get; set; }

    public int? CfUpdatedby { get; set; }

    public DateTime? CfUpdatedon { get; set; }

    public string? CfIpaddress { get; set; }
}
