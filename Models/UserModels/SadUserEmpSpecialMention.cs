using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UserEMP_SpecialMentions")]
public partial class SadUserEmpSpecialMention
{
    [Column("SUS_PKID")]
    public int? SusPkid { get; set; }

    [Column("SUS_UserEmpID")]
    public int? SusUserEmpId { get; set; }

    [Column("SUS_SpecialMention")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SusSpecialMention { get; set; }

    [Column("SUS_Date", TypeName = "datetime")]
    public DateTime? SusDate { get; set; }

    [Column("SUS_Particulars")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SusParticulars { get; set; }

    [Column("SUS_DealtWith")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SusDealtWith { get; set; }

    [Column("SUS_AttachID")]
    public int? SusAttachId { get; set; }

    [Column("SUS_CrBy")]
    public int? SusCrBy { get; set; }

    [Column("SUS_CrOn", TypeName = "datetime")]
    public DateTime? SusCrOn { get; set; }

    [Column("SUS_UpdatedBy")]
    public int? SusUpdatedBy { get; set; }

    [Column("SUS_UpdatedOn", TypeName = "datetime")]
    public DateTime? SusUpdatedOn { get; set; }

    [Column("SUS_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SusIpaddress { get; set; }

    [Column("SUS_CompID")]
    public int? SusCompId { get; set; }
}
