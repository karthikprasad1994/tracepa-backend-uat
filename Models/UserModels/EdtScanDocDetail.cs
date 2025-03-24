using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_ScanDoc_Details")]
public partial class EdtScanDocDetail
{
    [Column("SCAN_ID")]
    public int? ScanId { get; set; }

    [Column("SCAN_BATCHID")]
    public int? ScanBatchid { get; set; }

    [Column("SCAN_LOCATION")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanLocation { get; set; }

    [Column("SCAN_BUILDING")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanBuilding { get; set; }

    [Column("SCAN_FLOOR")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanFloor { get; set; }

    [Column("SCAN_ROOMNO")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanRoomno { get; set; }

    [Column("SCAN_ROW")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanRow { get; set; }

    [Column("SCAN_COLUMN")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanColumn { get; set; }

    [Column("SCAN_RACKNO")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanRackno { get; set; }

    [Column("SCAN_DESCRIPTION")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ScanDescription { get; set; }
}
