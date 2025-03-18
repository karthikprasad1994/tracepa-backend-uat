using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class TraceFolder
{
    public int? TfPkid { get; set; }

    public int? TfCabinetId { get; set; }

    public int? TfSubCabinetId { get; set; }

    public string? TfName { get; set; }

    public string? TfRemarks { get; set; }

    public string? TfStatus { get; set; }

    public DateTime? TfCrOn { get; set; }

    public int? TfCrBy { get; set; }

    public DateTime? TfUpdatedOn { get; set; }

    public int? TfUpdatedBy { get; set; }

    public string? TfIpaddress { get; set; }

    public int? TfCompId { get; set; }
}
