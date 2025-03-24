using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UserEMP_Assessment")]
public partial class SadUserEmpAssessment
{
    [Column("SUA_PKID")]
    public int? SuaPkid { get; set; }

    [Column("SUA_UserEmpID")]
    public int? SuaUserEmpId { get; set; }

    [Column("SUA_IssueDate", TypeName = "datetime")]
    public DateTime? SuaIssueDate { get; set; }

    [Column("SUA_Rating")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaRating { get; set; }

    [Column("SUA_PerformanceAwardPaid")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaPerformanceAwardPaid { get; set; }

    [Column("SUA_GradesPromotedFrom")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaGradesPromotedFrom { get; set; }

    [Column("SUA_GradesPromotedTo")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaGradesPromotedTo { get; set; }

    [Column("SUA_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SuaRemarks { get; set; }

    [Column("SUA_AttachID")]
    public int? SuaAttachId { get; set; }

    [Column("SUA_CrBy")]
    public int? SuaCrBy { get; set; }

    [Column("SUA_CrOn", TypeName = "datetime")]
    public DateTime? SuaCrOn { get; set; }

    [Column("SUA_UpdatedBy")]
    public int? SuaUpdatedBy { get; set; }

    [Column("SUA_UpdatedOn", TypeName = "datetime")]
    public DateTime? SuaUpdatedOn { get; set; }

    [Column("SUA_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SuaIpaddress { get; set; }

    [Column("SUA_CompID")]
    public int? SuaCompId { get; set; }
}
