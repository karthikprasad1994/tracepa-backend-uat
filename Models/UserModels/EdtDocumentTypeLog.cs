using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DOCUMENT_TYPE_log")]
public partial class EdtDocumentTypeLog
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

    [Column("DOT_DOCTYPEID")]
    public int? DotDoctypeid { get; set; }

    [Column("DOT_DOCNAME")]
    [StringLength(400)]
    [Unicode(false)]
    public string? DotDocname { get; set; }

    [Column("nDOT_DOCNAME")]
    [StringLength(400)]
    [Unicode(false)]
    public string? NDotDocname { get; set; }

    [Column("DOT_NOTE")]
    [StringLength(600)]
    [Unicode(false)]
    public string? DotNote { get; set; }

    [Column("nDOT_NOTE")]
    [StringLength(600)]
    [Unicode(false)]
    public string? NDotNote { get; set; }

    [Column("DOT_PGROUP")]
    public int? DotPgroup { get; set; }

    [Column("nDOT_PGROUP")]
    public int? NDotPgroup { get; set; }

    [Column("dot_operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DotOperation { get; set; }

    [Column("ndot_operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NdotOperation { get; set; }

    [Column("dot_operationby")]
    public int? DotOperationby { get; set; }

    [Column("ndot_operationby")]
    public int? NdotOperationby { get; set; }

    [Column("DOT_isGlobal")]
    public int? DotIsGlobal { get; set; }

    [Column("nDOT_isGlobal")]
    public int? NDotIsGlobal { get; set; }

    [Column("DOT_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? DotDelFlag { get; set; }

    [Column("nDOT_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NDotDelFlag { get; set; }

    [Column("DOT_CompId")]
    public int? DotCompId { get; set; }

    [Column("DOT_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? DotIpaddress { get; set; }
}
