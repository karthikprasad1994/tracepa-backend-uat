using Microsoft.EntityFrameworkCore;
using TracePca.Data;
using TracePca.Data.CustomerRegistration;
using TracePca.Interface;
using TracePca.Models;
using TracePca.Models.CustomerRegistration;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TracePca.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using TracePca.Models.UserModels;

namespace TracePca.Service
{
    public class Login : LoginInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly CustomerRegistrationContext _customerRegistrationContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DynamicDbContext _context;

        public Login(Trdmyus1Context dbContext, CustomerRegistrationContext customerDbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, DynamicDbContext context)
        {
            _dbcontext = dbContext;
            _customerRegistrationContext = customerDbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
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
                // Check if a customer with the same email already exists
                bool customerExists = await _customerRegistrationContext.MmcsCustomerRegistrations
                    .AnyAsync(c => c.McrCustomerEmail == registerModel.McrCustomerEmail);

                if (customerExists)
                {
                    return new ConflictObjectResult(new { statuscode = 409, message = "Customer with this email already exists." });
                }

                int maxMcrId = (await _customerRegistrationContext.MmcsCustomerRegistrations.MaxAsync(c => (int?)c.McrId) ?? 0) + 1;
                string currentYear = DateTime.Now.Year.ToString().Substring(2, 2);
                string yearPrefix = $"TR{currentYear}";
                int customerCount = await _customerRegistrationContext.MmcsCustomerRegistrations.CountAsync(c => c.McrCustomerCode.StartsWith(yearPrefix));
                int nextNumber = customerCount + 1;
                string newCustomerCode = $"{yearPrefix}_{nextNumber:D3}";
                string productKey = $"PRD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";


                // Map DTO to Entity Model
                var newCustomer = new MmcsCustomerRegistration
                {
                    McrId = maxMcrId,
                    McrMpId = 2,
                    McrCustomerName = registerModel.McrCustomerName,
                    McrCustomerEmail = registerModel.McrCustomerEmail,
                    McrCustomerTelephoneNo = registerModel.McrCustomerTelephoneNo,
                    McrStatus = "A",
                    McrTstatus = "T"
                };
                newCustomer.SetCustomerCodeAndProductKey(newCustomerCode, productKey);

                // Save to database
                await _customerRegistrationContext.MmcsCustomerRegistrations.AddAsync(newCustomer);
                await _customerRegistrationContext.SaveChangesAsync();

                    await CreateCustomerDatabaseAsync(newCustomerCode);
               // string userdbconnectionstring = _configuration.GetConnectionString("UserConnection");
             //   _context.Database.SetConnectionString(userdbconnectionstring);
             //   await _context.Database.OpenConnectionAsync();

                 string connectionStringTemplate = _configuration.GetConnectionString("NewDatabaseTemplate");
                  string newDbConnectionString = string.Format(connectionStringTemplate, newCustomerCode);

                 
                  string scriptPath = @"C:\Users\i5_4G_X2\Desktop\Cleaned_Sign-up.sql";
                    await ExecuteSqlScriptAsync(newDbConnectionString, scriptPath);
                int maxUserId = (await _context.SadUserDetails.MaxAsync(c => (int?)c.UsrId) ?? 0) + 1;
                var lastUserCode = await _context.SadUserDetails
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
                var lastUser = await _context.SadUserDetails
                  .Where(u => u.UsrPassWord.Contains($"@{currentYear}"))
                  .OrderByDescending(u => u.UsrPassWord)
                  .FirstOrDefaultAsync();
                int countNumber = 1;

                /*  if (lastUser != null)
                  {
                      string lastPassword = lastUser.UsrPassWord;
                      string numberPart = new string(lastPassword.Skip(1).TakeWhile(char.IsDigit).ToArray());

                      if (int.TryParse(numberPart, out int lastNumber))
                      {
                          countNumber = lastNumber + 1;
                      }
                  }*/

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("sa");


                var UserRegister = new Models.UserModels.SadUserDetail
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
                var optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();
                optionsBuilder.UseSqlServer(newDbConnectionString); // Set new database connection

                using (var newDbContext = new DynamicDbContext(optionsBuilder.Options))
                {
                    await newDbContext.SadUserDetails.AddAsync(UserRegister);
                    await newDbContext.SaveChangesAsync();
                }
                return new OkObjectResult(new { statuscode = 201, message = "Customer registered successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { statuscode = 500, message = "Internal server error", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        private async Task CreateCustomerDatabaseAsync(string customerCode)
        {
            string dbName = customerCode.Replace("-", "_"); // Replace hyphen for safety
            string CustomerConnectionString = _configuration.GetConnectionString("CustomerConnection");

            using (var connection = new SqlConnection(CustomerConnectionString))
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

            string newConnectionString = $"{CustomerConnectionString};Database={dbName};";

            // ✅ Update DbContext dynamically (optional)
            _customerRegistrationContext.Database.SetConnectionString(newConnectionString);
        }



        

public async Task ExecuteSqlScriptAsync(string connectionString, string scriptPath)
    {
        try
        {
            string script = await File.ReadAllTextAsync(scriptPath);

            // Split script based on "GO" (case-insensitive, ensures it's a whole word)
            string[] commands = Regex.Split(script, @"\bGO\b", RegexOptions.IgnoreCase);

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                foreach (string commandText in commands)
                {
                    string trimmedCommand = commandText.Trim();

                    if (!string.IsNullOrEmpty(trimmedCommand)) // Prevent empty command execution
                    {
                        using (var command = new SqlCommand(trimmedCommand, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing SQL script: {ex.Message}");
        }
    }



        public async Task<LoginResponse> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                // ✅ Step 1: Find Customer Code from Registration Database
                var customer = await _customerRegistrationContext.MmcsCustomerRegistrations
                    .AsNoTracking()
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
                        UsrId = userId
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

    }

}






