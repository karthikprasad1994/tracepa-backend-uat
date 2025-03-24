using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_FOLDER")]
public partial class EdtFolder
{
    [Column("FOL_FolID")]
    public int? FolFolId { get; set; }

    [Column("FOL_Name")]
    [StringLength(500)]
    [Unicode(false)]
    public string? FolName { get; set; }

    [Column("FOL_Note")]
    [Unicode(false)]
    public string? FolNote { get; set; }

    [Column("FOL_Cabinet")]
    public int? FolCabinet { get; set; }

    [Column("FOL_CreatedBy")]
    public int? FolCreatedBy { get; set; }

    [Column("FOL_CreatedOn", TypeName = "datetime")]
    public DateTime? FolCreatedOn { get; set; }

    [Column("FOL_UpdatedBy")]
    public int? FolUpdatedBy { get; set; }

    [Column("FOL_UpdatedOn", TypeName = "datetime")]
    public DateTime? FolUpdatedOn { get; set; }

    [Column("FOL_ApprovedBy")]
    public int? FolApprovedBy { get; set; }

    [Column("FOL_ApprovedOn", TypeName = "datetime")]
    public DateTime? FolApprovedOn { get; set; }

    [Column("FOL_DeletedBy")]
    public int? FolDeletedBy { get; set; }

    [Column("FOL_DeletedOn", TypeName = "datetime")]
    public DateTime? FolDeletedOn { get; set; }

    [Column("FOL_RecalledBy")]
    public int? FolRecalledBy { get; set; }

    [Column("FOL_RecalledOn", TypeName = "datetime")]
    public DateTime? FolRecalledOn { get; set; }

    [Column("FOL_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? FolStatus { get; set; }

    [Column("FOL_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? FolDelFlag { get; set; }

    [Column("FOL_CompID")]
    public int? FolCompId { get; set; }
}
