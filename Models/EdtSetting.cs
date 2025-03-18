using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtSetting
{
    public int? SetId { get; set; }

    public string? SetCode { get; set; }

    public string? SetValue { get; set; }

    public int? SadUpdatedBy { get; set; }

    public DateTime? SadUpdatedOn { get; set; }

    public string? SetOperation { get; set; }

    public string? SetIpaddress { get; set; }

    public int? SetCompId { get; set; }
}
