using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UserEMP_Courses")]
public partial class SadUserEmpCourse
{
    [Column("SUC_PKID")]
    public int? SucPkid { get; set; }

    [Column("SUC_UserEmpID")]
    public int? SucUserEmpId { get; set; }

    [Column("SUC_Date", TypeName = "datetime")]
    public DateTime? SucDate { get; set; }

    [Column("SUC_Subject")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SucSubject { get; set; }

    [Column("SUC_FeeEmployer")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SucFeeEmployer { get; set; }

    [Column("SUC_FeeEmployee")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SucFeeEmployee { get; set; }

    [Column("SUC_ConductedBy")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SucConductedBy { get; set; }

    [Column("SUC_CPEPoints")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SucCpepoints { get; set; }

    [Column("SUC_Papers")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SucPapers { get; set; }

    [Column("SUC_BriefDescription")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SucBriefDescription { get; set; }

    [Column("SUC_FeedBack")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SucFeedBack { get; set; }

    [Column("SUC_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SucRemarks { get; set; }

    [Column("SUC_AttachID")]
    public int? SucAttachId { get; set; }

    [Column("SUC_CrBy")]
    public int? SucCrBy { get; set; }

    [Column("SUC_CrOn", TypeName = "datetime")]
    public DateTime? SucCrOn { get; set; }

    [Column("SUC_UpdatedBy")]
    public int? SucUpdatedBy { get; set; }

    [Column("SUC_UpdatedOn", TypeName = "datetime")]
    public DateTime? SucUpdatedOn { get; set; }

    [Column("SUC_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SucIpaddress { get; set; }

    [Column("SUC_CompID")]
    public int? SucCompId { get; set; }
}
