namespace TracePca.Dto.SuperMaster
{
    public class ExcelInformationDto
    {
        //UploadEmployeeMasters
        public class UploadEmployeeMasterDto
        {
            public string EmpCode { get; set; }         
            public string EmployeeName { get; set; }      
            public string LoginName { get; set; }       
            public string Email { get; set; }
            public string OfficePhoneNo { get; set; }
            public string Designation { get; set; }      
            public string Role { get; set; }              
            public string Partner { get; set; }
            // 🔹 Optional Fields (present in Excel)
            public int? EmpId { get; set; }
            public int? EmpNode { get; set; }
            public int? LevelGrp { get; set; }
            public int? EmpCategory { get; set; }
            public int? CompanyId { get; set; }
            public string Password { get; set; }
            public string DelFlag { get; set; }
            public string Status { get; set; }
            public string IPAddress { get; set; }
            public int? CompId { get; set; }
            public string Type { get; set; }
            public int? IsSuperuser { get; set; }
            public int? DeptID { get; set; }
            public int? MemberType { get; set; }
            public int? Levelcode { get; set; }
            public int? Suggestions { get; set; }
            // 🔹 Extra fields required by SP but not in your DTO yet
            public string DutyStatus { get; set; }      
            public string PhoneNo { get; set; }         
            public string MobileNo { get; set; }
            public string OfficePhoneExtn { get; set; }
            public int? OrgnId { get; set; }
            public int? GrpOrUserLvlPerm { get; set; }
            // 🔹 Module flags
            public int? MasterModule { get; set; }
            public int? AuditModule { get; set; }
            public int? RiskModule { get; set; }
            public int? ComplianceModule { get; set; }
            public int? BCMModule { get; set; }
            public int? DigitalOfficeModule { get; set; }
            // 🔹 Role flags
            public int? MasterRole { get; set; }
            public int? AuditRole { get; set; }
            public int? RiskRole { get; set; }
            public int? ComplianceRole { get; set; }
            public int? BCMRole { get; set; }
            public int? DigitalOfficeRole { get; set; }
            // 🔹 Metadata
            public int? CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            //Validation
            public string ErrorMessage { get; set; }
        }

        //SaveEmployeeMaster
        public class SuperMasterSaveEmployeeMasterDto
        {
            public int iUserID { get; set; }
            public int iUsrNode { get; set; }
            public string? sUsrCode { get; set; }
            public string? sUsrFullName { get; set; }
            public string? sUsrLoginName { get; set; }
            public string? sUsrPassword { get; set; }
            public string? sUsrEmail { get; set; }
            public int iUsrSentMail { get; set; }
            public int iUsrSuggetions { get; set; }
            public int iUsrPartner { get; set; }
            public int iUsrLevelGrp { get; set; }
            public string? sUsrDutyStatus { get; set; }
            public string? sUsrPhoneNo { get; set; }
            public string? sUsrMobileNo { get; set; }
            public string? sUsrOfficePhone { get; set; }
            public string? sUsrOffPhExtn { get; set; }
            public int iUsrDesignation { get; set; } 
            public string? sUsrDesignationName { get; set; } 
            public int iUsrCompanyID { get; set; }
            public int iUsrOrgID { get; set; }
            public int iUsrGrpOrUserLvlPerm { get; set; }
            public int iUsrRole { get; set; }
            public int iUsrMasterModule { get; set; }
            public int iUsrAuditModule { get; set; }
            public int iUsrRiskModule { get; set; }
            public int iUsrComplianceModule { get; set; }
            public int iUsrBCMmodule { get; set; }
            public int iUsrDigitalOfficeModule { get; set; }
            public int iUsrMasterRole { get; set; }
            public int iUsrAuditRole { get; set; }
            public int iUsrRiskRole { get; set; }
            public int iUsrComplianceRole { get; set; }
            public int iUsrBCMRole { get; set; }
            public int iUsrDigitalOfficeRole { get; set; }
            public int iUsrCreatedBy { get; set; }
            public string? sUsrFlag { get; set; }
            public string? sUsrStatus { get; set; }
            public string? Usr_IPAdress { get; set; }
            public int iUsrCompID { get; set; }
            public string? sUsrType { get; set; }
            public int iusr_IsSuperuser { get; set; }
            public int iUSR_DeptID { get; set; }
            public int iUSR_MemberType { get; set; }
            public int iUSR_Levelcode { get; set; }
        }


        //UploadClientDetails
        public class UploadClientDetailsDto
        {
            public int CUST_ID { get; set; }
            public string CUST_NAME { get; set; }
            public string CUST_CODE { get; set; }
            public string CUST_WEBSITE { get; set; }
            public string CUST_EMAIL { get; set; }
            public string OrgTypeName { get; set; }
            public int CUST_ORGTYPEID { get; set; }
            public string LocationName { get; set; }
            public string Address { get; set; }
            public string ContactPerson { get; set; }
            public string Mobile { get; set; }
            public string Landline { get; set; }
            public string Email { get; set; }
            public string CIN { get; set; }
            public string TAN { get; set; }
            public string GST { get; set; }
            public int CUST_CRBY { get; set; }
            public int CUST_UpdatedBy { get; set; }
            public int CUST_CompID { get; set; }
        }


