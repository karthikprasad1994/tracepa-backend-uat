using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMARating_SupportProcess_log")]
public partial class CmaratingSupportProcessLog
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

    [Column("CMASR_ID")]
    public int? CmasrId { get; set; }

    [Column("CMASR_YearID")]
    public int? CmasrYearId { get; set; }

    [Column("nCMASR_YearID")]
    public int? NCmasrYearId { get; set; }

    [Column("CMASR_StartValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmasrStartValue { get; set; }

    [Column("nCMASR_StartValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NCmasrStartValue { get; set; }

    [Column("CMASR_EndValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmasrEndValue { get; set; }

    [Column("nCMASR_EndValue")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NCmasrEndValue { get; set; }

    [Column("CMASR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmasrDesc { get; set; }

    [Column("nCMASR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmasrDesc { get; set; }

    [Column("CMASR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmasrName { get; set; }

    [Column("nCMASR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmasrName { get; set; }

    [Column("CMASR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmasrColor { get; set; }

    [Column("nCMASR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NCmasrColor { get; set; }

    [Column("CMASR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmasrIpaddress { get; set; }

    [Column("CMASR_CompId")]
    public int? CmasrCompId { get; set; }
}
