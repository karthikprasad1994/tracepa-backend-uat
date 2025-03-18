using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditLogFormOperation
{
    public int? AlfoPkid { get; set; }

    public int? AlfoUserId { get; set; }

    public DateTime? AlfoDate { get; set; }

    public string? AlfoModule { get; set; }

    public string? AlfoForm { get; set; }

    public string? AlfoEvent { get; set; }

    public int? AlfoMasterId { get; set; }

    public string? AlfoMasterName { get; set; }

    public int? AlfoSubMasterId { get; set; }

    public string? AlfoSubMasterName { get; set; }

    public string? AlfoIpaddress { get; set; }

    public int? AlfoCompId { get; set; }
}
