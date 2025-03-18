using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtDescriptor
{
    public int? DesId { get; set; }

    public string? DescName { get; set; }

    public string? DescNote { get; set; }

    public string? DescDatatype { get; set; }

    public string? DescSize { get; set; }

    public string? DescDefaultValues { get; set; }

    public string? DescStatus { get; set; }

    public string? DescDelFlag { get; set; }

    public int? DescCrby { get; set; }

    public DateTime? DescCron { get; set; }

    public int? DescUpdatedby { get; set; }

    public DateTime? DescUpdatedon { get; set; }

    public int? DescRecallby { get; set; }

    public DateTime? DescRecallon { get; set; }

    public int? DescDeletedby { get; set; }

    public DateTime? DescDeletedon { get; set; }

    public int? DescApprovedby { get; set; }

    public DateTime? DescApprovedon { get; set; }

    public string? DescIpaddress { get; set; }

    public int? DescCompId { get; set; }
}
