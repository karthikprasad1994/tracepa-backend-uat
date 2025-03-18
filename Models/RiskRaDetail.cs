using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskRaDetail
{
    public int? RadPkid { get; set; }

    public int? RadRapkid { get; set; }

    public int? RadSemid { get; set; }

    public int? RadPmid { get; set; }

    public int? RadSpmid { get; set; }

    public int? RadRiskId { get; set; }

    public int? RadRiskTypeId { get; set; }

    public int? RadImpactId { get; set; }

    public int? RadLikelihoodId { get; set; }

    public int? RadRiskRating { get; set; }

    public int? RadControlId { get; set; }

    public int? RadOes { get; set; }

    public int? RadDes { get; set; }

    public int? RadControlRating { get; set; }

    public int? RadChecksId { get; set; }

    public int? RadResidualRiskRating { get; set; }

    public string? RadRemarks { get; set; }

    public string? RadIpaddress { get; set; }

    public int? RadCompId { get; set; }
}
