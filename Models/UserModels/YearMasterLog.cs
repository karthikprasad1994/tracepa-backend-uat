using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Year_Master_log")]
public partial class YearMasterLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("YMS_YEARID", TypeName = "decimal(5, 0)")]
    public decimal? YmsYearid { get; set; }

    [Column("YMS_FROMDATE", TypeName = "datetime")]
    public DateTime? YmsFromdate { get; set; }

    [Column("nYMS_FROMDATE", TypeName = "datetime")]
    public DateTime? NYmsFromdate { get; set; }

    [Column("YMS_TODATE", TypeName = "datetime")]
    public DateTime? YmsTodate { get; set; }

    [Column("nYMS_TODATE", TypeName = "datetime")]
    public DateTime? NYmsTodate { get; set; }

    [Column("YMS_ID")]
    [StringLength(20)]
    [Unicode(false)]
    public string? YmsId { get; set; }

    [Column("nYMS_ID")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NYmsId { get; set; }

    [Column("YMS_Default")]
    public int? YmsDefault { get; set; }

    [Column("nYMS_Default")]
    public int? NYmsDefault { get; set; }

    [Column("YMS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? YmsIpaddress { get; set; }

    [Column("YMS_CompID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? YmsCompId { get; set; }
}
