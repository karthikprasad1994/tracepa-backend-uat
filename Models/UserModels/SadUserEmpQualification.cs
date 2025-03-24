using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UserEMP_Qualification")]
public partial class SadUserEmpQualification
{
    [Column("SUQ_PKID")]
    public int? SuqPkid { get; set; }

    [Column("SUQ_UserEmpID")]
    public int? SuqUserEmpId { get; set; }

    [Column("SUQ_Education")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuqEducation { get; set; }

    [Column("SUQ_University")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuqUniversity { get; set; }

    [Column("SUQ_School")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuqSchool { get; set; }

    [Column("SUQ_Year")]
    public int? SuqYear { get; set; }

    [Column("SUQ_Marks")]
    public double? SuqMarks { get; set; }

    [Column("SUQ_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SuqRemarks { get; set; }

    [Column("SUQ_AttachID")]
    public int? SuqAttachId { get; set; }

    [Column("SUQ_CrBy")]
    public int? SuqCrBy { get; set; }

    [Column("SUQ_CrOn", TypeName = "datetime")]
    public DateTime? SuqCrOn { get; set; }

    [Column("SUQ_UpdatedBy")]
    public int? SuqUpdatedBy { get; set; }

    [Column("SUQ_UpdatedOn", TypeName = "datetime")]
    public DateTime? SuqUpdatedOn { get; set; }

    [Column("SUQ_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SuqIpaddress { get; set; }

    [Column("SUQ_CompID")]
    public int? SuqCompId { get; set; }
}
