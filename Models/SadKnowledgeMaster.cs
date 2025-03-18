using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadKnowledgeMaster
{
    public int? TkbId { get; set; }

    public string? TkbSubject { get; set; }

    public string? TkbContent { get; set; }

    public int? TkbCrBy { get; set; }

    public DateTime? TkbCrOn { get; set; }

    public int? TkbUpdatedBy { get; set; }

    public DateTime? TkbUpdatedOn { get; set; }

    public string? TkbStatus { get; set; }

    public int? TkbApprovedBy { get; set; }

    public DateTime? TkbApprovedOn { get; set; }

    public string? TkbIpaddress { get; set; }

    public int? TkbCompId { get; set; }
}
