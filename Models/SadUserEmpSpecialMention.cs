using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUserEmpSpecialMention
{
    public int? SusPkid { get; set; }

    public int? SusUserEmpId { get; set; }

    public string? SusSpecialMention { get; set; }

    public DateTime? SusDate { get; set; }

    public string? SusParticulars { get; set; }

    public string? SusDealtWith { get; set; }

    public int? SusAttachId { get; set; }

    public int? SusCrBy { get; set; }

    public DateTime? SusCrOn { get; set; }

    public int? SusUpdatedBy { get; set; }

    public DateTime? SusUpdatedOn { get; set; }

    public string? SusIpaddress { get; set; }

    public int? SusCompId { get; set; }
}
