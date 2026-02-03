using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Dapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Apis.Auth;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using MimeKit;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Utilities.Net;
using StackExchange.Redis;
using TracePca.Data;
using TracePca.Data.CustomerRegistration;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Dto.Authentication;
using TracePca.Dto.DigitalFilling;
using TracePca.Dto.Middleware;
using TracePca.Interface;
using TracePca.Interface.Middleware;
using TracePca.Models;
using TracePca.Models.CustomerRegistration;
using TracePca.Models.UserModels;
using Xceed.Document.NET;



namespace TracePca.Service
{
    public class Login : LoginInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly CustomerRegistrationContext _customerRegistrationContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DynamicDbContext _context;
        private readonly OtpService _otpService;
        private readonly IWebHostEnvironment _env;
        private readonly string _appSettingsPath;
        private readonly IDbConnection _db;
        private readonly ErrorLoggerInterface _errorLogger;


        public Login(Trdmyus1Context dbContext, CustomerRegistrationContext customerDbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, DynamicDbContext context, OtpService otpService, IWebHostEnvironment env, ErrorLoggerInterface errorLogger)
        {
            _dbcontext = dbContext;
            _customerRegistrationContext = customerDbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _otpService = otpService;
            _env = env;
            _errorLogger = errorLogger;
            _appSettingsPath = System.IO.Path.Combine(env.ContentRootPath, "appsettings.json");
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));


        }

        public async Task<object> GetAllUsersAsync()
        {
            try
            {
                var user_details = await _dbcontext.SadUserDetails.ToListAsync();

                if (!user_details.Any())
                {
                    return new
                    {
                        statuscode = 404,
                        message = "No Users found.",
                        users_details = new List<object>()
                    };
                }

                return new
                {
                    statuscode = 200,
                    message = "Successfully fetched User details.",
                    data = user_details
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    statuscode = 500,
                    message = $"An error occurred: {ex.Message}",
                    Users = new List<object>()
                };
            }
        }

        // Track users for whom we have already shown the "please login again" message
        //private static readonly HashSet<string> _shownOnceEmails = new HashSet<string>();

        //public async Task<IActionResult> SignUpUserViaGoogleAsync(GoogleAuthDto dto)
        //{
        //    try
        //    {
        //        var email = dto.Email?.Trim().ToLower();

        //        if (string.IsNullOrWhiteSpace(email))
        //        {
        //            return new ObjectResult(new
        //            {
        //                statuscode = 400,
        //                message = "Email is required."
        //            })
        //            { StatusCode = 400 };
        //        }

        //        using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
        //        await connection.OpenAsync();

        //        var existingCustomerCode = await connection.QueryFirstOrDefaultAsync<string>(
        //            @"SELECT TOP 1 MCR_CustomerCode
        //      FROM MMCS_CustomerRegistration
        //      CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails
        //      WHERE LTRIM(RTRIM(Emails.value)) = @Email", new { Email = email });

        //        if (!string.IsNullOrEmpty(existingCustomerCode))
        //        {
        //            // ✅ Existing user
        //            // First time → return message only
        //            if (!_shownOnceEmails.Contains(email))
        //            {
        //                _shownOnceEmails.Add(email);
        //                return new OkObjectResult(new
        //                {
        //                    statuscode = 200,
        //                    message = "User already exists. Please login again."
        //                });
        //            }
        //            // Second time → login and return login response
        //            var loginResult = await LoginUserAsync(email, "sa");  // use your login logic
        //            return loginResult.StatusCode == 200
        //                ? new OkObjectResult(loginResult)
        //                : new ObjectResult(loginResult) { StatusCode = loginResult.StatusCode };
        //        }
        //        else
        //        {
        //            // ✅ New user → register only
        //            var registrationDto = new RegistrationDto
        //            {
        //                McrCustomerEmail = email,
        //                McrCustomerTelephoneNo = dto.PhoneNumber?.Trim(),
        //                McrCustomerName = dto.CompanyName?.Trim()
        //            };

        //            return await SignUpUserAsync(registrationDto);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ObjectResult(new
        //        {
        //            statuscode = 500,
        //            message = "Internal server error",
        //            error = ex.Message
        //        })
        //        { StatusCode = 500 };
        //    }
        //}



        public async Task<IEnumerable<ModuleDto>> GetModulesByMpIdAsync(int mpId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
            await connection.OpenAsync();

            string query = @"
SELECT 
    MM_ID AS ModuleId,
    MM_MP_ID AS ProductId,
    MP_ModuleName AS ModuleName
FROM MMCS_MODULES
WHERE MM_MP_ID = @MpId
ORDER BY MM_ID
";

            var modules = await connection.QueryAsync<ModuleDto>(query, new
            {
                MpId = mpId
            });

            // Map SQL columns to DTO properties
            var result = modules.Select(x => new ModuleDto
            {
                ProductId = x.ProductId,           // MM_ID
                ModuleId = x.ModuleId,   // MM_MP_ID
                ModuleName = x.ModuleName // MP_ModuleName
            }).ToList();

            return result;
        }





        public async Task<IActionResult> SignUpUserViaGoogleAsync(GoogleAuthDto dto)
        {
            try
            {
                var email = dto.Email?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(email))
                {
                    return new ObjectResult(new
                    {
                        statuscode = 400,
                        message = "Email is required."
                    })
                    { StatusCode = 400 };
                }

                using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
                await connection.OpenAsync();


                var existingCustomerCode = await connection.QueryFirstOrDefaultAsync<string>(
                    @"SELECT TOP 1 MCR_CustomerCode
              FROM MMCS_CustomerRegistration
              CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails
              WHERE LOWER(LTRIM(RTRIM(Emails.value))) = LOWER(@Email)",
                    new { Email = email });

                Console.WriteLine($"existingCustomerCode: {existingCustomerCode}");

                if (!string.IsNullOrEmpty(existingCustomerCode))
                {
                    // ✅ Existing user → Login
                    var loginResult = await LoginUserAsync(email, null); // or passwordless version
                    return loginResult.StatusCode == 200
                        ? new OkObjectResult(loginResult)
                        : new ObjectResult(loginResult) { StatusCode = loginResult.StatusCode };
                }
                else
                {
                    // ✅ New user → register
                    var registrationDto = new RegistrationDto
                    {
                        McrCustomerEmail = email,
                        McrCustomerTelephoneNo = dto.PhoneNumber?.Trim(),
                        McrCustomerName = dto.CompanyName?.Trim(),
                        Address = dto.Address?.Trim(),
                        ModuleIds = dto.ModuleIds
                    };

                    return await SignUpUserAsync(registrationDto);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    statuscode = 500,
                    message = "Internal server error",
                    error = ex.Message
                })
                { StatusCode = 500 };
            }
        }


        //public async Task<IActionResult> SignUpUserViaGoogleAsync(GoogleAuthDto dto)

        //{
        //    try
        //    {
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(dto.Token, new GoogleJsonWebSignature.ValidationSettings
        //        {
        //            Audience = new[] { _configuration["GoogleAuth:ClientId"] } // your Google Client ID
        //        });

        //        var email = payload.Email;
        //        var fullName = payload.Name;

        //        // You may also fetch `payload.Subject` for Google ID (if needed)

        //        var registerModel = new RegistrationDto
        //        {
        //            McrCustomerEmail = email,
        //            McrCustomerName = fullName ?? "Google User",
        //           // McrCustomerTelephoneNo = "0000000000" // Placeholder or fetch from frontend if needed
        //        };

        //        // You can now call your existing method OR move common logic into a helper
        //        return await SignUpUserAsync(registerModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ObjectResult(new
        //        {
        //            statuscode = 401,
        //            message = "Google ID Token validation failed.",
        //            error = ex.Message
        //        })
        //        { StatusCode = 401 };
        //    }
        //}
        public static void AddOrUpdateConnectionString(
                string connectionName,
                string server,
                string database,
                string userId,
                string password)
        {
            string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("appsettings.json not found in the project root directory.");
            }

            // Read existing appsettings.json
            var json = File.ReadAllText(filePath);
            var jObject = JObject.Parse(json);

            // Ensure ConnectionStrings section exists
            if (jObject["ConnectionStrings"] == null)
                jObject["ConnectionStrings"] = new JObject();

            var connectionStrings = (JObject)jObject["ConnectionStrings"];

            // Build connection string
            string connString = $"Server={server};Database={database};User Id={userId};Password={password};TrustServerCertificate=True;MultipleActiveResultSets=True;";

            // Add or update
            connectionStrings[connectionName] = connString;

            // Write changes back to file (formatted)
            File.WriteAllText(filePath, jObject.ToString(Newtonsoft.Json.Formatting.Indented));

            Console.WriteLine($"✅ Connection string '{connectionName}' saved successfully!");
        }


        public async Task RestoreDatabaseFromBackup(string newDbName)
        {
            try
            {
                string masterConnection = _configuration.GetConnectionString("MasterConnection");

                if (string.IsNullOrEmpty(masterConnection))
                    throw new Exception("MasterConnection missing in appsettings.json");

                string backupPath = @"C:\Program Files\backups\TRACEPadefault.bak";

                if (!File.Exists(backupPath))
                    throw new Exception("Backup file NOT FOUND: " + backupPath);

                using var connection = new SqlConnection(masterConnection);
                await connection.OpenAsync();

                string restoreSql = $@"
ALTER DATABASE [{newDbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

RESTORE DATABASE [{newDbName}]
FROM DISK = '{backupPath}'
WITH REPLACE,
MOVE '{newDbName}' TO 'C:\Program Files (x86)\Plesk\Databases\MSSQL\MSSQL15.MSSQLSERVER2019\MSSQL\Backup\{newDbName}.mdf',
MOVE '{newDbName}_log' TO 'C:\Program Files (x86)\Plesk\Databases\MSSQL\MSSQL15.MSSQLSERVER2019\MSSQL\Backup\{newDbName}.ldf';

ALTER DATABASE [{newDbName}] SET MULTI_USER;
";

                // 🔥 Use Retry Here
                await ExecuteWithRetry(async () =>
                {
                    await connection.ExecuteAsync(restoreSql);
                }, retries: 5, delayMs: 1500);
            }
            catch (Exception ex)
            {
                throw new Exception($"Restore failed for DB '{newDbName}': {ex.Message}", ex);
            }
        }

        public async Task ExecuteWithRetry(Func<Task> action, int retries = 5, int delayMs = 1000)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    await action();
                    return;
                }
                catch (SqlException)
                {
                    attempt++;

                    if (attempt > retries)
                        throw;

                    await Task.Delay(delayMs);
                }
            }
        }

        public async Task<IActionResult> SignUpUserAsync(RegistrationDto registerModel)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
                await connection.OpenAsync();

                // Step 1: Check if customer already exists
                var existingCustomer = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT COUNT(1) 
              FROM MMCS_CustomerRegistration 
              WHERE ',' + MCR_emails + ',' LIKE @EmailPattern 
                 OR MCR_CustomerTelephoneNo = @Phone",
                    new
                    {
                        EmailPattern = "%," + registerModel.McrCustomerEmail + ",%",
                        Phone = registerModel.McrCustomerTelephoneNo
                    });

                if (existingCustomer > 0)
                {
                    return new ConflictObjectResult(new
                    {
                        statuscode = 409,
                        message = "Customer with this email or phone number already exists."
                    });
                }

                // Step 2: Generate Customer Code and IDs
                int maxMcrId = (await connection.ExecuteScalarAsync<int?>(
                    "SELECT ISNULL(MAX(MCR_ID), 0) FROM MMCS_CustomerRegistration") ?? 0) + 1;

                string currentYear = DateTime.Now.ToString("yy");
                string yearPrefix = $"TR{currentYear}";

                // Get latest MCR_CustomerCode with the year prefix
                string latestCode = await connection.ExecuteScalarAsync<string>(
                    @"SELECT TOP 1 MCR_CustomerCode 
              FROM MMCS_CustomerRegistration 
              WHERE MCR_CustomerCode LIKE @PrefixPattern 
              ORDER BY TRY_CAST(RIGHT(MCR_CustomerCode, LEN(MCR_CustomerCode) - LEN(@PrefixWithUnderscore)) AS INT) DESC",
                    new
                    {
                        PrefixPattern = yearPrefix + "_%",
                        PrefixWithUnderscore = yearPrefix + "_"
                    });

                int nextNumber = 1;
                if (!string.IsNullOrEmpty(latestCode))
                {
                    var parts = latestCode.Split('_');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                string newCustomerCode = $"{yearPrefix}_{nextNumber:D3}";
                string productKey = $"PRD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

                // Step 3: Insert into MMCS_CustomerRegistration
                //    string insertSql = @"
                //INSERT INTO MMCS_CustomerRegistration 
                //(MCR_ID, MCR_MP_ID, MCR_CustomerName, MCR_CustomerEmail, MCR_CustomerTelephoneNo, 
                // MCR_Status, MCR_TStatus, MCR_CustomerCode, MCR_ProductKey, MCR_emails, MCR_Address)
                //VALUES 
                //(@McrId, @McrMpId, @McrCustomerName, @McrCustomerEmail, @McrCustomerTelephoneNo, 
                // 'A', 'T', @McrCustomerCode, @McrProductKey, @MCR_emails, @Address)";

                //    var rowsInserted = await connection.ExecuteAsync(insertSql, new
                //    {
                //        McrId = maxMcrId,
                //        McrMpId = 1,
                //        McrCustomerName = registerModel.McrCustomerName,
                //        McrCustomerEmail = registerModel.McrCustomerEmail,
                //        McrCustomerTelephoneNo = registerModel.McrCustomerTelephoneNo,
                //        McrCustomerCode = newCustomerCode,
                //        McrProductKey = productKey,
                //        Address = registerModel.Address,
                //        MCR_Emails = registerModel.McrCustomerEmail + ","
                //    });

                var parameters = new DynamicParameters();

                parameters.Add("@MCR_ID", maxMcrId);
                parameters.Add("@MCR_MP_ID", 1);
                parameters.Add("@MCR_CustomerName", registerModel.McrCustomerName);
                parameters.Add("@MCR_CustomerCode", newCustomerCode);
                parameters.Add("@MCR_CustomerEmail", registerModel.McrCustomerEmail);
                parameters.Add("@MCR_CustomerTelephoneNo", registerModel.McrCustomerTelephoneNo);
                parameters.Add("@MCR_ContactPersonName", "");
                parameters.Add("@MCR_ContactPersonPhoneNo", "");
                parameters.Add("@MCR_ContactPersonEmail", "");
                parameters.Add("@MCR_GSTNo", "");
                parameters.Add("@MCR_NumberOfUsers", 0);
                parameters.Add("@MCR_Address", registerModel.Address);
                parameters.Add("@MCR_City", "");
                parameters.Add("@MCR_State", "");
                parameters.Add("@MCR_BillingFrequency", 1);
                parameters.Add("@MCR_FromDate", null);
                parameters.Add("@MCR_ToDate", null);
                parameters.Add("@MCR_CreatedDate", null);
                parameters.Add("@MCR_Status", 'A');
                parameters.Add("@MCR_ProductKey", productKey);
                parameters.Add("@MCR_TStatus", 'T');
                parameters.Add("@mcr_emails", registerModel.McrCustomerEmail + ",");
                parameters.Add("@MCR_IPAddress", registerModel.MCR_IPAddress);
                parameters.Add("@MCR_Location", registerModel.MCR_Location);
                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var rowsInserted = await connection.ExecuteAsync("spMMCS_CustomerRegistration", parameters, commandType: CommandType.StoredProcedure);

                if (rowsInserted == 0)
                {
                    return new ObjectResult(new { statuscode = 500, message = "Failed to insert customer." }) { StatusCode = 500 };
                }

                Console.WriteLine("✅ Customer inserted via Dapper.");

                // Step 4: Create Customer Database
                await CreateCustomerDatabaseAsync(newCustomerCode);
                await Task.Delay(500);

                // Step 5: Clone Database from Template
                await CloneDatabaseAsync("TracepaScriptDB", newCustomerCode);

                // Step 6: Add Connection String
                AddOrUpdateConnectionString(
                    connectionName: newCustomerCode,
                    server: "142.93.217.23",
                    database: newCustomerCode,
                    userId: "Sa",
                    password: "Mmcs@736"
                );

                // Step 7: Setup new database connection
                string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
                string newDbConnectionString = string.Format(connectionStringTemplate, newCustomerCode);

                // Step 8: Insert Seed Configuration Data
                string seedConfigSql = $@"
            INSERT [dbo].[Sad_Config_Settings] ([SAD_Config_ID], [SAD_Config_Key], [SAD_Config_Value], [SAD_UpdatedBy], [SAD_UpdatedOn], [SAD_Config_Operation], [SAD_Config_IPAddress], [SAD_CompID]) VALUES 
            (1, N'ImgPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (2, N'ExcelPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\Tempfolder\', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (3, N'FilesInDB', N'False', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (4, N'HTP', N'http://', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (5, N'AppName', N'TRACe', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (6, N'FtpServer', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\FTPROOT\ROOT\', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (7, N'RDBMS', N'SQL', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (8, N'Currency', N'1', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (9, N'ErrorLog', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\ErrorLog\ErrorLog.txt', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (10, N'DateFormat', N'1', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (11, N'FileSize', N'7', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (12, N'TimeOut', N'40', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (13, N'TimeOutWarning', N'5', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (14, N'FileInDBPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\TRACePA Doc', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (15, N'OutlookEMail', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\Outlook\MSG', 3, GETDATE(), N'U', N'192.168.0.118', 1),
            (16, N'TempPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\', 1, GETDATE(), N'U', N'192.168.0.118', 1),
            (17, N'DisplayPath', N'https://tracepacore.multimedia.interactivedns.com/{newCustomerCode}/', 1, GETDATE(), N'U', N'192.168.0.118', 1);";

                await using var newDbConnection = new SqlConnection(newDbConnectionString);
                await newDbConnection.OpenAsync();
                await newDbConnection.ExecuteAsync(seedConfigSql);

                // Step 9: Insert Customer Modules
                string customerDbConnectionString = _configuration.GetConnectionString("CustomerRegistrationConnection");
                await using var customerConnection = new SqlConnection(customerDbConnectionString);
                await customerConnection.OpenAsync();

                int currentMaxMcmId = await customerConnection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX(MCM_ID), 0) FROM MMCS_CustomerModules");

                var moduleInsertList = registerModel.ModuleIds?.Select((moduleId, index) => new
                {
                    MCM_ID = currentMaxMcmId + index + 1,
                    MCM_MCR_ID = maxMcrId,
                    MCM_ModuleID = moduleId
                }).ToList();

                const string insertQuery = @"
            INSERT INTO MMCS_CustomerModules (MCM_ID, MCM_MCR_ID, MCM_ModuleID)
            VALUES (@MCM_ID, @MCM_MCR_ID, @MCM_ModuleID);";

                await customerConnection.ExecuteAsync(insertQuery, moduleInsertList);

                // Step 10: Create Admin User in new DB
                var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
                optionsBuilder.UseSqlServer(newDbConnectionString);

                using var newDbContext = new DynamicDbContext(optionsBuilder.Options);

                int maxUserId = (await newDbContext.SadUserDetails.MaxAsync(c => (int?)c.UsrId) ?? 0) + 1;

                var lastUserCode = await newDbContext.SadUserDetails
                    .Where(u => u.UsrCode.StartsWith("EMP"))
                    .OrderByDescending(u => u.UsrCode)
                    .Select(u => u.UsrCode)
                    .FirstOrDefaultAsync();

                int nextUserCodeNumber = 1;
                if (!string.IsNullOrEmpty(lastUserCode) && int.TryParse(lastUserCode.Substring(3), out int lastCodeNumber))
                {
                    nextUserCodeNumber = lastCodeNumber + 1;
                }

                string newUserCode = $"EMP{nextUserCodeNumber:D3}";
                string hashedPassword = EncryptPassword(newCustomerCode + "@" + DateTime.Now.Year);

                // Send welcome email
                SendWelcomeEmailAsync(registerModel.McrCustomerEmail, newCustomerCode + "@" + DateTime.Now.Year);

                var adminUser = new Models.UserModels.SadUserDetail
                {
                    UsrId = maxUserId,
                    UsrCode = newUserCode,
                    UsrNode = 2,
                    UsrFullName = "Admin",
                    UsrLoginName = "sa",
                    UsrPassWord = hashedPassword,
                    UsrEmail = registerModel.McrCustomerEmail,
                    UsrMobileNo = registerModel.McrCustomerTelephoneNo,
                    UsrDutyStatus = "A",
                    UsrDelFlag = "A",
                    UsrStatus = "U",
                    UsrType = "U",
                    UsrRole = 1,
                    UsrIsLogin = "Y",
                    UsrCompId = 1
                };

                await newDbContext.SadUserDetails.AddAsync(adminUser);
                await newDbContext.SaveChangesAsync();

                // Step 11: Add Permissions for Admin User
                await using var customerConnection1 = new SqlConnection(newDbConnectionString);
                await customerConnection1.OpenAsync();

                var operations = await customerConnection1.QueryAsync(
                    @"SELECT * FROM sad_Mod_Operations WHERE OP_Status = 'A' AND OP_CompID = @OP_CompID",
                    new { OP_CompID = 1 });

                foreach (var op in operations)
                {
                    await customerConnection1.ExecuteAsync(
                        @"DECLARE @NewId INT;
                  SELECT @NewId = ISNULL(MAX(Perm_PKID), 0) + 1 FROM SAD_UsrOrGrp_Permission;
                  INSERT INTO SAD_UsrOrGrp_Permission
                  (Perm_PKID, Perm_PType, Perm_UsrORGrpID, Perm_ModuleID, Perm_OpPKID, 
                   Perm_Status, Perm_Crby, Perm_Cron, Perm_Operation, Perm_IPAddress, Perm_CompID)
                  VALUES
                  (@NewId, @Perm_PType, @Perm_UsrORGrpID, @Perm_ModuleID, @Perm_OpPKID, 
                   'A', @Perm_Crby, GETDATE(), 'C', '', @Perm_CompID);",
                        new
                        {
                            Perm_PType = 'R',
                            Perm_UsrORGrpID = 1,
                            Perm_ModuleID = op.OP_ModuleID,
                            Perm_OpPKID = op.OP_PKID,
                            Perm_Crby = maxUserId,
                            Perm_CompID = 1
                        });
                }
                string sql = @"
        IF NOT EXISTS (
            SELECT 1 FROM sys.default_constraints 
            WHERE name = 'DF_UserTokens_IsRevoked'
        )
        BEGIN
            ALTER TABLE UserTokens
            ADD CONSTRAINT DF_UserTokens_IsRevoked DEFAULT(0) FOR IsRevoked;
        END
    ";

                await customerConnection1.ExecuteAsync(sql);

                return new OkObjectResult(new
                {
                    statuscode = 201,
                    message = "Customer registered successfully.",
                    code = newCustomerCode
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    statuscode = 500,
                    message = "Internal server error",
                    error = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }

        //working code 02-12-2025
        //    public async Task<IActionResult> SignUpUserAsync(RegistrationDto registerModel)
        //    {
        //        try
        //        {
        //            using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
        //            await connection.OpenAsync();

        //            // Step 1: Check if customer already exists
        //            var existingCustomer = await connection.QueryFirstOrDefaultAsync<int>(
        //                @"SELECT COUNT(1) 
        //          FROM MMCS_CustomerRegistration 
        //          WHERE ',' + MCR_emails + ',' LIKE @EmailPattern 
        //             OR MCR_CustomerTelephoneNo = @Phone",
        //                new
        //                {
        //                    EmailPattern = "%," + registerModel.McrCustomerEmail + ",%",
        //                    Phone = registerModel.McrCustomerTelephoneNo
        //                });

        //            if (existingCustomer > 0)
        //            {
        //                return new ConflictObjectResult(new
        //                {
        //                    statuscode = 409,
        //                    message = "Customer with this email or phone number already exists."
        //                });
        //            }

        //            // Step 2: Generate Customer Code and IDs
        //            int maxMcrId = (await connection.ExecuteScalarAsync<int?>(
        //                "SELECT ISNULL(MAX(MCR_ID), 0) FROM MMCS_CustomerRegistration") ?? 0) + 1;

        //            string currentYear = DateTime.Now.ToString("yy");
        //            string yearPrefix = $"TR{currentYear}";

        //            // Get latest MCR_CustomerCode with the year prefix
        //            string latestCode = await connection.ExecuteScalarAsync<string>(
        //                @"SELECT TOP 1 MCR_CustomerCode 
        //          FROM MMCS_CustomerRegistration 
        //          WHERE MCR_CustomerCode LIKE @PrefixPattern 
        //          ORDER BY TRY_CAST(RIGHT(MCR_CustomerCode, LEN(MCR_CustomerCode) - LEN(@PrefixWithUnderscore)) AS INT) DESC",
        //                new
        //                {
        //                    PrefixPattern = yearPrefix + "_%",
        //                    PrefixWithUnderscore = yearPrefix + "_"
        //                });

        //            int nextNumber = 1;
        //            if (!string.IsNullOrEmpty(latestCode))
        //            {
        //                var parts = latestCode.Split('_');
        //                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
        //                {
        //                    nextNumber = lastNumber + 1;
        //                }
        //            }

        //            string newCustomerCode = $"{yearPrefix}_{nextNumber:D3}";
        //            string productKey = $"PRD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

        //            // Step 3: Insert into MMCS_CustomerRegistration
        //            string insertSql = @"
        //        INSERT INTO MMCS_CustomerRegistration 
        //        (MCR_ID, MCR_MP_ID, MCR_CustomerName, MCR_CustomerEmail, MCR_CustomerTelephoneNo, 
        //         MCR_Status, MCR_TStatus, MCR_CustomerCode, MCR_ProductKey, MCR_emails, MCR_Address)
        //        VALUES 
        //        (@McrId, @McrMpId, @McrCustomerName, @McrCustomerEmail, @McrCustomerTelephoneNo, 
        //         'A', 'T', @McrCustomerCode, @McrProductKey, @MCR_emails, @Address)";

        //            var rowsInserted = await connection.ExecuteAsync(insertSql, new
        //            {
        //                McrId = maxMcrId,
        //                McrMpId = 1,
        //                McrCustomerName = registerModel.McrCustomerName,
        //                McrCustomerEmail = registerModel.McrCustomerEmail,
        //                McrCustomerTelephoneNo = registerModel.McrCustomerTelephoneNo,
        //                McrCustomerCode = newCustomerCode,
        //                McrProductKey = productKey,
        //                Address = registerModel.Address,
        //                MCR_Emails = registerModel.McrCustomerEmail + ","
        //            });

        //            if (rowsInserted == 0)
        //            {
        //                return new ObjectResult(new { statuscode = 500, message = "Failed to insert customer." }) { StatusCode = 500 };
        //            }

        //            Console.WriteLine("✅ Customer inserted via Dapper.");

        //            // Step 4: Create Customer Database
        //            await CreateCustomerDatabaseAsync(newCustomerCode);
        //            await Task.Delay(500);

        //            // Step 5: Clone Database from Template
        //            await CloneDatabaseAsync("TracepaScriptDB", newCustomerCode);

        //            // Step 6: Add Connection String
        //            AddOrUpdateConnectionString(
        //                connectionName: newCustomerCode,
        //                server: "142.93.217.23",
        //                database: newCustomerCode,
        //                userId: "Sa",
        //                password: "Mmcs@736"
        //            );

        //            // Step 7: Setup new database connection
        //            string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
        //            string newDbConnectionString = string.Format(connectionStringTemplate, newCustomerCode);

        //            // Step 8: Insert Seed Configuration Data
        //            string seedConfigSql = $@"
        //        INSERT [dbo].[Sad_Config_Settings] ([SAD_Config_ID], [SAD_Config_Key], [SAD_Config_Value], [SAD_UpdatedBy], [SAD_UpdatedOn], [SAD_Config_Operation], [SAD_Config_IPAddress], [SAD_CompID]) VALUES 
        //        (1, N'ImgPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (2, N'ExcelPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\Tempfolder\', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (3, N'FilesInDB', N'False', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (4, N'HTP', N'http://', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (5, N'AppName', N'TRACe', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (6, N'FtpServer', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\FTPROOT\ROOT\', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (7, N'RDBMS', N'SQL', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (8, N'Currency', N'1', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (9, N'ErrorLog', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\ErrorLog\ErrorLog.txt', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (10, N'DateFormat', N'1', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (11, N'FileSize', N'7', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (12, N'TimeOut', N'40', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (13, N'TimeOutWarning', N'5', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (14, N'FileInDBPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\TRACePA Doc', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (15, N'OutlookEMail', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\Outlook\MSG', 3, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (16, N'TempPath', N'C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\{newCustomerCode}\', 1, GETDATE(), N'U', N'192.168.0.118', 1),
        //        (17, N'DisplayPath', N'https://tracepacore.multimedia.interactivedns.com/{newCustomerCode}/', 1, GETDATE(), N'U', N'192.168.0.118', 1);";

        //            await using var newDbConnection = new SqlConnection(newDbConnectionString);
        //            await newDbConnection.OpenAsync();
        //            await newDbConnection.ExecuteAsync(seedConfigSql);

        //            // Step 9: Insert Customer Modules
        //            string customerDbConnectionString = _configuration.GetConnectionString("CustomerRegistrationConnection");
        //            await using var customerConnection = new SqlConnection(customerDbConnectionString);
        //            await customerConnection.OpenAsync();

        //            int currentMaxMcmId = await customerConnection.ExecuteScalarAsync<int>(
        //                "SELECT ISNULL(MAX(MCM_ID), 0) FROM MMCS_CustomerModules");

        //            var moduleInsertList = registerModel.ModuleIds?.Select((moduleId, index) => new
        //            {
        //                MCM_ID = currentMaxMcmId + index + 1,
        //                MCM_MCR_ID = maxMcrId,
        //                MCM_ModuleID = moduleId
        //            }).ToList();

        //            const string insertQuery = @"
        //        INSERT INTO MMCS_CustomerModules (MCM_ID, MCM_MCR_ID, MCM_ModuleID)
        //        VALUES (@MCM_ID, @MCM_MCR_ID, @MCM_ModuleID);";

        //            await customerConnection.ExecuteAsync(insertQuery, moduleInsertList);

        //            // Step 10: Create Admin User in new DB
        //            var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
        //            optionsBuilder.UseSqlServer(newDbConnectionString);

        //            using var newDbContext = new DynamicDbContext(optionsBuilder.Options);

        //            int maxUserId = (await newDbContext.SadUserDetails.MaxAsync(c => (int?)c.UsrId) ?? 0) + 1;

        //            var lastUserCode = await newDbContext.SadUserDetails
        //                .Where(u => u.UsrCode.StartsWith("EMP"))
        //                .OrderByDescending(u => u.UsrCode)
        //                .Select(u => u.UsrCode)
        //                .FirstOrDefaultAsync();

        //            int nextUserCodeNumber = 1;
        //            if (!string.IsNullOrEmpty(lastUserCode) && int.TryParse(lastUserCode.Substring(3), out int lastCodeNumber))
        //            {
        //                nextUserCodeNumber = lastCodeNumber + 1;
        //            }

        //            string newUserCode = $"EMP{nextUserCodeNumber:D3}";
        //            string hashedPassword = EncryptPassword(newCustomerCode + "@" + DateTime.Now.Year);

        //            // Send welcome email
        //            SendWelcomeEmailAsync(registerModel.McrCustomerEmail, newCustomerCode + "@" + DateTime.Now.Year);

        //            var adminUser = new Models.UserModels.SadUserDetail
        //            {
        //                UsrId = maxUserId,
        //                UsrCode = newUserCode,
        //                UsrNode = 2,
        //                UsrFullName = "Admin",
        //                UsrLoginName = "sa",
        //                UsrPassWord = hashedPassword,
        //                UsrEmail = registerModel.McrCustomerEmail,
        //                UsrMobileNo = registerModel.McrCustomerTelephoneNo,
        //                UsrDutyStatus = "A",
        //                UsrDelFlag = "A",
        //                UsrStatus = "U",
        //                UsrType = "U",
        //                UsrRole = 1,
        //                UsrIsLogin = "Y",
        //                UsrCompId = 1
        //            };

        //            await newDbContext.SadUserDetails.AddAsync(adminUser);
        //            await newDbContext.SaveChangesAsync();

        //            // Step 11: Add Permissions for Admin User
        //            await using var customerConnection1 = new SqlConnection(newDbConnectionString);
        //            await customerConnection1.OpenAsync();

        //            var operations = await customerConnection1.QueryAsync(
        //                @"SELECT * FROM sad_Mod_Operations WHERE OP_Status = 'A' AND OP_CompID = @OP_CompID",
        //                new { OP_CompID = 1 });

        //            foreach (var op in operations)
        //            {
        //                await customerConnection1.ExecuteAsync(
        //                    @"DECLARE @NewId INT;
        //              SELECT @NewId = ISNULL(MAX(Perm_PKID), 0) + 1 FROM SAD_UsrOrGrp_Permission;
        //              INSERT INTO SAD_UsrOrGrp_Permission
        //              (Perm_PKID, Perm_PType, Perm_UsrORGrpID, Perm_ModuleID, Perm_OpPKID, 
        //               Perm_Status, Perm_Crby, Perm_Cron, Perm_Operation, Perm_IPAddress, Perm_CompID)
        //              VALUES
        //              (@NewId, @Perm_PType, @Perm_UsrORGrpID, @Perm_ModuleID, @Perm_OpPKID, 
        //               'A', @Perm_Crby, GETDATE(), 'C', '', @Perm_CompID);",
        //                    new
        //                    {
        //                        Perm_PType = 'R',
        //                        Perm_UsrORGrpID = 1,
        //                        Perm_ModuleID = op.OP_ModuleID,
        //                        Perm_OpPKID = op.OP_PKID,
        //                        Perm_Crby = maxUserId,
        //                        Perm_CompID = 1
        //                    });
        //            }
        //            string sql = @"
        //    IF NOT EXISTS (
        //        SELECT 1 FROM sys.default_constraints 
        //        WHERE name = 'DF_UserTokens_IsRevoked'
        //    )
        //    BEGIN
        //        ALTER TABLE UserTokens
        //        ADD CONSTRAINT DF_UserTokens_IsRevoked DEFAULT(0) FOR IsRevoked;
        //    END
        //";

        //            await customerConnection1.ExecuteAsync(sql);

        //            return new OkObjectResult(new
        //            {
        //                statuscode = 201,
        //                message = "Customer registered successfully.",
        //                code = newCustomerCode
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            return new ObjectResult(new
        //            {
        //                statuscode = 500,
        //                message = "Internal server error",
        //                error = ex.Message
        //            })
        //            {
        //                StatusCode = 500
        //            };
        //        }
        //    }

        // Database Cloning Method
        private async Task CloneDatabaseAsync(string sourceDb, string newDb)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("MasterConnection"));
            await connection.OpenAsync();

            try
            {
                string cloningScript = $@"
            DECLARE @SourceDB SYSNAME = '{sourceDb}';
            DECLARE @NewDB SYSNAME   = '{newDb}';

            -- 1. Clone TABLES
            DECLARE @tbl NVARCHAR(300), @sql NVARCHAR(MAX);
            DECLARE cur CURSOR FOR
            SELECT QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME)
            FROM   [{sourceDb}].INFORMATION_SCHEMA.TABLES
            WHERE  TABLE_TYPE = 'BASE TABLE';

            OPEN cur;
            FETCH NEXT FROM cur INTO @tbl;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SET @sql = 'SELECT * INTO [' + @NewDB + '].' + @tbl + ' FROM [' + @SourceDB + '].' + @tbl + ';';
                EXEC(@sql);
                FETCH NEXT FROM cur INTO @tbl;
            END
            CLOSE cur;
            DEALLOCATE cur;

            -- 2. Clone VIEWS, STORED PROCEDURES, FUNCTIONS, and TRIGGERS
            DECLARE @object_type CHAR(2), @object_name NVARCHAR(300), @object_definition NVARCHAR(MAX);
            DECLARE object_cur CURSOR FOR
            SELECT 
                o.type as object_type,
                QUOTENAME(SCHEMA_NAME(o.schema_id)) + '.' + QUOTENAME(o.name) as object_name,
                sm.definition
            FROM   [{sourceDb}].sys.sql_modules sm
            JOIN   [{sourceDb}].sys.objects o ON o.object_id = sm.object_id
            WHERE  o.type IN ('V', 'P', 'FN', 'TF', 'IF', 'TR');

            OPEN object_cur;
            FETCH NEXT FROM object_cur INTO @object_type, @object_name, @object_definition;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                IF CHARINDEX('USE ', @object_definition) > 0
                BEGIN
                    SET @object_definition = REPLACE(@object_definition, 'USE {sourceDb}', 'USE {newDb}');
                END
                
                DECLARE @exec_sql NVARCHAR(MAX) = 'USE [' + @NewDB + ']; EXEC(''' + REPLACE(@object_definition, '''', '''''') + ''')';
                
                BEGIN TRY
                    EXEC(@exec_sql);
                END TRY
                BEGIN CATCH
                    PRINT 'Error creating ' + 
                        CASE @object_type 
                            WHEN 'V' THEN 'view'
                            WHEN 'P' THEN 'stored procedure' 
                            WHEN 'FN' THEN 'function'
                            WHEN 'TF' THEN 'function'
                            WHEN 'IF' THEN 'function'
                            WHEN 'TR' THEN 'trigger'
                        END + ' ' + @object_name + ': ' + ERROR_MESSAGE();
                END CATCH
                
                FETCH NEXT FROM object_cur INTO @object_type, @object_name, @object_definition;
            END

            CLOSE object_cur;
            DEALLOCATE object_cur;

            PRINT 'Database cloning completed successfully from ' + @SourceDB + ' to ' + @NewDB;";

                using var command = new SqlCommand(cloningScript, connection);
                command.CommandTimeout = 0;
                await command.ExecuteNonQueryAsync();

                Console.WriteLine($"✅ Database cloned successfully from {sourceDb} to {newDb}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database cloning failed: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> SendWelcomeEmailAsync(string gmail, string password)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:FromEmail"];
            var fromName = _configuration["Smtp:FromName"];
            var portalUrl = _configuration["TracePA:PortalUrl"];

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", gmail));
                message.Subject = "Welcome to TracePA – Your Login Credentials";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                    <html>
                    <body style='font-family:Segoe UI,Arial,sans-serif; color:#222;'>
                        <p>Dear customer,</p>
                        <p>Welcome to <strong>TracePA</strong>!</p>
                        <p>Your account has been successfully created and you can now access the TracePA portal.</p>

                        <h3>Login Details:</h3>
                        <table cellpadding='6' cellspacing='0' style='border-collapse:collapse;'>
                          <tr><td><b>URL:</b></td><td><a href='{portalUrl}'>{portalUrl}</a></td></tr>
                          <tr><td><b>Username:</b></td><td>{gmail}</td></tr>
                          <tr><td><b>Password:</b></td><td>{password}</td></tr>
                        </table>

                        <p><strong>Important:</strong> Please do not share these credentials with others.</p>
                        <p>If you need any support, feel free to reach out.</p>
                        <p>Best regards,<br/>TracePA Team</p>
                    </body>
                    </html>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email sending failed: {ex.Message}");
                return false;
            }
        }

        private async Task CreateCustomerDatabaseAsync(string customerCode)
        {
            string dbName = customerCode.Replace("-", "_"); // Replace hyphen for safety
            string customerConnectionString = _configuration.GetConnectionString("CustomerConnection");

            using (var connection = new SqlConnection(customerConnectionString))
            {
                await connection.OpenAsync();

                string createDbQuery = $@"
        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbName) 
        BEGIN 
            CREATE DATABASE [{dbName}] 
        END";

                using (var command = new SqlCommand(createDbQuery, connection))
                {
                    command.Parameters.AddWithValue("@dbName", dbName);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // ✅ Optimized SQL script execution
        //public async Task ExecuteAllSqlScriptsAsync(string connectionString, string scriptFilePath)
        //{
        //    try
        //    {
        //        if (!File.Exists(scriptFilePath))
        //        {
        //            Console.WriteLine($"File not found: {scriptFilePath}");
        //            return;
        //        }

        //        string script = await File.ReadAllTextAsync(scriptFilePath);
        //        string[] commands = Regex.Split(script, @"(?i)^\s*GO\s*$", RegexOptions.Multiline);

        //        using (var connection = new SqlConnection(connectionString))
        //        {
        //            await connection.OpenAsync();
        //            using (var transaction = connection.BeginTransaction())
        //            {
        //                try
        //                {
        //                    foreach (string commandText in commands)
        //                    {
        //                        string trimmedCommand = commandText.Trim();
        //                        if (!string.IsNullOrEmpty(trimmedCommand))
        //                        {
        //                            try
        //                            {
        //                                using (var command = new SqlCommand(trimmedCommand, connection, transaction))
        //                                {
        //                                    command.CommandTimeout = 120;

        //                                    // Optional: Handle IDENTITY_INSERT if needed
        //                                    if (trimmedCommand.Contains("INSERT INTO") && trimmedCommand.Contains("IDENTITY"))
        //                                    {
        //                                        await new SqlCommand("SET IDENTITY_INSERT ON;", connection, transaction).ExecuteNonQueryAsync();
        //                                    }

        //                                    await command.ExecuteNonQueryAsync();
        //                                }
        //                            }
        //                            catch (SqlException sqlEx)
        //                            {
        //                                Console.WriteLine($"SQL Execution Error: {sqlEx.Message}\nCommand: {trimmedCommand}");
        //                                throw; // Rethrow to roll back
        //                            }
        //                        }
        //                    }

        //                    await transaction.CommitAsync();
        //                    Console.WriteLine("SQL script executed successfully.");
        //                }
        //                catch (Exception ex)
        //                {
        //                    await transaction.RollbackAsync();
        //                    Console.WriteLine($"Transaction failed: {ex.Message}");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"General error executing SQL script: {ex.Message}");
        //    }
        //}
        public async Task ExecuteAllSqlScriptsAsync(string connectionString, string scriptFilePath)
        {
            if (!File.Exists(scriptFilePath))
            {
                Console.WriteLine($"❌ File not found: {scriptFilePath}");
                return;
            }

            string script = await File.ReadAllTextAsync(scriptFilePath);
            string[] commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            Console.WriteLine($"✅ Connected to DB: {connection.Database}");

            foreach (var commandText in commands)
            {
                string trimmed = commandText.Trim();

                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Skip USE statements because connection already points to correct DB
                if (trimmed.StartsWith("USE ", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    await using var command = new SqlCommand(trimmed, connection)
                    {
                        CommandTimeout = 300 // increase if script is big
                    };
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"✅ Executed batch successfully");
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"❌ SQL Error: {sqlEx.Message}\nBatch:\n{trimmed}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ General Error: {ex.Message}\nBatch:\n{trimmed}");
                    throw;
                }
            }

            Console.WriteLine("✅ All SQL script batches executed successfully.");
        }







        //public async Task<LoginResponse> AuthenticateUserAsync(string email, string password)
        //{
        //    try
        //    {
        //        // ✅ Step 1: Find Customer Code from Registration Database
        //        var customer = await _customerRegistrationContext.MmcsCustomerRegistrations

        //            .Where(c => c.McrCustomerEmail == email)
        //            .Select(c => new { c.McrCustomerCode })
        //            .FirstOrDefaultAsync();

        //        if (customer == null)
        //        {
        //            return new LoginResponse { StatusCode = 404, Message = "Email not found" };
        //        }

        //        // ✅ Step 2: Get Connection String for Customer Database
        //        string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
        //        string newDbConnectionString = string.Format(connectionStringTemplate, customer.McrCustomerCode);

        //        // ✅ Step 3: Use DynamicDbContext with new Connection String
        //        var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
        //        optionsBuilder.UseSqlServer(newDbConnectionString);

        //        using (var newDbContext = new DynamicDbContext(optionsBuilder.Options))
        //        {
        //            // ✅ Step 4: Fetch user from the correct database
        //            var userDto = await newDbContext.SadUserDetails
        //                .AsNoTracking()
        //                .Where(u => u.UsrEmail == email)
        //                .Select(u => new LoginDto
        //                {
        //                    UsrEmail = u.UsrEmail,
        //                    UsrPassWord = u.UsrPassWord
        //                })
        //                .SingleOrDefaultAsync();

        //            if (userDto == null)
        //            {
        //                return new LoginResponse { StatusCode = 404, Message = "Invalid Email" };
        //            }

        //            // ✅ Debugging: Log stored hashed password
        //            Console.WriteLine($"[DEBUG] Stored Hashed Password: {userDto.UsrPassWord}");

        //            // ✅ Step 5: Verify Password
        //            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, userDto.UsrPassWord);

        //            if (!isPasswordValid)
        //            {
        //                return new LoginResponse { StatusCode = 401, Message = "Invalid Password" };
        //            }

        //            // ✅ Step 6: Fetch User ID
        //            var userId = await newDbContext.SadUserDetails
        //                .AsNoTracking()
        //                .Where(a => a.UsrEmail == email)
        //                .Select(a => a.UsrId)
        //                .FirstOrDefaultAsync();



        //            // ✅ Step 7: Generate JWT Token
        //            //string token = GenerateJwtToken(userDto);




        //            return new LoginResponse
        //            {
        //                StatusCode = 200,
        //                Message = "Login successful",
        //                Token = token,
        //                UsrId = userId,
        //                CustomerCode = customer.McrCustomerCode


        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[ERROR] Exception in AuthenticateUserAsync: {ex.Message}");
        //        return new LoginResponse
        //        {
        //            StatusCode = 500,
        //            Message = $"An error occurred: {ex.Message}"
        //        };
        //    }
        //}


        //public string GenerateJwtToken(LoginDto userDto)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var secretKey = _configuration["JwtSettings:Secret"];
        //        var issuer = _configuration["JwtSettings:Issuer"];
        //        var audience = _configuration["JwtSettings:Audience"];
        //        var expiryInHoursString = _configuration["JwtSettings:ExpiryInHours"];

        //        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) ||
        //            string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(expiryInHoursString))
        //        {
        //            throw new Exception("JWT configuration values are missing.");
        //        }

        //        int expiryInHours = int.Parse(expiryInHoursString);
        //        var key = Encoding.UTF8.GetBytes(secretKey);

        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(new[]
        //            {
        //        //new Claim(ClaimTypes.NameIdentifier, userDto.UsrId.ToString()),
        //        new Claim(ClaimTypes.Email, userDto.UsrEmail)
        //    }),
        //            Expires = DateTime.UtcNow.AddHours(expiryInHours),
        //            Issuer = issuer,
        //            Audience = audience,
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //        };

        //        var token = tokenHandler.CreateToken(tokenDescriptor);
        //        return tokenHandler.WriteToken(token);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error generating JWT token: {ex.Message}");
        //    }
        //}

        public async Task<(bool Success, string Message, string? OtpToken)> GenerateAndSendOtpJwtAsync(string email)
        {
            var (success, message, otpToken, _) = await _otpService.GenerateAndSendOtpJwtAsync(email);

            return (success, message, otpToken);
        }

        public async Task<(bool Success, string Message, string? OtpToken)> ForgPassSendOtpJwtAsync(string email)
        {
            var (success, message, otpToken, _) = await _otpService.ForgetPassSendOtpJwtAsync(email);

            return (success, message, otpToken);
        }
        //public async Task<(bool Success, string Message, string? OtpToken)> GenerateAndSendOtpJwtAsync(string email)
        //{
        //    return await _otpService.GenerateAndSendOtpJwtAsync(email);
        //}


        //public async Task<(string Token, string Otp)> GenerateAndSendOtpJwtAsync(string email)
        //{
        //    return await _otpService.GenerateAndSendOtpJwtAsync(email);
        //}


        public async Task<bool> VerifyOtpJwtAsync(string token, string enteredOtp)
        {
            return await Task.FromResult(_otpService.VerifyOtpJwt(token, enteredOtp)); // ✅ Use await correctly


        }

        public string GetLocalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "Unavailable";
        }

        public async Task<LoginResponse> LoginUserAsync(string email, string password)
        {
            try
            {
                email = email?.Trim().ToLower();
                password = password?.Trim();

                using var regConnection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
                await regConnection.OpenAsync();

                // Get customer code
                string customerCodeSql = @" SELECT TOP 1 MCR_CustomerCode 
                                            FROM mmcs_customerregistration 
                                            CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails 
                                            WHERE LTRIM(RTRIM(Emails.value)) = @Email";

                var customerCode = await regConnection.QuerySingleOrDefaultAsync<string>(customerCodeSql, new { Email = email });


                //Block login access for users whose trial or subscription has expired.
                //string sQuery = @"SELECT  CONVERT(varchar(10), MCR_ToDate, 103) AS MCR_ToDate
                //                            FROM mmcs_customerregistration
                //                            CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails 
                //                            WHERE LTRIM(RTRIM(Emails.value)) = @Email and MCR_TStatus = 'T' AND ( MCR_ToDate IS NULL OR MCR_ToDate < GetDate() ) ";
                //var blockUser = await regConnection.QuerySingleOrDefaultAsync<string?>(sQuery, new { Email = email });

                //if (blockUser != null)
                //{
                //    await _errorLogger.LogErrorAsync(new ErrorLogDto
                //    {
                //        FormName = "Login",
                //        Controller = "Auth",
                //        Action = "LoginUser",
                //        ErrorMessage = "Trial Period Expired on '" + blockUser  + "'",
                //        StackTrace = "",
                //        UserId =  0,
                //        CustomerId = 0,
                //        Description = $"Failed login attempt for {email}",
                //        ResponseTime = 0
                //    });

                //    return new LoginResponse
                //    {
                //        StatusCode = 401,
                //        Message = "Your trial period expired on - '" + blockUser + "'. Please renew to continue."
                //    };
                //}
                




                // Connect to customer's DB
                string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
                string customerDbConnection = string.Format(connectionStringTemplate, customerCode);

                using var connection = new SqlConnection(customerDbConnection);
                await connection.OpenAsync();

                string plainEmail = email.Trim().ToLower();

                // Fetch user details
                var user = await connection.QueryFirstOrDefaultAsync( @"SELECT 
                                        u.usr_Email AS UsrEmail, u.usr_Password AS UsrPassWord,  g.Mas_Description AS RoleName
                                        FROM Sad_UserDetails u LEFT JOIN SAD_GrpOrLvl_General_Master g  ON u.Usr_Role = g.Mas_ID
                                        WHERE LOWER(u.usr_Email) = @email",
                new { email = plainEmail });

                // Fetch user details with role
                //if (user == null || DecryptPassword(user.UsrPassWord) != password)
                // {
                //     return new LoginResponse
                //     {
                //         StatusCode = 401,
                //         Message = "Invalid username or password."
                //     };
                // }

                if (user == null || (password != null && DecryptPassword(user.UsrPassWord) != password))

                {
                    // Log failed login
                    await _errorLogger.LogErrorAsync(new ErrorLogDto
                    {
                        FormName = "Login",
                        Controller = "Auth",
                        Action = "LoginUser",
                        ErrorMessage = "Invalid username or password.",
                        StackTrace = "",
                        UserId = user?.UsrId ?? 0,
                        CustomerId = 0,
                        Description = $"Failed login attempt for {email}",
                        ResponseTime = 0
                    });

                    return new LoginResponse
                    {
                        StatusCode = 401,
                        Message = "Invalid username or password."
                    };
                }


                // Get user ID
                var userId = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT usr_Id FROM Sad_UserDetails WHERE LOWER(usr_Email) = @email",
                    new { email = plainEmail });

                var usertype = await connection.QueryFirstOrDefaultAsync<string>(
                @"select usr_Type from Sad_UserDetails where usr_id = @userId",
                new { userId = userId });


                string? userEmail = await connection.QueryFirstOrDefaultAsync<string>(
                    @"SELECT usr_Email FROM Sad_UserDetails WHERE usr_Id  = @UserId",
                    new { UserId = userId });


                var roleName = await connection.QueryFirstOrDefaultAsync<string>(
                        @"SELECT g.Mas_Description FROM Sad_UserDetails u
                        LEFT JOIN SAD_GrpOrLvl_General_Master g ON u.Usr_Role = g.Mas_ID WHERE LOWER(u.usr_Email) = @email",
               new { email = plainEmail });

                // Step 5: Check if user already logged in on another system
                //     var existingToken = await connection.QueryFirstOrDefaultAsync<string>(
                //         @"SELECT AccessToken 
                //        FROM UserTokens 
                //WHERE UserId = @UserId AND IsRevoked = 0
                //AND RefreshTokenExpiry > GETUTCDATE()", // still valid



                //new { UserId = userId });

                //if (!string.IsNullOrEmpty(existingToken))
                //{
                //    return new LoginResponse
                //    {
                //        StatusCode = 409, // Conflict
                //        Message = "User already logged in from another system.",
                //        UsrId = userId,

                //    };
                //}




                // Step 6: Generate JWT

                string accessToken = GenerateJwtToken(email, customerCode, userId);
                string refreshToken = Guid.NewGuid().ToString(); // Use JWT if you want, but GUID is fine too
                DateTime accessExpiry = DateTime.UtcNow.AddMinutes(15);  // Match with JWT 'exp'
                DateTime refreshExpiry = DateTime.UtcNow.AddDays(7);
                // Generate JWT
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Session.SetString("CustomerCode", customerCode);
                    httpContext.Session.SetInt32("UserId", userId);
                    _httpContextAccessor.HttpContext?.Session.SetString("IsLoggedIn", "true");
                }

                await InsertUserTokenAsync(userId, email, accessToken, refreshToken, accessExpiry, refreshExpiry, customerCode);
                //  _httpContextAccessor.HttpContext?.Session.SetString("CustomerCode", customerCode);
                // _httpContextAccessor.HttpContext?.Session.SetString("IsLoggedIn", "true");
                 

                string token = GenerateJwtToken(email, customerCode, userId);

                // Get year info

                string? ymsId = null;
                int? ymsYearId = null;

                using (var yearConnection = new SqlConnection(customerDbConnection))
                {
                    await yearConnection.OpenAsync();
                    const string query = @"SELECT YMS_ID, YMS_YEARID FROM Year_Master WHERE YMS_Default = 1";
                    var yearResult = await yearConnection
                        .QueryFirstOrDefaultAsync<YearDto>(query);

                    if (yearResult != null)
                    {
                        ymsId = yearResult.YMS_ID;
                        ymsYearId = yearResult.YMS_YEARID;
                    }
                }


                string clientIp = httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? httpContext?.Connection?.RemoteIpAddress?.ToString();
                string systemIp = GetLocalIp();
                string customerDbConnectionString = _configuration.GetConnectionString("CustomerRegistrationConnection");

                await using var customerConnection = new SqlConnection(customerDbConnectionString);
                await customerConnection.OpenAsync();

                var customerInfo = await customerConnection.QueryFirstOrDefaultAsync<(string CustomerCode, int CustomerId)>(
                    @"SELECT TOP 1  MCR_CustomerCode, MCR_ID FROM mmcs_customerregistration
                    CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails WHERE LTRIM(RTRIM(Emails.value)) = @Email",
                    new { Email = email });

                if (customerInfo.CustomerCode == null)
                    throw new Exception("Customer not found.");

                int customerId = customerInfo.CustomerId;
                customerCode = customerInfo.CustomerCode;

           

                var savedModules = await customerConnection.QueryAsync<CustomerModuleDto>(@"SELECT 
                    MCM_ID AS MCM_ID, MCM_ModuleID AS MCM_ModuleID
                    FROM MMCS_CustomerModules
                    WHERE MCM_MCR_ID = @CustomerId",
              new { CustomerId = customerId });


                var mcmIds = await customerConnection.QueryAsync<int?>(
                @"SELECT MCM_ID FROM MMCS_CustomerModules WHERE MCM_MCR_ID = @CustomerId",
                new { CustomerId = customerId });
                var validMcmIds = mcmIds.Where(id => id.HasValue).Select(id => id.Value).ToList();

            
                bool activePlan;

                using (var cmd = new SqlCommand(@"
    SELECT
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM PaymentTransactions
                WHERE Database_Id = @DatabaseId
                  AND IsFreeTrial = 1
                  AND PaymentStatus = 'SUCCESS'
                  AND GETDATE() BETWEEN TrialStartDate AND TrialEndDate
            )
            THEN 1

            WHEN EXISTS (
                SELECT 1
                FROM PaymentTransactions
                WHERE Database_Id = @DatabaseId
                  AND IsFreeTrial = 0
                  AND Amount > 0
                  AND PaymentStatus = 'SUCCESS'
                  AND GETDATE() BETWEEN SubscriptionStart AND SubscriptionEnd
            )
            THEN 1

            ELSE 0
        END
", customerConnection))
                {
                    cmd.Parameters.AddWithValue("@DatabaseId", customerId); // or Database_Id value

                    activePlan = Convert.ToBoolean(await cmd.ExecuteScalarAsync());
                }



                return new LoginResponse
                {
                    StatusCode = 200,
                    Message = "Login successful",
                    Token = accessToken,
                    UsrId = userId,
                    usertype = usertype,
                    CustomerId = customerId,
                    UserEmail = userEmail,
                    RoleName = roleName,
                    // UserEmail = userEmail,
                    YmsId = ymsId,
                    YmsYearId = ymsYearId,
                    CustomerCode = customerCode,
                    PkIds = savedModules
                   .Where(x => x.MCM_ID.HasValue)
                   .Select(x => x.MCM_ID.Value)
                   .ToList(),

                    ModuleIds = savedModules
                    .Where(x => x.MCM_ModuleID.HasValue)
                    .Select(x => x.MCM_ModuleID.Value)
                    .ToList(),
                    ClientIpAddress = clientIp,
                    SystemIpAddress = systemIp,
                    ActivePlan = activePlan
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    StatusCode = 500,
                    Message = $"An error occurred: {ex.Message}",
                    ModuleIds = new List<int>()
                };
            }
        }



        public async Task<List<CustomerModuleDetailDto>> GetCustomerModulesAsync(int customerId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
            await connection.OpenAsync();

            var modules = await connection.QueryAsync<CustomerModuleDetailDto>(
                @"
        SELECT cm.MCM_ModuleID AS ModuleId, m.MP_ModuleName AS ModuleName
        FROM MMCS_CustomerModules cm
        INNER JOIN MMCS_MODULES m ON cm.MCM_ModuleID = m.MM_ID
        WHERE cm.MCM_MCR_ID = @CustomerId",
                new { CustomerId = customerId });

            return modules.ToList();
        }


        public async Task UpdateCustomerModulesAsync(int customerId, List<int> moduleIds)
        {
            if (moduleIds == null) moduleIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
            await connection.OpenAsync();

            // Fetch existing modules
            var existingModules = await connection.QueryAsync<int>(
                "SELECT MCM_ModuleID FROM MMCS_CustomerModules WHERE MCM_MCR_ID = @CustomerId",
                new { CustomerId = customerId });

            // Insert missing modules
            var modulesToInsert = moduleIds.Except(existingModules);
            foreach (var moduleId in modulesToInsert)
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO MMCS_CustomerModules (MCM_MCR_ID, MCM_ModuleID)
              VALUES (@CustomerId, @ModuleId)",
                    new { CustomerId = customerId, ModuleId = moduleId });
            }

            // Delete modules that are no longer selected
            var modulesToDelete = existingModules.Except(moduleIds);
            foreach (var moduleId in modulesToDelete)
            {
                await connection.ExecuteAsync(
                    "DELETE FROM MMCS_CustomerModules WHERE MCM_MCR_ID = @CustomerId AND MCM_ModuleID = @ModuleId",
                    new { CustomerId = customerId, ModuleId = moduleId });
            }
        }


        public async Task<bool> LogoutUserAsync(string accessToken)
        {
            const string query = @"
UPDATE UserTokens
SET IsRevoked = 1,
    RevokedAt = GETUTCDATE()
WHERE UserId = @UserId
  AND AccessToken = @AccessToken
  AND IsRevoked = 0";

            // ✅ Step 1: Decode JWT to get UserId + CustomerCode
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken;

            try
            {
                jwtToken = handler.ReadJwtToken(accessToken);
            }
            catch
            {
                throw new InvalidOperationException("Invalid JWT token.");

            }


            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value
                      ?? jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("JWT token does not contain UserId.");

            var customerCode = jwtToken.Claims.FirstOrDefault(c => c.Type == "CustomerCode")?.Value;
            if (string.IsNullOrWhiteSpace(customerCode))
                throw new InvalidOperationException("JWT token does not contain CustomerCode.");

            // ✅ Step 2: Build customer-specific connection string here
            string? connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");

            if (string.IsNullOrWhiteSpace(connectionStringTemplate))
                throw new InvalidOperationException("NewDatabaseTemplate connection string is missing in configuration.");

            string customerDbConnection = string.Format(connectionStringTemplate, customerCode);

            // ✅ Step 3: Run revoke query
            await using var connection = new SqlConnection(customerDbConnection);
            await connection.OpenAsync();

            int rowsAffected;
            try
            {
                rowsAffected = await connection.ExecuteAsync(query, new { UserId = userId, AccessToken = accessToken });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LogoutUserAsync] SQL Error: {ex.Message}");
                throw;
            }

            return rowsAffected > 0;
        }



        //    public async Task<bool> LogoutUserAsync(string accessToken)
        //    {
        //        const string query = @"
        //UPDATE UserTokens
        //SET IsRevoked = 1,
        //    RevokedAt = GETUTCDATE()
        //WHERE AccessToken = @AccessToken 
        //  AND IsRevoked = 0";

        //        // ✅ Step 1: Get CustomerCode from Session
        //        var httpContext = _httpContextAccessor.HttpContext;
        //        if (httpContext == null)
        //            throw new InvalidOperationException("HttpContext is not available.");

        //        string? customerCode = httpContext.Session.GetString("CustomerCode");
        //        if (string.IsNullOrWhiteSpace(customerCode))
        //            throw new InvalidOperationException("CustomerCode is missing in session. Please log in again.");

        //        // ✅ Step 2: Build customer-specific connection string
        //        string? connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
        //        if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        //            throw new InvalidOperationException("NewDatabaseTemplate connection string is missing in configuration.");

        //        string customerDbConnection = string.Format(connectionStringTemplate, customerCode);

        //        // ✅ Step 3: Run logout query
        //        await using var connection = new SqlConnection(customerDbConnection);
        //        await connection.OpenAsync();

        //        int rowsAffected;
        //        try
        //        {
        //            rowsAffected = await connection.ExecuteAsync(query, new { AccessToken = accessToken });
        //        }
        //        catch (Exception ex)
        //        {
        //            // 🔎 Helps you debug why SQL didn't run
        //            Console.WriteLine($"[LogoutUserAsync] SQL Error: {ex.Message}");
        //            throw;
        //        }

        //        return rowsAffected > 0;
        //    }




        //   public async Task InsertUserTokenAsync(
        //int userId,
        //string email,
        //string accessToken,
        //string refreshToken,
        //DateTime accessExpiry,
        //DateTime refreshExpiry,
        //string customerCode
        // )
        //   {
        //       const string sql = @"
        //   INSERT INTO UserTokens 
        //   (UserEmail, AccessToken, RefreshToken, AccessTokenExpiry, RefreshTokenExpiry, RevokedAt, 
        //    UserId, IpAddress, Device, Browser, CreatedAt)
        //   VALUES 
        //   (@UserEmail, @AccessToken, @RefreshToken, @AccessTokenExpiry, @RefreshTokenExpiry, 
        //    @RevokedAt, @UserId, @IpAddress, @Device, @Browser, @CreatedAt)";

        //       // Get dynamic DB from session
        //       string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //       if (string.IsNullOrEmpty(dbName))
        //           throw new Exception("CustomerCode is missing in session. Please log in again.");

        //       string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
        //       string customerDbConnection = string.Format(connectionStringTemplate, dbName);
        //       string userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "";
        //       string ipAddress = null;

        //       // 1️⃣ Check X-Forwarded-For header first (for proxies/load balancers)
        //       if (_httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
        //       {
        //           var header = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        //           if (!string.IsNullOrEmpty(header))
        //               ipAddress = header.Split(',')[0].Trim(); // take first IP if multiple
        //       }

        //       // 2️⃣ Fallback to RemoteIpAddress if header not present
        //       if (string.IsNullOrEmpty(ipAddress))
        //       {
        //           ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        //       }

        //       // Detect device
        //       string device = "Unknown Device";
        //       if (!string.IsNullOrEmpty(userAgent))
        //       {
        //           if (userAgent.Contains("Windows")) device = "Windows PC";
        //           else if (userAgent.Contains("Mac")) device = "Mac";
        //           else if (userAgent.Contains("iPhone")) device = "iPhone";
        //           else if (userAgent.Contains("iPad")) device = "iPad";
        //           else if (userAgent.Contains("Android")) device = "Android Device";
        //       }

        //       // Detect browser
        //       string browser = "Unknown Browser";
        //       if (!string.IsNullOrEmpty(userAgent))
        //       {
        //           if (userAgent.Contains("Chrome") && !userAgent.Contains("Edge")) browser = "Chrome";
        //           else if (userAgent.Contains("Firefox")) browser = "Firefox";
        //           else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) browser = "Safari";
        //           else if (userAgent.Contains("Edge")) browser = "Edge";
        //       }

        //       var accessTokenExpiry = DateTime.UtcNow.AddHours(1);   // instead of DateTime.Now
        //       var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        //       using var connection = new SqlConnection(customerDbConnection);
        //       await connection.OpenAsync();

        //       await connection.ExecuteAsync(sql, new
        //       {
        //           UserId = userId,
        //           UserEmail = email,
        //           AccessToken = accessToken,
        //           RefreshToken = refreshToken,
        //           AccessTokenExpiry = accessTokenExpiry,  
        //           RefreshTokenExpiry = refreshTokenExpiry,
        //           RevokedAt = (DateTime?)null,
        //           Device = device,
        //           Browser = browser,
        //           IpAddress = ipAddress,
        //           CreatedAt = DateTime.UtcNow
        //       });
        //   }

        public async Task InsertUserTokenAsync(
    int userId,
    string email,
    string accessToken,
    string refreshToken,
    DateTime accessExpiry,
    DateTime refreshExpiry,
    string customerCode)
        {

            const string sql = @"
    INSERT INTO UserTokens 
    (UserEmail, AccessToken, RefreshToken, AccessTokenExpiry, RefreshTokenExpiry, RevokedAt, 
     UserId, IpAddress, Device, Browser, CreatedAt)
    VALUES 
    (@UserEmail, @AccessToken, @RefreshToken, @AccessTokenExpiry, @RefreshTokenExpiry, 
     @RevokedAt, @UserId, @IpAddress, @Device, @Browser, @CreatedAt)";

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
            string customerDbConnection = string.Format(connectionStringTemplate, dbName);

            var context = _httpContextAccessor.HttpContext;
            string userAgent = context?.Request.Headers["User-Agent"].ToString() ?? "";
            string ipAddress = null;

            // 1️⃣ Check X-Forwarded-For header (useful in production behind proxy/load balancer)
            if (context?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
            {
                var header = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(header))
                    ipAddress = header.Split(',')[0].Trim();
            }

            // 2️⃣ Fallback to RemoteIpAddress
            if (string.IsNullOrEmpty(ipAddress))
            {
                var remoteIp = context?.Connection.RemoteIpAddress;

                if (remoteIp != null)
                {
                    // ✅ Correct check for loopback (::1 or 127.0.0.1)
                    if (System.Net.IPAddress.IsLoopback(remoteIp))
                    {
                        ipAddress = "127.0.0.1";
                    }
                    else
                    {
                        // Convert IPv6 to IPv4 if possible
                        if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            var ipv4 = Dns.GetHostEntry(remoteIp)
                                .AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                            ipAddress = ipv4?.ToString() ?? remoteIp.ToString();
                        }
                        else
                        {
                            ipAddress = remoteIp.ToString();
                        }
                    }
                }
            }


            // ✅ If you want LAN IP (192.168.x.x) even for local tests:
            if (ipAddress == "127.0.0.1" || ipAddress == "::1")
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var lanIp = host.AddressList.FirstOrDefault(
                    ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                          ip.ToString().StartsWith("192.168."));
                if (lanIp != null)
                    ipAddress = lanIp.ToString();
            }

            // Detect device
            string device = "Unknown Device";
            if (!string.IsNullOrEmpty(userAgent))
            {
                if (userAgent.Contains("Windows")) device = "Windows PC";
                else if (userAgent.Contains("Mac")) device = "Mac";
                else if (userAgent.Contains("iPhone")) device = "iPhone";
                else if (userAgent.Contains("iPad")) device = "iPad";
                else if (userAgent.Contains("Android")) device = "Android Device";
            }

            // Detect browser
            string browser = "Unknown Browser";
            if (!string.IsNullOrEmpty(userAgent))
            {
                if (userAgent.Contains("Chrome") && !userAgent.Contains("Edge")) browser = "Chrome";
                else if (userAgent.Contains("Firefox")) browser = "Firefox";
                else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) browser = "Safari";
                else if (userAgent.Contains("Edge")) browser = "Edge";
            }

            using var connection = new SqlConnection(customerDbConnection);
            await connection.OpenAsync();

            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                UserEmail = email,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddHours(1),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                RevokedAt = false,
                Device = device,
                Browser = browser,
                IpAddress = ipAddress, // ✅ Always IPv4 or LAN IP
                CreatedAt = DateTime.UtcNow
            });
        }



        private string DecryptPassword(string encryptedBase64)
        {
            string decryptionKey = "ML736@mmcs";
            byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);

            byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D,
                               0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
                               0x76 };

            using var aes = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(decryptionKey, salt);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();

            return Encoding.Unicode.GetString(ms.ToArray());
        }
        private string EncryptPassword(string plainText)
        {
            string encryptionKey = "ML736@mmcs";
            byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D,
                               0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
                               0x76 };

            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

            using var aes = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, salt);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plainBytes, 0, plainBytes.Length);
            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }




        //     public async Task<LoginResponse> LoginUserAsync(string email, string password)

        //     {
        //         try
        //         {
        //             // ✅ Step 1: Format email to match DB format
        //             //string formattedEmail = email.Trim().ToLower() + ",";

        //             //// ✅ Step 2: Get customer code from central DB
        //             //var customerCodeResult = await _customerRegistrationContext.MmcsCustomerRegistrations
        //             //    .Where(c => c.MCR_emails.ToLower() == formattedEmail)
        //             //    .Select(c => new { c.McrCustomerCode })
        //             //    .FirstOrDefaultAsync();

        //             //if (customerCodeResult == null)
        //             //{
        //             //    return new LoginResponse { StatusCode = 404, Message = "Email not found in customer registration." };
        //             //}

        //             //string customerCode = customerCodeResult.McrCustomerCode;


        //             var connection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
        //             await connection.OpenAsync();

        //             // Step 1: Get single customer code matching email
        //             string customerCodeSql = @"
        //    SELECT TOP 1 MCR_CustomerCode 
        //    FROM mmcs_customerregistration 
        //    CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails
        //    WHERE LTRIM(RTRIM(Emails.value)) = @Email
        //";

        //             var customerCode = await connection.QuerySingleOrDefaultAsync<string>(customerCodeSql, new { Email = email });
        //             // ✅ Step 3: Build connection string for customer DB
        //             string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
        //             string customerDbConnection = string.Format(connectionStringTemplate, customerCode);

        //             using (connection = new SqlConnection(customerDbConnection))
        //             {
        //                 await connection.OpenAsync();

        //                 // ✅ Step 4: Check for user with email + password
        //                 string plainEmail = email.Trim().ToLower(); // No comma added

        //                 var user = await connection.QueryFirstOrDefaultAsync<LoginDto>(
        // @"SELECT usr_Email AS UsrEmail, usr_Password AS UsrPassWord
        //   FROM Sad_UserDetails
        //   WHERE LOWER(usr_Email) = @email",
        // new
        // {
        //     email = plainEmail
        // });

        //                 if (user == null)
        //                 {
        //                     return new LoginResponse { StatusCode = 404, Message = "Invalid email." };
        //                 }

        //                 // ✅ Compare the entered plain password with the hashed password in DB
        //                 //bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.UsrPassWord);
        //                 bool isPasswordValid = true;
        //                 if (password == user.UsrPassWord)
        //                 {
        //                     isPasswordValid = true;
        //                 }
        //                 else
        //                 {
        //                      isPasswordValid = false;
        //                 }



        //                     if (!isPasswordValid)
        //                 {
        //                     return new LoginResponse { StatusCode = 401, Message = "Invalid password." };
        //                 }

        //                 // ✅ Step 5: Get user ID
        //                 var userId = await connection.QueryFirstOrDefaultAsync<int>(
        //                     @"SELECT usr_Id
        //               FROM Sad_UserDetails 
        //               WHERE LOWER(usr_Email) = @email",
        //                     new { email = plainEmail });

        //                 // ✅ Step 6: Generate JWT
        //                 string token = GenerateJwtTokens(user);
        //                 string? ymsId = null;
        //                 int? ymsYearId = null;

        //                 using (var yearConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //                 {
        //                     await yearConnection.OpenAsync();

        //                     const string query = @"SELECT YMS_ID, YMS_YEARID FROM Year_Master WHERE YMS_Default = 1";

        //                     var yearResult = await yearConnection.QueryFirstOrDefaultAsync<YearDto>(query);

        //                     if (yearResult != null)
        //                     {
        //                         ymsId = yearResult.YMS_ID;
        //                         ymsYearId = yearResult.YMS_YEARID;
        //                     }
        //                 }

        //                 return new LoginResponse
        //                 {
        //                     StatusCode = 200,
        //                     Message = "Login successful",
        //                     Token = token,
        //                     UsrId = userId,
        //                     YmsId = ymsId,
        //                     YmsYearId = ymsYearId
        //                 };
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             return new LoginResponse
        //             {
        //                 StatusCode = 500,
        //                 Message = $"An error occurred: {ex.Message}"
        //             };
        //         }
        //     }

        public SqlConnection GetConnection(string CustomerCode)
        {
            var template = _configuration.GetConnectionString("NewDatabaseTemplate");
            var finalConnection = string.Format(template, CustomerCode);
            return new SqlConnection(finalConnection);
        }


        public string GenerateJwtToken(string userDto, string CustomerCode, int userId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration["JwtSettings:Secret"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                var expiryInHoursString = _configuration["JwtSettings:ExpiryInHours"];

                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) ||
                    string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(expiryInHoursString))
                {
                    throw new Exception("JWT configuration values are missing.");
                }

                int expiryInHours = int.Parse(expiryInHoursString);
                var key = Encoding.UTF8.GetBytes(secretKey);
                var tokenId = Guid.NewGuid().ToString();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                // new Claim(ClaimTypes.NameIdentifier, userDto.UsrId.ToString()),
                new Claim(ClaimTypes.Email, userDto),
                new Claim("CustomerCode", CustomerCode ?? string.Empty),
                 new Claim("UserId", userId.ToString()),
                 new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                  new Claim("TokenId", tokenId)  // 👈 Add CustomerCode as a custom claim
            }),
                    Expires = DateTime.UtcNow.AddHours(expiryInHours),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating JWT token: {ex.Message}");
            }
        }



        //public async Task<List<FormPermissionDto>> GetUserPermissionsWithFormNameAsync(int companyId, int userId)
        //{
        //    var userRoleLevel = await _db.ExecuteScalarAsync<int>(
        //        "SELECT Usr_GrpOrUserLvlPerm FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_CompID = @CompanyId",
        //        new { UserId = userId, CompanyId = companyId });

        //    var isPartner = await _db.ExecuteScalarAsync<int?>(
        //        "SELECT Usr_ID FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_Partner = 1 AND Usr_CompID = @CompanyId",
        //        new { UserId = userId, CompanyId = companyId });

        //    IEnumerable<FormPermissionDto> results;

        //    if (isPartner.HasValue)
        //    {
        //        results = await _db.QueryAsync<FormPermissionDto>(
        //            @"SELECT DISTINCT m.Mod_Description AS FormName, o.OP_OperationName AS Permission
        //      FROM SAD_MODULE m
        //      JOIN SAD_Mod_Operations o ON m.Mod_ID = o.OP_ModuleID
        //      WHERE  m.Mod_CompID = @CompanyId",
        //            new { CompanyId = companyId });
        //    }
        //    else
        //    {
        //        if (userRoleLevel == 1)
        //        {
        //            results = await _db.QueryAsync<FormPermissionDto>(
        //                @"SELECT DISTINCT m.Mod_Description AS FormName, o.OP_OperationName AS Permission
        //          FROM SAD_MODULE m
        //          JOIN SAD_Mod_Operations o ON m.Mod_ID = o.OP_ModuleID
        //          JOIN SAD_UsrOrGrp_Permission p ON o.OP_PKID = p.Perm_OPPKID
        //          WHERE p.Perm_UsrorGrpID = @UserId 
        //            AND p.Perm_PType = 'U' 
        //            AND m.Mod_Parent = 4
        //            AND p.Perm_CompID = @CompanyId",
        //                new { UserId = userId, CompanyId = companyId });
        //        }
        //        else
        //        {
        //            var roleId = await _db.ExecuteScalarAsync<int>(
        //                "SELECT Usr_Role FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_CompID = @CompanyId",
        //                new { UserId = userId, CompanyId = companyId });

        //            results = await _db.QueryAsync<FormPermissionDto>(
        //                @"SELECT DISTINCT m.Mod_Description AS FormName, o.OP_OperationName AS Permission
        //          FROM SAD_MODULE m
        //          JOIN SAD_Mod_Operations o ON m.Mod_ID = o.OP_ModuleID
        //          JOIN SAD_UsrOrGrp_Permission p ON o.OP_PKID = p.Perm_OPPKID
        //          WHERE p.Perm_UsrorGrpID = @RoleId 
        //            AND p.Perm_PType = 'R' 
        //            AND m.Mod_Parent = 4
        //            AND p.Perm_CompID = @CompanyId",
        //                new { RoleId = roleId, CompanyId = companyId });
        //        }
        //    }

        //    return results.ToList();
        //}

        public async Task<IEnumerable<LogInfoDto>> GetUserLoginLogsAsync()
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
SELECT 
    ut.UserId,
    ut.UserEmail,
    ut.CreatedAt,
    ut.RevokedAt,
    ut.AccessTokenExpiry,
    ut.IsRevoked,
    ut.IpAddress,
    ut.Device,
    ut.Browser,
    g.Mas_Description AS RoleName
