using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class GraceEmailSentDetail
{
    public int? EmdId { get; set; }

    public int? EmdMstPkid { get; set; }

    public int? EmdYearId { get; set; }

    public string? EmdFormName { get; set; }

    public string? EmdFromEmailId { get; set; }

    public string? EmdToEmailIds { get; set; }

    public string? EmdCcemailIds { get; set; }

    public string? EmdSubject { get; set; }

    public string? EmdBody { get; set; }

    public string? EmdEmailStatus { get; set; }

    public int? EmdSentUsrId { get; set; }

    public DateTime? EmdSentOn { get; set; }

    public int? EmdCreatedBy { get; set; }

    public DateTime? EmdCreatedOn { get; set; }

    public string? EmdAttachedPath { get; set; }

    public string? EmdAttachedDocIds { get; set; }

    public string? EmdIpaddress { get; set; }

    public int? EmdCompId { get; set; }
}
