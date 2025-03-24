using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMARating_CoreProcess_log")]
public partial class CmaratingCoreProcessLog
{
    [Column("Log_PKID")]
    public long LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("CMACR_ID")]
    public int? CmacrId { get; set; }

    [Column("CMACR_YearID")]
    public int? CmacrYearId { get; set; }

    [Column("nCMACR_YearID")]
    public int? NCmacrYearId { get; set; }

    [Column("CMACR_StartValue")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CmacrStartValue { get; set; }

    [Column("nCMACR_StartValue")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NCmacrStartValue { get; set; }

    [Column("CMACR_EndValue")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CmacrEndValue { get; set; }

    [Column("nCMACR_EndValue")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NCmacrEndValue { get; set; }

    [Column("CMACR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmacrName { get; set; }

    [Column("nCMACR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmacrName { get; set; }

    [Column("CMACR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CmacrColor { get; set; }

    [Column("nCMACR_Color")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NCmacrColor { get; set; }

    [Column("CMACR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmacrDesc { get; set; }

    [Column("nCMACR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmacrDesc { get; set; }

    [Column("CMACR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmacrIpaddress { get; set; }

    [Column("CMACR_CompId")]
    public int? CmacrCompId { get; set; }
}
