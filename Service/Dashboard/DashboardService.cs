using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TracePca.Dto.Audit;
using TracePca.Dto.Dashboard;
using TracePca.Interface.Dashboard;
using TracePca.Utility;
using static TracePca.Dto.Dashboard.DashboardDto;

namespace TracePca.Service.Dashboard
{
    public class DashboardService : DashboardInterface
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DashboardService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }



        public async Task<RemarksSummaryDto> GetRemarksSummaryAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                string sql = @"
            SELECT 
                CASE 
                    WHEN SAR_RemarksType = 'C' THEN 'Sent'
                    WHEN SAR_RemarksType = 'RC' THEN 'Received'
                END AS RemarkStatus,
                COUNT(*) AS RemarkCount
            FROM [dbo].[StandardAudit_Audit_DRLLog_RemarksHistory]
            WHERE SAR_RemarksType IN ('C', 'RC')
            GROUP BY 
                CASE 
                    WHEN SAR_RemarksType = 'C' THEN 'Sent'
                    WHEN SAR_RemarksType = 'RC' THEN 'Received'
                END;";

                var results = await connection.QueryAsync<RemarksCountDto>(sql);

                var summary = new RemarksSummaryDto();

                foreach (var item in results)
                {
                    if (item.RemarkStatus == "Sent")
                        summary.Sent = item.RemarkCount;
                    else if (item.RemarkStatus == "Received")
                        summary.Received = item.RemarkCount;
                }

                return summary;
            }
        }

        public async Task<IEnumerable<StandardAuditF1DTO>> GetStandardAuditsAsync()
        {

            var query = @"
            SELECT 
                ISNULL(a.CusT_Name, '') AS CustName, 
                SA_AuditNo, 
                b.cmm_Desc AS AuditType, 
                SA_AuditOpinionDate, 
                SA_MRLDate, 
                SA_FilingDatePCAOB, 
                SA_BinderCompletedDate
            FROM [dbo].[StandardAudit_Schedule]
            LEFT JOIN SAD_CUSTOMER_MASTER a ON a.CUST_ID = SA_CustID
            LEFT JOIN Content_Management_Master b ON b.cmm_ID = SA_AuditTypeID
            WHERE SA_AuditFrameworkId = 1;
        ";

            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<StandardAuditF1DTO>(query);
            }
        }
        public async Task<IEnumerable<StandardAuditF2DTO>> GetStandardAuditsFramework0Async()
        {
            var query = @"
            SELECT 
                ISNULL(a.CusT_Name, '') AS CustName, 
                SA_AuditNo, 
                b.cmm_Desc AS AuditType,
                SA_RptFilDate, 
                SA_RptRvDate, 
                SA_MRSDate
            FROM [dbo].[StandardAudit_Schedule]
            LEFT JOIN SAD_CUSTOMER_MASTER a ON a.CUST_ID = SA_CustID
            LEFT JOIN Content_Management_Master b ON b.cmm_ID = SA_AuditTypeID
            WHERE SA_AuditFrameworkId = 0;
        ";

            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<StandardAuditF2DTO>(query);
            }
        }

        public async Task<LOEStatusSummary> GetLOEProgressAsync(int compId, int yearId, int custId)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var parameters = new { CompId = compId, YearID = yearId, CustId = custId };

                    var result = await connection.QueryFirstOrDefaultAsync<LOEStatusSummary>(@"SELECT COUNT(*) AS TotalLOEs, SUM(CASE WHEN LOE_STATUS = 'A' THEN 1 ELSE 0 END) AS ApprovedLOEs,
                    SUM(CASE WHEN LOE_STATUS != 'A' THEN 1 ELSE 0 END) AS PendingLOEs FROM SAD_CUST_LOE WHERE LOE_CompID = @CompId And LOE_YearId = @YearId And (@CustId <= 0 OR LOE_CustomerId = @CustId)", parameters);
                    return result ?? new LOEStatusSummary();
                }               
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting LOE progress data.", ex);
            }
        }

        public async Task<AuditStatusSummary> GetAuditProgressAsync(int compId, int yearId, int custId)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var parameters = new { CompId = compId, YearID = yearId, CustId = custId };

                    var result = await connection.QueryFirstOrDefaultAsync<AuditStatusSummary>(@"SELECT
                    COUNT(*) AS TotalAudits,
                    SUM(CASE WHEN SA_Status = 1 THEN 1 ELSE 0 END) AS Scheduled,
                    SUM(CASE WHEN SA_Status = 2 THEN 1 ELSE 0 END) AS CommunicationWithClient,
                    SUM(CASE WHEN SA_Status = 3 THEN 1 ELSE 0 END) AS TBR,
                    SUM(CASE WHEN SA_Status = 4 THEN 1 ELSE 0 END) AS ConductAudit,
                    SUM(CASE WHEN SA_Status = 5 THEN 1 ELSE 0 END) AS Report,
                    SUM(CASE WHEN SA_Status = 10 THEN 1 ELSE 0 END) AS Completed,
                    SUM(CASE WHEN SA_Status = 0 THEN 1 ELSE 0 END) AS AuditStarted,
                    SUM(CASE WHEN SA_Status <> 10 THEN 1 ELSE 0 END) AS InProgress
                    FROM StandardAudit_Schedule WHERE SA_CompID = @CompId AND SA_YearID = @YearId AND (@CustId <= 0 OR SA_CustID = @CustId)", parameters);

                    return result ?? new AuditStatusSummary();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting Audit progress data.", ex);
            }
        }

        public async Task<PassedDueDatesSummary> GetAuditPassedDueDatesAsync(int compId, int yearId, int custId)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    await Task.CompletedTask;
                    return new PassedDueDatesSummary
                    {
                        OverdueAudits = 0,
                        LastDue = DateTime.MinValue,
                        HighRisk = false
                    };
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting Audit Passed due dates data.", ex);
            }
        }
    }
}
