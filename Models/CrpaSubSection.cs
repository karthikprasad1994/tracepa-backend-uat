using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaSubSection
{
    public int CasuId { get; set; }

    public string CasuCode { get; set; } = null!;

    public string? CasuSubsectionname { get; set; }

    public string? CasuSectionid { get; set; }

    public int? CasuPoints { get; set; }

    public string? CasuDesc { get; set; }

    public string CasuDelflg { get; set; } = null!;

    public DateTime? CasuCron { get; set; }

    public int? CasuCrby { get; set; }

    public int? CasuApprovedby { get; set; }

    public DateTime? CasuApprovedon { get; set; }

    public string? CasuStatus { get; set; }

    public int? CasuUpdatedby { get; set; }

    public DateTime? CasuUpdatedon { get; set; }

    public int? CasuDeletedby { get; set; }

    public DateTime? CasuDeletedon { get; set; }

    public int? CasuRecallby { get; set; }

    public DateTime? CasuRecallon { get; set; }

    public string? CasuIpaddress { get; set; }

    public int? CasuCompId { get; set; }

    public int? CasuYearid { get; set; }
}
