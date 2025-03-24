using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_document_type")]
public partial class EdtDocumentType
{
    [Column("DOT_DOCTYPEID")]
    public int? DotDoctypeid { get; set; }

    [Column("DOT_DOCNAME")]
    [StringLength(400)]
    [Unicode(false)]
    public string? DotDocname { get; set; }

    [Column("DOT_NOTE")]
    [StringLength(600)]
    [Unicode(false)]
    public string? DotNote { get; set; }

    [Column("DOT_PGROUP")]
    public int? DotPgroup { get; set; }

    [Column("DOT_CRBY")]
    public int? DotCrby { get; set; }

    [Column("DOT_CRON", TypeName = "datetime")]
    public DateTime? DotCron { get; set; }

    [Column("DOT_STATUS")]
    [StringLength(5)]
    [Unicode(false)]
    public string? DotStatus { get; set; }

    [Column("dot_operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DotOperation { get; set; }

    [Column("dot_operationby", TypeName = "numeric(18, 0)")]
    public decimal? DotOperationby { get; set; }

    [Column("DOT_isGlobal")]
    public int? DotIsGlobal { get; set; }

    [Column("DOT_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? DotDelFlag { get; set; }

    [Column("DOT_UPDATEDBY")]
    public int? DotUpdatedby { get; set; }

    [Column("DOT_UPDATEDON", TypeName = "datetime")]
    public DateTime? DotUpdatedon { get; set; }

    [Column("DOT_RECALLBY")]
    public int? DotRecallby { get; set; }

    [Column("DOT_RECALLON", TypeName = "datetime")]
    public DateTime? DotRecallon { get; set; }

    [Column("DOT_DELETEDBY")]
    public int? DotDeletedby { get; set; }

    [Column("DOT_DELETEDON", TypeName = "datetime")]
    public DateTime? DotDeletedon { get; set; }

    [Column("DOT_APPROVEDBY")]
    public int? DotApprovedby { get; set; }

    [Column("DOT_APPROVEDON", TypeName = "datetime")]
    public DateTime? DotApprovedon { get; set; }

    [Column("DOT_CompId")]
    public int? DotCompId { get; set; }

    [Column("DOT_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? DotIpaddress { get; set; }
}
