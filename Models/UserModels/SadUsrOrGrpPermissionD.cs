using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UsrOrGrp_permissionD")]
public partial class SadUsrOrGrpPermissionD
{
    [Column("SGP_ID")]
    public int? SgpId { get; set; }

    [Column("SGP_ModID")]
    public int? SgpModId { get; set; }

    [Column("SGP_LevelGroup")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SgpLevelGroup { get; set; }

    [Column("SGP_LevelGroupID")]
    public int? SgpLevelGroupId { get; set; }

    [Column("SGP_View")]
    public int? SgpView { get; set; }

    [Column("SGP_SaveOrUpdate")]
    public int? SgpSaveOrUpdate { get; set; }

    [Column("SGP_ActiveOrDeactive")]
    public int? SgpActiveOrDeactive { get; set; }

    [Column("SGP_Report")]
    public int? SgpReport { get; set; }

    [Column("SGP_CreatedBy")]
    public int? SgpCreatedBy { get; set; }

    [Column("SGP_CreatedOn", TypeName = "datetime")]
    public DateTime? SgpCreatedOn { get; set; }

    [Column("SGP_ApprovedBy")]
    public int? SgpApprovedBy { get; set; }

    [Column("SGP_ApprovedOn", TypeName = "datetime")]
    public DateTime? SgpApprovedOn { get; set; }

    [Column("SGP_UpdatedBy")]
    public int? SgpUpdatedBy { get; set; }

    [Column("SGP_UpdatedOn", TypeName = "datetime")]
    public DateTime? SgpUpdatedOn { get; set; }

    [Column("SGP_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SgpStatus { get; set; }

    [Column("SGP_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SgpDelFlag { get; set; }

    [Column("SGP_CompID")]
    public int? SgpCompId { get; set; }

    [Column("SGP_Download")]
    public int? SgpDownload { get; set; }

    [Column("SGP_Annotaion")]
    public int? SgpAnnotaion { get; set; }
}
