using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_CONTROL_Library")]
public partial class MstControlLibrary
{
    [Column("MCL_PKID")]
    public int? MclPkid { get; set; }

    [Column("MCL_ControlName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MclControlName { get; set; }

    [Column("MCL_ControlDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? MclControlDesc { get; set; }

    [Column("MCL_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? MclCode { get; set; }

    [Column("MCL_IsKey")]
    public int? MclIsKey { get; set; }

    [Column("MCL_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MclDelFlag { get; set; }

    [Column("MCL_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MclStatus { get; set; }

    [Column("MCL_CrBy")]
    public int? MclCrBy { get; set; }

    [Column("MCL_CrOn", TypeName = "datetime")]
    public DateTime? MclCrOn { get; set; }

    [Column("MCL_UpdatedBy")]
    public int? MclUpdatedBy { get; set; }

    [Column("MCL_UpdatedOn", TypeName = "datetime")]
    public DateTime? MclUpdatedOn { get; set; }

    [Column("MCL_ApprovedBy")]
    public int? MclApprovedBy { get; set; }

    [Column("MCL_ApprovedOn", TypeName = "datetime")]
    public DateTime? MclApprovedOn { get; set; }

    [Column("MCL_DeletedBy")]
    public int? MclDeletedBy { get; set; }

    [Column("MCL_DeletedOn", TypeName = "datetime")]
    public DateTime? MclDeletedOn { get; set; }

    [Column("MCL_RecallBy")]
    public int? MclRecallBy { get; set; }

    [Column("MCL_RecallOn", TypeName = "datetime")]
    public DateTime? MclRecallOn { get; set; }

    [Column("MCL_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MclIpaddress { get; set; }

    [Column("MCL_CompID")]
    public int? MclCompId { get; set; }
}
