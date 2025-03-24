using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_SubProcess")]
public partial class CrpaSubProcess
{
    [Column("CASP_ID")]
    public int CaspId { get; set; }

    [Column("CASP_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CaspCode { get; set; }

    [Column("CASP_SUBPROCESSNAME")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? CaspSubprocessname { get; set; }

    [Column("CASP_POINTS")]
    public int? CaspPoints { get; set; }

    [Column("CASP_SECTIONID")]
    public int? CaspSectionid { get; set; }

    [Column("CASP_SUBSECTIONID")]
    public int? CaspSubsectionid { get; set; }

    [Column("CASP_PROCESSID")]
    public int? CaspProcessid { get; set; }

    [Column("CASP_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CaspDesc { get; set; }

    [Column("CASP_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CaspDelflg { get; set; }

    [Column("CASP_CRON", TypeName = "datetime")]
    public DateTime? CaspCron { get; set; }

    [Column("CASP_CRBY")]
    public int? CaspCrby { get; set; }

    [Column("CASP_APPROVEDBY")]
    public int? CaspApprovedby { get; set; }

    [Column("CASP_APPROVEDON", TypeName = "datetime")]
    public DateTime? CaspApprovedon { get; set; }

    [Column("CASP_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CaspStatus { get; set; }

    [Column("CASP_UPDATEDBY")]
    public int? CaspUpdatedby { get; set; }

    [Column("CASP_UPDATEDON", TypeName = "datetime")]
    public DateTime? CaspUpdatedon { get; set; }

    [Column("CASP_DELETEDBY")]
    public int? CaspDeletedby { get; set; }

    [Column("CASP_DELETEDON", TypeName = "datetime")]
    public DateTime? CaspDeletedon { get; set; }

    [Column("CASP_RECALLBY")]
    public int? CaspRecallby { get; set; }

    [Column("CASP_RECALLON", TypeName = "datetime")]
    public DateTime? CaspRecallon { get; set; }

    [Column("CASP_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CaspIpaddress { get; set; }

    [Column("CASP_CompId")]
    public int? CaspCompId { get; set; }

    [Column("CASP_YEARId")]
    public int? CaspYearid { get; set; }
}
