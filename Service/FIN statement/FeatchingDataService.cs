using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using Dapper;

namespace TracePca.Service.FIN_statement
{
    public class FeatchingDataService : FeatchingDataInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FeatchingDataService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //Trdm
        public async Task<string> ExportTrdmFullDatabaseAsync()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string outputFile = "full_database.jsonl";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            DataTable tables = connection.GetSchema("Tables");
            await using var writer = new StreamWriter(outputFile, false);

            foreach (DataRow table in tables.Rows)
            {
                string schema = table["TABLE_SCHEMA"].ToString()!;
                string tableName = table["TABLE_NAME"].ToString()!;
                string query = $"SELECT * FROM [{schema}].[{tableName}]";

                var records = await connection.QueryAsync(query);

                foreach (var record in records)
                {
                    // Convert the DapperRow (dynamic) to JObject safely
                    var dict = (IDictionary<string, object>)record;
                    var obj = new JObject();

                    foreach (var kvp in dict)
                    {
                        obj[kvp.Key] = kvp.Value != null ? JToken.FromObject(kvp.Value) : JValue.CreateNull();
                    }

                    obj["table"] = tableName;

                    // Use first column as ID or fallback to a GUID
                    string firstKey = dict.Keys.FirstOrDefault() ?? "rowid";
                    string idValue = dict[firstKey]?.ToString() ?? Guid.NewGuid().ToString();

                    obj["id"] = $"{tableName}_{idValue}";

                    await writer.WriteLineAsync(JsonConvert.SerializeObject(obj));
                }
            }
            return outputFile;
        }

        //Tr25_44
        public async Task<string> ExportTr25_44FullDatabaseAsync()
        {
            string connectionString = _configuration.GetConnectionString("TR25_044");
            string outputFile = "full_database1.jsonl";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            DataTable tables = connection.GetSchema("Tables");
            await using var writer = new StreamWriter(outputFile, false);

            foreach (DataRow table in tables.Rows)
            {
                string schema = table["TABLE_SCHEMA"].ToString()!;
                string tableName = table["TABLE_NAME"].ToString()!;
                string query = $"SELECT * FROM [{schema}].[{tableName}]";

                var records = await connection.QueryAsync(query);

                foreach (var record in records)
                {
                    // Convert the DapperRow (dynamic) to JObject safely
                    var dict = (IDictionary<string, object>)record;
                    var obj = new JObject();

                    foreach (var kvp in dict)
                    {
                        obj[kvp.Key] = kvp.Value != null ? JToken.FromObject(kvp.Value) : JValue.CreateNull();
                    }

                    obj["table"] = tableName;

                    // Use first column as ID or fallback to a GUID
                    string firstKey = dict.Keys.FirstOrDefault() ?? "rowid";
                    string idValue = dict[firstKey]?.ToString() ?? Guid.NewGuid().ToString();

                    obj["id"] = $"{tableName}_{idValue}";

                    await writer.WriteLineAsync(JsonConvert.SerializeObject(obj));
                }
            }

            return outputFile;
        }
    }
}

