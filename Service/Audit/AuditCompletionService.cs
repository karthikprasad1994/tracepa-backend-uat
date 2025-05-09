using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.Audit;

namespace TracePca.Service.Audit
{
    public class AuditCompletionService
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public AuditCompletionService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<AuditDropDownListDataDTO> LoadAllAuditDDLDataAsync(int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var parameters = new { CompId = compId };

            var currentYear = connection.QueryFirstOrDefaultAsync<DropDownListData>(@"SELECT YMS_YEARID AS ID, YMS_ID AS Name FROM YEAR_MASTER
                WHERE YMS_Default = 1 AND YMS_CompId = @CompId", parameters);

            var customerList = connection.QueryAsync<DropDownListData>(@"SELECT CUST_ID AS ID, CUST_NAME AS Name FROM SAD_CUSTOMER_MASTER
                WHERE CUST_DELFLG = 'A' AND CUST_CompID = @CompId ORDER BY CUST_NAME ASC", parameters);

            var auditCompletionCheckPointList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                WHERE CMM_Category = 'ASF' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

            var signedByList = connection.QueryAsync<DropDownListData>(@"SELECT Usr_ID AS ID, USr_FullName AS Name from sad_userdetails 
                WHERE usr_compID = @CompId And USR_Partner = 1 And(usr_DelFlag = 'A' or usr_DelFlag = 'B' or usr_DelFlag = 'L') order by USr_FullName ASC", parameters);

            await Task.WhenAll(currentYear, customerList, auditCompletionCheckPointList);

            return new AuditDropDownListDataDTO
            {
                CurrentYear = await currentYear,
                CustomerList = customerList.Result.ToList(),
                AuditCompletionCheckPointList = auditCompletionCheckPointList.Result.ToList(),
                SignedByList = signedByList.Result.ToList()
            };
        }

        public async Task<IEnumerable<ReportTypeDetails>> GetReportTypeDetails(int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var contentIds = await connection.QueryAsync<int>(@"SELECT TEM_ContentId FROM SAD_Finalisation_Report_Template WHERE TEM_FunctionId = @ReportTypeId And TEM_Delflag = 'A' And TEM_CompID = @CompId",
                new { CompId = compId, ReportTypeId = 16 });

            string query;
            object parameters;
            if (contentIds.Count() == 1)
            {
                query = @"SELECT RCM_Id, RCM_ReportName, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_Id IN (@ContentId) AND RCM_ReportId = @ReportTypeId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                parameters = new { ContentId = contentIds.First(), CompId = compId, ReportTypeId = 16 };
            }
            else
            {
                query = @"SELECT RCM_Id, RCM_ReportName, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_ReportId = @ReportTypeId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                parameters = new { CompId = compId, ReportTypeId = 16 };
            }

            var result = await connection.QueryAsync<ReportTypeDetails>(query, parameters);
            return result;
        }

        public async Task<AuditDropDownListDataDTO> LoadAuditNoDDLAsync(int compId, int yearId, int custId, int userId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var sql = @"SELECT SA.SA_ID As ID, SA.SA_AuditNo + ' - ' + CMM.CMM_Desc AS Name FROM StandardAudit_Schedule SA
                LEFT JOIN Content_Management_Master CMM ON CMM.CMM_ID = SA.SA_AuditTypeID WHERE SA.SA_CompID = @CompId AND SA.SA_YearID = @YearId";
            if (custId > 0)
            {
                sql += " AND SA.SA_CustID = @CustId";
            }
            sql += @"AND (EXISTS (SELECT 1 FROM sad_userdetails WHERE usr_CompID = @CompId AND usr_ID = @UserId AND usr_Partner = 1 AND usr_DelFlag IN ('A','B','L'))
                OR (',' + ISNULL(SA.SA_AdditionalSupportEmployeeID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR ',' + ISNULL(SA.SA_EngagementPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR
                    ',' + ISNULL(SA.SA_ReviewPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR ',' + ISNULL(SA.SA_PartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%')) ORDER BY SA.SA_ID DESC";

            var parameters = new
            {
                CompId = compId,
                YearId = yearId,
                CustId = custId,
                UserId = userId
            };

            var auditNoList = await connection.QueryAsync<DropDownListData>(sql, parameters);
            return new AuditDropDownListDataDTO
            {
                ExistingAuditNoList = auditNoList.ToList()
            };
        }        

        public async Task<AuditDropDownListDataDTO> LoadAuditWorkPaperDDLAsync(int compId, int auditId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var auditWorkPaper = await connection.QueryAsync<DropDownListData>(@"Select SSW_ID As ID, SSW_WorkpaperRef As Name From StandardAudit_ScheduleConduct_WorkPaper Where SSW_SA_ID = @AuditId And SSW_CompID = @CompId", new { CompId = compId, AuditId = auditId });
            
            return new AuditDropDownListDataDTO
            {
                AuditWorkpaperList = auditWorkPaper.ToList()
            };
        }
    }
}
