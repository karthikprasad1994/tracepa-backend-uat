using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("WF_Inward_Masters")]
public partial class WfInwardMaster
{
    [Column("WIM_PKID")]
    public int? WimPkid { get; set; }

    [Column("WIM_InwardNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? WimInwardNo { get; set; }

    [Column("WIM_MonthID")]
    public int? WimMonthId { get; set; }

    [Column("WIM_YearID")]
    public int? WimYearId { get; set; }

    [Column("WIM_InwardDate", TypeName = "datetime")]
    public DateTime? WimInwardDate { get; set; }

    [Column("WIM_InwardTime")]
    [StringLength(100)]
    [Unicode(false)]
    public string? WimInwardTime { get; set; }

    [Column("WIM_Title")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? WimTitle { get; set; }

    [Column("WIM_DocReferenceno")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? WimDocReferenceno { get; set; }

    [Column("WIM_DocRecievedDate", TypeName = "datetime")]
    public DateTime? WimDocRecievedDate { get; set; }

    [Column("WIM_DateOnDocument", TypeName = "datetime")]
    public DateTime? WimDateOnDocument { get; set; }

    [Column("WIM_ReceiptMode")]
    public int? WimReceiptMode { get; set; }

    [Column("WIM_Deptartment")]
    public int? WimDeptartment { get; set; }

    [Column("WIM_Customer")]
    public int? WimCustomer { get; set; }

    [Column("WIM_ContactPerson")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? WimContactPerson { get; set; }

    [Column("WIM_ContactEmailID")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? WimContactEmailId { get; set; }

    [Column("WIM_ContactPhNO")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? WimContactPhNo { get; set; }

    [Column("WIM_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? WimRemarks { get; set; }

    [Column("WIM_AttachID")]
    public int? WimAttachId { get; set; }

    [Column("WIM_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? WimStatus { get; set; }

    [Column("WIM_Delflag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? WimDelflag { get; set; }

    [Column("WIM_InwardOrWorkFlow")]
    public int? WimInwardOrWorkFlow { get; set; }

    [Column("WIM_Address")]
    [Unicode(false)]
    public string? WimAddress { get; set; }

    [Column("WIM_Stage")]
    public int? WimStage { get; set; }

    [Column("WIM_CreatedOn", TypeName = "datetime")]
    public DateTime? WimCreatedOn { get; set; }

    [Column("WIM_CreatedBy")]
    public int? WimCreatedBy { get; set; }

    [Column("WIM_UpdatedOn", TypeName = "datetime")]
    public DateTime? WimUpdatedOn { get; set; }

    [Column("WIM_UpdatedBy")]
    public int? WimUpdatedBy { get; set; }

    [Column("WIM_ApprovedBy")]
    public int? WimApprovedBy { get; set; }

    [Column("WIM_ApprovedOn", TypeName = "datetime")]
    public DateTime? WimApprovedOn { get; set; }

    [Column("WIM_DeletedOn", TypeName = "datetime")]
    public DateTime? WimDeletedOn { get; set; }

    [Column("WIM_DeletedBy")]
    public int? WimDeletedBy { get; set; }

    [Column("WIM_RecalledOn", TypeName = "datetime")]
    public DateTime? WimRecalledOn { get; set; }

    [Column("WIM_RecalledBy")]
    public int? WimRecalledBy { get; set; }

    [Column("WIM_IPAdress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? WimIpadress { get; set; }

    [Column("WIM_CompID")]
    public int? WimCompId { get; set; }

    [Column("WIM_WorkFLowCreatedBy")]
    public int? WimWorkFlowCreatedBy { get; set; }

    [Column("WIM_WorkFLowCreatedOn", TypeName = "datetime")]
    public DateTime? WimWorkFlowCreatedOn { get; set; }

    [Column("WIM_WorkFLowID")]
    public int? WimWorkFlowId { get; set; }

    [Column("WIM_WorkFLowArchiveID")]
    public int? WimWorkFlowArchiveId { get; set; }

    [Column("WIM_WorkFlowComments")]
    [Unicode(false)]
    public string? WimWorkFlowComments { get; set; }

    [Column("WIM_Progress_Status")]
    public int? WimProgressStatus { get; set; }
}
