using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_Log")]
public partial class AuditLog
{
    [Column("ADT_KEYID")]
    public long? AdtKeyid { get; set; }

    [Column("ADT_USERID")]
    public int? AdtUserid { get; set; }

    [Column("ADT_LOGIN", TypeName = "datetime")]
    public DateTime? AdtLogin { get; set; }

    [Column("ADT_LOGOUT", TypeName = "datetime")]
    public DateTime? AdtLogout { get; set; }

    [Column("ADT_CompID")]
    public int? AdtCompId { get; set; }
}
