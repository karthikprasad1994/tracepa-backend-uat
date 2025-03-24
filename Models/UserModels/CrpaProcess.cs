using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_Process")]
public partial class CrpaProcess
{
    [Column("CAP_ID")]
    public int CapId { get; set; }

    [Column("CAP_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string CapCode { get; set; } = null!;

    [Column("CAP_PROCESSNAME")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? CapProcessname { get; set; }

    [Column("CAP_POINTS")]
    public int? CapPoints { get; set; }

    [Column("CAP_SECTIONID")]
    public int? CapSectionid { get; set; }

    [Column("CAP_SUBSECTIONID")]
    public int? CapSubsectionid { get; set; }

    [Column("CAP_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CapDesc { get; set; }

    [Column("CAP_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string CapDelflg { get; set; } = null!;

    [Column("CAP_CRON", TypeName = "datetime")]
    public DateTime? CapCron { get; set; }

    [Column("CAP_CRBY")]
    public int? CapCrby { get; set; }

    [Column("CAP_APPROVEDBY")]
    public int? CapApprovedby { get; set; }

    [Column("CAP_APPROVEDON", TypeName = "datetime")]
    public DateTime? CapApprovedon { get; set; }

    [Column("CAP_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CapStatus { get; set; }

    [Column("CAP_UPDATEDBY")]
    public int? CapUpdatedby { get; set; }

    [Column("CAP_UPDATEDON", TypeName = "datetime")]
    public DateTime? CapUpdatedon { get; set; }

    [Column("CAP_DELETEDBY")]
    public int? CapDeletedby { get; set; }

    [Column("CAP_DELETEDON", TypeName = "datetime")]
    public DateTime? CapDeletedon { get; set; }

    [Column("CAP_RECALLBY")]
    public int? CapRecallby { get; set; }

    [Column("CAP_RECALLON", TypeName = "datetime")]
    public DateTime? CapRecallon { get; set; }

    [Column("CAP_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CapIpaddress { get; set; }

    [Column("CAP_CompId")]
    public int? CapCompId { get; set; }

    [Column("CAP_YEARId")]
    public int? CapYearid { get; set; }
}
