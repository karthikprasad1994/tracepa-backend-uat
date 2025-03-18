using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class UploadedSharedDocumentsLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? UsdPkid { get; set; }

    public string? UsdFname { get; set; }

    public string? NUsdFname { get; set; }

    public string? UsdExt { get; set; }

    public string? NUsdExt { get; set; }

    public string? UsdSize { get; set; }

    public string? NUsdSize { get; set; }

    public int? UsdUploadedDocType { get; set; }

    public int? NUsdUploadedDocType { get; set; }

    public string? UsdCreatedBy { get; set; }

    public DateTime? UsdCreatedOn { get; set; }

    public string? UsdUpdatedBy { get; set; }

    public DateTime? UsdUpdatedOn { get; set; }

    public int? UsdDeletedBy { get; set; }

    public DateTime? UsdDeletedOn { get; set; }

    public int? UsdRecalledBy { get; set; }

    public DateTime? UsdRecalledOn { get; set; }

    public string? UsdSharedWith { get; set; }

    public string? NUsdSharedWith { get; set; }

    public DateTime? UsdSharedOn { get; set; }

    public DateTime? NUsdSharedOn { get; set; }

    public int? UsdYearId { get; set; }

    public int? NUsdYearId { get; set; }

    public string? UsdIpaddress { get; set; }

    public int? UsdCompId { get; set; }
}
