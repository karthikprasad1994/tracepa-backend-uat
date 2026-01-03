// Helpers/DBHelper.cs
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;

public interface IDBHelper
{
    //Task<int> ExecuteScalarAsync<T>(string sql, object parameters = null);
    Task<int> ExecuteNonQueryAsync(string sql, object parameters = null);
    Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object parameters = null);
    Task<T> QuerySingleAsync<T>(string sql, object parameters = null);
    Task<IDbConnection> CreateConnectionAsync();
}

public class DBHelper : IDBHelper
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DBHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    //public async Task<int> ExecuteScalarAsync<T>(string sql, object parameters = null)
    //{
    //    using (var connection = await CreateConnectionAsync())
    //    {
    //        return await connection.ExecuteScalarAsync<T>(sql, parameters);
    //    }
    //}

    public async Task<int> ExecuteNonQueryAsync(string sql, object parameters = null)
    {
        using (var connection = await CreateConnectionAsync())
        {
            return await connection.ExecuteAsync(sql, parameters);
        }
    }

    public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object parameters = null)
    {
        using (var connection = await CreateConnectionAsync())
        {
            return await connection.QueryAsync<T>(sql, parameters);
        }
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object parameters = null)
    {
        using (var connection = await CreateConnectionAsync())
        {
            return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
        }
    }
}