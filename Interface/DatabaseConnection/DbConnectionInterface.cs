using Microsoft.Data.SqlClient;

namespace TracePca.Interface.DatabaseConnection
{
    public interface DbConnectionInterface
    {
        SqlConnection GetConnection();
        void SetCustomerCode(string customerCode);
    }
}
