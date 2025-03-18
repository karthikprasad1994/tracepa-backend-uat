using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtSettingsLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? SetId { get; set; }

    public string? SetCode { get; set; }

    public string? NSetCode { get; set; }

    public string? SetValue { get; set; }

    public string? NSetValue { get; set; }

    public DateTime? SadRunDate { get; set; }

    public string? SetCompId { get; set; }

    public string? SetIpaddress { get; set; }

    public string? SetOperation { get; set; }

    public string? NSetOperation { get; set; }
}
