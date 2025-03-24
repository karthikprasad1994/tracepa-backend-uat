using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_ReportContentMaster")]
public partial class SadReportContentMaster
{
    [Column("RCM_Id")]
    public int? RcmId { get; set; }

    [Column("RCM_ReportId")]
    public int? RcmReportId { get; set; }

    [Column("RCM_ReportName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? RcmReportName { get; set; }

    [Column("RCM_Heading")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RcmHeading { get; set; }

    [Column("RCM_Description")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? RcmDescription { get; set; }

    [Column("RCM_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? RcmDelflag { get; set; }

    [Column("RCM_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? RcmStatus { get; set; }

    [Column("RCM_CrBy")]
    public int? RcmCrBy { get; set; }

    [Column("RCM_CrOn", TypeName = "datetime")]
    public DateTime? RcmCrOn { get; set; }

    [Column("RCM_UpdatedBy")]
    public int? RcmUpdatedBy { get; set; }

    [Column("RCM_UpdatedOn", TypeName = "datetime")]
    public DateTime? RcmUpdatedOn { get; set; }

    [Column("RCM_DeletedBy")]
    public int? RcmDeletedBy { get; set; }

    [Column("RCM_DeletedOn", TypeName = "datetime")]
    public DateTime? RcmDeletedOn { get; set; }

    [Column("RCM_AppBy")]
    public int? RcmAppBy { get; set; }

    [Column("RCM_AppOn", TypeName = "datetime")]
    public DateTime? RcmAppOn { get; set; }

    [Column("RCM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcmIpaddress { get; set; }

    [Column("RCM_CompID")]
    public int? RcmCompId { get; set; }

    [Column("RCM_Yearid")]
    public int? RcmYearid { get; set; }
}
