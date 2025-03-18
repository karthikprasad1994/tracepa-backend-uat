using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadFinalisationReportContent
{
    public int? FptId { get; set; }

    public int? FptYearid { get; set; }

    public int? FptFunctionId { get; set; }

    public string? FptFunctionName { get; set; }

    public string? FptTitle { get; set; }

    public string? FptDetails { get; set; }

    public string? FptDelflag { get; set; }

    public string? FptStatus { get; set; }

    public int? FptCrBy { get; set; }

    public DateTime? FptCrOn { get; set; }

    public int? FptUpdatedBy { get; set; }

    public DateTime? FptUpdatedOn { get; set; }

    public int? FptDeletedBy { get; set; }

    public DateTime? FptDeletedOn { get; set; }

    public int? FptAppBy { get; set; }

    public DateTime? FptAppOn { get; set; }

    public string? FptIpaddress { get; set; }

    public int? FptCompId { get; set; }
}
