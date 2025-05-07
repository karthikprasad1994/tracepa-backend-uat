using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
          AND CMM_Category = 'OE'
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



        public async Task<bool> SaveAllLoeDataAsync(AddEngagementDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Insert into SAD_CUST_LOE
                dto.LoeId = await connection.ExecuteScalarAsync<int>(
                    @"DECLARE @NewId INT = (SELECT ISNULL(MAX(LOE_Id), 0) + 1 FROM SAD_CUST_LOE);
              INSERT INTO SAD_CUST_LOE (
                  LOE_Id, LOE_YearId, LOE_CustomerId, LOE_ServiceTypeId, LOE_NatureOfService,
                  LOE_LocationIds, LOE_TimeSchedule, LOE_ReportDueDate,
                  LOE_ProfessionalFees, LOE_OtherFees, LOE_ServiceTax, LOE_RembFilingFee,
                  LOE_CrBy, LOE_CrOn, LOE_Total, LOE_Name, LOE_Frequency,
                  LOE_FunctionId, LOE_SubFunctionId, LOE_STATUS, LOE_Delflag, LOE_IPAddress, LOE_CompID
              )
              VALUES (
                  @NewId, @LoeYearId, @LoeCustomerId, @LoeServiceTypeId, @LoeNatureOfService,
                  '0', @LoeTimeSchedule, @LoeReportDueDate,
                  '0', '0', '0', '0',
                  1, GETDATE(), @LoeTotal, @LoeName, @LoeFrequency,
                  0, '1', 'A', 'A', @LoeIpaddress, @LoeCompId);
              SELECT @NewId;", dto, transaction);

                // 2. Insert into LOE_Template
                dto.LOET_Id = await connection.ExecuteScalarAsync<int>(
                    @"DECLARE @TemplateId INT = (SELECT ISNULL(MAX(LOET_Id), 0) + 1 FROM LOE_Template);
              INSERT INTO LOE_Template (
                  LOET_Id, LOET_LOEID , LOET_CustomerId, LOET_FunctionId, LOET_ScopeOfWork,
                  LOET_Frequency, LOET_ProfessionalFees, LOET_Delflag, LOET_STATUS,
                  LOET_CrOn, LOET_CrBy, LOET_IPAddress, LOET_CompID, LOE_AttachID
              )
              VALUES (
                  @TemplateId, @LoeId, @LoeCustomerId, 0, @LoeNatureOfService,
                  @LoeFrequency, '0', 'A', 'A', GETDATE(), 1,
                  @LoeIpaddress, @LoeCompId, @LoeAttachId);
              SELECT @TemplateId;", dto, transaction);

                // 3. Insert into LOE_AdditionalFees
                dto.FeeName = await connection.QueryFirstOrDefaultAsync<string>(
                    @"SELECT cmm_Desc FROM Content_Management_Master WHERE cmm_Category = 'OE' AND CMM_CompID = @LoeCompId",
                    new { dto.LoeCompId }, transaction);

                dto.ExpensesId = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT cmm_ID FROM Content_Management_Master WHERE cmm_Category = 'OE' AND CMM_CompID = @LoeCompId",
                    new { dto.LoeCompId }, transaction);

                dto.FeesId = await connection.ExecuteScalarAsync<int>(
                    @"DECLARE @NewFeesId INT = (SELECT ISNULL(MAX(LAF_ID), 0) + 1 FROM LOE_AdditionalFees);
              INSERT INTO LOE_AdditionalFees (
                  LAF_ID, LAF_LOEID, LAF_OtherExpensesID, LAF_Charges, LAF_OtherExpensesName,
                  LAF_Delflag, LAF_STATUS, LAF_CrBy, LAF_CrOn, LAF_IPAddress, LAF_CompID
              )
              VALUES (
                  @NewFeesId, @LoeId, @ExpensesId, @LoeTotal, @FeeName, 'A', 'C', 1,
                  GETDATE(), @LoeIpaddress, @LoeCompId);
              SELECT @NewFeesId;", dto, transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }






    }
}