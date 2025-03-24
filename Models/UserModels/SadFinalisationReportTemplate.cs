using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_Finalisation_Report_Template")]
public partial class SadFinalisationReportTemplate
{
    [Column("TEM_Id")]
    public int? TemId { get; set; }

    [Column("TEM_Yearid")]
    public int? TemYearid { get; set; }

    [Column("TEM_FunctionId")]
    public int? TemFunctionId { get; set; }

    [Column("TEM_Module")]
    [StringLength(500)]
    [Unicode(false)]
    public string? TemModule { get; set; }

    [Column("TEM_ReportTitle")]
    public int? TemReportTitle { get; set; }

    [Column("TEM_ContentId")]
    [StringLength(100)]
    [Unicode(false)]
    public string? TemContentId { get; set; }

    [Column("TEM_SortOrder")]
    [StringLength(100)]
    [Unicode(false)]
    public string? TemSortOrder { get; set; }

    [Column("TEM_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TemDelflag { get; set; }

    [Column("TEM_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TemStatus { get; set; }

    [Column("TEM_CrBy")]
    public int? TemCrBy { get; set; }

    [Column("TEM_CrOn", TypeName = "datetime")]
    public DateTime? TemCrOn { get; set; }

    [Column("TEM_UpdatedBy")]
    public int? TemUpdatedBy { get; set; }

    [Column("TEM_UpdatedOn", TypeName = "datetime")]
    public DateTime? TemUpdatedOn { get; set; }

    [Column("TEM_DeletedBy")]
    public int? TemDeletedBy { get; set; }

    [Column("TEM_DeletedOn", TypeName = "datetime")]
    public DateTime? TemDeletedOn { get; set; }

    [Column("TEM_AppBy")]
    public int? TemAppBy { get; set; }

    [Column("TEM_AppOn", TypeName = "datetime")]
    public DateTime? TemAppOn { get; set; }

    [Column("TEM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TemIpaddress { get; set; }

    [Column("TEM_Compid")]
    public int? TemCompid { get; set; }
}
