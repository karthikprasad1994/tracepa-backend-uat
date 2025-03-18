using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsEmailLog
{
    public int? Id { get; set; }

    public int? CustId { get; set; }

    public string? CustName { get; set; }

    public string? Toemail { get; set; }

    public string? Ccemail { get; set; }

    public string? Mailsubject { get; set; }

    public string? Mailbody { get; set; }

    public string? EmailStatus { get; set; }

    public string? SenderId { get; set; }

    public DateTime? SentOn { get; set; }

    public string? SentBy { get; set; }

    public string? AutomailStatus { get; set; }
}