FROM UserTokens ut
INNER JOIN Sad_UserDetails sud ON sud.usr_Id = ut.UserId
LEFT JOIN SAD_GrpOrLvl_General_Master g ON sud.Usr_Role = g.Mas_ID
ORDER BY ut.Id DESC";

            var logs = await connection.QueryAsync(query);

            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var utcNow = DateTime.UtcNow;

            var formattedLogs = logs.Select(x =>
            {
                var createdAt = (DateTime)x.CreatedAt;
                var revokedAt = x.RevokedAt as DateTime?;
                var accessTokenExpiry = (DateTime)x.AccessTokenExpiry;

                // Login IST
                var loginDateTimeIST = TimeZoneInfo.ConvertTimeFromUtc(createdAt, istZone);

                // Logout IST
                DateTime logoutUtc = revokedAt ?? accessTokenExpiry;
                var logoutDateTimeIST = TimeZoneInfo.ConvertTimeFromUtc(logoutUtc, istZone);

                // Status
                bool isRevoked = x.IsRevoked != null && (bool)x.IsRevoked;
                bool isActive = (!isRevoked && accessTokenExpiry > utcNow);
                string status = isActive ? "Active" : "Inactive";

                // User timeline
                string userTimeLine = isActive
                    ? $"Active {(utcNow - createdAt).Hours}h {(utcNow - createdAt).Minutes % 60}m"
                    : $"Idle {((logoutUtc - createdAt).TotalMinutes):0}m";

                return new LogInfoDto
                {
                    UserId = x.UserId,
                    UserEmail = x.UserEmail,
                    LoginDate = loginDateTimeIST.ToString("dd/MM/yyyy"),
                    LoginTime = loginDateTimeIST.ToString("hh:mm tt"),
                    LogoutDate = revokedAt != null ? logoutDateTimeIST.ToString("dd/MM/yyyy") : null,
                    LogoutTime = revokedAt != null ? logoutDateTimeIST.ToString("hh:mm tt") : null,
                    IpAddress = x.IpAddress,
                    Device = x.Device,
                    Browser = x.Browser,
                    Status = status,
                    UserTimeLine = userTimeLine,
                    RoleName = x.RoleName
                };
            }).ToList();

            return formattedLogs;
        }

        //        public async Task<IEnumerable<LogInfoDto>> GetUserLoginLogsAsync()
        //        {
        //            // Get dynamic database name from session
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

        //            string query = @"
        //SELECT 
        //    ut.UserId AS UserId,
        //    ut.UserEmail,
        //    ut.CreatedAt,
        //    ut.RevokedAt,
        //    ut.AccessTokenExpiry,
        //    ut.IsRevoked,
        //    ut.IpAddress,
        //    ut.Device,
        //    ut.Browser
        //FROM UserTokens ut
        //INNER JOIN Sad_UserDetails sud ON sud.usr_Id = ut.UserId
        //ORDER BY ut.Id DESC";

        //            var logs = await connection.QueryAsync<dynamic>(query);

        //            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //            var utcNow = DateTime.UtcNow;

        //            var formattedLogs = new List<LogInfoDto>();

        //            foreach (var x in logs)
        //            {
        //                // Login IST
        //                var loginDateTimeIST = TimeZoneInfo.ConvertTimeFromUtc((DateTime)x.CreatedAt, istZone);

        //                // Logout IST (RevokedAt if exists, otherwise AccessTokenExpiry)
        //                DateTime logoutUtc = x.RevokedAt ?? (DateTime)x.AccessTokenExpiry;
        //                var logoutDateTimeIST = TimeZoneInfo.ConvertTimeFromUtc(logoutUtc, istZone);

        //                // Status
        //                bool isRevoked = x.IsRevoked == null ? false : (bool)x.IsRevoked;
        //                bool isActive = (!isRevoked && (DateTime)x.AccessTokenExpiry > utcNow);
        //                string status = isActive ? "Active" : "Inactive";

        //                // User timeline
        //                string userTimeLine = isActive
        //                    ? $"Active {(utcNow - (DateTime)x.CreatedAt).Hours}h {(utcNow - (DateTime)x.CreatedAt).Minutes % 60}m"
        //                    : $"Idle {((logoutUtc - (DateTime)x.CreatedAt).TotalMinutes):0}m";

        //                // Fetch role name
        //                string roleName = await connection.QueryFirstOrDefaultAsync<string>(
        //                    @"SELECT g.Mas_Description 
        //              FROM Sad_UserDetails u
        //              LEFT JOIN SAD_GrpOrLvl_General_Master g ON u.Usr_Role = g.Mas_ID
        //              WHERE u.usr_Id = @UserId",
        //                    new { UserId = x.UserId });

        //                formattedLogs.Add(new LogInfoDto
        //                {
        //                    UserId = x.UserId,
        //                    UserEmail = x.UserEmail,
        //                    LoginDate = loginDateTimeIST.ToString("dd/MM/yyyy"),
        //                    LoginTime = loginDateTimeIST.ToString("hh:mm tt"),
        //                    LogoutDate = x.RevokedAt != null ? logoutDateTimeIST.ToString("dd/MM/yyyy") : null,
        //                    LogoutTime = x.RevokedAt != null ? logoutDateTimeIST.ToString("hh:mm tt") : null,
        //                    IpAddress = x.IpAddress,
        //                    Device = x.Device,
        //                    Browser = x.Browser,
        //                    Status = status,
        //                    UserTimeLine = userTimeLine,
        //                    RoleName = roleName
        //                });
        //            }

        //            return formattedLogs;
        //        }



        public async Task<List<FormPermissionDto>> GetUserPermissionsWithFormNameAsync(int companyId, int userId)
        {
            // Get customer-specific database name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Create a dynamic connection for this customer
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            await connection.OpenAsync();

            // Get user role level
            var userRoleLevel = await connection.ExecuteScalarAsync<int>(
                "SELECT Usr_GrpOrUserLvlPerm FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_CompID = @CompanyId",
                new { UserId = userId, CompanyId = companyId });

            // Check if user is a partner
            var isPartner = await connection.ExecuteScalarAsync<int?>(
                "SELECT Usr_ID FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_Partner = 1 AND Usr_CompID = @CompanyId",
                new { UserId = userId, CompanyId = companyId });

            IEnumerable<FormPermissionDto> results;

            if (isPartner.HasValue)
            {
                results = await connection.QueryAsync<FormPermissionDto>(
                    @"SELECT DISTINCT m.Mod_Description AS FormName, o.OP_OperationName AS Permission
              FROM SAD_MODULE m
              JOIN SAD_Mod_Operations o ON m.Mod_ID = o.OP_ModuleID
              WHERE m.Mod_CompID = @CompanyId",
                    new { CompanyId = companyId });
            }
            else
            {
                if (userRoleLevel == 1)
                {
                    results = await connection.QueryAsync<FormPermissionDto>(
                        @"SELECT DISTINCT m.Mod_Description AS FormName, o.OP_OperationName AS Permission
                  FROM SAD_MODULE m
                  JOIN SAD_Mod_Operations o ON m.Mod_ID = o.OP_ModuleID
                  JOIN SAD_UsrOrGrp_Permission p ON o.OP_PKID = p.Perm_OPPKID
                  WHERE p.Perm_UsrorGrpID = @UserId
                    AND p.Perm_PType = 'U'
                    AND m.Mod_Parent = 4
                    AND p.Perm_CompID = @CompanyId",
                        new { UserId = userId, CompanyId = companyId });
                }
                else
                {
                    var roleId = await connection.ExecuteScalarAsync<int>(
                        "SELECT Usr_Role FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_CompID = @CompanyId",
                        new { UserId = userId, CompanyId = companyId });

                    results = await connection.QueryAsync<FormPermissionDto>(
                        @"SELECT DISTINCT m.Mod_Description AS FormName, o.OP_OperationName AS Permission
                  FROM SAD_MODULE m
                  JOIN SAD_Mod_Operations o ON m.Mod_ID = o.OP_ModuleID
                  JOIN SAD_UsrOrGrp_Permission p ON o.OP_PKID = p.Perm_OPPKID
                  WHERE p.Perm_UsrorGrpID = @RoleId
                    AND p.Perm_PType = 'R'
                    AND m.Mod_Parent = 4
                    AND p.Perm_CompID = @CompanyId",
                        new { RoleId = roleId, CompanyId = companyId });
                }
            }

            return results.ToList();
        }



        public async Task<(bool Success, string Message)> CheckAndAddAccessCodeConnectionStringAsync(string accessCode)
        {
            try
            {
                var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

                using (var connection = new SqlConnection(mmcsConnection))
                {
                    await connection.OpenAsync();

                    string query = "SELECT MCR_ID FROM MMCS_CustomerRegistration WHERE MCR_CustomerCode = @AccessCode AND MCR_MP_ID = 1";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccessCode", accessCode);
                        var result = await command.ExecuteScalarAsync();

                        if (result == null)
                        {
                            return (false, $"Access Code {accessCode} was not found in the Customer Registration.");
                        }
                    }
                }

                if (!File.Exists(_appSettingsPath))
                {
                    return (false, "appsettings.json file not found.");
                }

                var json = File.ReadAllText(_appSettingsPath);
                var jsonObject = JsonNode.Parse(json)?.AsObject();
                if (jsonObject == null)
                {
                    return (false, "Failed to parse appsettings.json.");
                }

                var connectionStrings = jsonObject["ConnectionStrings"]?.AsObject();
                if (connectionStrings == null)
                {
                    return (false, "ConnectionStrings section is missing in appsettings.json.");
                }

                if (connectionStrings.ContainsKey(accessCode))
                {
                    return (true, $"Access Code {accessCode} is valid. Connection string already exists.");
                }

                var sqlDetails = jsonObject["SQLDetails"]?.AsObject();
                if (sqlDetails == null)
                {
                    return (false, "SQLDetails section is missing in appsettings.json.");
                }

                string server = sqlDetails["Server"]?.ToString() ?? "";
                string userId = sqlDetails["User Id"]?.ToString() ?? "";
                string password = sqlDetails["Password"]?.ToString() ?? "";
                string trust = sqlDetails["TrustServerCertificate"]?.ToString() ?? "True";
                string mars = sqlDetails["MultipleActiveResultSets"]?.ToString() ?? "True";

                string newConnString = $"Server={server};Database={accessCode};User Id={userId};Password={password};TrustServerCertificate={trust};MultipleActiveResultSets={mars};";

                connectionStrings[accessCode] = newConnString;

                var updatedJson = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_appSettingsPath, updatedJson);

                return (true, $"Access Code {accessCode} is valid. Connection string has been successfully added.");
            }
            catch (Exception ex)
            {
                return (false, $"Exception occurred: {ex.Message}");
            }
        }



        #region API for Dashboards
        public async Task<int> GetTotalClientsAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"SELECT COUNT(*) 
                         FROM MMCS_CustomerRegistration 
                         WHERE MCR_Status = @MCR_Status";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MCR_Status", "A");

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetNewSignup30DaysAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"SELECT COUNT(*) 
                         FROM MMCS_CustomerRegistration 
                         WHERE MCR_FromDate >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE)) and MCR_Status = @MCR_Status";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MCR_Status", "A");

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetTrialUsersAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"SELECT COUNT(*) 
                         FROM MMCS_CustomerRegistration 
                         WHERE MCR_TStatus = @MCR_TStatus";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MCR_TStatus", "T");

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetPendingIssueAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"SELECT COUNT(*) 
                         FROM MMCS_CustomerRegistration 
                         WHERE MCR_TStatus = @MCR_TStatus";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MCR_TStatus", "MM");

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetResolvedIssueAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"SELECT COUNT(*) 
                         FROM MMCS_CustomerRegistration 
                         WHERE MCR_TStatus = @MCR_TStatus";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MCR_TStatus", "MM");

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetApprovalStatusAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"SELECT COUNT(*) 
                         FROM MMCS_CustomerRegistration 
                         WHERE MCR_Status = @MCR_Status";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MCR_Status", "A");

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<DashboardCounts> GetDashboardCardDetailsAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"
            SELECT 
                (SELECT COUNT(*) FROM MMCS_CustomerRegistration WHERE MCR_Status = 'A') AS TotalClients,
                (SELECT COUNT(*) FROM MMCS_CustomerRegistration 
                    WHERE MCR_FromDate >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE)) 
                    AND MCR_Status = 'A') AS NewSignup30Days,
                (SELECT COUNT(*) FROM MMCS_CustomerRegistration WHERE MCR_TStatus = 'T') AS TrialUsers,
                (SELECT COUNT(*) FROM MMCS_CustomerRegistration WHERE MCR_TStatus = 'MM') AS PendingIssues,
                (SELECT COUNT(*) FROM MMCS_CustomerRegistration WHERE MCR_TStatus = 'MM') AS ResolvedIssues,
                (SELECT COUNT(*) FROM MMCS_CustomerRegistration WHERE MCR_Status = 'A') AS ApprovalStatus,
                (SELECT CASE WHEN COUNT(*) = 0 THEN 'NA' ELSE CAST(COUNT(*) AS VARCHAR(10))  end  FROM MMCS_CustomerRegistration WHERE MCR_TStatus = 'MM') AS PendingStatus, 
                (SELECT CASE WHEN COUNT(*) = 0 THEN 'NA' ELSE CAST(COUNT(*) AS VARCHAR(10))  end FROM MMCS_CustomerRegistration WHERE MCR_TStatus = 'MM') AS RejectedStatus";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new DashboardCounts
                        {
                            TotalClients = reader.GetInt32(0),
                            NewSignup30Days = reader.GetInt32(1),
                            TrialUsers = reader.GetInt32(2),
                            PendingIssues = reader.GetInt32(3),
                            ResolvedIssues = reader.GetInt32(4),
                            ApprovalStatus = reader.GetInt32(5),
                            PendingStatus = reader.GetString(6),
                            RejectedStatus = reader.GetString(7),
                        };
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<ClientDetails>> GetClientDetailsAsync()
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"
                        SELECT A.MCR_ID as FirmID,
                        A.MCR_CustomerName as FirmName,
                         A.MCR_CustomerEmail as Email,
                        STRING_AGG(
                                CASE 
                                    WHEN B.MP_ModuleName = 'Masters' THEN 'Settings'
			                        WHEN B.MP_ModuleName = 'Digital Office' THEN 'Documents'
			                        WHEN B.MP_ModuleName = 'Digital Audit Office - Fixed Asset' THEN 'Fixed Asset'
			                        WHEN B.MP_ModuleName = 'Digital Audit Office - Assignments' THEN 'Task Management'
			                        WHEN B.MP_ModuleName = 'Digital Audit Office - Financial Audit' THEN 'Account Verification'
                                    ELSE B.MP_ModuleName
                                END, ', '
                            ) AS ModuleNames,

                        case when MCR_NumberOfUsers IS NULL then '0' else MCR_NumberOfUsers end as NumberOfUsers,
                        CONVERT(varchar(10), A.MCR_FromDate, 103) + ' - ' + CONVERT(varchar(10), A.MCR_ToDate, 103) AS SignedDate,
                        Case when MCR_TStatus = 'T' then 'Trial' else 'Subscribed' end as Types, '0' as IssueIDentified
                        FROM MMCS_CustomerRegistration A
                        JOIN MMCS_Modules B 
                        ON A.MCR_MP_ID = B.MM_MP_ID
                        WHERE A.MCR_Status = @MCR_Status
                        GROUP BY A.MCR_CustomerName,
                        A.MCR_CustomerEmail,
                        A.MCR_FromDate,
                        A.MCR_ToDate, MCR_NumberOfUsers,MCR_TStatus,MCR_ID
                        Order by MCR_FromDate desc";

                return await connection.QueryAsync<ClientDetails>(query, new
                {
                    MCR_Status = "A"
                });
            }
        }
        public async Task<(bool Success, string Message)> UpdatePasswordAsync(UpdatePasswordDto dto)
        {
            // 1️⃣ Validate password strength
            if (!PasswordValidator.IsStrong(dto.Password))
                return (false, "Password must contain Upper, Lower, Number & Special character.");

            // 2️⃣ Get customer code from Registration DB
            using var regCon = new SqlConnection(
                _configuration.GetConnectionString("CustomerRegistrationConnection"));

            await regCon.OpenAsync();

            var customerCode = await regCon.QueryFirstOrDefaultAsync<string>(
                @"SELECT TOP 1 MCR_CustomerCode
          FROM MMCS_CustomerRegistration
          CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails
          WHERE LOWER(LTRIM(RTRIM(Emails.value))) = LOWER(@Email)",
                new { Email = dto.Email });

            if (string.IsNullOrEmpty(customerCode))
                return (false, "Customer not found.");

            // 3️⃣ Get CUSTOMER DB connection
            var customerDbConnStr = _configuration.GetConnectionString(customerCode);
            if (string.IsNullOrEmpty(customerDbConnStr))
                return (false, "Customer database not configured.");

            using var custCon = new SqlConnection(customerDbConnStr);
            await custCon.OpenAsync();

            // 4️⃣ Check user exists in CUSTOMER DB
            int exists = await custCon.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
          FROM Sad_Userdetails
          WHERE LOWER(usr_Email) = LOWER(@Email)",
                new { Email = dto.Email });

            if (exists == 0)
                return (false, "User not found.");

            // 5️⃣ Hash password
            string hashedPassword = EncryptPassword(dto.Password);

            // 6️⃣ Update password
            await custCon.ExecuteAsync(
                @"UPDATE Sad_Userdetails
          SET usr_PassWord = @Password,
              Usr_UpdatedOn = GETDATE()
          WHERE LOWER(usr_Email) = LOWER(@Email)",
                new { Password = hashedPassword, Email = dto.Email });

            return (true, "Password updated successfully.");
        }

        public static class PasswordHasher
        {
            public static string Hash(string password)
            {
                byte[] salt = RandomNumberGenerator.GetBytes(16);

                byte[] hash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 32);

                return Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(hash);
            }
        }
        public static class PasswordValidator
        {
            public static bool IsStrong(string password)
            {
                if (string.IsNullOrWhiteSpace(password))
                    return false;

                bool hasUpper = password.Any(char.IsUpper);
                bool hasLower = password.Any(char.IsLower);
                bool hasDigit = password.Any(char.IsDigit);
                bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

                return hasUpper && hasLower && hasDigit && hasSpecial;
            }
        }
        private bool ValidateOtpJwt(string token, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwt = (JwtSecurityToken)validatedToken;

                var tokenEmail = jwt.Claims.First(x => x.Type == "email").Value;

                return tokenEmail.Equals(email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetTodayLoginAsync(int CompID)
        {

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"SELECT ISNULL(Count(*), 0) as TodayLogin FROM [dbo].[Audit_Log]
                                WHERE CAST(adt_Login AS DATE) = CAST(GETDATE() AS DATE) and ADT_CompID =@ADT_CompID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ADT_CompID", CompID);

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }


        public async Task<int> GetTodayLogoutAsync(int CompID)
        {

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"SELECT ISNULL(Count(*), 0) as TodayLogin FROM [dbo].[Audit_Log]
                                WHERE CAST(adt_Logout AS DATE) = CAST(GETDATE() AS DATE) and ADT_CompID =@ADT_CompID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ADT_CompID", CompID);

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }


        public async Task<int> GetTotalTimeSpentAsync(int CompID)
        {

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"SELECT 
                                CAST(SUM(DATEDIFF(MINUTE, adt_Login, ISNULL(adt_Logout, GETDATE()))) / 60.0 AS DECIMAL(10,2)) 
                                AS TotalHoursSpent
                                FROM [dbo].[Audit_Log]
                                WHERE adt_Login >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE)) and ADT_CompID =@ADT_CompID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ADT_CompID", CompID);

                    var result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }



        public async Task<IEnumerable<ClientViewDetails>> GetClientFullDetailsAsync(int FirmID)
        {
            var mmcsConnection = _configuration.GetConnectionString("CustomerRegistrationConnection");

            using (var connection = new SqlConnection(mmcsConnection))
            {
                await connection.OpenAsync();

                string query = @"
                                SELECT 
                                A.MCR_ID as FirmID,
                                A.MCR_CustomerName as FirmName,
                                 A.MCR_CustomerEmail as Email,
                                Case when MCR_TStatus = 'T' then 'Trial' else 'Subscribed' end as Types,
	                            CONVERT(varchar(10), A.MCR_FromDate, 103) + ' - ' +
                                CONVERT(varchar(10), A.MCR_ToDate, 103) AS SignedDate,
                                MCR_CustomerCode as AccessCode, '' as FirstLogin, '' as LastLogin, '' as TimeSpent, '' as TimeLogs
 
                                FROM MMCS_CustomerRegistration A
                                JOIN MMCS_Modules B 
                                ON A.MCR_MP_ID = B.MM_MP_ID
                                WHERE A.MCR_Status = 'A' and A.MCR_ID = 463
                                GROUP BY A.MCR_CustomerName,
                                A.MCR_CustomerEmail,
                                A.MCR_FromDate,
                                A.MCR_ToDate, MCR_NumberOfUsers,MCR_TStatus,MCR_ID,MCR_CustomerCode
                                Order by MCR_FromDate desc";

                return await connection.QueryAsync<ClientViewDetails>(query, new
                {
                    MCR_Status = "A"
                });
            }
        }


        #endregion

    } 
}






