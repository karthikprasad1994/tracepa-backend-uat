using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_Log_Operations")]
public partial class AuditLogOperation
{
    [Column("ALP_PKID")]
    public long? AlpPkid { get; set; }

    [Column("ALP_UserName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AlpUserName { get; set; }

    [Column("ALP_UserID")]
    public int? AlpUserId { get; set; }

    [Column("ALP_Password")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AlpPassword { get; set; }

    [Column("ALP_Date", TypeName = "datetime")]
    public DateTime? AlpDate { get; set; }

    [Column("ALP_LogType")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AlpLogType { get; set; }

    [Column("ALP_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlpIpaddress { get; set; }

    [Column("ALP_CompID")]
    public int? AlpCompId { get; set; }

    [Column("ALP_Logout", TypeName = "datetime")]
    public DateTime? AlpLogout { get; set; }
}
