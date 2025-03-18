using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtDoctypeLinkLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? EddPk { get; set; }

    public int? EddDoctypeid { get; set; }

    public int? NEddDoctypeid { get; set; }

    public int? EddDptrid { get; set; }

    public int? NEddDptrid { get; set; }

    public string? EddIsrequired { get; set; }

    public string? NEddIsrequired { get; set; }

    public int? EddSize { get; set; }

    public int? NEddSize { get; set; }

    public string? EddValues { get; set; }

    public string? NEddValues { get; set; }

    public string? EddValidate { get; set; }

    public string? NEddValidate { get; set; }

    public int? EddCompId { get; set; }

    public string? EddIpaddress { get; set; }
}
