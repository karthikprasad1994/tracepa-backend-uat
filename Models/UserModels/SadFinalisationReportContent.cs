using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_Finalisation_Report_Contents")]
public partial class SadFinalisationReportContent
{
    [Column("FPT_Id")]
    public int? FptId { get; set; }

    [Column("FPT_Yearid")]
    public int? FptYearid { get; set; }

    [Column("FPT_FunctionId")]
    public int? FptFunctionId { get; set; }

    [Column("FPT_FunctionName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? FptFunctionName { get; set; }

    [Column("FPT_Title")]
    [StringLength(500)]
    [Unicode(false)]
    public string? FptTitle { get; set; }

    [Column("FPT_Details")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? FptDetails { get; set; }

    [Column("FPT_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? FptDelflag { get; set; }

    [Column("FPT_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? FptStatus { get; set; }

    [Column("FPT_CrBy")]
    public int? FptCrBy { get; set; }

    [Column("FPT_CrOn", TypeName = "datetime")]
    public DateTime? FptCrOn { get; set; }

    [Column("FPT_UpdatedBy")]
    public int? FptUpdatedBy { get; set; }

    [Column("FPT_UpdatedOn", TypeName = "datetime")]
    public DateTime? FptUpdatedOn { get; set; }

    [Column("FPT_DeletedBy")]
    public int? FptDeletedBy { get; set; }

    [Column("FPT_DeletedOn", TypeName = "datetime")]
    public DateTime? FptDeletedOn { get; set; }

    [Column("FPT_AppBy")]
    public int? FptAppBy { get; set; }

    [Column("FPT_AppOn", TypeName = "datetime")]
    public DateTime? FptAppOn { get; set; }

    [Column("FPT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? FptIpaddress { get; set; }

    [Column("FPT_CompID")]
    public int? FptCompId { get; set; }
}
