using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Trace_Report_Master1")]
public partial class TraceReportMaster1
{
    [Column("TRM_ID")]
    public int? TrmId { get; set; }

    [Column("TRM_HeaderName")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? TrmHeaderName { get; set; }

    [Column("TRM_Description")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? TrmDescription { get; set; }

    [Column("TRM_Parent")]
    public int? TrmParent { get; set; }

    [Column("TRM_SubParent")]
    public int? TrmSubParent { get; set; }

    [Column("TRM_Head")]
    public int? TrmHead { get; set; }

    [Column("TRM_RptID")]
    public int? TrmRptId { get; set; }

    [Column("TRM_IndID")]
    public int? TrmIndId { get; set; }

    [Column("TRM_CustID")]
    public int? TrmCustId { get; set; }
}
