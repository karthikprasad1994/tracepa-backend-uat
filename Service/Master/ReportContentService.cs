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
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

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
                //var sql = @"SELECT RCM_Id, RCM_ReportId, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_ReportId = @ReportId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                var sql = @"SELECT RCM.RCM_Id, RCM.RCM_ReportId, RCM.RCM_Heading, RCM.RCM_Description FROM SAD_ReportContentMaster RCM
                    LEFT JOIN SAD_Finalisation_Report_Template TEM ON TEM.TEM_FunctionId = RCM.RCM_ReportId AND TEM.TEM_CompId = @CompId
                    WHERE RCM.RCM_ReportId = @ReportId AND RCM.RCM_CompID = @CompId AND RCM.RCM_Delflag = 'A'
                    ORDER BY CASE WHEN TEM.TEM_ContentId IS NULL THEN RCM.RCM_Id ELSE CHARINDEX(',' + CAST(RCM.RCM_Id AS VARCHAR) + ',', ',' + TEM.TEM_ContentId + ',') END";
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
                        VALUES (@Id, @ReportId, @Heading, @Description, 'A', 'A', @UserId, GETDATE(), @Ip, @CompId, @YearId)";
                     
                    await con.ExecuteAsync(sql, new
                    {
                        Id = newId,
                        ReportId = dto.RCM_ReportId,
                        ReportName = dto.RCM_ReportName,
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

        public async Task<(bool Success, string Message)> SaveOrUpdateReportTemplateSortOrderAsync(ReportTemplateSortOrderSaveDTO dto)
        {
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var tran = con.BeginTransaction();
            try
            {
                var existingId = await con.ExecuteScalarAsync<int?>(@"SELECT TEM_Id FROM SAD_Finalisation_Report_Template WHERE TEM_FunctionId = @FunctionId AND TEM_Compid = @CompId", new
                {
                    FunctionId = dto.TEM_FunctionId,
                    CompId = dto.CompId
                }, tran);

                bool isUpdate = existingId.HasValue;
                if (isUpdate)
                {
                    var sql = @"UPDATE SAD_Finalisation_Report_Template SET TEM_ContentId = @ContentId, TEM_SortOrder = @SortOrder, TEM_UpdatedBy = @UserId, TEM_UpdatedOn = GETDATE(), TEM_IPAddress = @Ip
                        WHERE TEM_Id = @Id And TEM_FunctionId = @FunctionId AND TEM_Compid = @CompId";

                    await con.ExecuteAsync(sql, new
                    {
                        Id = existingId.Value,
                        FunctionId = dto.TEM_FunctionId,
                        ContentId = dto.TEM_ContentId,
                        SortOrder = dto.TEM_SortOrder,
                        UserId = dto.UserId,
                        Ip = dto.IpAddress,
                        CompId = dto.CompId
                    }, tran);

                    await tran.CommitAsync();
                    return (true, "Template Sort Order updated successfully.");
                }
                else
                {
                    var newId = await con.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(TEM_Id), 0) + 1 FROM SAD_Finalisation_Report_Template WHERE TEM_Compid = @CompId", new { dto.CompId }, tran);

                    var sql = @"INSERT INTO SAD_Finalisation_Report_Template
                        (TEM_Id, TEM_Yearid, TEM_FunctionId, TEM_Module, TEM_ReportTitle, TEM_ContentId, TEM_SortOrder, TEM_Delflag, TEM_Status, TEM_CrBy, TEM_CrOn, TEM_IPAddress, TEM_Compid)
                        VALUES (@Id, @YearId, @FunctionId, @Module, @ReportTitle, @ContentId, @SortOrder, @Delflag, @Status, @UserId, GETDATE(), @Ip, @CompId);";

                    await con.ExecuteAsync(sql, new
                    {
                        Id = newId,
                        YearId = dto.TEM_Yearid,
                        FunctionId = dto.TEM_FunctionId,
                        Module = "Financial Audit",
                        ReportTitle = "0",
                        ContentId = dto.TEM_ContentId,
                        SortOrder = dto.TEM_SortOrder,
                        Delflag = "W",
                        Status = "C",
                        UserId = dto.UserId,
                        Ip = dto.IpAddress,
                        CompId = dto.CompId
                    }, tran);

                    await tran.CommitAsync();
                    return (true, "Template Sort Order saved successfully.");
                }
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return (false, "Error saving template sort order: " + ex.Message);
            }
        }
    }
}
