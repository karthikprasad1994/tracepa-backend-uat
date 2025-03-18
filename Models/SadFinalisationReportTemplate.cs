using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadFinalisationReportTemplate
{
    public int? TemId { get; set; }

    public int? TemYearid { get; set; }

    public int? TemFunctionId { get; set; }

    public string? TemModule { get; set; }

    public int? TemReportTitle { get; set; }

    public string? TemContentId { get; set; }

    public string? TemSortOrder { get; set; }

    public string? TemDelflag { get; set; }

    public string? TemStatus { get; set; }

    public int? TemCrBy { get; set; }

    public DateTime? TemCrOn { get; set; }

    public int? TemUpdatedBy { get; set; }

    public DateTime? TemUpdatedOn { get; set; }

    public int? TemDeletedBy { get; set; }

    public DateTime? TemDeletedOn { get; set; }

    public int? TemAppBy { get; set; }

    public DateTime? TemAppOn { get; set; }

    public string? TemIpaddress { get; set; }

    public int? TemCompid { get; set; }
}
