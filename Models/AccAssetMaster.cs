using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccAssetMaster
{
    public int? AmId { get; set; }

    public string? AmDescription { get; set; }

    public string? AmCode { get; set; }

    public int? AmLevelCode { get; set; }

    public int? AmParentId { get; set; }

    public decimal? AmWdvitact { get; set; }

    public decimal? AmItrate { get; set; }

    public int? AmResidualValue { get; set; }

    public int? AmCreatedBy { get; set; }

    public DateTime? AmCreatedOn { get; set; }

    public int? AmUpdatedBy { get; set; }

    public DateTime? AmUpdatedOn { get; set; }

    public string? AmDelFlag { get; set; }

    public string? AmStatus { get; set; }

    public int? AmYearId { get; set; }

    public int? AmCompId { get; set; }

    public int? AmCustId { get; set; }

    public int? AmApprovedBy { get; set; }

    public DateTime? AmApprovedOn { get; set; }

    public string? AmOpeartion { get; set; }

    public string? AmIpaddress { get; set; }

    public int? AmInitialDep { get; set; }

    public decimal? AmOriginalCost { get; set; }
}
