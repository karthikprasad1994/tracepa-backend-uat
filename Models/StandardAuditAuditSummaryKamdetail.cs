using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditAuditSummaryKamdetail
{
    public int? SakamdPkid { get; set; }

    public int? SakamSaifcdPkid { get; set; }

    public string? SakamDescriptionOrReasonForSelectionAsKam { get; set; }

    public string? SakamAuditProcedureUndertakenToAddressTheKam { get; set; }

    public int? SakamAttachId { get; set; }

    public int? SakamCrBy { get; set; }

    public DateTime? SakamCrOn { get; set; }
}
