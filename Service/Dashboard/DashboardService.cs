using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
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

    }
}
