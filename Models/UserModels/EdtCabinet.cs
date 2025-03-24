using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_Cabinet")]
public partial class EdtCabinet
{
    [Column("CBN_ID")]
    public int? CbnId { get; set; }

    [Column("CBN_Name")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CbnName { get; set; }

    [Column("CBN_Parent")]
    public int? CbnParent { get; set; }

    [Column("CBN_Note")]
    [Unicode(false)]
    public string? CbnNote { get; set; }

    [Column("CBN_UserID")]
    public int? CbnUserId { get; set; }

    [Column("CBN_Department")]
    public int? CbnDepartment { get; set; }

    [Column("CBN_SubCabCount")]
    public int? CbnSubCabCount { get; set; }

    [Column("CBN_FolderCount")]
    public int? CbnFolderCount { get; set; }

    [Column("CBN_CreatedBy")]
    public int? CbnCreatedBy { get; set; }

    [Column("CBN_CreatedOn", TypeName = "datetime")]
    public DateTime? CbnCreatedOn { get; set; }

    [Column("CBN_UpdatedBy")]
    public int? CbnUpdatedBy { get; set; }

    [Column("CBN_UpdatedOn", TypeName = "datetime")]
    public DateTime? CbnUpdatedOn { get; set; }

    [Column("CBN_ApprovedBy")]
    public int? CbnApprovedBy { get; set; }

    [Column("CBN_ApprovedOn", TypeName = "datetime")]
    public DateTime? CbnApprovedOn { get; set; }

    [Column("CBN_DeletedBy")]
    public int? CbnDeletedBy { get; set; }

    [Column("CBN_DeletedOn", TypeName = "datetime")]
    public DateTime? CbnDeletedOn { get; set; }

    [Column("CBN_RecalledBy")]
    public int? CbnRecalledBy { get; set; }

    [Column("CBN_RecalledOn", TypeName = "datetime")]
    public DateTime? CbnRecalledOn { get; set; }

    [Column("CBN_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CbnStatus { get; set; }

    [Column("CBN_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CbnDelFlag { get; set; }

    [Column("CBN_CompID")]
    public int? CbnCompId { get; set; }
}
