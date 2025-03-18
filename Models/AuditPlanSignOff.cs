using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditPlanSignOff
{
    public int? ApsoId { get; set; }

    public int? ApsoYearId { get; set; }

    public string? ApsoAuditCode { get; set; }

    public int? ApsoCustId { get; set; }

    public int? ApsoFunctionId { get; set; }

    public int? ApsoAuditReview { get; set; }

    public int? ApsoAuditPlanStatus { get; set; }

    public string? ApsoRemarks { get; set; }

    public int? ApsoCrBy { get; set; }

    public DateTime? ApsoCrOn { get; set; }

    public int? ApsoUpdatedBy { get; set; }

    public DateTime? ApsoUpdatedOn { get; set; }

    public int? ApsoAppBy { get; set; }

    public DateTime? ApsoAppOn { get; set; }

    public string? ApsoStatus { get; set; }

    public string? ApsoIpaddress { get; set; }

    public int? ApsoAttachId { get; set; }

    public int? ApsoCompId { get; set; }

    public int? ApsoPgedetailId { get; set; }
}
