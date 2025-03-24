using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DOCTYPE_PERMISSION")]
public partial class EdtDoctypePermission
{
    [Column("EDP_PID")]
    public int? EdpPid { get; set; }

    [Column("EDP_DOCTYPEID")]
    public int? EdpDoctypeid { get; set; }

    [Column("EDP_PTYPE")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EdpPtype { get; set; }

    [Column("EDP_GRPID")]
    public short EdpGrpid { get; set; }

    [Column("EDP_USRID")]
    public short EdpUsrid { get; set; }

    [Column("EDP_INDEX")]
    public byte? EdpIndex { get; set; }

    [Column("EDP_SEARCH")]
    public byte? EdpSearch { get; set; }

    [Column("EDP_MFY_TYPE")]
    public byte? EdpMfyType { get; set; }

    [Column("EDP_MFY_DOCUMENT")]
    public byte? EdpMfyDocument { get; set; }

    [Column("EDP_DEL_DOCUMENT")]
    public byte? EdpDelDocument { get; set; }

    [Column("EDP_OTHER")]
    public byte? EdpOther { get; set; }

    [Column("EDP_When")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EdpWhen { get; set; }

    [Column("EDP_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? EdpStatus { get; set; }

    [Column("EDP_CRBY")]
    public int? EdpCrby { get; set; }

    [Column("EDP_CRON", TypeName = "datetime")]
    public DateTime? EdpCron { get; set; }

    [Column("EDP_UPDATEDBY")]
    public int? EdpUpdatedby { get; set; }

    [Column("EDP_UPDATEDON", TypeName = "datetime")]
    public DateTime? EdpUpdatedon { get; set; }

    [Column("EDP_RECALLBY")]
    public int? EdpRecallby { get; set; }

    [Column("EDP_RECALLON", TypeName = "datetime")]
    public DateTime? EdpRecallon { get; set; }

    [Column("EDP_DELETEDBY")]
    public int? EdpDeletedby { get; set; }

    [Column("EDP_DELETEDON", TypeName = "datetime")]
    public DateTime? EdpDeletedon { get; set; }

    [Column("EDP_APPROVEDBY")]
    public int? EdpApprovedby { get; set; }

    [Column("EDP_APPROVEDON", TypeName = "datetime")]
    public DateTime? EdpApprovedon { get; set; }

    [Column("EDP_CompId")]
    public int? EdpCompId { get; set; }

    [Column("EDP_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EdpIpaddress { get; set; }
}
