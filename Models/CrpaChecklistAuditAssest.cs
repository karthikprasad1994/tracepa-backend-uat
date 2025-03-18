using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaChecklistAuditAssest
{
    public int? CradPkid { get; set; }

    public int? CradCauditId { get; set; }

    public int? CradLocationid { get; set; }

    public int? CradSectionid { get; set; }

    public int? CradSubsectionid { get; set; }

    public int? CradProcessid { get; set; }

    public int? CradSubprocessid { get; set; }

    public int? CradFindings { get; set; }

    public int? CradScoreStandard { get; set; }

    public int? CradScoreResult { get; set; }

    public string? CradComments { get; set; }

    public DateTime? CradDate { get; set; }

    public int? CradYearid { get; set; }

    public int? CradCreatedby { get; set; }

    public DateTime? CradCreatedon { get; set; }

    public int? CradUpdatedby { get; set; }

    public DateTime? CradUpdatedon { get; set; }

    public string? CradIpaddress { get; set; }

    public int? CradCompId { get; set; }
}
