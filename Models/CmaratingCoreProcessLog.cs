using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CmaratingCoreProcessLog
{
    public long LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? CmacrId { get; set; }

    public int? CmacrYearId { get; set; }

    public int? NCmacrYearId { get; set; }

    public string? CmacrStartValue { get; set; }

    public string? NCmacrStartValue { get; set; }

    public string? CmacrEndValue { get; set; }

    public string? NCmacrEndValue { get; set; }

    public string? CmacrName { get; set; }

    public string? NCmacrName { get; set; }

    public string? CmacrColor { get; set; }

    public string? NCmacrColor { get; set; }

    public string? CmacrDesc { get; set; }

    public string? NCmacrDesc { get; set; }

    public string? CmacrIpaddress { get; set; }

    public int? CmacrCompId { get; set; }
}
