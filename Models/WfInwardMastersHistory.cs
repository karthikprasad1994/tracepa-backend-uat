using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class WfInwardMastersHistory
{
    public int? WimhPkid { get; set; }

    public int? WimhInwardPkid { get; set; }

    public int? WimhUser { get; set; }

    public int? WimhSentToid { get; set; }

    public DateTime? WimhDatetime { get; set; }

    public string? WimhRemarks { get; set; }

    public int? WimhLineNo { get; set; }

    public int? WimhStage { get; set; }

    public int? WimhCompId { get; set; }

    public int? WimhReplyOrForward { get; set; }
}
