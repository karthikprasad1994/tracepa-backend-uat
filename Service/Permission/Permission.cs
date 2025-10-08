using Dapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using StackExchange.Redis;
using System.IO;
using System.IO.Compression;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Transactions;
using TracePca.Data;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFilling;
using TracePca.Interface;
using TracePca.Interface.DigitalFilling;
using TracePca.Interface.Permission;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

namespace TracePca.Service.Permission
{
    public class Permission : PermissionInterface
	{

        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
          
		private readonly IWebHostEnvironment _env;
		private readonly DbConnectionProvider _dbConnectionProvider;

		public Permission(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, DbConnectionProvider dbConnectionProvider)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}
         
        public async Task<IEnumerable<SearchDto>> SearchDocumentsAsync(string sValue)
        {
            if (string.IsNullOrWhiteSpace(sValue))
            {
                return Enumerable.Empty<SearchDto>();
            }

            try
            {
				//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
				//await connection.OpenAsync();

				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");

				// ✅ Step 2: Get the connection string
				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				string query = @"
                  SELECT Cab.CBN_Name AS Cabinet, SubCab.CBN_Name AS SubCabinet, Fol.Fol_Name AS Folder, 
                        A.PGE_Title AS Title, A.PGE_Ext AS Extension,PGE_BASENAME,
                        (select SAD_Config_Value from [Sad_Config_Settings] where sad_Config_key='DisplayPath') +
						'BITMAPS\' + CAST(FLOOR(CAST(PGE_BASENAME AS numeric)/301) AS varchar) + '\' +
						CAST(PGE_BASENAME AS varchar) + '.'  + A.PGE_Ext AS URLPath
                        FROM edt_page A 
                        JOIN edt_Cabinet Cab ON A.PGE_CABINET = Cab.CBN_ID 
                        JOIN EDT_Cabinet SubCab ON A.PGE_SubCabinet = SubCab.cbn_Id
                        JOIN edt_Folder Fol ON A.PGE_FOLDER = Fol.FOL_FolID 
                        WHERE A.Pge_Status = 'A' 
                        AND (
                                A.PGE_KeyWord LIKE '%' + @SearchTerm + '%'
                                OR A.pge_Title LIKE '%' + @SearchTerm + '%'
                        OR EXISTS (
                                SELECT 1 FROM edt_page_details 
                                WHERE epd_baseid = A.PGE_Details_ID 
                                AND EPD_Value LIKE '%' + @SearchTerm + '%'
                        )
                        OR EXISTS (
                                SELECT 1 FROM edt_Cabinet 
                                WHERE CBN_ID = A.PGE_Cabinet 
                                AND CBN_Name LIKE '%' + @SearchTerm + '%'
                        )
                        OR EXISTS (
                                SELECT 1 FROM edt_Cabinet 
                                WHERE CBN_ID = A.PGE_SubCabinet 
                                AND CBN_Name LIKE '%' + @SearchTerm + '%'
                        )
                        OR EXISTS (
                                SELECT 1 FROM edt_Folder 
                                WHERE Fol_FolID = A.PGE_Folder 
                                AND FOL_Name LIKE '%' + @SearchTerm + '%'
                        )
                  )
                  ORDER BY A.pge_BaseName";

                var result = await connection.QueryAsync<SearchDto>(query, new { SearchTerm = sValue });
                return result ?? Enumerable.Empty<SearchDto>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
           
	}
}


