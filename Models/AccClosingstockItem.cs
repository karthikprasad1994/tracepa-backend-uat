using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccClosingstockItem
{
    public int AcsiId { get; set; }

    public string AcsiItemdescCode { get; set; } = null!;

    public string? AcsiItemdesc { get; set; }

    public string? AcsiClassification { get; set; }

    public string? AcsiType { get; set; }

    public int? AcsiCustid { get; set; }

    public int? AcsiQty { get; set; }

    public decimal? AcsiRate { get; set; }

    public decimal? AcsiTotal { get; set; }

    public string? AcsiDelflg { get; set; }

    public DateTime? AcsiCron { get; set; }

    public int? AcsiCrby { get; set; }

    public int? AcsiApprovedby { get; set; }

    public DateTime? AcsiApprovedon { get; set; }

    public string? AcsiStatus { get; set; }

    public int? AcsiUpdatedby { get; set; }

    public DateTime? AcsiUpdatedon { get; set; }

    public int? AcsiDeletedby { get; set; }

    public DateTime? AcsiDeletedon { get; set; }

    public int? AcsiRecallby { get; set; }

    public DateTime? AcsiRecallon { get; set; }

    public string? AcsiIpaddress { get; set; }

    public int? AcsiCompId { get; set; }

    public int? AcsiYearid { get; set; }
}
