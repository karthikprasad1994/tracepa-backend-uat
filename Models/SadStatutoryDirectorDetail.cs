using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadStatutoryDirectorDetail
{
    public int? SsdId { get; set; }

    public int? SsdCustId { get; set; }

    public string? SsdDirectorName { get; set; }

    public DateTime? SsdDob { get; set; }

    public string? SsdDin { get; set; }

    public string? SsdMobileNo { get; set; }

    public string? SsdEmail { get; set; }

    public string? SsdRemarks { get; set; }

    public DateTime? SsdCron { get; set; }

    public int? SsdCrby { get; set; }

    public DateTime? SsdUpdatedOn { get; set; }

    public int? SsdUpdatedBy { get; set; }

    public string? SsdDelFlag { get; set; }

    public string? SsdStatus { get; set; }

    public string? SsdIpaddress { get; set; }

    public int? SsdCompId { get; set; }
}
