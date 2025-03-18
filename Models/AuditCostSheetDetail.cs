using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditCostSheetDetail
{
    public int? CsdId { get; set; }

    public int? CsdYearId { get; set; }

    public int? CsdAuditCodeId { get; set; }

    public int? CsdFunId { get; set; }

    public int? CsdCustId { get; set; }

    public int? CsdDescId { get; set; }

    public int? CsdUserId { get; set; }

    public string? CsdDate { get; set; }

    public string? CsdComments { get; set; }

    public int? CsdKmsTravelled { get; set; }

    public int? CsdCosts { get; set; }

    public string? CsdStatus { get; set; }

    public int? CsdCrBy { get; set; }

    public DateTime? CsdCrOn { get; set; }

    public int? CsdUpdateBy { get; set; }

    public DateTime? CsdUpdatedOn { get; set; }

    public int? CsdApprovedBy { get; set; }

    public DateTime? CsdApprovedOn { get; set; }

    public string? CsdIpaddress { get; set; }

    public int? CsdCompId { get; set; }
}
