using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Table("Audit_Log_Details")]
public partial class AuditLogDetail
{
    [Key]
    [Column("ALD_ID")]
    public int AldId { get; set; }

    [Column("ALD_MASID")]
    public int? AldMasid { get; set; }

    [Column("ALD_UserId")]
    public int? AldUserId { get; set; }

    [Column("ALD_ModuleName")]
    [StringLength(50)]
    [Unicode(false)]
    public string AldModuleName { get; set; } = null!;

    [Column("ALD_ModuleTime")]
    public int? AldModuleTime { get; set; }

    [Column("ALD_TotalIdleTime")]
    public int? AldTotalIdleTime { get; set; }

    [Column("ALD_ScreenTotalTime")]
    public int? AldScreenTotalTime { get; set; }

    [Column("ALD_CompId")]
    public int? AldCompId { get; set; }
}
