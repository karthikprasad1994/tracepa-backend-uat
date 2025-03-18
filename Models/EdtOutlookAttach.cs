using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtOutlookAttach
{
    public int? EdtId { get; set; }

    public int? EdtBaseName { get; set; }

    public string? EdtOutlookId { get; set; }

    public int? EdtCreatedBy { get; set; }

    public DateTime? EdtCreatedOn { get; set; }

    public DateTime? EdtReceivedDate { get; set; }

    public int? EdtCompId { get; set; }
}
