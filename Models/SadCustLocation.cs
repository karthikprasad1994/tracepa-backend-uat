using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadCustLocation
{
    public int? MasId { get; set; }

    public string? MasCode { get; set; }

    public string? MasDescription { get; set; }

    public string? MasDelFlag { get; set; }

    public int? MasCustId { get; set; }

    public string? MasLocAddress { get; set; }

    public string? MasContactPerson { get; set; }

    public string? MasContactMobileNo { get; set; }

    public string? MasContactLandLineNo { get; set; }

    public string? MasContactEmail { get; set; }

    public string? MasDesignation { get; set; }

    public DateTime? MasCron { get; set; }

    public int? MasCrby { get; set; }

    public DateTime? MasUpdatedOn { get; set; }

    public int? MasUpdatedBy { get; set; }

    public string? MasStatus { get; set; }

    public string? MasIpaddress { get; set; }

    public int? MasCompId { get; set; }
}
