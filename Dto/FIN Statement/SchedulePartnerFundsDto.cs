namespace TracePca.Dto.FIN_Statement
{
    public class SchedulePartnerFundsDto
    {
        //GetAllPartnershipFirms
        public class PartnershipFirmRequestDto
        {
                     public int iFinancialYearID { get; set; }
            public int iCustomerID { get; set; }
            public int sFY1 { get; set; }
            public string sFY2 { get; set; }
            public string sIsReport { get; set; }
        }
        public class PartnershipFirmRowDto
        {
            public string SlNo { get; set; }
            public string Particulars { get; set; }
            public string FYCData { get; set; }
            public string FYPData { get; set; }
        }

        //GetPartnernName
        public class PartnerDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        //SavePartnershipFirms
        public class SavePartnershipFirmDto
        {
            public int iAPF_ID { get; set; }
            public int iAPF_YearID { get; set; }
            public int iAPF_Cust_ID { get; set; }
            public int iAPF_Branch_ID { get; set; }
            public int iAPF_Partner_ID { get; set; }
            public decimal dAPF_OpeningBalance { get; set; }
            public decimal dAPF_UnsecuredLoanTreatedAsCapital { get; set; }
            public decimal dAPF_InterestOnCapital { get; set; }
            public decimal dAPF_PartnersSalary { get; set; }
            public decimal dAPF_ShareOfprofit { get; set; }
            public decimal dAPF_TransferToFixedCapital { get; set; }
            public decimal dAPF_Drawings { get; set; }
            public decimal dAPF_AddOthers { get; set; }
            public decimal dAPF_LessOthers { get; set; }
            public string? sAPF_CapitalAmount { get; set; }   // varchar in DB
            public int iAPF_CrBy { get; set; }
            public int iAPF_UpdateBy { get; set; }
            public string? sAPF_IPAddress { get; set; }
            public int iAPF_CompID { get; set; }
        }

        //UpdatePartnershipFirms
        public class UpdatePartnershipFirmDto
        {
            public int APF_ID { get; set; }
            public int APF_YearID { get; set; }
            public int APF_Cust_ID { get; set; }
            public int APF_Branch_ID { get; set; }
            public int APF_Partner_ID { get; set; }
            public decimal APF_OpeningBalance { get; set; }
            public decimal APF_UnsecuredLoanTreatedAsCapital { get; set; }
            public decimal APF_InterestOnCapital { get; set; }
            public decimal APF_PartnersSalary { get; set; }
            public decimal APF_ShareOfProfit { get; set; }
            public decimal APF_TransferToFixedCapital { get; set; }
            public decimal APF_Drawings { get; set; }
            public decimal APF_AddOthers { get; set; }
            public decimal APF_LessOthers { get; set; }
            public string? APF_CapitalAmount { get; set; }
            public int APF_CrBy { get; set; }
            public int APF_UpdateBy { get; set; }
            public string? APF_IPAddress { get; set; }
            public int APF_CompID { get; set; }
        }


        //GetSelectedPartnershipFirms
        public class SelectedPartnershipFirmRowDto
        {
            public int APF_ID { get; set; }
            public int APF_YearID { get; set; }
            public int APF_Cust_ID { get; set; }
            public int APF_Branch_ID { get; set; }
            public int APF_Partner_ID { get; set; }
            public decimal APF_OpeningBalance { get; set; }
            public decimal APF_UnsecuredLoanTreatedAsCapital { get; set; }
            public decimal APF_InterestOnCapital { get; set; }
            public decimal APF_PartnersSalary { get; set; }
            public decimal APF_ShareOfProfit { get; set; }
            public decimal APF_TransferToFixedCapital { get; set; }
            public decimal APF_Drawings { get; set; }
            public decimal APF_AddOthers { get; set; }
            public decimal APF_LessOthers { get; set; }
            public string? APF_CapitalAmount { get; set; }
            public int APF_CrBy { get; set; }
            public DateTime? APF_CrOn { get; set; }
            public int APF_UpdateBy { get; set; }
            public DateTime? APF_UpdateOn { get; set; }
            public string? APF_IPAddress { get; set; }
            public int APF_CompID { get; set; }
        }

        //UpdateAndCalculate
        public class PartnershipFirmCalculationDto
        {
            public int PartnershipFirmId { get; set; }
            public int CompId { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal UnsecuredLoanTreatedAsCapital { get; set; }
            public decimal InterestOnCapital { get; set; }
            public decimal PartnersSalary { get; set; }
            public decimal ShareOfProfit { get; set; }
            public decimal AddOthers { get; set; }
        }

        //GetCustomerPartnerDetails
        public class PartnerDetailsDto
        {
            public int PartnerPkID { get; set; }
            public string Name { get; set; } = string.Empty;
            public DateTime? DOJ { get; set; }
            public string PAN { get; set; } = string.Empty;
            public decimal ShareOfProfit { get; set; }
            public decimal CapitalAmount { get; set; }
            public string Status { get; set; }
        }

        //SaveCustomerStatutoryPartner
        public class SaveCustomerStatutoryPartnerDto
        {
            public int SSP_Id { get; set; }                 // 0 → Save, >0 → Update
            public int SSP_CustID { get; set; }
            public string SSP_PartnerName { get; set; }
            public DateTime SSP_DOJ { get; set; }
            public string SSP_PAN { get; set; }
            public decimal SSP_ShareOfProfit { get; set; }
            public decimal SSP_CapitalAmount { get; set; }
            public DateTime SSP_CRON { get; set; }
            public int SSP_CRBY { get; set; }
            public DateTime SSP_UpdatedOn { get; set; }
            public int SSP_UpdatedBy { get; set; }
            public string SSP_DelFlag { get; set; }
            public string SSP_STATUS { get; set; }
            public string SSP_IPAddress { get; set; }
            public int SSP_CompID { get; set; }
        }
    }
}
