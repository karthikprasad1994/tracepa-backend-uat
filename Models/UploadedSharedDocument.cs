using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class UploadedSharedDocument
{
    public int? UsdPkid { get; set; }

    public string? UsdFname { get; set; }

    public string? UsdExt { get; set; }

    public string? UsdSize { get; set; }

    public int? UsdUploadedDocType { get; set; }

    public string? UsdCreatedBy { get; set; }

    public DateTime? UsdCreatedOn { get; set; }

    public string? UsdUpdatedBy { get; set; }

    public DateTime? UsdUpdatedOn { get; set; }

    public int? UsdDeletedBy { get; set; }

    public DateTime? UsdDeletedOn { get; set; }

    public int? UsdRecalledBy { get; set; }

    public DateTime? UsdRecalledOn { get; set; }

    public string? UsdSharedWith { get; set; }

    public DateTime? UsdSharedOn { get; set; }

    public int? UsdYearId { get; set; }

    public string? UsdStatus { get; set; }

    public string? UsdIpaddress { get; set; }

    public int? UsdCompId { get; set; }
}
