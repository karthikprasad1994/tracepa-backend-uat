using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_CONTROL_Library_Log")]
public partial class MstControlLibraryLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(30)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("MCL_PKID")]
    public int? MclPkid { get; set; }

    [Column("MCL_ControlName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MclControlName { get; set; }

    [Column("nMCL_ControlName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? NMclControlName { get; set; }

    [Column("MCL_ControlDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? MclControlDesc { get; set; }

    [Column("nMCL_ControlDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NMclControlDesc { get; set; }

    [Column("MCL_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? MclCode { get; set; }

    [Column("nMCL_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NMclCode { get; set; }

    [Column("MCL_IsKey")]
    public int? MclIsKey { get; set; }

    [Column("nMCL_IsKey")]
    public int? NMclIsKey { get; set; }

    [Column("MCL_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MclIpaddress { get; set; }

    [Column("MCL_CompID")]
    public int? MclCompId { get; set; }
}
