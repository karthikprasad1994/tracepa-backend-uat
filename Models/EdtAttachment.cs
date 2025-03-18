using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtAttachment
{
    public int? AtchId { get; set; }

    public int? AtchDocid { get; set; }

    public string? AtchFname { get; set; }

    public string? AtchExt { get; set; }

    public string? AtchDesc { get; set; }

    public byte[]? AtchOle { get; set; }

    public long? AtchSize { get; set; }

    public int? AtchFlag { get; set; }

    public int? AtchAudscheduleId { get; set; }

    public int? AtchAuditId { get; set; }

    public int? AtchSubProcessId { get; set; }

    public int? AtchCreatedby { get; set; }

    public DateTime? AtchCreatedon { get; set; }

    public int? AtchModifiedby { get; set; }

    public int? AtchVersion { get; set; }

    public string? AtchFrom { get; set; }

    public int? AtchBasename { get; set; }

    public string? AtchVstatus { get; set; }

    public string? AtchStatus { get; set; }

    public int? AtchCompId { get; set; }

    public int? AtchReportType { get; set; }

    public int? AtchDrlid { get; set; }
}
