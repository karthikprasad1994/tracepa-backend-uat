using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UserEMP_AcademicProgress")]
public partial class SadUserEmpAcademicProgress
{
    [Column("SUAP_PKID")]
    public int? SuapPkid { get; set; }

    [Column("SUAP_UserEmpID")]
    public int? SuapUserEmpId { get; set; }

    [Column("SUAP_ExamTakenOn", TypeName = "datetime")]
    public DateTime? SuapExamTakenOn { get; set; }

    [Column("SUAP_LeaveGranted")]
    public int? SuapLeaveGranted { get; set; }

    [Column("SUAP_MonthofExam")]
    public int? SuapMonthofExam { get; set; }

    [Column("SUAP_Groups")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuapGroups { get; set; }

    [Column("SUAP_Result")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuapResult { get; set; }

    [Column("SUAP_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SuapRemarks { get; set; }

    [Column("SUAP_AttachID")]
    public int? SuapAttachId { get; set; }

    [Column("SUAP_CrBy")]
    public int? SuapCrBy { get; set; }

    [Column("SUAP_CrOn", TypeName = "datetime")]
    public DateTime? SuapCrOn { get; set; }

    [Column("SUAP_UpdatedBy")]
    public int? SuapUpdatedBy { get; set; }

    [Column("SUAP_UpdatedOn", TypeName = "datetime")]
    public DateTime? SuapUpdatedOn { get; set; }

    [Column("SUAP_IPAddress")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SuapIpaddress { get; set; }

    [Column("SUAP_CompId")]
    public int? SuapCompId { get; set; }
}
