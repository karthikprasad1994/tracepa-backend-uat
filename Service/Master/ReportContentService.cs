using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.Master;
using TracePca.Interface.Audit;
using TracePca.Interface.Master;

namespace TracePca.Service.Master
{
    public class ReportTemplateService : ReportTemplateInterface
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportTemplateService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<(bool Success, string Message, List<ReportTypeDTO> Data)>GetReportTypesByFunctionAsync(int functionId, int compId)
        {
            using var con = new SqlConnection(_connectionString);
            try
            {
                var sql = @"SELECT RTM_Id, RTM_ReportTypeName FROM SAD_ReportTypeMaster WHERE RTM_TemplateId = @FunctionId AND RTM_CompID = @CompId AND RTM_DelFlag = 'A' ORDER BY RTM_ReportTypeName";
                var data = (await con.QueryAsync<ReportTypeDTO>(sql, new { FunctionId = functionId, CompId = compId })).ToList();
                return (true, "Loaded successfully.", data);
            }
            catch (Exception ex)
            {
                return (false, "Error loading report types: " + ex.Message, new List<ReportTypeDTO>());
            }
        }

        public async Task<(bool Success, string Message, List<ReportContentDTO> Data)>GetReportContentByReportTypeAsync(int reportTypeId, int compId)
        {
            using var con = new SqlConnection(_connectionString);
            try
            {
                var sql = @"SELECT RCM_Id, RCM_ReportId, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_ReportId = @ReportId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                var data = (await con.QueryAsync<ReportContentDTO>(sql, new { ReportId = reportTypeId, CompId = compId })).ToList();
                return (true, "Content loaded.", data);
            }
            catch (Exception ex)
            {
                return (false, "Error loading contents: " + ex.Message, new List<ReportContentDTO>());
            }
        }

        public async Task<(bool Success, string Message)>SaveOrUpdateReportContentAsync(ReportContentSaveDTO dto)
        {
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var tran = con.BeginTransaction();
            try
            {
                bool isUpdate = dto.RCM_Id > 0;

                if (isUpdate)
                {
                    var sql = @" UPDATE SAD_ReportContentMaster SET RCM_Heading = @Heading, RCM_Description = @Description, RCM_UpdatedBy = @UserId, RCM_UpdatedOn = GETDATE(), RCM_IPAddress = @Ip WHERE RCM_Id = @Id";

                    await con.ExecuteAsync(sql, new
                    {
                        Heading = dto.RCM_Heading,
                        Description = dto.RCM_Description,
                        UserId = dto.UserId,
                        Ip = dto.IpAddress,
                        Id = dto.RCM_Id
                    }, tran);

                    await tran.CommitAsync();
                    return (true, "Updated successfully.");
                }
                else
                {
                    var newId = await con.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(RCM_Id), 0) + 1 FROM SAD_ReportContentMaster WHERE RCM_CompID = @CompId", new { CompId = dto.CompId }, tran);

                    var sql = @"INSERT INTO SAD_ReportContentMaster (RCM_Id, RCM_ReportId, RCM_Heading, RCM_Description, RCM_Delflag, RCM_Status, RCM_CrBy, RCM_CrOn, RCM_IPAddress, RCM_CompID, RCM_Yearid)
                        VALUES (@Id, @ReportId, @Heading, @Description, 'N', 'A', @UserId, GETDATE(), @Ip, @CompId, @YearId)";

                    await con.ExecuteAsync(sql, new
                    {
                        Id = newId,
                        ReportId = dto.RCM_ReportId,
                        Heading = dto.RCM_Heading,
                        Description = dto.RCM_Description,
                        UserId = dto.UserId,
                        Ip = dto.IpAddress,
                        CompId = dto.CompId,
                        YearId = dto.YearId
                    }, tran);

                    await tran.CommitAsync();
                    return (true, "Saved successfully.");
                }
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return (false, "Error saving content: " + ex.Message);
            }
        }
    }
}
