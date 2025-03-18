using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadReportContentMaster
{
    public int? RcmId { get; set; }

    public int? RcmReportId { get; set; }

    public string? RcmReportName { get; set; }

    public string? RcmHeading { get; set; }

    public string? RcmDescription { get; set; }

    public string? RcmDelflag { get; set; }

    public string? RcmStatus { get; set; }

    public int? RcmCrBy { get; set; }

    public DateTime? RcmCrOn { get; set; }

    public int? RcmUpdatedBy { get; set; }

    public DateTime? RcmUpdatedOn { get; set; }

    public int? RcmDeletedBy { get; set; }

    public DateTime? RcmDeletedOn { get; set; }

    public int? RcmAppBy { get; set; }

    public DateTime? RcmAppOn { get; set; }

    public string? RcmIpaddress { get; set; }

    public int? RcmCompId { get; set; }

    public int? RcmYearid { get; set; }
}
