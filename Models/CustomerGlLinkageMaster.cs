using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CustomerGlLinkageMaster
{
    public int? ClmId { get; set; }

    public int? ClmHead { get; set; }

    public int? ClmGroupId { get; set; }

    public int? ClmSubGroupId { get; set; }

    public string? ClmGlledger { get; set; }

    public int? ClmCreatedBy { get; set; }

    public DateTime? ClmCreatedOn { get; set; }

    public string? ClmStatus { get; set; }

    public int? ClmYearId { get; set; }

    public int? ClmCompId { get; set; }

    public int? ClmDeletedBy { get; set; }

    public DateTime? ClmDeletedOn { get; set; }

    public int? ClmUpdatedBy { get; set; }

    public DateTime? ClmUpdatedOn { get; set; }

    public string? ClmOperation { get; set; }

    public string? ClmIpaddress { get; set; }

    public int? ClmCustId { get; set; }

    public int? ClmOrgId { get; set; }

    public int? ClmGl { get; set; }

    public int? ClmSubGl { get; set; }

    public int? ClmGlid { get; set; }
}
