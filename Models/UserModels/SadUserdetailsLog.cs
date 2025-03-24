using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("sad_userdetails_Log")]
public partial class SadUserdetailsLog
{
    [Column("Log_PKID")]
    public long LogPkid { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_Operation")]
    [StringLength(50)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("USR_ID")]
    public int? UsrId { get; set; }

    [Column("USR_Node")]
    public int? UsrNode { get; set; }

    [Column("nUSR_Node")]
    public int? NUsrNode { get; set; }

    [Column("USR_Code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrCode { get; set; }

    [Column("nUSR_Code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrCode { get; set; }

    [Column("USR_FullName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrFullName { get; set; }

    [Column("nUSR_FullName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrFullName { get; set; }

    [Column("USR_LoginName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrLoginName { get; set; }

    [Column("nUSR_LoginName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrLoginName { get; set; }

    [Column("USR_PassWord")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrPassWord { get; set; }

    [Column("nUSR_PassWord")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrPassWord { get; set; }

    [Column("USR_Email")]
    [StringLength(200)]
    [Unicode(false)]
    public string? UsrEmail { get; set; }

    [Column("nUSR_Email")]
    [StringLength(200)]
    [Unicode(false)]
    public string? NUsrEmail { get; set; }

    [Column("USR_LevelGrp")]
    public int? UsrLevelGrp { get; set; }

    [Column("nUSR_LevelGrp")]
    public int? NUsrLevelGrp { get; set; }

    [Column("USR_DutyStatus")]
    [StringLength(1)]
    [Unicode(false)]
    public string? UsrDutyStatus { get; set; }

    [Column("nUSR_DutyStatus")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NUsrDutyStatus { get; set; }

    [Column("USR_PhoneNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrPhoneNo { get; set; }

    [Column("nUSR_PhoneNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrPhoneNo { get; set; }

    [Column("USR_MobileNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrMobileNo { get; set; }

    [Column("nUSR_MobileNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrMobileNo { get; set; }

    [Column("USR_OfficePhone")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrOfficePhone { get; set; }

    [Column("nUSR_OfficePhone")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrOfficePhone { get; set; }

    [Column("USR_OffPhExtn")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrOffPhExtn { get; set; }

    [Column("nUSR_OffPhExtn")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrOffPhExtn { get; set; }

    [Column("USR_Designation")]
    public int? UsrDesignation { get; set; }

    [Column("nUSR_Designation")]
    public int? NUsrDesignation { get; set; }

    [Column("USR_UserTypeID")]
    public int? UsrUserTypeId { get; set; }

    [Column("nUSR_UserTypeID")]
    public int? NUsrUserTypeId { get; set; }

    [Column("USR_CompanyID")]
    [StringLength(300)]
    [Unicode(false)]
    public string? UsrCompanyId { get; set; }

    [Column("nUSR_CompanyID")]
    [StringLength(300)]
    [Unicode(false)]
    public string? NUsrCompanyId { get; set; }

    [Column("USR_OrgnID")]
    public int? UsrOrgnId { get; set; }

    [Column("nUSR_OrgnID")]
    public int? NUsrOrgnId { get; set; }

    [Column("USR_DelFlag")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrDelFlag { get; set; }

    [Column("nUSR_DelFlag")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NUsrDelFlag { get; set; }

    [Column("USR_Type")]
    [StringLength(1)]
    [Unicode(false)]
    public string? UsrType { get; set; }

    [Column("nUSR_Type")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NUsrType { get; set; }

    [Column("USR_GrpOrUserLvlPerm")]
    public int? UsrGrpOrUserLvlPerm { get; set; }

    [Column("nUSR_GrpOrUserLvlPerm")]
    public int? NUsrGrpOrUserLvlPerm { get; set; }

    [Column("USR_NoOfUnSucsfAtteptts")]
    public int? UsrNoOfUnSucsfAtteptts { get; set; }

    [Column("nUSR_NoOfUnSucsfAtteptts")]
    public int? NUsrNoOfUnSucsfAtteptts { get; set; }

    [Column("usr_Que")]
    [StringLength(250)]
    [Unicode(false)]
    public string? UsrQue { get; set; }

    [Column("nusr_Que")]
    [StringLength(250)]
    [Unicode(false)]
    public string? NusrQue { get; set; }

    [Column("usr_ans")]
    [StringLength(250)]
    [Unicode(false)]
    public string? UsrAns { get; set; }

    [Column("nusr_ans")]
    [StringLength(250)]
    [Unicode(false)]
    public string? NusrAns { get; set; }

    [Column("USR_Category")]
    public int? UsrCategory { get; set; }

    [Column("nUSR_Category")]
    public int? NUsrCategory { get; set; }

    [Column("usr_partner")]
    public int? UsrPartner { get; set; }

    [Column("nusr_partner")]
    public int? NusrPartner { get; set; }

    [Column("USR_NoOfLogin")]
    public int? UsrNoOfLogin { get; set; }

    [Column("nUSR_NoOfLogin")]
    public int? NUsrNoOfLogin { get; set; }

    [Column("USR_LastLoginDate", TypeName = "datetime")]
    public DateTime? UsrLastLoginDate { get; set; }

    [Column("nUSR_LastLoginDate", TypeName = "datetime")]
    public DateTime? NUsrLastLoginDate { get; set; }

    [Column("USR_ReasonPwd_Block")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? UsrReasonPwdBlock { get; set; }

    [Column("nUSR_ReasonPwd_Block")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? NUsrReasonPwdBlock { get; set; }

    [Column("Usr_Role")]
    public int? UsrRole { get; set; }

    [Column("nUsr_Role")]
    public int? NUsrRole { get; set; }

    [Column("usr_masterModule")]
    public int? UsrMasterModule { get; set; }

    [Column("nusr_masterModule")]
    public int? NusrMasterModule { get; set; }

    [Column("usr_masterRole")]
    public int? UsrMasterRole { get; set; }

    [Column("nusr_masterRole")]
    public int? NusrMasterRole { get; set; }

    [Column("usr_AuditModule")]
    public int? UsrAuditModule { get; set; }

    [Column("nusr_AuditModule")]
    public int? NusrAuditModule { get; set; }

    [Column("usr_AuditRole")]
    public int? UsrAuditRole { get; set; }

    [Column("nusr_AuditRole")]
    public int? NusrAuditRole { get; set; }

    [Column("usr_RiskModule")]
    public int? UsrRiskModule { get; set; }

    [Column("nusr_RiskModule")]
    public int? NusrRiskModule { get; set; }

    [Column("usr_RiskRole")]
    public int? UsrRiskRole { get; set; }

    [Column("nusr_RiskRole")]
    public int? NusrRiskRole { get; set; }

    [Column("usr_ComplianceModule")]
    public int? UsrComplianceModule { get; set; }

    [Column("nusr_ComplianceModule")]
    public int? NusrComplianceModule { get; set; }

    [Column("usr_ComplianceRole")]
    public int? UsrComplianceRole { get; set; }

    [Column("nusr_ComplianceRole")]
    public int? NusrComplianceRole { get; set; }

    [Column("usr_BCMModule")]
    public int? UsrBcmmodule { get; set; }

    [Column("nusr_BCMModule")]
    public int? NusrBcmmodule { get; set; }

    [Column("usr_BCMRole")]
    public int? UsrBcmrole { get; set; }

    [Column("nusr_BCMRole")]
    public int? NusrBcmrole { get; set; }

    [Column("usr_skillSet")]
    [Unicode(false)]
    public string? UsrSkillSet { get; set; }

    [Column("nusr_skillSet")]
    [Unicode(false)]
    public string? NusrSkillSet { get; set; }

    [Column("usr_Experience")]
    public int? UsrExperience { get; set; }

    [Column("nusr_Experience")]
    public int? NusrExperience { get; set; }

    [Column("usr_qualification")]
    [Unicode(false)]
    public string? UsrQualification { get; set; }

    [Column("nusr_qualification")]
    [Unicode(false)]
    public string? NusrQualification { get; set; }

    [Column("usr_othersQualification")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? UsrOthersQualification { get; set; }

    [Column("nusr_othersQualification")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? NusrOthersQualification { get; set; }

    [Column("Usr_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? UsrIpaddress { get; set; }

    [Column("Usr_CompID")]
    public int? UsrCompId { get; set; }
}
