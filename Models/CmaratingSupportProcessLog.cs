using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CmaratingSupportProcessLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? CmasrId { get; set; }

    public int? CmasrYearId { get; set; }

    public int? NCmasrYearId { get; set; }

    public string? CmasrStartValue { get; set; }

    public string? NCmasrStartValue { get; set; }

    public string? CmasrEndValue { get; set; }

    public string? NCmasrEndValue { get; set; }

    public string? CmasrDesc { get; set; }

    public string? NCmasrDesc { get; set; }

    public string? CmasrName { get; set; }

    public string? NCmasrName { get; set; }

    public string? CmasrColor { get; set; }

    public string? NCmasrColor { get; set; }

    public string? CmasrIpaddress { get; set; }

    public int? CmasrCompId { get; set; }
}
