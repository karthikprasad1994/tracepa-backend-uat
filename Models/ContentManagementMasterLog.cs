using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ContentManagementMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? CmmId { get; set; }

    public string? CmmCode { get; set; }

    public string? NCmmCode { get; set; }

    public string? CmmDesc { get; set; }

    public string? NCmmDesc { get; set; }

    public string? CmmCategory { get; set; }

    public string? NCmmCategory { get; set; }

    public string? CmmRemarks { get; set; }

    public string? NCmmRemarks { get; set; }

    public int? CmmKeyComponent { get; set; }

    public int? NCmmKeyComponent { get; set; }

    public string? CmmModule { get; set; }

    public string? NCmmModule { get; set; }

    public int? CmmRiskCategory { get; set; }

    public int? NCmmRiskCategory { get; set; }

    public string? CmmIpaddress { get; set; }

    public int? CmmCompId { get; set; }
}
