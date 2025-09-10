using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
    }
}
