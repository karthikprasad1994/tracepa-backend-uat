using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsplUserLogin
{
    public int? MulPkid { get; set; }

    public string? MulFullName { get; set; }

    public string? MulLoginName { get; set; }

    public string? MulPassword { get; set; }

    public DateTime? MulLastLogindate { get; set; }

    public string? MulCreatedBy { get; set; }

    public DateTime? MulCreatedOn { get; set; }

    public int? MulUnSuccessfullAttempts { get; set; }

    public int? MulNoOfLogins { get; set; }

    public string? MulIpaddress { get; set; }
}
