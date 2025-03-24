using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UserEMP_ProfExperiance")]
public partial class SadUserEmpProfExperiance
{
    [Column("SUP_PKID")]
    public int? SupPkid { get; set; }

    [Column("SUP_UserEmpID")]
    public int? SupUserEmpId { get; set; }

    [Column("SUP_Assignment")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SupAssignment { get; set; }

    [Column("SUP_ReportingTo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SupReportingTo { get; set; }

    [Column("SUP_From")]
    public int? SupFrom { get; set; }

    [Column("SUP_To")]
    public int? SupTo { get; set; }

    [Column("SUP_SalaryPerAnnum")]
    public double? SupSalaryPerAnnum { get; set; }

    [Column("SUP_Position")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SupPosition { get; set; }

    [Column("SUP_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SupRemarks { get; set; }

    [Column("SUP_AttachID")]
    public int? SupAttachId { get; set; }

    [Column("SUP_CrBy")]
    public int? SupCrBy { get; set; }

    [Column("SUP_CrOn", TypeName = "datetime")]
    public DateTime? SupCrOn { get; set; }

    [Column("SUP_UpdatedBy")]
    public int? SupUpdatedBy { get; set; }

    [Column("SUP_UpdatedOn", TypeName = "datetime")]
    public DateTime? SupUpdatedOn { get; set; }

    [Column("SUP_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SupIpaddress { get; set; }

    [Column("SUP_CompId")]
    public int? SupCompId { get; set; }
}
