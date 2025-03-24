using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_UserDetails")]
public partial class SadUserDetail
{
    [Key]
    [Column("usr_Id")]
    public int UsrId { get; set; }

    [Column("usr_Node")]
    public int? UsrNode { get; set; }

    [Column("usr_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? UsrCode { get; set; }

    [Column("usr_FullName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrFullName { get; set; }

    [Column("usr_LoginName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrLoginName { get; set; }

    [Column("usr_PassWord")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? UsrPassWord { get; set; }

    [Column("usr_Email")]
    [StringLength(200)]
    [Unicode(false)]
    public string? UsrEmail { get; set; }

    [Column("usr_DOB", TypeName = "datetime")]
    public DateTime? UsrDob { get; set; }

    [Column("usr_DOJ", TypeName = "datetime")]
    public DateTime? UsrDoj { get; set; }

    [Column("usr_LevelGrp")]
    public short? UsrLevelGrp { get; set; }

    [Column("usr_DutyStatus")]
    [StringLength(2)]
    [Unicode(false)]
    public string? UsrDutyStatus { get; set; }

    [Column("usr_HusOrFathName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrHusOrFathName { get; set; }

    [Column("usr_PhoneNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrPhoneNo { get; set; }

    [Column("usr_MobileNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrMobileNo { get; set; }

    [Column("usr_OfficePhone")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrOfficePhone { get; set; }

    [Column("usr_OffPhExtn")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrOffPhExtn { get; set; }

    [Column("usr_CurWrkAddId")]
    public byte? UsrCurWrkAddId { get; set; }

    [Column("usr_PermAddId")]
    public short? UsrPermAddId { get; set; }

    [Column("usr_ResAddId")]
    public short? UsrResAddId { get; set; }

    [Column("usr_OfficialAddId")]
    public short? UsrOfficialAddId { get; set; }

    [Column("usr_Photo")]
    public int? UsrPhoto { get; set; }

    [Column("usr_Signature")]
    public int? UsrSignature { get; set; }

    [Column("usr_Designation")]
    public short? UsrDesignation { get; set; }

    [Column("usr_UserTypeId")]
    public short? UsrUserTypeId { get; set; }

    [Column("usr_CompanyId")]
    public short? UsrCompanyId { get; set; }

    [Column("usr_OrgnId")]
    public short? UsrOrgnId { get; set; }

    [Column("usr_RefStaffNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrRefStaffNo { get; set; }

    [Column("usr_KinName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrKinName { get; set; }

    [Column("usr_KinAddId")]
    public short? UsrKinAddId { get; set; }

    [Column("usr_BloodGroup")]
    [StringLength(15)]
    [Unicode(false)]
    public string? UsrBloodGroup { get; set; }

    [Column("usr_ExpDate", TypeName = "datetime")]
    public DateTime? UsrExpDate { get; set; }

    [Column("usr_DelFlag")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrDelFlag { get; set; }

    [Column("usr_AppBy")]
    public int? UsrAppBy { get; set; }

    [Column("usr_AppOn", TypeName = "datetime")]
    public DateTime? UsrAppOn { get; set; }

    [Column("usr_CreatedBy")]
    public int? UsrCreatedBy { get; set; }

    [Column("usr_CreatedOn", TypeName = "datetime")]
    public DateTime? UsrCreatedOn { get; set; }

    [Column("usr_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? UsrStatus { get; set; }

    [Column("usr_Type")]
    [StringLength(1)]
    [Unicode(false)]
    public string? UsrType { get; set; }

    [Column("Usr_GrpOrUserLvlPerm")]
    public byte? UsrGrpOrUserLvlPerm { get; set; }

    [Column("usr_NoOfUnSucsfAtteptts")]
    public short? UsrNoOfUnSucsfAtteptts { get; set; }

    [Column("usr_Que")]
    [StringLength(250)]
    [Unicode(false)]
    public string? UsrQue { get; set; }

    [Column("usr_Ans")]
    [StringLength(250)]
    [Unicode(false)]
    public string? UsrAns { get; set; }

   // [Column("usr_Category")]
   // public int? UsrCategory { get; set; }

    [Column("usr_partner")]
    public int? UsrPartner { get; set; }

    [Column("USR_NoOfLogin")]
    public int? UsrNoOfLogin { get; set; }

    [Column("USR_LastLoginDate", TypeName = "datetime")]
    public DateTime? UsrLastLoginDate { get; set; }

    [Column("usr_ReasonPwd_Block")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? UsrReasonPwdBlock { get; set; }

    [Column("Usr_Role")]
    public int? UsrRole { get; set; }

    [Column("Usr_MasterModule")]
    public int? UsrMasterModule { get; set; }

    [Column("Usr_MasterRole")]
    public int? UsrMasterRole { get; set; }

    [Column("Usr_AuditModule")]
    public int? UsrAuditModule { get; set; }

    [Column("Usr_AuditRole")]
    public int? UsrAuditRole { get; set; }

    [Column("Usr_RiskModule")]
    public int? UsrRiskModule { get; set; }

    [Column("Usr_RiskRole")]
    public int? UsrRiskRole { get; set; }

    [Column("Usr_ComplianceModule")]
    public int? UsrComplianceModule { get; set; }

    [Column("Usr_ComplianceRole")]
    public int? UsrComplianceRole { get; set; }

    [Column("Usr_BCMModule")]
    public int? UsrBcmmodule { get; set; }

    [Column("Usr_BCMRole")]
    public int? UsrBcmrole { get; set; }

    [Column("Usr_SkillSet")]
    [Unicode(false)]
    public string? UsrSkillSet { get; set; }

    [Column("Usr_Experience")]
    [StringLength(500)]
    [Unicode(false)]
    public string? UsrExperience { get; set; }

    [Column("Usr_Qualification")]
    [Unicode(false)]
    public string? UsrQualification { get; set; }

    [Column("Usr_UpdatedBy")]
    public int? UsrUpdatedBy { get; set; }

    [Column("Usr_UpdatedOn", TypeName = "datetime")]
    public DateTime? UsrUpdatedOn { get; set; }

    [Column("Usr_DisableBy")]
    public int? UsrDisableBy { get; set; }

    [Column("Usr_DisableOn", TypeName = "datetime")]
    public DateTime? UsrDisableOn { get; set; }

    [Column("usr_EnableBy")]
    public int? UsrEnableBy { get; set; }

    [Column("usr_EnableOn", TypeName = "datetime")]
    public DateTime? UsrEnableOn { get; set; }

    [Column("usr_UnBlockLockBy")]
    public int? UsrUnBlockLockBy { get; set; }

    [Column("usr_UnBlockLockOn", TypeName = "datetime")]
    public DateTime? UsrUnBlockLockOn { get; set; }

    [Column("Usr_IPAddress")]
    [StringLength(500)]
    [Unicode(false)]
    public string? UsrIpaddress { get; set; }

    [Column("Usr_CompId")]
    public int? UsrCompId { get; set; }

    [Column("usr_othersQualification")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? UsrOthersQualification { get; set; }

    [Column("Usr_IsPasswordReset")]
    public int? UsrIsPasswordReset { get; set; }

    [Column("usr_Gender")]
    public int? UsrGender { get; set; }

    [Column("usr_MaritalStatus")]
    public int? UsrMaritalStatus { get; set; }

    [Column("usr_NoOfChildren")]
    public int? UsrNoOfChildren { get; set; }

    [Column("usr_Resume")]
    public int? UsrResume { get; set; }

    [Column("usr_IsSuperuser")]
    public int? UsrIsSuperuser { get; set; }

    [Column("usr_deptid")]
    public int? UsrDeptid { get; set; }

    [Column("USR_Levelcode")]
    public int? UsrLevelcode { get; set; }

    [Column("USR_MemberType")]
    public int? UsrMemberType { get; set; }

    [Column("Usr_IsLogin")]
    [StringLength(5)]
    [Unicode(false)]
    public string? UsrIsLogin { get; set; }

    [Column("Usr_Browser")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrBrowser { get; set; }

    [Column("Usr_DigitalOfficeModule")]
    public int? UsrDigitalOfficeModule { get; set; }

    [Column("Usr_DigitalOfficeRole")]
    public int? UsrDigitalOfficeRole { get; set; }

    [Column("Usr_MiddleName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrMiddleName { get; set; }

    [Column("Usr_LastName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrLastName { get; set; }

    [Column("Usr_PCAOB")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrPcaob { get; set; }

    [Column("Usr_OtherCertificate")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrOtherCertificate { get; set; }

    [Column("Usr_CustDesignation")]
    [StringLength(100)]
    [Unicode(false)]
    public string? UsrCustDesignation { get; set; }

    [Column("Usr_EductionDetail")]
    [StringLength(500)]
    [Unicode(false)]
    public string? UsrEductionDetail { get; set; }

    [Column("Usr_PCAOBRegNo")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrPcaobregNo { get; set; }

    [Column("Usr_PCAOBDateReg")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrPcaobdateReg { get; set; }

    [Column("Usr_PCAOBRenewalDate")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrPcaobrenewalDate { get; set; }

    [Column("Usr_ICWARegNo")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrIcwaregNo { get; set; }

    [Column("Usr_ICWADateReg")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrIcwadateReg { get; set; }

    [Column("Usr_ICWARenewalDate")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrIcwarenewalDate { get; set; }

    [Column("Usr_CMARegNo")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrCmaregNo { get; set; }

    [Column("Usr_CMADateReg")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrCmadateReg { get; set; }

    [Column("Usr_CMARenewalDate")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrCmarenewalDate { get; set; }

    [Column("Usr_OtherRegNo")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrOtherRegNo { get; set; }

    [Column("Usr_OtherDateReg")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrOtherDateReg { get; set; }

    [Column("Usr_OtherRenewalDate")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrOtherRenewalDate { get; set; }

    [Column("usr_AttachId")]
    public int? UsrAttachId { get; set; }

    [Column("Usr_CADateReg")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrCadateReg { get; set; }

    [Column("Usr_CARenewalDate")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrCarenewalDate { get; set; }

    [Column("Usr_CAUKMembershipNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrCaukmembershipNo { get; set; }

    [Column("Usr_CAUKDateReg")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrCaukdateReg { get; set; }

    [Column("Usr_CAUKRenewalDate")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrCaukrenewalDate { get; set; }

    [Column("Usr_ACCAMembershipNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrAccamembershipNo { get; set; }

    [Column("Usr_ACCADateReg")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrAccadateReg { get; set; }

    [Column("Usr_ACCARenewalDate")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrAccarenewalDate { get; set; }

    [Column("Usr_OtherCertificateDateReg")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrOtherCertificateDateReg { get; set; }

    [Column("Usr_OtherCertificateRenewalDate")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsrOtherCertificateRenewalDate { get; set; }

    [Column("Usr_CPAMembershipNo")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrCpamembershipNo { get; set; }

    [Column("Usr_CPADateReg")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrCpadateReg { get; set; }

    [Column("Usr_CPARenewalDate")]
    [StringLength(30)]
    [Unicode(false)]
    public string? UsrCparenewalDate { get; set; }

    [Column("Usr_Suggetions")]
    public int? UsrSuggetions { get; set; }
}
