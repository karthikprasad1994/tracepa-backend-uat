using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUsersInOtherDept
{
    public int? SuoPkid { get; set; }

    public int? SuoUserId { get; set; }

    public int? SuoDeptId { get; set; }

    public int? SuoIsDeptHead { get; set; }

    public int? SuoCreatedBy { get; set; }

    public DateTime? SuoCreatedOn { get; set; }

    public string? SuoIpaddress { get; set; }

    public int? SuoCompId { get; set; }
}
