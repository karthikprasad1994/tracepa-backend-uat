using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ExcelUploadStructure
{
    public int EusId { get; set; }

    public string? EusName { get; set; }

    public string? EusFields { get; set; }

    public string? EusDelflag { get; set; }

    public string? EusValues { get; set; }

    public int? EusValue { get; set; }

    public int? EusCompId { get; set; }
}
