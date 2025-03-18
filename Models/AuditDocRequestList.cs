using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditDocRequestList
{
    public int? DrlDrlid { get; set; }

    public int? DrlDocTypeId { get; set; }

    public string? DrlName { get; set; }

    public string? DrlDescription { get; set; }

    public string? DrlDocumentType { get; set; }

    public int? DrlCrBy { get; set; }

    public DateTime? DrlCron { get; set; }

    public int? DrlUpdatedBy { get; set; }

    public DateTime? DrlUpdatedOn { get; set; }

    public string? DrlStatus { get; set; }

    public string? DrlDtype { get; set; }

    public string? DrlSize { get; set; }

    public string? DrlSampleId { get; set; }

    public string? DrlIpaddress { get; set; }

    public int? DrlCompId { get; set; }
}
