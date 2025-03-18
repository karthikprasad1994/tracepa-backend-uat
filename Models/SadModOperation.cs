using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadModOperation
{
    public int? OpPkid { get; set; }

    public int? OpModuleId { get; set; }

    public string? OpOperationCode { get; set; }

    public string? OpOperationName { get; set; }

    public string? OpStatus { get; set; }

    public int? OpCompId { get; set; }
}
