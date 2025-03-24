using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DOCTYPE_PERMISSION_log")]
public partial class EdtDoctypePermissionLog
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

    [Column("EDP_PID")]
    public int? EdpPid { get; set; }

    [Column("EDP_DOCTYPEID")]
    public int? EdpDoctypeid { get; set; }

    [Column("nEDP_DOCTYPEID")]
    public int? NEdpDoctypeid { get; set; }

    [Column("EDP_PTYPE")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EdpPtype { get; set; }

    [Column("nEDP_PTYPE")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NEdpPtype { get; set; }

    [Column("EDP_GRPID")]
    public int? EdpGrpid { get; set; }

    [Column("nEDP_GRPID")]
    public int? NEdpGrpid { get; set; }

    [Column("EDP_USRID")]
    public int? EdpUsrid { get; set; }

    [Column("nEDP_USRID")]
    public int? NEdpUsrid { get; set; }

    [Column("EDP_INDEX")]
    public int? EdpIndex { get; set; }

    [Column("nEDP_INDEX")]
    public int? NEdpIndex { get; set; }

    [Column("EDP_SEARCH")]
    public int? EdpSearch { get; set; }

    [Column("nEDP_SEARCH")]
    public int? NEdpSearch { get; set; }

    [Column("EDP_MFY_TYPE")]
    public int? EdpMfyType { get; set; }

    [Column("nEDP_MFY_TYPE")]
    public int? NEdpMfyType { get; set; }

    [Column("EDP_MFY_DOCUMENT")]
    public int? EdpMfyDocument { get; set; }

    [Column("nEDP_MFY_DOCUMENT")]
    public int? NEdpMfyDocument { get; set; }

    [Column("EDP_DEL_DOCUMENT")]
    public int? EdpDelDocument { get; set; }

    [Column("nEDP_DEL_DOCUMENT")]
    public int? NEdpDelDocument { get; set; }

    [Column("EDP_OTHER")]
    public int? EdpOther { get; set; }

    [Column("nEDP_OTHER")]
    public int? NEdpOther { get; set; }

    [Column("EDP_When")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EdpWhen { get; set; }

    [Column("nEDP_When")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NEdpWhen { get; set; }

    [Column("EDP_CompId")]
    public int? EdpCompId { get; set; }

    [Column("EDP_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EdpIpaddress { get; set; }
}
