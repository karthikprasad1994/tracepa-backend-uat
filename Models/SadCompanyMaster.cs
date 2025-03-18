using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadCompanyMaster
{
    public int? CmId { get; set; }

    public string? CmAccessCode { get; set; }

    public string? CmCompanyName { get; set; }

    public string? CmDelFlag { get; set; }

    public int? CmCreatedBy { get; set; }

    public DateTime? CmCreatedOn { get; set; }
}