        //SaveClientDetails
        public class SuperMasterSaveCustomerDto
        {
            //CONTENT_MANAGEMENT_MASTER related
            public int Cmm_ID { get; set; }            
            public string Cmm_Code { get; set; }       
            public string Cmm_Category { get; set; }  
            public string OrgTypeName { get; set; }   
            public string DelFlg { get; set; }        

            //CUSTOMER MASTER FIELDS (SAD_CUSTOMER_MASTER) 
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
            public decimal CUST_RoundOff { get; set; }
            public int Cust_DurtnId { get; set; }
            public int Cust_FY { get; set; }

            //LOCATION FIELDS (SAD_CUST_LOCATION)
            public int Mas_Id { get; set; }
            public string Mas_code { get; set; }
            public string LocationName { get; set; }
            public string DelFlag { get; set; }
            public string Address { get; set; }
            public string ContactPerson { get; set; }
            public string Mobile { get; set; }
            public string Landline { get; set; }
            public string Email { get; set; }
            public string Designation { get; set; }

            //STATUTORY REFS (SAD_CUST_Accounting_Template) 
            public string CIN { get; set; }
            public string TAN { get; set; }
            public string GST { get; set; }
        }

        //UploadClientUser
        public class UploadClientUserDto
        {
            public string EmpCode { get; set; }         
            public string EmployeeName { get; set; }     
            public string LoginName { get; set; }       
            public string Email { get; set; }
            public string PhoneNo { get; set; }
            public int? Designation { get; set; }     
            public int? Role { get; set; }            
            public string Partner { get; set; }
            // 🔹 Optional Fields (present in Excel)
            public int? EmpId { get; set; }
            public int? EmpNode { get; set; }
            public int? LevelGrp { get; set; }
            public int? EmpCategory { get; set; }
            public string CompanyId { get; set; }
            public string Password { get; set; }
            public string DelFlag { get; set; }
            public string Status { get; set; }
            public string IPAddress { get; set; }
            public int? CompId { get; set; }
            public string Type { get; set; }
            public int? IsSuperuser { get; set; }
            public int? DeptID { get; set; }
            public int? MemberType { get; set; }
            public int? Levelcode { get; set; }
            public int? Suggestions { get; set; }

            // 🔹 Extra fields required by SP but not in your DTO yet
            public string DutyStatus { get; set; }
            public string OfficePhoneNo { get; set; }
            public string MobileNo { get; set; }
            public string OfficePhoneExtn { get; set; }
            public int? OrgnId { get; set; }
            public int? GrpOrUserLvlPerm { get; set; }
            public int? MasterModule { get; set; }
            public int? AuditModule { get; set; }
            public int? RiskModule { get; set; }
            public int? ComplianceModule { get; set; }
            public int? BCMModule { get; set; }
            public int? DigitalOfficeModule { get; set; }
            public int? MasterRole { get; set; }
            public int? AuditRole { get; set; }
            public int? RiskRole { get; set; }
            public int? ComplianceRole { get; set; }
            public int? BCMRole { get; set; }
            public int? DigitalOfficeRole { get; set; }
            // 🔹 Metadata
            public int? CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            //Validation
            public string ErrorMessage { get; set; }

        }

        //SaveCleintUser
        public class SaveClientUserDto
        {
            //Vendor Related
            public string VendorName { get; set; }       
            public string EmailId { get; set; }  
            //Designation Related 
            public string DesignationName { get; set; }   
            //User Master Fields
            public int iUserID { get; set; }
            public int iUsrNode { get; set; }
            public string sUsrCode { get; set; }
            public string sUsrFullName { get; set; }
            public string sUsrLoginName { get; set; }
            public string sUsrPassword { get; set; }
            public string sUsrEmail { get; set; }
            public int iUsrSentMail { get; set; }
            public int iUsrSuggetions { get; set; }
            public int iUsrPartner { get; set; }
            public int iUsrLevelGrp { get; set; }
            public string sUsrDutyStatus { get; set; }
            public string sUsrPhoneNo { get; set; }
            public string sUsrMobileNo { get; set; }
            public string sUsrOfficePhone { get; set; }
            public string sUsrOffPhExtn { get; set; }
            public int iUsrDesignation { get; set; }  
            public int iUsrCompanyID { get; set; }   
            public int iUsrOrgID { get; set; }
            public int iUsrGrpOrUserLvlPerm { get; set; }
            public int iUsrRole { get; set; }
            public int iUsrMasterModule { get; set; }
            public int iUsrAuditModule { get; set; }
            public int iUsrRiskModule { get; set; }
            public int iUsrComplianceModule { get; set; }
            public int iUsrBCMmodule { get; set; }
            public int iUsrDigitalOfficeModule { get; set; }
            public int iUsrMasterRole { get; set; }
            public int iUsrAuditRole { get; set; }
            public int iUsrRiskRole { get; set; }
            public int iUsrComplianceRole { get; set; }
            public int iUsrBCMRole { get; set; }
            public int iUsrDigitalOfficeRole { get; set; }
            public int iUsrCreatedBy { get; set; }
            public string sUsrFlag { get; set; }
            public string sUsrStatus { get; set; }
            public string Usr_IPAdress { get; set; }
            public int iUsrCompID { get; set; }
            public string sUsrType { get; set; }
            public int iusr_IsSuperuser { get; set; }
            public int iUSR_DeptID { get; set; }
            public int iUSR_MemberType { get; set; }
            public int iUSR_Levelcode { get; set; }
        }
    }
}

