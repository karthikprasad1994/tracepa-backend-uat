using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UserEMP_ParticularsofArticles")]
public partial class SadUserEmpParticularsofArticle
{
    [Column("SUP_PKID")]
    public int? SupPkid { get; set; }

    [Column("SUP_UserEmpID")]
    public int? SupUserEmpId { get; set; }

    [Column("SUP_PrincipleName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SupPrincipleName { get; set; }

    [Column("SUP_RegistrationNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SupRegistrationNo { get; set; }

    [Column("SUP_PracticeNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SupPracticeNo { get; set; }

    [Column("SUP_ArticlesFrom", TypeName = "datetime")]
    public DateTime? SupArticlesFrom { get; set; }

    [Column("SUP_ArticlesTo", TypeName = "datetime")]
    public DateTime? SupArticlesTo { get; set; }

    [Column("SUP_ExtendedTo", TypeName = "datetime")]
    public DateTime? SupExtendedTo { get; set; }

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

    [Column("SUP_CompID")]
    public int? SupCompId { get; set; }
}
