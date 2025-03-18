using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MmcsplDbAccess
{
    public int MdaId { get; set; }

    public string? MdaDatabaseName { get; set; }

    public string? MdaAccessCode { get; set; }

    public string? MdaCompanyName { get; set; }

    public DateTime? MdaCreatedDate { get; set; }

    public string? MdaIpaddress { get; set; }

    public int? MdaApplication { get; set; }
}
