using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMACheckMaster")]
public partial class CmacheckMaster
{
    [Column("CM_Id")]
    public int CmId { get; set; }

    [Column("CM_FunctionId")]
    public int? CmFunctionId { get; set; }

    [Column("CM_AreaId")]
    public int? CmAreaId { get; set; }

    [Column("CM_RiskCategory")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CmRiskCategory { get; set; }

    [Column("CM_RiskWeight")]
    public double? CmRiskWeight { get; set; }

    [Column("CM_CheckPointNo")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CmCheckPointNo { get; set; }

    [Column("CM_CheckPoint")]
    [StringLength(600)]
    [Unicode(false)]
    public string? CmCheckPoint { get; set; }

    [Column("CM_MethodologyId")]
    public int? CmMethodologyId { get; set; }

    [Column("CM_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmDelflag { get; set; }

    [Column("CM_SampleSize")]
    public int? CmSampleSize { get; set; }

    [Column("CM_AreaNo")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmAreaNo { get; set; }

    [Column("CM_YearId")]
    public int? CmYearId { get; set; }

    [Column("CM_FunType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmFunType { get; set; }

    [Column("CM_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CmStatus { get; set; }

    [Column("CM_CrBy")]
    public int? CmCrBy { get; set; }

    [Column("CM_CrOn", TypeName = "datetime")]
    public DateTime? CmCrOn { get; set; }

    [Column("CM_UpdatedBy")]
    public int? CmUpdatedBy { get; set; }

    [Column("CM_UpdatedOn", TypeName = "datetime")]
    public DateTime? CmUpdatedOn { get; set; }

    [Column("CM_ApprovedBy")]
    public int? CmApprovedBy { get; set; }

    [Column("CM_ApprovedOn", TypeName = "datetime")]
    public DateTime? CmApprovedOn { get; set; }

    [Column("CM_DeletedBy")]
    public int? CmDeletedBy { get; set; }

    [Column("CM_DeletedOn", TypeName = "datetime")]
    public DateTime? CmDeletedOn { get; set; }

    [Column("CM_RecallBy")]
    public int? CmRecallBy { get; set; }

    [Column("CM_RecallOn", TypeName = "datetime")]
    public DateTime? CmRecallOn { get; set; }

    [Column("CM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmIpaddress { get; set; }

    [Column("CM_CompID")]
    public int? CmCompId { get; set; }
}
