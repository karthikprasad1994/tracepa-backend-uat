using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UserEMP_TransferFirm")]
public partial class SadUserEmpTransferFirm
{
    [Column("SUTF_PKID")]
    public int? SutfPkid { get; set; }

    [Column("SUTF_UserEmpID")]
    public int? SutfUserEmpId { get; set; }

    [Column("SUTF_EarlierPrinciple")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SutfEarlierPrinciple { get; set; }

    [Column("SUTF_NewPrinciple")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SutfNewPrinciple { get; set; }

    [Column("SUTF_DateofTransfer", TypeName = "datetime")]
    public DateTime? SutfDateofTransfer { get; set; }

    [Column("SUTF_DurationWithNewPrinciple")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SutfDurationWithNewPrinciple { get; set; }

    [Column("SUTF_CompletionDate", TypeName = "datetime")]
    public DateTime? SutfCompletionDate { get; set; }

    [Column("SUTF_ExtendedTo", TypeName = "datetime")]
    public DateTime? SutfExtendedTo { get; set; }

    [Column("SUTF_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SutfRemarks { get; set; }

    [Column("SUTF_AttachID")]
    public int? SutfAttachId { get; set; }

    [Column("SUTF_CrBy")]
    public int? SutfCrBy { get; set; }

    [Column("SUTF_CrOn", TypeName = "datetime")]
    public DateTime? SutfCrOn { get; set; }

    [Column("SUTF_UpdatedBy")]
    public int? SutfUpdatedBy { get; set; }

    [Column("SUTF_UpdatedOn", TypeName = "datetime")]
    public DateTime? SutfUpdatedOn { get; set; }

    [Column("SUTF_IPAddress")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SutfIpaddress { get; set; }

    [Column("SUTF_CompId")]
    public int? SutfCompId { get; set; }
}
