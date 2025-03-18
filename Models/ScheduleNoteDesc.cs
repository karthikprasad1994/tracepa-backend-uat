using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ScheduleNoteDesc
{
    public int SndId { get; set; }

    public int? SndCustId { get; set; }

    public string? SndDescription { get; set; }

    public string? SndCategory { get; set; }

    public int? SndYearid { get; set; }

    public int? SndCompId { get; set; }

    public string? SndStatus { get; set; }

    public string? SndDelflag { get; set; }

    public DateTime? SndCron { get; set; }

    public int? SndCrby { get; set; }

    public int? SndUpdatedby { get; set; }

    public DateTime? SndUpdatedon { get; set; }

    public string? SndIpaddress { get; set; }
}
