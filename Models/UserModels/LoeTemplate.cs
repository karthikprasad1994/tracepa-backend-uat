using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("LOE_Template")]
public partial class LoeTemplate
{
    [Column("LOET_Id")]
    public int? LoetId { get; set; }

    [Column("LOET_LOEID")]
    public int? LoetLoeid { get; set; }

    [Column("LOET_CustomerId")]
    public int? LoetCustomerId { get; set; }

    [Column("LOET_FunctionId")]
    public int? LoetFunctionId { get; set; }

    [Column("LOET_ScopeOfWork")]
    [StringLength(500)]
    [Unicode(false)]
    public string? LoetScopeOfWork { get; set; }

    [Column("LOET_Frequency")]
    [StringLength(50)]
    [Unicode(false)]
    public string? LoetFrequency { get; set; }

    [Column("LOET_Deliverable")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? LoetDeliverable { get; set; }

    [Column("LOET_ProfessionalFees")]
    [StringLength(500)]
    [Unicode(false)]
    public string? LoetProfessionalFees { get; set; }

    [Column("LOET_StdsInternalAudit")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? LoetStdsInternalAudit { get; set; }

    [Column("LOET_Responsibilities")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? LoetResponsibilities { get; set; }

    [Column("LOET_Infrastructure")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? LoetInfrastructure { get; set; }

    [Column("LOET_NDA")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? LoetNda { get; set; }

    [Column("LOET_General")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? LoetGeneral { get; set; }

    [Column("LOET_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LoetDelflag { get; set; }

    [Column("LOET_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? LoetStatus { get; set; }

    [Column("LOE_AttachID")]
    public int? LoeAttachId { get; set; }

    [Column("LOET_CrOn", TypeName = "datetime")]
    public DateTime? LoetCrOn { get; set; }

    [Column("LOET_CrBy")]
    public int? LoetCrBy { get; set; }

    [Column("LOET_UpdatedOn", TypeName = "datetime")]
    public DateTime? LoetUpdatedOn { get; set; }

    [Column("LOET_UpdatedBy")]
    public int? LoetUpdatedBy { get; set; }

    [Column("LOET_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? LoetIpaddress { get; set; }

    [Column("LOET_CompID")]
    public int? LoetCompId { get; set; }
}
