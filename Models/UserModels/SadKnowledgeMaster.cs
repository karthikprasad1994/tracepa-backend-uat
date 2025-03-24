using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_Knowledge_Master")]
public partial class SadKnowledgeMaster
{
    [Column("TKB_ID")]
    public int? TkbId { get; set; }

    [Column("TKB_Subject")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? TkbSubject { get; set; }

    [Column("TKB_Content")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? TkbContent { get; set; }

    [Column("TKB_CrBy")]
    public int? TkbCrBy { get; set; }

    [Column("TKB_CrOn", TypeName = "datetime")]
    public DateTime? TkbCrOn { get; set; }

    [Column("TKB_UpdatedBy")]
    public int? TkbUpdatedBy { get; set; }

    [Column("TKB_UpdatedOn", TypeName = "datetime")]
    public DateTime? TkbUpdatedOn { get; set; }

    [Column("TKB_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TkbStatus { get; set; }

    [Column("TKB_ApprovedBy")]
    public int? TkbApprovedBy { get; set; }

    [Column("TKB_ApprovedOn", TypeName = "datetime")]
    public DateTime? TkbApprovedOn { get; set; }

    [Column("TKB_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TkbIpaddress { get; set; }

    [Column("TKB_CompID")]
    public int? TkbCompId { get; set; }
}
