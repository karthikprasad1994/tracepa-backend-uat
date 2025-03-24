using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("acc_Year_Master")]
public partial class AccYearMaster
{
    [Column("YMS_ID")]
    public int YmsId { get; set; }

    [Column("YMS_FROMDATE", TypeName = "datetime")]
    public DateTime? YmsFromdate { get; set; }

    [Column("YMS_TODATE", TypeName = "datetime")]
    public DateTime? YmsTodate { get; set; }

    [Column("YMS_CRBY")]
    public int? YmsCrby { get; set; }

    [Column("YMS_CRON", TypeName = "datetime")]
    public DateTime? YmsCron { get; set; }

    [Column("yms_FreezeYear")]
    [StringLength(1)]
    [Unicode(false)]
    public string? YmsFreezeYear { get; set; }

    [Column("YMS_FROM_YEAR")]
    public int? YmsFromYear { get; set; }

    [Column("YMS_TO_YEAR")]
    public int? YmsToYear { get; set; }

    [Column("YMS_CompID")]
    public int? YmsCompId { get; set; }

    [Column("YMS_Status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? YmsStatus { get; set; }

    [Column("YMS_DeletedBy")]
    public int? YmsDeletedBy { get; set; }

    [Column("YMS_ReCalledBy")]
    public int? YmsReCalledBy { get; set; }

    [Column("YMS_DelFlag")]
    [StringLength(20)]
    [Unicode(false)]
    public string? YmsDelFlag { get; set; }

    [Column("YMS_UpdatedBy")]
    public int? YmsUpdatedBy { get; set; }

    [Column("YMS_UpdatedOn", TypeName = "datetime")]
    public DateTime? YmsUpdatedOn { get; set; }

    [Column("YMS_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? YmsOperation { get; set; }

    [Column("YMS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? YmsIpaddress { get; set; }

    [Column("YMS_ApprovedBy")]
    public int? YmsApprovedBy { get; set; }

    [Column("YMS_ApprovedOn", TypeName = "datetime")]
    public DateTime? YmsApprovedOn { get; set; }

    [Column("YMS_Default")]
    public int? YmsDefault { get; set; }
}
