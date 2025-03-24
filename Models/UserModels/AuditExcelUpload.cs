using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_Excel_Upload")]
public partial class AuditExcelUpload
{
    [Column("AEU_ID")]
    public int AeuId { get; set; }

    [Column("AEU_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AeuDescription { get; set; }

    [Column("AEU_CustId")]
    public int? AeuCustId { get; set; }

    [Column("AEU_ODAmount", TypeName = "money")]
    public decimal? AeuOdamount { get; set; }

    [Column("AEU_OCAmount", TypeName = "money")]
    public decimal? AeuOcamount { get; set; }

    [Column("AEU_TRDAmount", TypeName = "money")]
    public decimal? AeuTrdamount { get; set; }

    [Column("AEU_TRCAmount", TypeName = "money")]
    public decimal? AeuTrcamount { get; set; }

    [Column("AEU_CDAmount", TypeName = "money")]
    public decimal? AeuCdamount { get; set; }

    [Column("AEU_CCAmount", TypeName = "money")]
    public decimal? AeuCcamount { get; set; }

    [Column("AEU_Observation")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AeuObservation { get; set; }

    [Column("AEU_ReviewerObservation")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AeuReviewerObservation { get; set; }

    [Column("AEU_ClientComments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AeuClientComments { get; set; }

    [Column("AEU_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AeuDelflg { get; set; }

    [Column("AEU_CRON", TypeName = "datetime")]
    public DateTime? AeuCron { get; set; }

    [Column("AEU_CRBY")]
    public int? AeuCrby { get; set; }

    [Column("AEU_APPROVEDBY")]
    public int? AeuApprovedby { get; set; }

    [Column("AEU_APPROVEDON", TypeName = "datetime")]
    public DateTime? AeuApprovedon { get; set; }

    [Column("AEU_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AeuStatus { get; set; }

    [Column("AEU_UPDATEDBY")]
    public int? AeuUpdatedby { get; set; }

    [Column("AEU_UPDATEDON", TypeName = "datetime")]
    public DateTime? AeuUpdatedon { get; set; }

    [Column("AEU_DELETEDBY")]
    public int? AeuDeletedby { get; set; }

    [Column("AEU_DELETEDON", TypeName = "datetime")]
    public DateTime? AeuDeletedon { get; set; }

    [Column("AEU_RECALLBY")]
    public int? AeuRecallby { get; set; }

    [Column("AEU_RECALLON", TypeName = "datetime")]
    public DateTime? AeuRecallon { get; set; }

    [Column("AEU_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AeuIpaddress { get; set; }

    [Column("AEU_CompId")]
    public int? AeuCompId { get; set; }

    [Column("AEU_YEARId")]
    public int? AeuYearid { get; set; }

    [Column("AEU_AuditId")]
    public int? AeuAuditId { get; set; }

    [Column("AEU_AuditTypeId")]
    public int? AeuAuditTypeId { get; set; }

    [Column("AEU_AttachmentId")]
    public int? AeuAttachmentId { get; set; }
}
