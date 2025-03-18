using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtScanDocDetail
{
    public int? ScanId { get; set; }

    public int? ScanBatchid { get; set; }

    public string? ScanLocation { get; set; }

    public string? ScanBuilding { get; set; }

    public string? ScanFloor { get; set; }

    public string? ScanRoomno { get; set; }

    public string? ScanRow { get; set; }

    public string? ScanColumn { get; set; }

    public string? ScanRackno { get; set; }

    public string? ScanDescription { get; set; }
}
