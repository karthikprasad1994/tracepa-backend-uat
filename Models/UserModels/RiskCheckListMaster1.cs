using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_CheckList_Master1")]
public partial class RiskCheckListMaster1
{
    [Column("RCM_Id")]
    public int RcmId { get; set; }

    [Column("RCM_FunctionId")]
    public int? RcmFunctionId { get; set; }

    [Column("RCM_AreaId")]
    public int? RcmAreaId { get; set; }

    [Column("RCM_RiskCategory")]
    [StringLength(10)]
    [Unicode(false)]
    public string? RcmRiskCategory { get; set; }

    [Column("RCM_RiskWeight")]
    public double? RcmRiskWeight { get; set; }

    [Column("RCM_CheckPointNo")]
    [StringLength(10)]
    [Unicode(false)]
    public string? RcmCheckPointNo { get; set; }

    [Column("RCM_CheckPoint")]
    [StringLength(600)]
    [Unicode(false)]
    public string? RcmCheckPoint { get; set; }

    [Column("RCM_MethodologyId")]
    public int? RcmMethodologyId { get; set; }

    [Column("RCM_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? RcmDelflag { get; set; }

    [Column("RCM_SampleSize")]
    public int? RcmSampleSize { get; set; }

    [Column("RCM_AreaNo")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RcmAreaNo { get; set; }

    [Column("RCM_YearId")]
    public int? RcmYearId { get; set; }

    [Column("RCM_FunType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? RcmFunType { get; set; }

    [Column("RCM_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? RcmStatus { get; set; }

    [Column("RCM_CrBy")]
    public int? RcmCrBy { get; set; }

    [Column("RCM_CrOn", TypeName = "datetime")]
    public DateTime? RcmCrOn { get; set; }

    [Column("RCM_UpdatedBy")]
    public int? RcmUpdatedBy { get; set; }

    [Column("RCM_UpdatedOn", TypeName = "datetime")]
    public DateTime? RcmUpdatedOn { get; set; }

    [Column("RCM_ApprovedBy")]
    public int? RcmApprovedBy { get; set; }

    [Column("RCM_ApprovedOn", TypeName = "datetime")]
    public DateTime? RcmApprovedOn { get; set; }

    [Column("RCM_DeletedBy")]
    public int? RcmDeletedBy { get; set; }

    [Column("RCM_DeletedOn", TypeName = "datetime")]
    public DateTime? RcmDeletedOn { get; set; }

    [Column("RCM_RecallBy")]
    public int? RcmRecallBy { get; set; }

    [Column("RCM_RecallOn", TypeName = "datetime")]
    public DateTime? RcmRecallOn { get; set; }

    [Column("RCM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcmIpaddress { get; set; }

    [Column("RCM_CompID")]
    public int? RcmCompId { get; set; }
}
