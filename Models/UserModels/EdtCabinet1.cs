using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_CABINET1")]
public partial class EdtCabinet1
{
    [Column("CBN_NODE", TypeName = "numeric(18, 0)")]
    public decimal? CbnNode { get; set; }

    [Column("CBN_NAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CbnName { get; set; }

    [Column("CBN_PARENT", TypeName = "numeric(18, 0)")]
    public decimal? CbnParent { get; set; }

    [Column("CBN_Note")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CbnNote { get; set; }

    [Column("CBN_USERGROUP", TypeName = "numeric(18, 0)")]
    public decimal? CbnUsergroup { get; set; }

    [Column("CBN_USERID", TypeName = "numeric(18, 0)")]
    public decimal? CbnUserid { get; set; }

    [Column("CBN_ParGrp", TypeName = "numeric(18, 0)")]
    public decimal? CbnParGrp { get; set; }

    [Column("CBN_MAILCABINET", TypeName = "numeric(18, 0)")]
    public decimal? CbnMailcabinet { get; set; }

    [Column("CBN_CRON", TypeName = "datetime")]
    public DateTime? CbnCron { get; set; }

    [Column("CBN_PERMISSION", TypeName = "numeric(18, 0)")]
    public decimal? CbnPermission { get; set; }

    [Column("cbn_DelStatus")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CbnDelStatus { get; set; }

    [Column("CBN_SCCount", TypeName = "numeric(18, 0)")]
    public decimal? CbnSccount { get; set; }

    [Column("CBN_FolCount", TypeName = "numeric(18, 0)")]
    public decimal? CbnFolCount { get; set; }

    [Column("cbn_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CbnOperation { get; set; }

    [Column("cbn_OperationBy")]
    public int? CbnOperationBy { get; set; }
}
