using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_Risk_Color_Matrix")]
public partial class MstRiskColorMatrix
{
    [Column("RCM_RowID")]
    public int? RcmRowId { get; set; }

    [Column("RCM_ColumnID")]
    public int? RcmColumnId { get; set; }

    [Column("RCM_Category")]
    [StringLength(10)]
    [Unicode(false)]
    public string? RcmCategory { get; set; }

    [Column("RCM_ColorsName")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcmColorsName { get; set; }

    [Column("RCM_CreatedBy")]
    public int? RcmCreatedBy { get; set; }

    [Column("RCM_CreatedOn", TypeName = "datetime")]
    public DateTime? RcmCreatedOn { get; set; }

    [Column("RCM_UpdatedBy")]
    public int? RcmUpdatedBy { get; set; }

    [Column("RCM_UpdatedOn", TypeName = "datetime")]
    public DateTime? RcmUpdatedOn { get; set; }

    [Column("RCM_CompID")]
    public int? RcmCompId { get; set; }

    [Column("RCM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcmIpaddress { get; set; }
}
