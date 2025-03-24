using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMARating_Log")]
public partial class CmaratingLog
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

    [Column("CMAR_ID")]
    public int? CmarId { get; set; }

    [Column("CMAR_YearID")]
    public int? CmarYearId { get; set; }

    [Column("nCMAR_YearID")]
    public int? NCmarYearId { get; set; }

    [Column("CMAR_StartValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmarStartValue { get; set; }

    [Column("nCMAR_StartValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NCmarStartValue { get; set; }

    [Column("CMAR_EndValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmarEndValue { get; set; }

    [Column("nCMAR_EndValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NCmarEndValue { get; set; }

    [Column("CMAR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmarDesc { get; set; }

    [Column("nCMAR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmarDesc { get; set; }

    [Column("CMAR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmarName { get; set; }

    [Column("nCMAR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmarName { get; set; }

    [Column("CMAR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmarColor { get; set; }

    [Column("nCMAR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NCmarColor { get; set; }

    [Column("CMAR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmarIpaddress { get; set; }

    [Column("CMAR_CompId")]
    public int? CmarCompId { get; set; }
}
