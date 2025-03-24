using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Year_Master")]
public partial class YearMaster
{
    [Column("YMS_ID")]
    [StringLength(10)]
    public string YmsId { get; set; } = null!;

    [Column("YMS_FROMDATE", TypeName = "datetime")]
    public DateTime? YmsFromdate { get; set; }

    [Column("YMS_TODATE", TypeName = "datetime")]
    public DateTime? YmsTodate { get; set; }

    [Column("YMS_YEARID", TypeName = "decimal(5, 0)")]
    public decimal? YmsYearid { get; set; }

    [Column("YMS_Default")]
    public int? YmsDefault { get; set; }

    [Column("YMS_CreatedBy")]
    public int? YmsCreatedBy { get; set; }

    [Column("YMS_CreatedOn", TypeName = "datetime")]
    public DateTime? YmsCreatedOn { get; set; }

    [Column("YMS_UpdatedBy")]
    public int? YmsUpdatedBy { get; set; }

    [Column("YMS_UpdatedOn", TypeName = "datetime")]
    public DateTime? YmsUpdatedOn { get; set; }

    [Column("YMS_ApprovedBy")]
    public int? YmsApprovedBy { get; set; }

    [Column("YMS_ApprovedOn", TypeName = "datetime")]
    public DateTime? YmsApprovedOn { get; set; }

    [Column("YMS_DeletedBy")]
    public int? YmsDeletedBy { get; set; }

    [Column("YMS_DeletedOn", TypeName = "datetime")]
    public DateTime? YmsDeletedOn { get; set; }

    [Column("YMS_RecalledBy")]
    public int? YmsRecalledBy { get; set; }

    [Column("YMS_RecalledOn", TypeName = "datetime")]
    public DateTime? YmsRecalledOn { get; set; }

    [Column("YMS_Delflag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? YmsDelflag { get; set; }

    [Column("YMS_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? YmsStatus { get; set; }

    [Column("YMS_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? YmsIpaddress { get; set; }

    [Column("YMS_CompID")]
    public int? YmsCompId { get; set; }
}
