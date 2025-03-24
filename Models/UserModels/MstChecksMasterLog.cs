using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_Checks_Master_Log")]
public partial class MstChecksMasterLog
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

    [Column("CHK_ID")]
    public int? ChkId { get; set; }

    [Column("CHK_ControlID")]
    public int? ChkControlId { get; set; }

    [Column("nCHK_ControlID")]
    public int? NChkControlId { get; set; }

    [Column("CHK_CheckName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ChkCheckName { get; set; }

    [Column("nCHK_CheckName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? NChkCheckName { get; set; }

    [Column("CHK_CheckDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? ChkCheckDesc { get; set; }

    [Column("nCHK_CheckDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NChkCheckDesc { get; set; }

    [Column("CHK_CatId")]
    public int? ChkCatId { get; set; }

    [Column("nCHK_CatId")]
    public int? NChkCatId { get; set; }

    [Column("CHK_IsKey")]
    public int? ChkIsKey { get; set; }

    [Column("nCHK_IsKey")]
    public int? NChkIsKey { get; set; }

    [Column("CHK_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ChkIpaddress { get; set; }

    [Column("CHK_CompID")]
    public int? ChkCompId { get; set; }
}
