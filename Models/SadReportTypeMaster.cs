using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadReportTypeMaster
{
    public int? RtmId { get; set; }

    public string? RtmReportTypeName { get; set; }

    public int? RtmTemplateId { get; set; }

    public int? RtmCommunicationId { get; set; }

    public int? RtmCreatedby { get; set; }

    public DateTime? RtmCreatedOn { get; set; }

    public int? RtmUpdatedBy { get; set; }

    public DateTime? RtmUpdatedOn { get; set; }

    public int? RtmApprovedBy { get; set; }

    public DateTime? RtmApprovedOn { get; set; }

    public string? RtmStatus { get; set; }

    public string? RtmDelFlag { get; set; }

    public string? RtmIpaddress { get; set; }

    public int? RtmCompId { get; set; }

    public int? RtmAudrptType { get; set; }
}
