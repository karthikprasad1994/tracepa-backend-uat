using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CMACheckMaster_Log")]
public partial class CmacheckMasterLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(30)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("CM_Id")]
    public int? CmId { get; set; }

    [Column("CM_FunctionId")]
    public int? CmFunctionId { get; set; }

    [Column("nCM_FunctionId")]
    public int? NCmFunctionId { get; set; }

    [Column("CM_AreaId")]
    public int? CmAreaId { get; set; }

    [Column("nCM_AreaId")]
    public int? NCmAreaId { get; set; }

    [Column("CM_RiskCategory")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CmRiskCategory { get; set; }

    [Column("nCM_RiskCategory")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NCmRiskCategory { get; set; }

    [Column("CM_RiskWeight")]
    public float? CmRiskWeight { get; set; }

    [Column("nCM_RiskWeight")]
    public float? NCmRiskWeight { get; set; }

    [Column("CM_CheckPointNo")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CmCheckPointNo { get; set; }

    [Column("nCM_CheckPointNo")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NCmCheckPointNo { get; set; }

    [Column("CM_CheckPoint")]
    [StringLength(600)]
    [Unicode(false)]
    public string? CmCheckPoint { get; set; }

    [Column("nCM_CheckPoint")]
    [StringLength(600)]
    [Unicode(false)]
    public string? NCmCheckPoint { get; set; }

    [Column("CM_MethodologyId")]
    public int? CmMethodologyId { get; set; }

    [Column("nCM_MethodologyId")]
    public int? NCmMethodologyId { get; set; }

    [Column("CM_SampleSize")]
    public int? CmSampleSize { get; set; }

    [Column("nCM_SampleSize")]
    public int? NCmSampleSize { get; set; }

    [Column("CM_AreaNo")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmAreaNo { get; set; }

    [Column("nCM_AreaNo")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmAreaNo { get; set; }

    [Column("CM_YearId")]
    public int? CmYearId { get; set; }

    [Column("nCM_YearId")]
    public int? NCmYearId { get; set; }

    [Column("CM_FunType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmFunType { get; set; }

    [Column("nCM_FunType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NCmFunType { get; set; }

    [Column("CM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmIpaddress { get; set; }

    [Column("CM_CompID")]
    public int? CmCompId { get; set; }
}
