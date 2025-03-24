using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_Checks_Master")]
public partial class MstChecksMaster
{
    [Column("CHK_ID")]
    public int? ChkId { get; set; }

    [Column("CHK_ControlID")]
    public int? ChkControlId { get; set; }

    [Column("CHK_CheckName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ChkCheckName { get; set; }

    [Column("CHK_CheckDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? ChkCheckDesc { get; set; }

    [Column("CHK_CatId")]
    public int? ChkCatId { get; set; }

    [Column("CHK_IsKey")]
    public int? ChkIsKey { get; set; }

    [Column("CHK_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ChkDelFlag { get; set; }

    [Column("CHK_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ChkStatus { get; set; }

    [Column("CHK_CrBy")]
    public int? ChkCrBy { get; set; }

    [Column("CHK_CrOn", TypeName = "datetime")]
    public DateTime? ChkCrOn { get; set; }

    [Column("CHK_UpdatedBy")]
    public int? ChkUpdatedBy { get; set; }

    [Column("CHK_UpdatedOn", TypeName = "datetime")]
    public DateTime? ChkUpdatedOn { get; set; }

    [Column("CHK_ApprovedBy")]
    public int? ChkApprovedBy { get; set; }

    [Column("CHK_ApprovedOn", TypeName = "datetime")]
    public DateTime? ChkApprovedOn { get; set; }

    [Column("CHK_DeletedBy")]
    public int? ChkDeletedBy { get; set; }

    [Column("CHK_DeletedOn", TypeName = "datetime")]
    public DateTime? ChkDeletedOn { get; set; }

    [Column("CHK_RecallBy")]
    public int? ChkRecallBy { get; set; }

    [Column("CHK_RecallOn", TypeName = "datetime")]
    public DateTime? ChkRecallOn { get; set; }

    [Column("CHK_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ChkIpaddress { get; set; }

    [Column("CHK_CompID")]
    public int? ChkCompId { get; set; }
}
