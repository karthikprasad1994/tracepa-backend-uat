using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_COLLATE_log")]
public partial class EdtCollateLog
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

    [Column("CLT_COLLATENO")]
    public int? CltCollateno { get; set; }

    [Column("CLT_COLLATEREF")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CltCollateref { get; set; }

    [Column("nCLT_COLLATEREF")]
    [StringLength(200)]
    [Unicode(false)]
    public string? NCltCollateref { get; set; }

    [Column("CLT_ALLOW")]
    public int? CltAllow { get; set; }

    [Column("nCLT_ALLOW")]
    public int? NCltAllow { get; set; }

    [Column("CLT_Comment")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CltComment { get; set; }

    [Column("nCLT_Comment")]
    [StringLength(200)]
    [Unicode(false)]
    public string? NCltComment { get; set; }

    [Column("clt_Group")]
    public int? CltGroup { get; set; }

    [Column("nclt_Group")]
    public int? NcltGroup { get; set; }

    [Column("clt_operationby")]
    public int? CltOperationby { get; set; }

    [Column("nclt_operationby")]
    public int? NcltOperationby { get; set; }

    [Column("clt_operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CltOperation { get; set; }

    [Column("nclt_operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NcltOperation { get; set; }

    [Column("CLT_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CltDelFlag { get; set; }

    [Column("nCLT_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NCltDelFlag { get; set; }

    [Column("CLT_CompId")]
    public int? CltCompId { get; set; }

    [Column("CLT_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CltIpaddress { get; set; }
}
