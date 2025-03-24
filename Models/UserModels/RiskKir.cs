using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_KIR")]
public partial class RiskKir
{
    [Column("KIR_Pkid")]
    public int KirPkid { get; set; }

    [Column("KIR_TraceRefNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirTraceRefNo { get; set; }

    [Column("KIR_RiskActionable")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirRiskActionable { get; set; }

    [Column("KIR_AssignmentDate", TypeName = "datetime")]
    public DateTime? KirAssignmentDate { get; set; }

    [Column("KIR_Month")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirMonth { get; set; }

    [Column("KIR_Email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirEmail { get; set; }

    [Column("KIR_Trigger")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirTrigger { get; set; }

    [Column("KIR_CaseSummary")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? KirCaseSummary { get; set; }

    [Column("KIR_EntityInv")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirEntityInv { get; set; }

    [Column("KIR_AdvisorCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirAdvisorCode { get; set; }

    [Column("KIR_AdvisorName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirAdvisorName { get; set; }

    [Column("KIR_EmpCode")]
    public int? KirEmpCode { get; set; }

    [Column("KIR_EmpName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirEmpName { get; set; }

    [Column("KIR_Channel")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirChannel { get; set; }

    [Column("KIR_CaseClassification")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirCaseClassification { get; set; }

    [Column("KIR_RiskType")]
    public int? KirRiskType { get; set; }

    [Column("KIR_FraudReptdStage")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirFraudReptdStage { get; set; }

    [Column("KIR_ContractNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirContractNo { get; set; }

    [Column("KIR_ActualLoss")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirActualLoss { get; set; }

    [Column("KIR_NotionalLoss")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirNotionalLoss { get; set; }

    [Column("KIR_LossAmtRecvd")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirLossAmtRecvd { get; set; }

    [Column("KIR_AsgnDate", TypeName = "datetime")]
    public DateTime? KirAsgnDate { get; set; }

    [Column("KIR_InvOutcome")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirInvOutcome { get; set; }

    [Column("KIR_InvSummary")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? KirInvSummary { get; set; }

    [Column("KIR_ClosureDate", TypeName = "datetime")]
    public DateTime? KirClosureDate { get; set; }

    [Column("KIR_ClosureDays")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirClosureDays { get; set; }

    [Column("KIR_CauseInitiationDate", TypeName = "datetime")]
    public DateTime? KirCauseInitiationDate { get; set; }

    [Column("KIR_PreDispAction")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirPreDispAction { get; set; }

    [Column("KIR_ActionAgainstInter")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirActionAgainstInter { get; set; }

    [Column("KIR_ActionAgainstEmp")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirActionAgainstEmp { get; set; }

    [Column("KIR_NoActionRsn")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? KirNoActionRsn { get; set; }

    [Column("KIR_MatrixAction")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? KirMatrixAction { get; set; }

    [Column("KIR_DeviationRsn")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? KirDeviationRsn { get; set; }

    [Column("KIR_ZEDCDate", TypeName = "datetime")]
    public DateTime? KirZedcdate { get; set; }

    [Column("KIR_CEDCDate", TypeName = "datetime")]
    public DateTime? KirCedcdate { get; set; }

    [Column("KIR_KIRStatus")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirKirstatus { get; set; }

    [Column("KIR_FIRfrwdDate", TypeName = "datetime")]
    public DateTime? KirFirfrwdDate { get; set; }

    [Column("KIR_LawName")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirLawName { get; set; }

    [Column("KIR_PreventiveStep")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirPreventiveStep { get; set; }

    [Column("KIR_RCAstatus")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirRcastatus { get; set; }

    [Column("KIR_RCAName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirRcaname { get; set; }

    [Column("KIR_CustName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirCustName { get; set; }

    [Column("KIR_Zone")]
    public int? KirZone { get; set; }

    [Column("KIR_SMCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirSmcode { get; set; }

    [Column("KIR_SMName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirSmname { get; set; }

    [Column("KIR_Region")]
    public int? KirRegion { get; set; }

    [Column("KIR_Location")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirLocation { get; set; }

    [Column("KIR_Plan")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KirPlan { get; set; }

    [Column("KIR_Term")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirTerm { get; set; }

    [Column("KIR_LoginDate", TypeName = "datetime")]
    public DateTime? KirLoginDate { get; set; }

    [Column("KIR_IssuanceDate", TypeName = "datetime")]
    public DateTime? KirIssuanceDate { get; set; }

    [Column("KIR_Premium")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirPremium { get; set; }

    [Column("KIR_SumAssured")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirSumAssured { get; set; }

    [Column("KIR_BusinessSegment")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirBusinessSegment { get; set; }

    [Column("KIR_ZCAR")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KirZcar { get; set; }

    [Column("KIR_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? KirDelFlag { get; set; }

    [Column("KIR_STATUS")]
    [StringLength(3)]
    [Unicode(false)]
    public string? KirStatus { get; set; }

    [Column("KIR_CrBy")]
    public int? KirCrBy { get; set; }

    [Column("KIR_CrOn", TypeName = "datetime")]
    public DateTime? KirCrOn { get; set; }

    [Column("KIR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KirIpaddress { get; set; }

    [Column("KIR_YearID")]
    public int? KirYearId { get; set; }

    [Column("KIR_CompID")]
    public int? KirCompId { get; set; }
}
