using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class GraceOverallFunctionRatingDetail
{
    public int? GodPkid { get; set; }

    public int? GodYearId { get; set; }

    public int? GodCustId { get; set; }

    public int? GodFunId { get; set; }

    public int? GodSubFunId { get; set; }

    public float? GodRanetScore { get; set; }

    public int? GodRanetRatingId { get; set; }

    public float? GodIanetScore { get; set; }

    public int? GodIamnetRatingId { get; set; }

    public int? GodCrBy { get; set; }

    public DateTime? GodCrOn { get; set; }

    public string? GodIpaddress { get; set; }

    public int? GodCompId { get; set; }
}
