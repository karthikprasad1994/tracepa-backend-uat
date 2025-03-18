using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtAnnotationDetail
{
    public int? EadPkid { get; set; }

    public int? EadDocumentId { get; set; }

    public int? EadFileId { get; set; }

    public string? EadOriginalName { get; set; }

    public string? EadExt { get; set; }

    public byte[]? EadOle { get; set; }

    public long? EadSize { get; set; }

    public int? EadCreatedBy { get; set; }

    public DateTime? EadCreatedOn { get; set; }

    public string? EadIpaddress { get; set; }

    public int? EadCompId { get; set; }
}
