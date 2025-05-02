using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface;
using TracePca.Interface.Audit;

namespace TracePca.Service
{
    public class Engagement : EngagementPlanInterface
    {

        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public Engagement(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }




        public async Task<LoEDto> GetLoeIdAsync(int customerId, int compId, int serviceId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
    SELECT LOE_ID, LOE_Name
    FROM SAD_CUST_LOE
    WHERE LOE_CustomerId = @CustomerId
      AND LOE_CompID = @CompId
      AND LOE_ServiceTypeId = @ServiceId";

            var loeId = await connection.QueryFirstOrDefaultAsync<int?>(query, new
            {
                CustomerId = customerId,
                CompId = compId,
                ServiceId = serviceId
            });

            return new LoEDto
            {
                LoeId = loeId ?? 0  // Default to 0 if null
            };
        }


        public async Task<List<AuditTypeDto>> GetAuditTypesAsync(int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT cmm_ID AS CmmId, cmm_Desc AS CmmDesc
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompId
          AND CMM_Category = 'AuditType'
          AND CMM_Delflag = 'A'
        ORDER BY cmm_Desc ASC";

            var auditTypes = await connection.QueryAsync<AuditTypeDto>(query, new { CompId = compId });
            return auditTypes.ToList();
        }


        public async Task<List<ReportTypeDto>> GetReportTypesAsync(int compId, int templateId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT RTM_Id, RTM_ReportTypeName
        FROM SAD_ReportTypeMaster
        WHERE RTM_CompID = @CompId
          AND RTM_DelFlag = 'A'";

            if (templateId > 0)
            {
                query += " AND RTM_TemplateId = @TemplateId";
            }
            query += " ORDER BY RTM_ReportTypeName";

            var reportTypes = await connection.QueryAsync<ReportTypeDto>(query, new { CompId = compId, TemplateId = templateId });
            return reportTypes.ToList();
        }


        public async Task<DropDownDataDto> LoadAllDropdownDataAsync(int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // Open the connection once for better performance
            await connection.OpenAsync();

            var auditTypesTask = connection.QueryAsync<AuditTypeDto>(@"
        SELECT cmm_ID AS AuditId, cmm_Desc AS  AuditDesc
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompId
          AND CMM_Category = 'AT'
          AND CMM_Delflag = 'A'
        ORDER BY cmm_Desc ASC", new { CompId = compId });

            var reportTypesTask = connection.QueryAsync<ReportTypeDto>(@"
        SELECT RTM_Id, RTM_ReportTypeName
        FROM SAD_ReportTypeMaster
        WHERE RTM_CompID = @CompId
          AND RTM_DelFlag = 'A'
        ORDER BY RTM_ReportTypeName", new { CompId = compId });

            // If you want LOE list based on only compId (not customerId/serviceId), modify your LOE fetch logic slightly:
            var loeListTask = connection.QueryAsync<LoEDto>(@"
        SELECT LOE_ID AS LoeId, LOE_Name
        FROM SAD_CUST_LOE
        WHERE LOE_CompID = @CompId", new { CompId = compId });

            var FeesType = connection.QueryAsync<FeeTypeDto>(@"
        SELECT cmm_ID AS FeeId, cmm_Desc AS FeeName
        FROM Content_Management_Master
        WHERE cmm_Category = 'OE' and CMM_CompID = @CompId", new { CompId = compId });
            await Task.WhenAll(auditTypesTask, reportTypesTask, loeListTask, FeesType);

            return new DropDownDataDto
            {
                AuditTypes = auditTypesTask.Result.ToList(),
                ReportTypes = reportTypesTask.Result.ToList(),
                FeeTypes = FeesType.Result.ToList(),
                Loenames = loeListTask.Result.ToList()
            };
        }



    }
}
