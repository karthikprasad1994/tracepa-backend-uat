using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class DocReviewremarksHistory
{
    public int? DrhId { get; set; }

    public int? DrhMasid { get; set; }

    public int? DrhCustid { get; set; }

    public int? DrhLoeid { get; set; }

    public string? DrhRemarksType { get; set; }

    public string? DrhRemarks { get; set; }

    public int? DrhRemarksBy { get; set; }

    public string? DrhStatus { get; set; }

    public DateTime? DrhDate { get; set; }

    public string? DrhIpaddress { get; set; }

    public int? DrhCompId { get; set; }

    public int? DrhYearid { get; set; }

    public int? DrhAttchmentid { get; set; }
}
