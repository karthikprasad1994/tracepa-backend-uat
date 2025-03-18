using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadMembershipDetail
{
    public int SmdId { get; set; }

    public int? SmdMasterMembershipId { get; set; }

    public int? SmdEmployeeId { get; set; }

    public string? SmdMembershipNo { get; set; }

    public string? SmdDateOfReg { get; set; }

    public string? SmdRenewalDate { get; set; }

    public string? SmdStatus { get; set; }

    public int? SmdCrBy { get; set; }

    public DateTime? SmdCreatedOn { get; set; }

    public int? SmdCompId { get; set; }

    public string? SmdRegistrationNo { get; set; }
}
