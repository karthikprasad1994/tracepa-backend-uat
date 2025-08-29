using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class DashboardAndScheduleDto
    {
        public int? SrNo { get; set; }                     // SA_ID or row number
        public int? SA_YearID { get; set; }
        public string? FY { get; set; }                    // Fixed from int? to string? for values like '2024-2025'
        public string? AuditNo { get; set; }
        public int SA_AuditTypeID { get; set; }
        public int AuditID { get; set; }
        public int? CustID { get; set; }                   // Cust_Id
        public string? CustomerName { get; set; }          // Cust_Name
        public string? CustomerShortName { get; set; }     // Short version of Cust_Name
        public int? SA_IntervalId { get; set; }
        public int? CheckPointID { get; set; }             // CMM_ID / AuditTypeID
        public string? AuditType { get; set; }             // CMM_Desc
        public string? SA_EngagementPartnerID { get; set; }
        public string? SA_ReviewPartnerID { get; set; }
        public string? SA_PartnerID { get; set; }
        public string? SA_AdditionalSupportEmployeeID { get; set; }
        public string? SA_ScopeOfAudit { get; set; }
        public string? AuditDate { get; set; }

        public string? SA_RptRvDate { get; set; }

        public string? SA_RptFilDate { get; set; }

        public string? SA_MRSDate { get; set; }
        public string? SA_StartDate { get; set; }
        public string? SA_ExpCompDate { get; set; }
        public string? SA_AuditOpinionDate { get; set; }
        public string? SA_FilingDateSEC { get; set; }
        public string? SA_MRLDate { get; set; }
        public string? SA_FilingDatePCAOB { get; set; }
        public string? SA_BinderCompletedDate { get; set; }
        public int? SA_SignedBy { get; set; }
        public string? SA_UDIN { get; set; }
        public string? SA_UDINdate { get; set; }
        public int? StatusID { get; set; }
        public string? AuditStatus { get; set; }
        public string? Partner { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
    }
    public class QuarterDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }

    public class AuditTypeHeadingDto
    {
        public string? ACM_Heading { get; set; }
        public int ACM_ID { get; set; }
    }
    public class AuditTypeCheckListDto
    {
        public int SlNo { get; set; }
        public string? ACM_Heading { get; set; }
        public int ACM_ID { get; set; }
        public string? ACM_Checkpoint { get; set; }
    }
    public class AssignedCheckpointDto
    {
        // For the first query (AuditType_Checklist_Master)
        public int SlNo { get; set; }             // DENSE_RANK() result
        public string ACM_Heading { get; set; }   // Section heading
        public int ACM_ID { get; set; }           // Checkpoint ID
        public string ACM_Checkpoint { get; set; } // Checkpoint description

        // For the second query (StandardAudit_Checklist_Details)
        public int SACD_ID { get; set; }
        public string SACD_CheckpointId { get; set; }
        public string SACD_Heading { get; set; }
        public string Employee { get; set; }
        public int NoCheckpoints { get; set; }
        public int NoEmployee { get; set; }
        public decimal Working_Hours { get; set; }
        public string Timeline { get; set; }
        public string SAC_Mandatory { get; set; }
    }


    public class StandardAuditScheduleDto
    {
        public int SA_ID { get; set; } // Required for sp
        public string SA_AuditNo { get; set; }
        public int SA_CustID { get; set; }
        public int SA_YearID { get; set; }
        public int SA_AuditTypeID { get; set; }
        public string? SA_EngagementPartnerID { get; set; }
        public string? SA_ReviewPartnerID { get; set; }
        public string? SA_PartnerID { get; set; }
        public string? SA_AdditionalSupportEmployeeID { get; set; }
        public string? SA_ScopeOfAudit { get; set; }
        public int SA_Status { get; set; }
        public int SA_AttachID { get; set; }
        public DateTime? SA_StartDate { get; set; }
        public DateTime? SA_ExpCompDate { get; set; }
        public DateTime? SA_AuditOpinionDate { get; set; }
        public DateTime? SA_FilingDateSEC { get; set; }
        public DateTime? SA_MRLDate { get; set; }
        public DateTime? SA_FilingDatePCAOB { get; set; }
        public DateTime? SA_BinderCompletedDate { get; set; }

        public DateTime? SA_RptRvDate { get; set; }
        public DateTime? SA_RptFilDate { get; set; }
        public DateTime? SA_MRSDate { get; set; }
        public int SA_IntervalId { get; set; }
        public int SA_CrBy { get; set; }
        public int SA_UpdatedBy { get; set; }
        public string SA_IPAddress { get; set; }
        public int SA_CompID { get; set; }

        public int SA_AuditFrameworkId { get; set; }
        public int iUpdateOrSave { get; set; }  // Required for sp
        public int iOper { get; set; }          // Required for sp
        public List<QuarterAuditDto> Quarters { get; set; } = new List<QuarterAuditDto>();
    }


    public class QuarterAuditDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IntervalID { get; set; }
        public int SubIntervalID { get; set; }
    }
    public class StandardAuditChecklistDetailsDto
    {
        public int SACD_ID { get; set; }
        public int SACD_CustId { get; set; }
        public int SACD_AuditId { get; set; }
        public int SACD_AuditType { get; set; }
        public string? SACD_Heading { get; set; }
        public string? SACD_CheckpointId { get; set; }
        public int SACD_EmpId { get; set; }
        public int SACD_WorkType { get; set; }
        public decimal? SACD_HrPrDay { get; set; }
        public DateTime SACD_StartDate { get; set; }
        public DateTime SACD_EndDate { get; set; }
        public decimal? SACD_TotalHr { get; set; }
        public int SACD_CRBY { get; set; }
        public int SACD_UPDATEDBY { get; set; }
        public string? SACD_IPAddress { get; set; }
        public int SACD_CompId { get; set; }

        public int AuditStatusID { get; set; }
        public int CheckPointsAndTeamMemberPKID { get; set; }
        public string? SelectedCheckPointsPKID { get; set; }
        public List<ScheduleCheckpointListDto>? Checkpoints { get; set; }
    }

    public class ScheduleCheckpointListDto
    {
        public int SAC_ID { get; set; }
        public int SAC_SA_ID { get; set; }
        public int SAC_CheckPointID { get; set; }
        public int SAC_Mandatory { get; set; }
        public int SAC_Status { get; set; }
        public int SAC_AttachID { get; set; }
        public int SAC_CrBy { get; set; }
        public string? SAC_IPAddress { get; set; }
        public int SAC_CompID { get; set; }
    }

    public class StandardAuditScheduleDTO
    {
        public int SA_ID { get; set; } // Primary Key
        public string AuditNo { get; set; } // Audit Number
        public int CustID { get; set; } // Customer ID
        public int YearID { get; set; } // Year ID
        public int AuditTypeID { get; set; } // Audit Type ID
        public string EngagementPartnerIDs { get; set; } // Varchar(500)
        public string ReviewPartnerIDs { get; set; } // Varchar(500)
        public string PartnerIDs { get; set; } // Varchar(500)
        public string SupportEmployeeIDs { get; set; } // Varchar(500)
        public string ScopeOfAudit { get; set; } // Varchar(5000)
        public DateTime? SA_RptRvDate { get; set; } // Report Review Date
        public DateTime? SA_RptFilDate { get; set; } // Report Filing Date

        public int Status { get; set; } // SA_Status
        public int AttachID { get; set; } // SA_AttachID
        public DateTime? FromDate { get; set; } // SA_StartDate
        public DateTime? ToDate { get; set; } // SA_ExpCompDate
        public DateTime? ReportReviewDate { get; set; } // SA_AuditOpinionDate
        public DateTime? ReportFilingDate { get; set; } // SA_FilingDateSEC
        public DateTime? DateForMRS { get; set; } // SA_MRSDate

        public DateTime? SA_MRLDate { get; set; } // SA_MRLDate
        public DateTime? SA_AuditOpinionDate { get; set; } // SA_AuditOpinionDate
        public DateTime?  SA_FilingDateSEC { get; set; } // SA_AuditOpinionDate
        public DateTime? ReportFilingDatePCAOB { get; set; } // SA_FilingDatePCAOB
        public DateTime? BinderCompletedDate { get; set; } // SA_BinderCompletedDate
        public int IntervalId { get; set; }
        public int UserID { get; set; } // CrBy and UpdatedBy
        public string IPAddress { get; set; }
        public int CompID { get; set; } // Company ID

        public int SA_AuditFrameworkId { get; set; }

        public int CustRegAccessCodeId { get; set; }
        public string AccessCode { get; set; }
        public int SAI_ID { get; set; } // Primary Key
        public int SAI_SA_ID { get; set; } // Foreign Key to StandardAudit_Schedule
        public int SAI_IntervalID { get; set; }
        public int SAI_IntervalSubID { get; set; }
        public DateTime? SAI_StartDate { get; set; }
        public DateTime? SAI_EndDate { get; set; }
        public int SAI_CrBy { get; set; } // Created By
        public DateTime SAI_CrOn { get; set; } // Created On
        public string SAI_IPAddress { get; set; }
        public int SAI_CompID { get; set; }
        public int SACD_ID { get; set; } // Primary Key
        public int SACD_CustId { get; set; } // Customer ID
        public int SACD_AuditId { get; set; } // Audit ID
        public int SACD_AuditType { get; set; } // Audit Type
        public string SACD_Heading { get; set; } // Heading
        public int SACD_CheckpointId { get; set; } // Checkpoint ID
        public int SACD_EmpId { get; set; } // Employee ID
        public string SACD_WorkType { get; set; } // Work Type
        public decimal SACD_HrPrDay { get; set; } // Hours Per Day
        public DateTime? SACD_StartDate { get; set; }
        public DateTime? SACD_EndDate { get; set; }
        public decimal SACD_TotalHr { get; set; } // Total Hours
        public DateTime SACD_CRON { get; set; } // Created On
        public int SACD_CRBY { get; set; } // Created By
        public string SACD_IPAddress { get; set; }
        public int SACD_CompId { get; set; }
    }



    public class StandardAuditScheduleCheckpointDTO
    {
        public int SAC_ID { get; set; } // Primary Key
        public int SAC_SA_ID { get; set; } // Foreign Key to StandardAudit_Schedule
        public int SAC_CheckPointID { get; set; } // Checkpoint ID
        public bool SAC_Mandatory { get; set; } // Mandatory flag
        public int SAC_Status { get; set; } // Status
        public int SAC_AttachID { get; set; } // Attach ID
        public int SAC_CrBy { get; set; } // Created By
        public DateTime SAC_CrOn { get; set; } // Created On
        public string SAC_IPAddress { get; set; }
        public int SAC_CompID { get; set; }
    }

    public class CustomerRegistrationDTO
    {
        public int CustRegAccessCodeId { get; set; }
        public string AccessCode { get; set; }
    }

    public class AuditQuarterDTO
    {
        public int QuarterId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class AuditReviewDTO
    {
        public int SAR_AuditReviewID { get; set; }  // Should be set after insert if needed
        public int SAR_CustID { get; set; }
        public string SAR_ScopeOfReview { get; set; }

        // Add any additional fields used in your insert query:
        // public string ReviewerName { get; set; }
        // public DateTime ReviewDate { get; set; }
    }
    public class AuditScheduleCheckpoint
    {
        public int CheckpointID { get; set; }
        public int AuditScheduleID { get; set; }
        public int? AuditReviewID { get; set; }

        // Add any other fields from the table that you need, e.g.:
        public string? CheckpointName { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
    }
    public class AuditChecklist
    {
        public int ChecklistID { get; set; }
        public int CustID { get; set; }
        public int AuditScheduleID { get; set; }
        public int? AuditReviewID { get; set; }

        // Optional fields based on your DB schema (adjust as necessary)
        public string? ChecklistName { get; set; }
        public string? Remarks { get; set; }
        public bool? IsCompleted { get; set; }
    }
    public class AuditSchedule
    {
        public int SA_ID { get; set; }
        public string? SA_AuditNo { get; set; }
        public int SA_CustID { get; set; }
        public int SA_YearID { get; set; }
        public int SA_AuditTypeID { get; set; }
        public int SA_Status { get; set; }
        public DateTime SA_StartDate { get; set; }
        public DateTime SA_ExpCompDate { get; set; }
        public DateTime RptRvDate { get; set; }

        // Optional fields based on your DB schema (adjust as necessary)
        public string? AdditionalField1 { get; set; }  // Add any other properties if required
        public string? AdditionalField2 { get; set; }
    }
    public class AuditChecklistDto
    {
        public string ACM_Heading { get; set; }
        public int ACM_ID { get; set; }
        public string ACM_Checkpoint { get; set; }
        public string SAC_Mandatory { get; set; }  // Yes or No
    }
    public class DeleteCheckpointDto
    {
        public int AuditId { get; set; }
        public int CustomerId { get; set; }
        public int PkId { get; set; }
        public string CheckpointIds { get; set; } // optional if used for logging or extension
        public int CompId { get; set; } // optional if used in logic
    }
    public class AuditCustomerDetailsDto
    {
        public int CUST_ID { get; set; }
        public string CUST_NAME { get; set; }
        public string CUST_CODE { get; set; }
        public string CUST_WEBSITE { get; set; }
        public string CUST_EMAIL { get; set; }
        public string CUST_GROUPNAME { get; set; }
        public int CUST_GROUPINDIVIDUAL { get; set; }
        public int CUST_ORGTYPEID { get; set; }
        public int CUST_INDTYPEID { get; set; }
        public int CUST_MGMTTYPEID { get; set; }
        public DateTime? CUST_CommitmentDate { get; set; }
        public string CUSt_BranchId { get; set; }
        public string CUST_COMM_ADDRESS { get; set; }
        public string CUST_COMM_CITY { get; set; }
        public string CUST_COMM_PIN { get; set; }
        public string CUST_COMM_STATE { get; set; }
        public string CUST_COMM_COUNTRY { get; set; }
        public string CUST_COMM_FAX { get; set; }
        public string CUST_COMM_TEL { get; set; }
        public string CUST_COMM_Email { get; set; }
        public string CUST_ADDRESS { get; set; }
        public string CUST_CITY { get; set; }
        public string CUST_PIN { get; set; }
        public string CUST_STATE { get; set; }
        public string CUST_COUNTRY { get; set; }
        public string CUST_FAX { get; set; }
        public string CUST_TELPHONE { get; set; }
        public string CUST_ConEmailID { get; set; }
        public string CUST_LOCATIONID { get; set; }
        public string CUST_TASKS { get; set; }
        public int CUST_ORGID { get; set; }
        public int CUST_CRBY { get; set; }
        public int CUST_UpdatedBy { get; set; }
        public string CUST_BOARDOFDIRECTORS { get; set; }
        public int CUST_DEPMETHOD { get; set; }
        public string CUST_IPAddress { get; set; }
        public int CUST_CompID { get; set; }
        public int CUST_Amount_Type { get; set; }
        public int CUST_RoundOff { get; set; }
        public int Cust_FY { get; set; }
        public int Cust_DurtnId { get; set; }
    }
    public class GeneralMasterDto
    {
        public int Cmm_ID { get; set; }
        public string Cmm_Desc { get; set; }
    }
    public class AuditTypeCustomerDto
    {
        public int PKID { get; set; }
        public string? Name { get; set; }
    }
    public class AuditTypeRequestDto
    {
        public string Type { get; set; }
        public int CompanyId { get; set; }
        public int FinancialYearId { get; set; }
        public int CustomerId { get; set; }
        public int AuditTypeId { get; set; }
    }
    public class CustomerDetailsDto
    {
        public string FinancialYear { get; set; }
        public string CIKRegistrationNo { get; set; }
        public string Address { get; set; }
    }
    public class AuditStatusDto
    {
        public string SA_Status { get; set; }
    }

    public class ScheduleQuarterCheckDto
    {
        public int CompId { get; set; }               // Company ID
        public string AuditNo { get; set; }         // Audit number
        public int QuarterID { get; set; }           // Quarter ID
    }
    public class EmployeeDto
    {
        public int UserID { get; set; }
        public int UsrNode { get; set; }
        public string UsrCode { get; set; }
        public string UsrFullName { get; set; }
        public string UsrLoginName { get; set; }
        public string UsrPassword { get; set; }
        public string UsrEmail { get; set; }
        public int UsrSentMail { get; set; }
        public int UsrSuggetions { get; set; }
        public int UsrPartner { get; set; }
        public int UsrLevelGrp { get; set; }
        public string UsrDutyStatus { get; set; }
        public string UsrPhoneNo { get; set; }
        public string UsrMobileNo { get; set; }
        public string UsrOfficePhone { get; set; }
        public string UsrOffPhExtn { get; set; }
        public int UsrDesignation { get; set; }
        public int UsrCompanyID { get; set; }
        public int UsrOrgID { get; set; }
        public int UsrGrpOrUserLvlPerm { get; set; }
        public int UsrRole { get; set; }
        public int UsrMasterModule { get; set; }
        public int UsrAuditModule { get; set; }
        public int UsrRiskModule { get; set; }
        public int UsrComplianceModule { get; set; }
        public int UsrBCMmodule { get; set; }
        public int UsrDigitalOfficeModule { get; set; }
        public int UsrMasterRole { get; set; }
        public int UsrAuditRole { get; set; }
        public int UsrRiskRole { get; set; }
        public int UsrComplianceRole { get; set; }
        public int UsrBCMRole { get; set; }
        public int UsrDigitalOfficeRole { get; set; }
        public int UsrCreatedBy { get; set; }
        public string UsrFlag { get; set; }
        public string UsrStatus { get; set; }
        public string UsrIPAddress { get; set; }
        public int UsrCompID { get; set; }
        public string UsrType { get; set; }
        public int IsSuperuser { get; set; }
        public int DeptID { get; set; }
    }
    public class AuditReportRequestDto
    {
        public int AuditId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string AuditNo { get; set; }
        public string ScopeOfAudit { get; set; }
        public List<int> EngagementPartnerIds { get; set; } = new();

        public List<int> EngagementQualityReviewer { get; set; } = new();
        public List<int> Partner { get; set; } = new();
        public List<int> AuditAssistants { get; set; } = new();
        public string AuditOpinionDate { get; set; }
        public string ManagementRepLetterDate { get; set; }
        public string FilingDatewithSECbyIssuer { get; set; }
        public string FilingwithPCAOB { get; set; }
        public string BinderCompletedDate { get; set; }
        public string AccessCode { get; set; }
        public int AccessCodeID { get; set; }
        public string UserID { get; set; }
        public string IPAddress { get; set; }
        public int YearID { get; set; }
        public string YearName { get; set; }
        public string Format { get; set; } = "dd-MMM-yyyy";
    }
    public class DiscoveryRequestDto
    {
        public string Question { get; set; }
    }
    public class DiscoveryResponseDto
    {
        public string Answer { get; set; }
        public string RedirectLinkText { get; set; }
    }
    public class LoeAuditFrameworkRequest
    {
        public int CustomerId { get; set; }
        public int YearId { get; set; }
        public int ServiceTypeId { get; set; }
    }

    public class LoeAuditFrameworkResponse
    {
        public int LoeAuditFrameworkId { get; set; }
    }

    public class UpdateAttachmentStatusDto
    {
        public int DocId { get; set; }
        public int CompId { get; set; }
        public string? Status { get; set; } 
    }

}
