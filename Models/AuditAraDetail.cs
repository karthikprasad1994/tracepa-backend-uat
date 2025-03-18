using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAraDetail
{
    public int? AradPkid { get; set; }

    public int? AradArapkid { get; set; }

    public int? AradSemid { get; set; }

    public int? AradPmid { get; set; }

    public int? AradSpmid { get; set; }

    public string? AradIssueHeading { get; set; }

    public int? AradRiskId { get; set; }

    public int? AradRiskTypeId { get; set; }

    public int? AradImpactId { get; set; }

    public int? AradLikelihoodId { get; set; }

    public int? AradRiskRating { get; set; }

    public int? AradControlId { get; set; }

    public int? AradOes { get; set; }

    public int? AradDes { get; set; }

    public int? AradControlRating { get; set; }

    public int? AradChecksId { get; set; }

    public int? AradResidualRiskRating { get; set; }

    public string? AradRemarks { get; set; }

    public string? AradIpaddress { get; set; }

    public int? AradCompId { get; set; }
}
