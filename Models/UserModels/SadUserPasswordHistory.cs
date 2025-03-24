using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UserPassword_History")]
public partial class SadUserPasswordHistory
{
    [Column("USP_ID", TypeName = "decimal(18, 0)")]
    public decimal UspId { get; set; }

    [Column("USP_USERID")]
    public int? UspUserid { get; set; }

    [Column("USP_PASSWORD")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UspPassword { get; set; }

    [Column("USP_DATE", TypeName = "datetime")]
    public DateTime? UspDate { get; set; }

    [Column("USP_CompID")]
    public int? UspCompId { get; set; }
}
