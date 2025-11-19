using Dapper;
using Microsoft.Data.SqlClient;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.TaskManagement;
using TracePca.Interface.TaskManagement;

namespace TracePca.Service.TaskManagement
{
    public class TaskInvoiceAndReportService : TaskInvoiceAndReportInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public TaskInvoiceAndReportService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _connectionString = GetConnectionStringFromSession();
        }

        private string GetConnectionStringFromSession()
        {
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connStr = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connStr))
                throw new Exception($"Connection string for '{dbName}' not found in configuration.");

            return connStr;
        }

        public async Task<TaskDropDownListDataDTO> LoadAllDDLDataAsync(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { CompId = compId };

                var currentYearTask = connection.QueryFirstOrDefaultAsync<DropDownListData>(
                    @"SELECT YMS_YEARID AS ID, YMS_ID AS Name FROM YEAR_MASTER WHERE YMS_Default = 1 AND YMS_CompId = @CompId", parameters);

                var yearList = connection.QueryAsync<DropDownListData>(
                    @"SELECT YMS_YEARID AS ID, YMS_ID AS Name FROM YEAR_MASTER WHERE YMS_CompId = 1 And YMS_YEARID <= (SELECT YMS_YEARID FROM YEAR_MASTER WHERE YMS_Default = 1 AND YMS_CompId = @CompId) ORDER BY YMS_YEARID Asc", parameters);

                var customerListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT CUST_ID AS ID, CUST_NAME AS Name FROM SAD_CUSTOMER_MASTER WHERE CUST_DELFLG = 'A' AND CUST_CompID = @CompId ORDER BY CUST_NAME ASC", parameters);

                var companyList = connection.QueryAsync<DropDownListData>(
                    @"Select Company_ID AS ID, Company_Name AS Name From Trace_CompanyDetails Where Company_CompID = @CompId ORDER BY Company_Name Asc", parameters);

                var taskListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = 'ASGT' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var partnerListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT Usr_ID AS ID, USr_FullName AS Name FROM sad_userdetails WHERE usr_compID = @CompId AND Usr_Role IN (SELECT Mas_ID FROM SAD_GrpOrLvl_General_Master WHERE Mas_Delflag = 'A' AND Mas_CompID = @CompId AND Mas_Description = 'Partner') AND usr_DelFlag IN ('A', 'B', 'L') ORDER BY USr_FullName", parameters);

                var taxListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = 'FRE' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var workStatusListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = 'WS' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                await Task.WhenAll(currentYearTask, customerListTask, taskListTask, partnerListTask, taxListTask, workStatusListTask);

                return new TaskDropDownListDataDTO
                {
                    CurrentYear = await currentYearTask,
                    YearList = (await yearList).ToList(),
                    ClientList = (await customerListTask).ToList(),
                    CompanyList = (await companyList).ToList(),
                    TaskList = (await taskListTask).ToList(),
                    PartnerList = (await partnerListTask).ToList(),
                    TaxListTask = (await taxListTask).ToList(),
                    WorkStatusList = (await workStatusListTask).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all DDL data.", ex);
            }
        }

        public async Task<CompanyInvoiceDetailsDto> GetCompanyInvoiceDetailsAsync(int compId, int companyId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"SELECT * FROM Trace_CompanyDetails c WHERE c.Company_ID = @CompanyId AND c.Company_CompId = @CompId;";
                var src = await connection.QueryFirstOrDefaultAsync(sql, new { companyId, compId });

                var result = new CompanyInvoiceDetailsDto
                {
                    Company_Code = src?.Company_Code == null ? string.Empty : Convert.ToString(src.Company_Code),
                    Company_Name = src?.Company_Name == null ? string.Empty : Convert.ToString(src.Company_Name),
                    Company_Address = src?.Company_Address == null ? string.Empty : Convert.ToString(src.Company_Address),
                    Company_City_PinCode = $"{(src?.Company_City == null ? string.Empty : Convert.ToString(src.Company_City))} {(src?.Company_PinCode == null ? string.Empty : Convert.ToString(src.Company_PinCode))}".Trim(),
                    Company_State = $"State: {(src?.Company_State == null ? string.Empty : Convert.ToString(src.Company_State))}",
                    Company_PlaceOfSupply = $"Place of Supply: {(src?.Company_State == null ? string.Empty : Convert.ToString(src.Company_State))}",
                    Company_EmailID = $"Email: {(src?.Company_EmailID == null ? string.Empty : Convert.ToString(src.Company_EmailID))}",
                    Company_TelephoneNo = $"Phone no.: {(src?.Company_TelephoneNo == null ? string.Empty : Convert.ToString(src.Company_TelephoneNo))}",
                    Company_HolderName = $"Account Holder Name : {(string.IsNullOrWhiteSpace(Convert.ToString(src?.Company_HolderName)) ? "-" : Convert.ToString(src?.Company_HolderName))}",
                    Company_BankName = $"Bank Name : {(string.IsNullOrWhiteSpace(Convert.ToString(src?.Company_Bankname)) ? "-" : Convert.ToString(src?.Company_Bankname))}",
                    Company_Branch = $"Branch : {(string.IsNullOrWhiteSpace(Convert.ToString(src?.Company_Branch)) ? "-" : Convert.ToString(src?.Company_Branch))}",
                    Company_BankAccountNo = $"Bank Account No. : {(string.IsNullOrWhiteSpace(Convert.ToString(src?.Company_AccountNo)) ? "-" : Convert.ToString(src?.Company_AccountNo))}",
                    Company_Conditions = src?.Company_Conditions == null ? string.Empty : Convert.ToString(src.Company_Conditions),
                    Company_Paymentterms = $"{(src?.Company_Paymentterms == null ? string.Empty : Convert.ToString(src.Company_Paymentterms))} {(src?.Company_Conditions == null ? string.Empty : Convert.ToString(src.Company_Conditions))}".Trim(),
                    Company_PaymenttermsAndConditions = $"{(src?.Company_Paymentterms == null ? string.Empty : Convert.ToString(src.Company_Paymentterms))} {(src?.Company_Conditions == null ? string.Empty : Convert.ToString(src.Company_Conditions))}".Trim()
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading Company Invoice details.", ex);
            }
        }
    }
}