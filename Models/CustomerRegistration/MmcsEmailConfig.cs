using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsEmailConfig
{
    public int? EcId { get; set; }

    public string? EcSmtpaddress { get; set; }

    public string? EcPortno { get; set; }

    public string? EcSenderEmailId { get; set; }

    public string? EcPassword { get; set; }

    public string? EcReferenceCcEmailId { get; set; }

    public string? EcCreatedby { get; set; }

    public DateTime? EcCreatedOn { get; set; }
}
