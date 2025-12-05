using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleReportDto
    {

        //GetCompanyName
        public class CompanyDetailsDto
        {
            public int Company_ID { get; set; }
            public string Company_Name { get; set; }
        }

        //GetPartners
        public class PartnersDto
        {
            public int usr_Id { get; set; }
            public string Fullname { get; set; }
            public string usr_PhoneNo { get; set; }   
            public string org_name { get; set; }       
        }

        //GetSubHeading
        public class SubHeadingDto
        {
            public int SubheadingID { get; set; }
            public string SubheadingName { get; set; }
        }

        //GetItem
        public class ItemDto
        {
            public int ItemsId { get; set; }
            public string ItemsName { get; set; }
        }

        //GetSummaryReportForPandL
        public class SummaryReportPnL
        {
            public int YearID { get; set; }
            public int CustID { get; set; }
            public string BranchId { get; set; }
        }
        public class SummaryReportPnLRow
        {
            public string SrNo { get; set; }
            public string Name { get; set; }
            public string HeaderSLNo { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
            public string status { get; set; }
            public decimal? HeadingId { get; set; }
            public decimal? SubHeadingID { get; set; }
            public decimal? ItemID { get; set; }
            public decimal? subItemID { get; set; }
            public decimal? CrTotal { get; set; }
            public decimal? DbTotal { get; set; }
            public decimal? CrTotalPrev { get; set; }
            public decimal? DbTotalPrev { get; set; }
            public decimal? PrevCrTotal { get; set; }
            public decimal? PrevDbTotal { get; set; }
        }

        //GetSummaryReportForBalanceSheet
        public class SummaryReportBalanceSheet
        {
            public int YearID { get; set; }
            public int CustID { get; set; }
            public string BranchId { get; set; }
        }
        public class SummaryReportBalanceSheetRow
        {
            public string SrNo { get; set; }
            public string Name { get; set; }
            public string HeaderSLNo { get; set; }
            public string YearId { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
            public string status { get; set; }
        }

        //GetDetaledReportPandL
        public class DetailedReportPandL
        {
            // Request Parameters
            public int YearID { get; set; }
            public int CustID { get; set; }
            public string BranchId { get; set; }
        }
        public class DetailedReportPandLRow
        {
            public string SrNo { get; set; }
            public string Status { get; set; }
            public string Name { get; set; }
            public string HeadingName { get; set; }
            public string SubHeadingName { get; set; }
            public string ItemName { get; set; }
            public string SubItemName { get; set; }
            public string HeaderSLNo { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
            public decimal? CrTotal { get; set; }
            public decimal? DbTotal { get; set; }
            public decimal? CrTotalPrev { get; set; }
            public decimal? DbTotalPrev { get; set; }
            public decimal? HeadingId { get; set; }
            public decimal? SubHeadingID { get; set; }
            public decimal? ItemID { get; set; }
            public decimal? subItemID { get; set; }
            public decimal? CrTotal1 { get; set; }
            public decimal? DbTotal1 { get; set; }
            public decimal? PrevCrTotal { get; set; }
            public decimal? PrevDbTotal { get; set; }
        }

        //GetDetaledReportBalanceSheet
        public class DetailedReportBalanceSheet
        {
            // Request Parameters
            public int YearID { get; set; }
            public int CustID { get; set; }
            public string BranchId { get; set; }
        }
        public class DetailedReportBalanceSheetRow
        {
            public string SrNo { get; set; }
            public string Status { get; set; }
            public string Name { get; set; }
            public string HeadingName { get; set; }
            public string SubHeadingName { get; set; }
            public string ItemName { get; set; }
            public string SubItemName { get; set; }
            public string HeaderSLNo { get; set; }
            public string PrevYearTotal { get; set; }
            public string Notes { get; set; }
            public decimal? CrTotal { get; set; }
            public decimal? DbTotal { get; set; }
            public decimal? CrTotalPrev { get; set; }
            public decimal? DbTotalPrev { get; set; }
            public decimal? HeadingId { get; set; }
            public decimal? SubHeadingID { get; set; }
            public decimal? ItemID { get; set; }
            public decimal? subItemID { get; set; }
            public decimal? CrTotal1 { get; set; }
            public decimal? DbTotal1 { get; set; }
            public decimal? PrevCrTotal { get; set; }
            public decimal? PrevDbTotal { get; set; }
        }

        public class ScheduleReportRequestDto
        {
            public int CustomerId { get; set; }
            public int PartnerId { get; set; }
            public int CompanyId { get; set; }
        }
        public class ScheduleReportResponseDto
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerAddress { get; set; }
            public string CINNumber { get; set; }
            public List<PartnerDto> Partners { get; set; } = new();
        }

        public class PartnerDto
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string PhoneNo { get; set; }
            public string OrgName { get; set; }
        }
        // Dtos/OrgTypeRequestDto.cs
        public class OrgTypeRequestDto
        {
            public int CustomerId { get; set; }
            public int CompanyId { get; set; }
        }

        // Dtos/PersonDto.cs
        public class PersonDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DIN { get; set; }

        }

        // Dtos/OrgTypeResponseDto.cs
        public class OrgTypeResponseDto
        {
            public string OrgType { get; set; }
            public string Label { get; set; } // "Partners" or "Directors"
            public List<PersonDto> Persons { get; set; }
        }
        public class CompanyDto
        {
            public int Company_ID { get; set; }
            public string Company_Name { get; set; }
        }

        //UpdatePnL
        public class UpdatePnlRequestDto
        {
            public string PnLAmount { get; set; }
            public int CompId { get; set; }
            public int CustId { get; set; }
            public int UserId { get; set; }
            public int YearId { get; set; }
            public string BranchId { get; set; }
            public int DurationId { get; set; }
        }
        public class StatutoryPartnerDto
        {
            public int SSP_Id { get; set; }
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
        public class StatutoryDirectorDto
        {
            public int SSD_Id { get; set; }
            public int SSD_CustID { get; set; }
            public string SSD_DirectorName { get; set; }
            public DateTime? SSD_DOB { get; set; }
            public string SSD_DIN { get; set; }
            public string SSD_MobileNo { get; set; }
            public string SSD_Email { get; set; }
            public string SSD_Remarks { get; set; }
            public DateTime? SSD_CRON { get; set; }
            public int SSD_CRBY { get; set; }
            public DateTime? SSD_UpdatedOn { get; set; }
            public int SSD_UpdatedBy { get; set; }
            public string SSD_DelFlag { get; set; }
            public string SSD_STATUS { get; set; }
            public string SSD_IPAddress { get; set; }
            public int SSD_CompID { get; set; }
        }

    }
}
