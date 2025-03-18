using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadStatutoryPartnerDetail
{
    public int? SspId { get; set; }

    public int? SspCustId { get; set; }

    public string? SspPartnerName { get; set; }

    public string? SspPan { get; set; }

    public DateTime? SspDoj { get; set; }

    public decimal? SspShareOfProfit { get; set; }

    public decimal? SspCapitalAmount { get; set; }

    public DateTime? SspCron { get; set; }

    public int? SspCrby { get; set; }

    public DateTime? SspUpdatedOn { get; set; }

    public int? SspUpdatedBy { get; set; }

    public string? SspDelFlag { get; set; }

    public string? SspStatus { get; set; }

    public string? SspIpaddress { get; set; }

    public int? SspCompId { get; set; }

    public int? SspAttachId { get; set; }
}
