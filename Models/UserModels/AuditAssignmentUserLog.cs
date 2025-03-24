using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditAssignment_UserLog")]
public partial class AuditAssignmentUserLog
{
    [Column("AAUL_ID")]
    public int AaulId { get; set; }

    [Column("AAUL_ADT_KEYID")]
    public int? AaulAdtKeyid { get; set; }

    [Column("AAUL_UserID")]
    public int? AaulUserId { get; set; }

    [Column("AAUL_Date", TypeName = "datetime")]
    public DateTime? AaulDate { get; set; }

    [Column("AAUL_AAS_ID")]
    public int? AaulAasId { get; set; }

    [Column("AAUL_AAS_Status")]
    public int? AaulAasStatus { get; set; }

    [Column("AAUL_CompID")]
    public int? AaulCompId { get; set; }
}
