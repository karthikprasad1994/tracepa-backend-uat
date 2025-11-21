using Dapper;
using TracePca.Interface.ClientPortal;
using TracePca.Dto.ClientPortal;
using TracePca.Utility;
using System.Threading.Tasks;
using static TracePca.Dto.ClientPortal.ClientPortalDto;

namespace TracePca.Service.ClientPortal
{
    public class ClientPortalService : IClientPortalInterface
    {
        private readonly IDbConnectionFactory _db;

        public ClientPortalService(IDbConnectionFactory dbConnectionFactory)
        {
            _db = dbConnectionFactory;
        }

        public async Task<UserCustomerResponse> GetUserCustomerDetailsAsync(GetUserCustomerIdRequest request)
        {
            using (var connection = _db.CreateConnection())
            {
                string sql = @"
                    SELECT 
                        usr_CompanyId AS CompanyId,
                        Cust_Name AS CustomerName
                    FROM Sad_Userdetails
                    LEFT JOIN SAD_Customer_Master a 
                        ON a.Cust_Id = usr_CompanyId
                    WHERE Usr_CompId = @CompId
                      AND Usr_ID = @UserId";

                var result = await connection.QueryFirstOrDefaultAsync<UserCustomerResponse>(
                    sql,
                    new
                    {
                        CompId = request.CompanyId,
                        UserId = request.UserId
                    }
                );

                return result ?? new UserCustomerResponse();
            }
        }
    }
}

