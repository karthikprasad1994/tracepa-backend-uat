using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TracePca.Models;

public partial class SadUserDetail
{
    [Key]
    public int UsrId { get; set; }

    public int? UsrNode { get; set; }

    public string? UsrCode { get; set; }

    public string? UsrFullName { get; set; }

    public string? UsrLoginName { get; set; }

    public string? UsrPassWord { get; set; }

    public string? UsrEmail { get; set; }

    public DateTime? UsrDob { get; set; }

    public DateTime? UsrDoj { get; set; }

    public short? UsrLevelGrp { get; set; }

    public string? UsrDutyStatus { get; set; }

    public string? UsrHusOrFathName { get; set; }

    public string? UsrPhoneNo { get; set; }

    public string? UsrMobileNo { get; set; }

    public string? UsrOfficePhone { get; set; }

    public string? UsrOffPhExtn { get; set; }

    public byte? UsrCurWrkAddId { get; set; }

    public short? UsrPermAddId { get; set; }

    public short? UsrResAddId { get; set; }

    public short? UsrOfficialAddId { get; set; }

    public int? UsrPhoto { get; set; }

    public int? UsrSignature { get; set; }

    public short? UsrDesignation { get; set; }

    public short? UsrUserTypeId { get; set; }

    public short? UsrCompanyId { get; set; }

    public short? UsrOrgnId { get; set; }

    public string? UsrRefStaffNo { get; set; }

    public string? UsrKinName { get; set; }

    public short? UsrKinAddId { get; set; }

    public string? UsrBloodGroup { get; set; }

    public DateTime? UsrExpDate { get; set; }

    public string? UsrDelFlag { get; set; }

    public int? UsrAppBy { get; set; }

    public DateTime? UsrAppOn { get; set; }

    public int? UsrCreatedBy { get; set; }

    public DateTime? UsrCreatedOn { get; set; }

    public string? UsrStatus { get; set; }

    public string? UsrType { get; set; }

    public byte? UsrGrpOrUserLvlPerm { get; set; }

    public short? UsrNoOfUnSucsfAtteptts { get; set; }

    public string? UsrQue { get; set; }

    public string? UsrAns { get; set; }

    public int? UsrCategory { get; set; }

    public int? UsrPartner { get; set; }

    public int? UsrNoOfLogin { get; set; }

    public DateTime? UsrLastLoginDate { get; set; }

    public string? UsrReasonPwdBlock { get; set; }

    public int? UsrRole { get; set; }

    public int? UsrMasterModule { get; set; }

    public int? UsrMasterRole { get; set; }

    public int? UsrAuditModule { get; set; }

    public int? UsrAuditRole { get; set; }

    public int? UsrRiskModule { get; set; }

    public int? UsrRiskRole { get; set; }

    public int? UsrComplianceModule { get; set; }

    public int? UsrComplianceRole { get; set; }

    public int? UsrBcmmodule { get; set; }

    public int? UsrBcmrole { get; set; }

    public string? UsrSkillSet { get; set; }

    public string? UsrExperience { get; set; }

    public string? UsrQualification { get; set; }

    public int? UsrUpdatedBy { get; set; }

    public DateTime? UsrUpdatedOn { get; set; }

    public int? UsrDisableBy { get; set; }

    public DateTime? UsrDisableOn { get; set; }

    public int? UsrEnableBy { get; set; }

    public DateTime? UsrEnableOn { get; set; }

    public int? UsrUnBlockLockBy { get; set; }

    public DateTime? UsrUnBlockLockOn { get; set; }

    public string? UsrIpaddress { get; set; }

    public int? UsrCompId { get; set; }

    public string? UsrOthersQualification { get; set; }

    public int? UsrIsPasswordReset { get; set; }

    public int? UsrGender { get; set; }

    public int? UsrMaritalStatus { get; set; }

    public int? UsrNoOfChildren { get; set; }

    public int? UsrResume { get; set; }

    public int? UsrIsSuperuser { get; set; }

    public int? UsrDeptid { get; set; }

    public int? UsrLevelcode { get; set; }

    public int? UsrMemberType { get; set; }

    public string? UsrIsLogin { get; set; }

    public string? UsrBrowser { get; set; }

    public int? UsrDigitalOfficeModule { get; set; }

    public int? UsrDigitalOfficeRole { get; set; }

    public string? UsrMiddleName { get; set; }

    public string? UsrLastName { get; set; }

    public string? UsrPcaob { get; set; }

    public string? UsrOtherCertificate { get; set; }

    public string? UsrCustDesignation { get; set; }

    public string? UsrEductionDetail { get; set; }

    public int? UsrAttachId { get; set; }

    public string? UsrPcaobregNo { get; set; }

    public string? UsrPcaobdateReg { get; set; }

    public string? UsrPcaobrenewalDate { get; set; }

    public string? UsrIcwaregNo { get; set; }

    public string? UsrIcwadateReg { get; set; }

    public string? UsrIcwarenewalDate { get; set; }

    public string? UsrCmaregNo { get; set; }

    public string? UsrCmadateReg { get; set; }

    public string? UsrCmarenewalDate { get; set; }

    public string? UsrOtherRegNo { get; set; }

    public string? UsrOtherDateReg { get; set; }

    public string? UsrOtherRenewalDate { get; set; }

    public string? UsrCadateReg { get; set; }

    public string? UsrCarenewalDate { get; set; }

    public string? UsrCaukmembershipNo { get; set; }

    public string? UsrCaukdateReg { get; set; }

    public string? UsrCaukrenewalDate { get; set; }

    public string? UsrAccamembershipNo { get; set; }

    public string? UsrAccadateReg { get; set; }

    public string? UsrAccarenewalDate { get; set; }

    public string? UsrOtherCertificateDateReg { get; set; }

    public string? UsrOtherCertificateRenewalDate { get; set; }

    public string? UsrCpamembershipNo { get; set; }

    public string? UsrCpadateReg { get; set; }

    public string? UsrCparenewalDate { get; set; }

    public int? UsrSuggetions { get; set; }
}
