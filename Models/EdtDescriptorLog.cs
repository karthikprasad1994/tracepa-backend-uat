using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtDescriptorLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? DesId { get; set; }

    public string? DescName { get; set; }

    public string? NDescName { get; set; }

    public string? DescNote { get; set; }

    public string? NDescNote { get; set; }

    public string? DescDatatype { get; set; }

    public string? NDescDatatype { get; set; }

    public string? DescSize { get; set; }

    public string? NDescSize { get; set; }

    public string? DescDefaultValues { get; set; }

    public string? NDescDefaultValues { get; set; }

    public string? DescStatus { get; set; }

    public string? NDescStatus { get; set; }

    public string? DescDelFlag { get; set; }

    public string? NDescDelFlag { get; set; }

    public int? DescCompId { get; set; }

    public string? DescIpaddress { get; set; }
}
