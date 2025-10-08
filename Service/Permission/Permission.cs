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
         
        public async Task<IEnumerable<PermissionDto>> LoadPermissionDetailsAsync(int CompID)
        {
            try
            {
 
				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");
 
				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				string query = @"WITH ModuleHierarchy AS (
                                    SELECT 
                                        Mod_ID,
                                        Mod_Description,
                                        Mod_NavFunc,
                                        Mod_Parent,
                                        CAST(RIGHT('0000' + CAST(Mod_ID AS VARCHAR(4)), 4) AS VARCHAR(200)) AS SortPath
                                    FROM Sad_Module
                                    WHERE Mod_Parent = 0 and Mod_CompID = @Mod_CompID

                                    UNION ALL

                                    SELECT 
                                        m.Mod_ID,
                                        m.Mod_Description,
                                        m.Mod_NavFunc,
                                        m.Mod_Parent,
                                        CAST(h.SortPath + '-' + RIGHT('0000' + CAST(m.Mod_ID AS VARCHAR(4)), 4) AS VARCHAR(200))
                                    FROM Sad_Module m
                                    INNER JOIN ModuleHierarchy h 
                                        ON m.Mod_Parent = h.Mod_ID
                                ),
                                ModuleOperations AS (
                                    SELECT 
                                        h.Mod_ID,
                                        h.Mod_Description,
                                        h.Mod_NavFunc,
                                        o.OP_OperationName,
                                        o.OP_PKID
                                    FROM ModuleHierarchy h
                                    LEFT JOIN SAD_Mod_Operations o 
                                        ON o.OP_ModuleID = h.Mod_ID
                                ),
                                ModulePermissions AS (
                                    SELECT 
                                        mo.Mod_ID,
                                        mo.Mod_Description,
                                        mo.Mod_NavFunc,
                                        mo.OP_OperationName,
                                        mo.OP_PKID,
                                        CASE 
                                            WHEN p.Perm_PKID IS NOT NULL THEN 1
                                            ELSE 0
                                        END AS PermissionFlag
                                    FROM ModuleOperations mo
                                    LEFT JOIN SAD_UsrOrGrp_Permission p
                                        ON p.Perm_ModuleID = mo.Mod_ID
                                        AND p.Perm_OpPKID = mo.OP_PKID
                                        AND p.Perm_UsrORGrpID = 1  
                                        AND p.Perm_PType = 'R'
                                )
                                SELECT 
                                    mh.Mod_ID,
                                    mh.Mod_Description,
                                    mh.Mod_NavFunc,
                                    STRING_AGG(mp.OP_OperationName, ', ') AS Operations,
                                    STRING_AGG(CAST(mp.OP_PKID AS VARCHAR), ', ') AS OperationsID,
                                    STRING_AGG(CAST(mp.PermissionFlag AS VARCHAR), ', ') AS PermissionsFlag
                                FROM ModuleHierarchy mh
                                LEFT JOIN ModulePermissions mp
                                    ON mh.Mod_ID = mp.Mod_ID
                                GROUP BY mh.SortPath, mh.Mod_ID, mh.Mod_Description, mh.Mod_NavFunc
                                ORDER BY mh.SortPath;";

                var result = await connection.QueryAsync<PermissionDto>(query, new { Mod_CompID = CompID });
                return result ?? Enumerable.Empty<PermissionDto>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
         
		public async Task<IEnumerable<PermissionDto>> LoadPermissionModuleAsync(int CompID)
		{
			try
			{

				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");

				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				string query = @"SELECT 0 AS Mod_ID, 'All Module' AS Mod_Description UNION ALL SELECT Mod_ID, Mod_Description FROM Sad_Module WHERE Mod_Parent = 0 AND Mod_DelFlag = 'X' AND Mod_CompID = @Mod_CompID ORDER BY Mod_ID";


				var result = await connection.QueryAsync<PermissionDto>(query, new { Mod_CompID = CompID });
				return result ?? Enumerable.Empty<PermissionDto>();
			}
			catch (Exception ex)
			{

				throw;
			}
		}


		public async Task<IEnumerable<PermissionDto>> LoadPermissionRoleAsync(string sPermissionType, int CompID)
		{
			try
			{

				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");

				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

                string query = "";

				if (sPermissionType == "R")
                {
					query = @"SELECT 0 AS Mod_ID, 'Select Role' AS Mod_Description UNION ALL Select Mas_ID as Mod_ID,Mas_Description as Mod_Description from SAD_GrpOrLvl_General_Master where Mas_Delflag='A' and Mas_CompID=@Mod_CompID order by Mod_ID";
				}
                else if(sPermissionType == "U")
                {
                    query = @"SELECT 0 AS Mod_ID, 'Select User' AS Mod_Description UNION ALL
                             Select Usr_ID as Mod_ID,(Usr_FullName + ' - ' + Usr_Code) as Mod_Description from Sad_UserDetails Where usr_delflag = 'A' and  
                             Usr_GrpOrUserLvlPerm='1' and Usr_CompId=@Mod_CompID order by Mod_ID";
                }
				 
				var result = await connection.QueryAsync<PermissionDto>(query, new { Mod_CompID = CompID });
				return result ?? Enumerable.Empty<PermissionDto>();
			}
			catch (Exception ex)
			{

				throw;
			}
		}

	}
}


