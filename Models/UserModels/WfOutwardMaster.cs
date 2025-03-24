using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Table("WF_Outward_Masters")]
public partial class WfOutwardMaster
{
    [Key]
    [Column("WOM_ID")]
    public int WomId { get; set; }

    [Column("WOM_OutwardNo")]
    [StringLength(200)]
    [Unicode(false)]
    public string? WomOutwardNo { get; set; }

    [Column("WOM_MonthID")]
    public int? WomMonthId { get; set; }

    [Column("WOM_YearID")]
    public int? WomYearId { get; set; }

    [Column("WOM_OutwardDate", TypeName = "datetime")]
    public DateTime? WomOutwardDate { get; set; }

    [Column("WOM_OutwardTime")]
    [StringLength(100)]
    [Unicode(false)]
    public string? WomOutwardTime { get; set; }

    [Column("WOM_Department")]
    public int? WomDepartment { get; set; }

    [Column("WOM_Customer")]
    public int? WomCustomer { get; set; }

    [Column("WOM_InwardID")]
    public int? WomInwardId { get; set; }

    [Column("WOM_InwardRefNo")]
    [StringLength(200)]
    [Unicode(false)]
    public string? WomInwardRefNo { get; set; }

    [Column("WOM_InwardName")]
    [StringLength(200)]
    [Unicode(false)]
    public string? WomInwardName { get; set; }

    [Column("WOM_Address")]
    [Unicode(false)]
    public string? WomAddress { get; set; }

    [Column("WOM_Page")]
    [StringLength(100)]
    [Unicode(false)]
    public string? WomPage { get; set; }

    [Column("WOM_Sensitivity")]
    public int? WomSensitivity { get; set; }

    [Column("WOM_OutwardRefNo")]
    [StringLength(200)]
    [Unicode(false)]
    public string? WomOutwardRefNo { get; set; }

    [Column("WOM_DispathMode")]
    public int? WomDispathMode { get; set; }

    [Column("WOM_ReplyAwaited")]
    public int? WomReplyAwaited { get; set; }

    [Column("WOM_DocumentType")]
    public int? WomDocumentType { get; set; }

    [Column("WOM_MailingExpenses")]
    [StringLength(200)]
    [Unicode(false)]
    public string? WomMailingExpenses { get; set; }

    [Column("WOM_AttachmentDetails")]
    [Unicode(false)]
    public string? WomAttachmentDetails { get; set; }

    [Column("WOM_Remarks")]
    [Unicode(false)]
    public string? WomRemarks { get; set; }

    [Column("WOM_SendTo")]
    [StringLength(200)]
    [Unicode(false)]
    public string? WomSendTo { get; set; }

    [Column("WOM_AttachID")]
    public int? WomAttachId { get; set; }

    [Column("WOM_CreatedBy")]
    public int? WomCreatedBy { get; set; }

    [Column("WOM_CreatedOn", TypeName = "datetime")]
    public DateTime? WomCreatedOn { get; set; }

    [Column("WOM_UpdatedBy")]
    public int? WomUpdatedBy { get; set; }

    [Column("WOM_UpdatedOn", TypeName = "datetime")]
    public DateTime? WomUpdatedOn { get; set; }

    [Column("WOM_ApprovedBy")]
    public int? WomApprovedBy { get; set; }

    [Column("WOM_ApprovedOn", TypeName = "datetime")]
    public DateTime? WomApprovedOn { get; set; }

    [Column("WOM_DeletedBy")]
    public int? WomDeletedBy { get; set; }

    [Column("WOM_DeletedOn", TypeName = "datetime")]
    public DateTime? WomDeletedOn { get; set; }

    [Column("WOM_RecalledBy")]
    public int? WomRecalledBy { get; set; }

    [Column("WOM_RecalledOn", TypeName = "datetime")]
    public DateTime? WomRecalledOn { get; set; }

    [Column("WOM_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? WomStatus { get; set; }

    [Column("WOM_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? WomDelflag { get; set; }

    [Column("WOM_CompID")]
    public int? WomCompId { get; set; }

    [Column("WOM_IPAddress")]
    [StringLength(200)]
    [Unicode(false)]
    public string? WomIpaddress { get; set; }
}
