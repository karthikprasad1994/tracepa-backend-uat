using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class LoeResource
{
    public int? LoerId { get; set; }

    public int? LoerLoeid { get; set; }

    public int? LoerCategoryId { get; set; }

    public short? LoerNoResources { get; set; }

    public int? LoerChargesPerDay { get; set; }

    public string? LoerCategoryName { get; set; }

    public int? LoerNoDays { get; set; }

    public int? LoerResTotal { get; set; }

    public string? LoerDelflag { get; set; }

    public string? LoerStatus { get; set; }

    public int? LoerCrBy { get; set; }

    public DateTime? LoerCrOn { get; set; }

    public int? LoerUpdatedBy { get; set; }

    public DateTime? LoerUpdatedOn { get; set; }

    public string? LoerIpaddress { get; set; }

    public int? LoerCompId { get; set; }
}
