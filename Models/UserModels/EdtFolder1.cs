using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_FOLDER1")]
public partial class EdtFolder1
{
    [Column("FOL_FOLID", TypeName = "numeric(10, 0)")]
    public decimal? FolFolid { get; set; }

    [Column("FOL_CABINET", TypeName = "numeric(10, 0)")]
    public decimal? FolCabinet { get; set; }

    [Column("FOL_NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string? FolName { get; set; }

    [Column("FOL_NOTES")]
    [StringLength(200)]
    [Unicode(false)]
    public string? FolNotes { get; set; }

    [Column("FOL_CRBY")]
    public int? FolCrby { get; set; }

    [Column("FOL_CRON", TypeName = "datetime")]
    public DateTime? FolCron { get; set; }

    [Column("FOL_STATUS")]
    [StringLength(1)]
    [Unicode(false)]
    public string? FolStatus { get; set; }

    [Column("FOL_EXPIRYDATE", TypeName = "datetime")]
    public DateTime? FolExpirydate { get; set; }

    [Column("FOL_PAGECOUNT")]
    public int? FolPagecount { get; set; }

    [Column("fol_operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? FolOperation { get; set; }

    [Column("fol_operationBy", TypeName = "numeric(3, 0)")]
    public decimal? FolOperationBy { get; set; }
}
