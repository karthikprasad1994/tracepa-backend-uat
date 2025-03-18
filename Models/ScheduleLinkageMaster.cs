using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ScheduleLinkageMaster
{
    public int? SlmId { get; set; }

    public int? SlmHead { get; set; }

    public int? SlmGroupId { get; set; }

    public int? SlmSubGroupId { get; set; }

    public int? SlmGlid { get; set; }

    public string? SlmGlledger { get; set; }

    public int? SlmCreatedBy { get; set; }

    public DateTime? SlmCreatedOn { get; set; }

    public string? SlmStatus { get; set; }

    public int? SlmYearId { get; set; }

    public int? SlmCompId { get; set; }

    public int? SlmNoteNo { get; set; }

    public int? SlmDeletedBy { get; set; }

    public DateTime? SlmDeletedOn { get; set; }

    public int? SlmUpdatedBy { get; set; }

    public DateTime? SlmUpdatedOn { get; set; }

    public string? SlmOperation { get; set; }

    public string? SlmIpaddress { get; set; }

    public int? SlmOrgId { get; set; }

    public int? SlmCustId { get; set; }
}
