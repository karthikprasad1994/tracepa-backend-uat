using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CmaratingLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? CmarId { get; set; }

    public int? CmarYearId { get; set; }

    public int? NCmarYearId { get; set; }

    public string? CmarStartValue { get; set; }

    public string? NCmarStartValue { get; set; }

    public string? CmarEndValue { get; set; }

    public string? NCmarEndValue { get; set; }

    public string? CmarDesc { get; set; }

    public string? NCmarDesc { get; set; }

    public string? CmarName { get; set; }

    public string? NCmarName { get; set; }

    public string? CmarColor { get; set; }

    public string? NCmarColor { get; set; }

    public string? CmarIpaddress { get; set; }

    public int? CmarCompId { get; set; }
}
