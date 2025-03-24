using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Holiday_Master_Log")]
public partial class HolidayMasterLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("Hol_YearId")]
    public int? HolYearId { get; set; }

    [Column("Hol_Date", TypeName = "datetime")]
    public DateTime? HolDate { get; set; }

    [Column("nHol_Date", TypeName = "datetime")]
    public DateTime? NHolDate { get; set; }

    [Column("Hol_Remarks")]
    [StringLength(500)]
    [Unicode(false)]
    public string? HolRemarks { get; set; }

    [Column("nHol_Remarks")]
    [StringLength(500)]
    [Unicode(false)]
    public string? NHolRemarks { get; set; }

    [Column("HOL_CompID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? HolCompId { get; set; }

    [Column("Hol_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? HolIpaddress { get; set; }
}
