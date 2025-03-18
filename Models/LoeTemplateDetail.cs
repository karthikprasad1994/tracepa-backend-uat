using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class LoeTemplateDetail
{
    public int? LtdId { get; set; }

    public int? LtdLoeId { get; set; }

    public int? LtdReportTypeId { get; set; }

    public int? LtdHeadingId { get; set; }

    public string? LtdHeading { get; set; }

    public string? LtdDecription { get; set; }

    public string? LtdFormName { get; set; }

    public int? LtdCrBy { get; set; }

    public DateTime? LtdCrOn { get; set; }

    public string? LtdIpaddress { get; set; }

    public int? LtdCompId { get; set; }

    public int? LtdUpdatedBy { get; set; }

    public DateTime? LtdUpdatedOn { get; set; }
}
