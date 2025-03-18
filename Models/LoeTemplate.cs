using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class LoeTemplate
{
    public int? LoetId { get; set; }

    public int? LoetLoeid { get; set; }

    public int? LoetCustomerId { get; set; }

    public int? LoetFunctionId { get; set; }

    public string? LoetScopeOfWork { get; set; }

    public string? LoetFrequency { get; set; }

    public string? LoetDeliverable { get; set; }

    public string? LoetProfessionalFees { get; set; }

    public string? LoetStdsInternalAudit { get; set; }

    public string? LoetResponsibilities { get; set; }

    public string? LoetInfrastructure { get; set; }

    public string? LoetNda { get; set; }

    public string? LoetGeneral { get; set; }

    public string? LoetDelflag { get; set; }

    public string? LoetStatus { get; set; }

    public DateTime? LoetCrOn { get; set; }

    public int? LoetCrBy { get; set; }

    public DateTime? LoetUpdatedOn { get; set; }

    public int? LoetUpdatedBy { get; set; }

    public string? LoetIpaddress { get; set; }

    public int? LoetCompId { get; set; }

    public int? LoeAttachId { get; set; }

    public int? LoetApprovedBy { get; set; }

    public DateTime? LoetApprovedOn { get; set; }
}
