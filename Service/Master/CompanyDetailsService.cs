using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.Master;
using TracePca.Interface.Master;

namespace TracePca.Service.Master
{
    public class CompanyDetailsService : CompanyDetailsInterface
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyDetailsService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<(bool Success, string Message, List<CompanyDetailsListDto> Data)> GetCompanyDetailsListAsync(int compId)
        {
            using var con = new SqlConnection(_connectionString);
            try
            {
                var sql = @"SELECT Company_ID As Id, Company_Name As Name FROM TRACe_CompanyDetails WHERE Company_CompID = @CompId ORDER BY Company_Name";
                var data = (await con.QueryAsync<CompanyDetailsListDto>(sql, new { CompId = compId })).ToList();
                return (true, "Company list loaded.", data);
            }
            catch (Exception ex)
            {
                return (false, "Error: " + ex.Message, new List<CompanyDetailsListDto>());
            }
        }

        public async Task<(bool Success, string Message, CompanyDetailsDto? Data)> GetCompanyDetailsByIdAsync(int id, int compId)
        {
            using var con = new SqlConnection(_connectionString);
            try
            {
                var sql = @"SELECT * FROM TRACe_CompanyDetails WHERE Company_ID = @Id AND Company_CompID = @CompId";
                var row = await con.QueryFirstOrDefaultAsync<CompanyDetailsDto>(sql, new { Id = id, CompId = compId });

                if (row == null)
                    return (false, "Company details not found.", null);

                return (true, "Company details loaded.", row);
            }
            catch (Exception ex)
            {
                return (false, "Error: " + ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, int PkId)> SaveOrUpdateCompanyDetailsAsync(CompanyDetailsDto dto)
        {
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var tran = con.BeginTransaction();
            try
            {
                bool isUpdate = dto.Company_ID > 0;
                if (!isUpdate)
                {
                    dto.Company_ID = await con.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(Company_ID), 0) + 1 FROM TRACe_CompanyDetails WHERE Company_CompID = @CompId", new { dto.CompId }, tran);
                }

                string sql = isUpdate
                    ? @"UPDATE TRACe_CompanyDetails SET
                            Company_Code = @Company_Code,
                            Company_Name = @Company_Name,
                            Company_Address = @Company_Address,
                            Company_City = @Company_City,
                            Company_State = @Company_State,
                            Company_Country = @Company_Country,
                            Company_PinCode = @Company_PinCode,
                            Company_EmailID = @Company_EmailID,
                            Company_Establishment_Date = @Company_Establishment_Date,
                            Company_ContactPerson = @Company_ContactPerson,
                            Company_MobileNo = @Company_MobileNo,
                            Company_ContactEmailID = @Company_ContactEmailID,
                            Company_TelephoneNo = @Company_TelephoneNo,
                            Company_WebSite = @Company_WebSite,
                            Company_ContactNo1 = @Company_ContactNo1,
                            Company_ContactNo2 = @Company_ContactNo2,
                            Company_HolderName = @Company_HolderName,
                            Company_AccountNo = @Company_AccountNo,
                            Company_Bankname = @Company_Bankname,
                            Company_Branch = @Company_Branch,
                            Company_Conditions = @Company_Conditions,
                            Company_Paymentterms = @Company_Paymentterms,
                            Company_UpdatedBy = @UserId,
                            Company_UpdatedOn = GETDATE(),
                            Company_IPAddress = @IpAddress
                        WHERE Company_ID = @Company_ID"
                    :
                        @"INSERT INTO TRACe_CompanyDetails
                        (Company_ID, Company_Code, Company_Name, Company_Address, Company_City, Company_State,
                         Company_Country, Company_PinCode, Company_EmailID, Company_Establishment_Date,
                         Company_ContactPerson, Company_MobileNo, Company_ContactEmailID,
                         Company_TelephoneNo, Company_WebSite, Company_ContactNo1, Company_ContactNo2,
                         Company_HolderName, Company_AccountNo, Company_Bankname, Company_Branch,
                         Company_Conditions, Company_Paymentterms, Company_Status,
                         Company_CrBy, Company_CrOn, Company_IPAddress, Company_CompID)
                        VALUES
                        (@Company_ID, @Company_Code, @Company_Name, @Company_Address, @Company_City, @Company_State,
                         @Company_Country, @Company_PinCode, @Company_EmailID, @Company_Establishment_Date,
                         @Company_ContactPerson, @Company_MobileNo, @Company_ContactEmailID,
                         @Company_TelephoneNo, @Company_WebSite, @Company_ContactNo1, @Company_ContactNo2,
                         @Company_HolderName, @Company_AccountNo, @Company_Bankname, @Company_Branch,
                         @Company_Conditions, @Company_Paymentterms, 'A',
                         @UserId, GETDATE(), @IpAddress, @CompId)";

                await con.ExecuteAsync(sql, dto, tran);
                await tran.CommitAsync();

                return (true, isUpdate ? "Company details updated successfully." : "Company details saved successfully.", dto.Company_ID);
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return (false, "Error : " + ex.Message, 0);
            }
        }
    }
}
