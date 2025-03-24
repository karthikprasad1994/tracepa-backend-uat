using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UserEMP_AssetsLoan")]
public partial class SadUserEmpAssetsLoan
{
    [Column("SUAL_PKID")]
    public int? SualPkid { get; set; }

    [Column("SUAL_UserEmpID")]
    public int? SualUserEmpId { get; set; }

    [Column("SUAL_AssetType")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SualAssetType { get; set; }

    [Column("SUAL_SerialNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SualSerialNo { get; set; }

    [Column("SUAL_ApproValue")]
    public int? SualApproValue { get; set; }

    [Column("SUAL_IssueDate", TypeName = "datetime")]
    public DateTime? SualIssueDate { get; set; }

    [Column("SUAL_DueDate", TypeName = "datetime")]
    public DateTime? SualDueDate { get; set; }

    [Column("SUAL_ConditionIssue")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SualConditionIssue { get; set; }

    [Column("SUAL_ConditionReceipt")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SualConditionReceipt { get; set; }

    [Column("SUAL_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SualRemarks { get; set; }

    [Column("SUAL_AttachID")]
    public int? SualAttachId { get; set; }

    [Column("SUAL_CrBy")]
    public int? SualCrBy { get; set; }

    [Column("SUAL_CrOn", TypeName = "datetime")]
    public DateTime? SualCrOn { get; set; }

    [Column("SUAL_UpdatedBy")]
    public int? SualUpdatedBy { get; set; }

    [Column("SUAL_UpdatedOn", TypeName = "datetime")]
    public DateTime? SualUpdatedOn { get; set; }

    [Column("SUAL_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SualIpaddress { get; set; }

    [Column("SUAL_CompId")]
    public int? SualCompId { get; set; }

    [Column("SUAL_RecievedDate", TypeName = "datetime")]
    public DateTime? SualRecievedDate { get; set; }
}
