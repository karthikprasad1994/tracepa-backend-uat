using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ScheduleNoteFirst
{
    public int SnfId { get; set; }

    public int? SnfCustId { get; set; }

    public string? SnfDescription { get; set; }

    public string? SnfCategory { get; set; }

    public decimal? SnfCyearAmount { get; set; }

    public decimal? SnfPyearAmount { get; set; }

    public int? SnfYearid { get; set; }

    public int? SnfCompId { get; set; }

    public string? SnfStatus { get; set; }

    public string? SnfDelflag { get; set; }

    public DateTime? SnfCron { get; set; }

    public int? SnfCrby { get; set; }

    public int? SnfUpdatedby { get; set; }

    public DateTime? SnfUpdatedon { get; set; }

    public string? SnfIpaddress { get; set; }
}
