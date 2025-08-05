namespace TracePca.Dto.SuperMaster
{
    public class ExcelInformationDto
    {
        //ValidateClientDetails
        public class SuperMasterValidateClientDetailsResult
        {

            public int RowNumber { get; set; }
            public string? CustomerName { get; set; }
            public string? Email { get; set; }
            public List<string> MissingFields { get; set; } = new();
            public bool IsDuplicate { get; set; }
            public string? GeneratedCustomerId { get; set; }
            public Dictionary<string, string> Data { get; set; } = new();
            public bool IsValid => !IsDuplicate && MissingFields.Count == 0;
        }

        //SaveEmployeeMaster
        public class SuperMasterSaveEmployeeMasterDto
        {
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

        //SaveClientDetails
        public class SuperMasterSaveClientDetailsDto
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
            public DateTime CUST_CommitmentDate { get; set; }
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
            public decimal Cust_DurtnId { get; set; }
            public decimal Cust_FY { get; set; }
        }

        //SaveCleintUser
        public class SuperMasterSaveClientUserDto
        {
            public int CDET_ID { get; set; }
            public int CDET_CUSTID { get; set; }
            public string? CDET_STANDINGININDUSTRY { get; set; }
            public string? CDET_PUBLICPERCEPTION { get; set; }
            public string? CDET_GOVTPERCEPTION { get; set; }
            public string? CDET_LITIGATIONISSUES { get; set; }
            public string? CDET_PRODUCTSMANUFACTURED { get; set; }
            public string? CDET_SERVICESOFFERED { get; set; }
            public string? CDET_TURNOVER { get; set; }
            public string? CDET_PROFITABILITY { get; set; }
            public string? CDET_FOREIGNCOLLABORATIONS { get; set; }
            public string? CDET_EMPLOYEESTRENGTH { get; set; }
            public string? CDET_PROFESSIONALSERVICES { get; set; }
            public string? CDET_GATHEREDBYAUDITFIRM { get; set; }
            public string? CDET_LEGALADVISORS { get; set; }
            public string? CDET_AUDITINCHARGE { get; set; }
            public string? CDET_FileNo { get; set; }
            public int CDET_CRBY { get; set; }
            public int CDET_UpdatedBy { get; set; }
            public string? CDET_STATUS { get; set; }
            public string? CDET_IPAddress { get; set; }
            public int CDET_CompID { get; set; }
        }

    }
}

