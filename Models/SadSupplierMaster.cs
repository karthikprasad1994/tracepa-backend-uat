using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadSupplierMaster
{
    public int? SupId { get; set; }

    public string? SupName { get; set; }

    public string? SupCode { get; set; }

    public string? SupContactPerson { get; set; }

    public string? SupAddress { get; set; }

    public string? SupPhoneNo { get; set; }

    public string? SupFax { get; set; }

    public string? SupEmail { get; set; }

    public string? SupWebsite { get; set; }

    public int? SupCrby { get; set; }

    public DateTime? SupCron { get; set; }

    public string? SupStatus { get; set; }

    public string? SupIpaddress { get; set; }

    public int? SupCompId { get; set; }
}
