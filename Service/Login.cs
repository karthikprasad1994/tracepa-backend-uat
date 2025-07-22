using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TracePca.Data;
using TracePca.Data.CustomerRegistration;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface;
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

        public Login(Trdmyus1Context dbContext, CustomerRegistrationContext customerDbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, DynamicDbContext context, OtpService otpService, IWebHostEnvironment env)
        {
            _dbcontext = dbContext;
            _customerRegistrationContext = customerDbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _otpService = otpService;
            _env = env;
            _appSettingsPath = Path.Combine(env.ContentRootPath, "appsettings.json");
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
                string insertSql = @"
            INSERT INTO MMCS_CustomerRegistration 
            (MCR_ID, MCR_MP_ID, MCR_CustomerName, MCR_CustomerEmail, MCR_CustomerTelephoneNo, 
             MCR_Status, MCR_TStatus, MCR_CustomerCode, MCR_ProductKey, MCR_emails)
            VALUES 
            (@McrId, @McrMpId, @McrCustomerName, @McrCustomerEmail, @McrCustomerTelephoneNo, 
             'A', 'T', @McrCustomerCode, @McrProductKey, @MCR_emails)";

                var rowsInserted = await connection.ExecuteAsync(insertSql, new
                {
                    McrId = maxMcrId,
                    McrMpId = 1,
                    McrCustomerName = registerModel.McrCustomerName,
                    McrCustomerEmail = registerModel.McrCustomerEmail,
                    McrCustomerTelephoneNo = registerModel.McrCustomerTelephoneNo,
                    McrCustomerCode = newCustomerCode,
                    McrProductKey = productKey,
                    MCR_Emails = registerModel.McrCustomerEmail + ","

                });
                await CheckAndAddAccessCodeConnectionStringAsync(newCustomerCode);
                if (rowsInserted == 0)
                {
                    return new ObjectResult(new { statuscode = 500, message = "Failed to insert customer." }) { StatusCode = 500 };
                }

                Console.WriteLine("✅ Customer inserted via Dapper.");

                // Step 4: Create Customer Database
                await CreateCustomerDatabaseAsync(newCustomerCode);

                // Step 5: Setup Schema
                string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
                string newDbConnectionString = string.Format(connectionStringTemplate, newCustomerCode);

               string scriptFilePath = Path.Combine(@"C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\SQL_Scripts", "Tables.txt");

                // Ensure the folder and file exist
                if (!Directory.Exists(Path.GetDirectoryName(scriptFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(scriptFilePath));
                }

                if (!File.Exists(scriptFilePath))
                {
                    string defaultSql = "-- Initial SQL script\n-- Example: CREATE TABLE TestTable (Id INT PRIMARY KEY);";
                    File.WriteAllText(scriptFilePath, defaultSql);
                }

                // Execute the script
                await ExecuteAllSqlScriptsAsync(newDbConnectionString, scriptFilePath);


                //   string scriptsFolderPath = Path.Combine(_env.ContentRootPath, "SqlScripts", "Cleaned_Sign-up.sql");
                //   await ExecuteAllSqlScriptsAsync(newDbConnectionString, scriptsFolderPath);

                // Step 6: Insert Admin User in new DB (EF Core)
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
                string hashedPassword = EncryptPassword("sa");

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
                    UsrType = "C",
                    UsrIsLogin = "Y"
                };

                await newDbContext.SadUserDetails.AddAsync(adminUser);
                await newDbContext.SaveChangesAsync();

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
        public async Task ExecuteAllSqlScriptsAsync(string connectionString, string scriptFilePath)
        {
            try
            {
                if (!File.Exists(scriptFilePath))
                {
                    Console.WriteLine($"File not found: {scriptFilePath}");
                    return;
                }

                string script = await File.ReadAllTextAsync(scriptFilePath);
                string[] commands = Regex.Split(script, @"(?i)^\s*GO\s*$", RegexOptions.Multiline);

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (string commandText in commands)
                            {
                                string trimmedCommand = commandText.Trim();
                                if (!string.IsNullOrEmpty(trimmedCommand))
                                {
                                    try
                                    {
                                        using (var command = new SqlCommand(trimmedCommand, connection, transaction))
                                        {
                                            command.CommandTimeout = 120;

                                            // Optional: Handle IDENTITY_INSERT if needed
                                            if (trimmedCommand.Contains("INSERT INTO") && trimmedCommand.Contains("IDENTITY"))
                                            {
                                                await new SqlCommand("SET IDENTITY_INSERT ON;", connection, transaction).ExecuteNonQueryAsync();
                                            }

                                            await command.ExecuteNonQueryAsync();
                                        }
                                    }
                                    catch (SqlException sqlEx)
                                    {
                                        Console.WriteLine($"SQL Execution Error: {sqlEx.Message}\nCommand: {trimmedCommand}");
                                        throw; // Rethrow to roll back
                                    }
                                }
                            }

                            await transaction.CommitAsync();
                            Console.WriteLine("SQL script executed successfully.");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine($"Transaction failed: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error executing SQL script: {ex.Message}");
            }
        }








        public async Task<LoginResponse> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                // ✅ Step 1: Find Customer Code from Registration Database
                var customer = await _customerRegistrationContext.MmcsCustomerRegistrations

                    .Where(c => c.McrCustomerEmail == email)
                    .Select(c => new { c.McrCustomerCode })
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return new LoginResponse { StatusCode = 404, Message = "Email not found" };
                }

                // ✅ Step 2: Get Connection String for Customer Database
                string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
                string newDbConnectionString = string.Format(connectionStringTemplate, customer.McrCustomerCode);

                // ✅ Step 3: Use DynamicDbContext with new Connection String
                var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
                optionsBuilder.UseSqlServer(newDbConnectionString);

                using (var newDbContext = new DynamicDbContext(optionsBuilder.Options))
                {
                    // ✅ Step 4: Fetch user from the correct database
                    var userDto = await newDbContext.SadUserDetails
                        .AsNoTracking()
                        .Where(u => u.UsrEmail == email)
                        .Select(u => new LoginDto
                        {
                            UsrEmail = u.UsrEmail,
                            UsrPassWord = u.UsrPassWord
                        })
                        .SingleOrDefaultAsync();

                    if (userDto == null)
                    {
                        return new LoginResponse { StatusCode = 404, Message = "Invalid Email" };
                    }

                    // ✅ Debugging: Log stored hashed password
                    Console.WriteLine($"[DEBUG] Stored Hashed Password: {userDto.UsrPassWord}");

                    // ✅ Step 5: Verify Password
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, userDto.UsrPassWord);

                    if (!isPasswordValid)
                    {
                        return new LoginResponse { StatusCode = 401, Message = "Invalid Password" };
                    }

                    // ✅ Step 6: Fetch User ID
                    var userId = await newDbContext.SadUserDetails
                        .AsNoTracking()
                        .Where(a => a.UsrEmail == email)
                        .Select(a => a.UsrId)
                        .FirstOrDefaultAsync();

                

                    // ✅ Step 7: Generate JWT Token
                    string token = GenerateJwtToken(userDto);
                   



                    return new LoginResponse
                    {
                        StatusCode = 200,
                        Message = "Login successful",
                        Token = token,
                        UsrId = userId,
                        CustomerCode = customer.McrCustomerCode


                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception in AuthenticateUserAsync: {ex.Message}");
                return new LoginResponse
                {
                    StatusCode = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }


        public string GenerateJwtToken(LoginDto userDto)
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

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                //new Claim(ClaimTypes.NameIdentifier, userDto.UsrId.ToString()),
                new Claim(ClaimTypes.Email, userDto.UsrEmail)
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

        public async Task<(bool Success, string Message, string? OtpToken)> GenerateAndSendOtpJwtAsync(string email)
        {
            return await _otpService.GenerateAndSendOtpJwtAsync(email);
        }


        //public async Task<(string Token, string Otp)> GenerateAndSendOtpJwtAsync(string email)
        //{
        //    return await _otpService.GenerateAndSendOtpJwtAsync(email);
        //}


        public async Task<bool> VerifyOtpJwtAsync(string token, string enteredOtp)
        {
            return await Task.FromResult(_otpService.VerifyOtpJwt(token, enteredOtp)); // ✅ Use await correctly
        }

        public async Task<LoginResponse> LoginUserAsync(string email, string password)
        {
            try
            {
                email = email?.Trim().ToLower();
                password = password?.Trim();
                using var regConnection = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
                await regConnection.OpenAsync();

                // Step 1: Get customer code from MCR_emails
                string customerCodeSql = @"
            SELECT TOP 1 MCR_CustomerCode 
            FROM mmcs_customerregistration 
            CROSS APPLY STRING_SPLIT(MCR_emails, ',') AS Emails
            WHERE LTRIM(RTRIM(Emails.value)) = @Email";

                var customerCode = await regConnection.QuerySingleOrDefaultAsync<string>(customerCodeSql, new { Email = email });

                if (string.IsNullOrEmpty(customerCode))
                {
                    return new LoginResponse { StatusCode = 404, Message = "Email not found in customer registration." };
                   }

                // Step 2: Connect to the customer's DB
                string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
                string customerDbConnection = string.Format(connectionStringTemplate, customerCode);

                using var connection = new SqlConnection(customerDbConnection);
                await connection.OpenAsync();

                string plainEmail = email.Trim().ToLower();

                // Step 3: Fetch user details
                var user = await connection.QueryFirstOrDefaultAsync<LoginDto>(
                    @"SELECT usr_Email AS UsrEmail, usr_Password AS UsrPassWord
              FROM Sad_UserDetails
              WHERE LOWER(usr_Email) = @email",
                    new { email = plainEmail });

                if (user == null)
                {
                    return new LoginResponse { StatusCode = 404, Message = "Invalid email." };
                }

                bool isPasswordValid = false;

                // Step 4: Try BCrypt verification
                try
                {
                   isPasswordValid = DecryptPassword(user.UsrPassWord) == password;


                }
                catch
                {
                    // Not a BCrypt hash, try legacy AES decryption
                    try
                    {
                        string decryptedPassword = DecryptPassword(user.UsrPassWord);
                        isPasswordValid = password == decryptedPassword;

                        if (isPasswordValid)
                        {
                            // ✅ Migrate password to BCrypt
                            string newHash = BCrypt.Net.BCrypt.HashPassword(password);

                          //  Update the user's password
                            //await connection.ExecuteAsync(
                            //    "UPDATE Sad_UserDetails SET usr_Password = @newHash WHERE LOWER(usr_Email) = @email",
                            //    new { newHash, email = plainEmail });

                            //Console.WriteLine("🔁 Legacy password migrated to BCrypt.");
                        }
                    }
                    catch
                    {
                        // Decryption failed — bad format
                        isPasswordValid = false;
                    }
                }

                if (!isPasswordValid)
                {
                    return new LoginResponse { StatusCode = 401, Message = "Invalid password." };
                }

                // Step 5: Get user ID
                var userId = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT usr_Id FROM Sad_UserDetails WHERE LOWER(usr_Email) = @email",
                    new { email = plainEmail });

                // Step 6: Generate JWT
                string token = GenerateJwtTokens(user);
                string? ymsId = null;
                int? ymsYearId = null;

                using (var yearConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await yearConnection.OpenAsync();

                    const string query = @"SELECT YMS_ID, YMS_YEARID FROM Year_Master WHERE YMS_Default = 1";
                    var yearResult = await yearConnection.QueryFirstOrDefaultAsync<YearDto>(query);

                    if (yearResult != null)
                    {
                        ymsId = yearResult.YMS_ID;
                        ymsYearId = yearResult.YMS_YEARID;
                    }
                }

                var httpContext = _httpContextAccessor.HttpContext;
                string clientIp = httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? httpContext?.Connection?.RemoteIpAddress?.ToString();




                return new LoginResponse
                {
                    StatusCode = 200,
                    Message = "Login successful",
                    Token = token,
                    UsrId = userId,
                    YmsId = ymsId,
                    YmsYearId = ymsYearId,
                    CustomerCode = customerCode,
                    ClientIpAddress = clientIp
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    StatusCode = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
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

        public SqlConnection GetConnection(string customerCode)
        {
            var template = _configuration.GetConnectionString("NewDatabaseTemplate");
            var finalConnection = string.Format(template, customerCode);
            return new SqlConnection(finalConnection);
        }


        public string GenerateJwtTokens(LoginDto userDto)
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

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                //new Claim(ClaimTypes.NameIdentifier, userDto.UsrId.ToString()),
                new Claim(ClaimTypes.Email, userDto.UsrEmail)
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

        public async Task<List<FormPermissionDto>> GetUserPermissionsWithFormNameAsync(int companyId, int userId)
        {
            var userRoleLevel = await _db.ExecuteScalarAsync<int>(
                "SELECT Usr_GrpOrUserLvlPerm FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_CompID = @CompanyId",
                new { UserId = userId, CompanyId = companyId });

            var isPartner = await _db.ExecuteScalarAsync<int?>(
                "SELECT Usr_ID FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_Partner = 1 AND Usr_CompID = @CompanyId",
                new { UserId = userId, CompanyId = companyId });

            IEnumerable<FormPermissionDto> results;

            if (isPartner.HasValue)
            {
                results = await _db.QueryAsync<FormPermissionDto>(
                    @"SELECT DISTINCT m.Mod_Description AS FormName, o.OP_OperationName AS Permission
              FROM SAD_MODULE m
              JOIN SAD_Mod_Operations o ON m.Mod_ID = o.OP_ModuleID
              WHERE  m.Mod_CompID = @CompanyId",
                    new { CompanyId = companyId });
            }
            else
            {
                if (userRoleLevel == 1)
                {
                    results = await _db.QueryAsync<FormPermissionDto>(
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
                    var roleId = await _db.ExecuteScalarAsync<int>(
                        "SELECT Usr_Role FROM Sad_UserDetails WHERE Usr_ID = @UserId AND Usr_CompID = @CompanyId",
                        new { UserId = userId, CompanyId = companyId });

                    results = await _db.QueryAsync<FormPermissionDto>(
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
    }
}






