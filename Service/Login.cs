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

namespace TracePca.Service
{
    public class Login : LoginInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly CustomerRegistrationContext _customerRegistrationContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Login(Trdmyus1Context dbContext, CustomerRegistrationContext customerDbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbContext;
            _customerRegistrationContext = customerDbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<object> AddUsersAsync(SadUserDetail User, MmcsCustomerRegistration Customer)
        {
            try
            {
                if (User == null || string.IsNullOrEmpty(User.UsrEmail) ||
                    string.IsNullOrEmpty(User.UsrMobileNo) ||
                    Customer == null || string.IsNullOrEmpty(Customer.McrCustomerName))
                {
                    return new
                    {
                        statuscode = 400,
                        message = "Email, Phone, and Customer Name are required.",
                        data = new object()
                    };
                }

                try
                {
                    // Validate required fields
                   
                    

                    // Check if a user with the same email, phone, and customer name already exists
                    var existingUser = await _dbcontext.SadUserDetails
                        .FirstOrDefaultAsync(u => u.UsrEmail == User.UsrEmail &&
                                                  u.UsrMobileNo == User.UsrMobileNo &&
                                                  _customerRegistrationContext.MmcsCustomerRegistrations
                                  .Any(c => c.McrCustomerName == Customer.McrCustomerName));

                    if (existingUser != null)
                    {
                        return new
                        {
                            statuscode = 409, // 409 Conflict - Indicates a duplicate entry
                            message = "A user with this Email, Phoneno, and Customer Name already exists.",
                            data = new object()
                        };
                    }

                    // Proceed with registration logic...
                }
                catch (Exception ex)
                {
                    return new
                    {
                        statuscode = 500,
                        message = $"An error occurred: {ex.Message}",
                        data = new object()
                    };
                }

                if (!DateTime.TryParse(Customer.McrFromDate.ToString(), out _) ||
                    !DateTime.TryParse(Customer.McrToDate.ToString(), out _) ||
                    !DateTime.TryParse(Customer.McrCreatedDate.ToString(), out _))
                {
                    return new
                    {
                        statuscode = 400,
                        message = "Invalid date format provided."
                    };
                }

                string currentYear = DateTime.Now.Year.ToString().Substring(2, 2);
                string yearPrefix = $"TR{currentYear}";
                int customerCount = await _customerRegistrationContext.MmcsCustomerRegistrations.CountAsync(c => c.McrCustomerCode.StartsWith(yearPrefix));
                int nextNumber = customerCount + 1;
                string newCustomerCode = $"{yearPrefix}_{nextNumber:D3}";

                string productKey = $"PRD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                int maxMcrId = (await _customerRegistrationContext.MmcsCustomerRegistrations.MaxAsync(c => (int?)c.McrId) ?? 0) + 1;
                Customer.McrId = maxMcrId;
                int MaxUserId = (await _dbcontext.SadUserDetails.MaxAsync(c => (int?)c.UsrId) ?? 0) + 1;
                User.UsrId = MaxUserId;


                // string currentYear = DateTime.Now.Year.ToString().Substring(2, 2);
                //string yearPrefix = $"T{currentYear}";

                var lastUser = await _dbcontext.SadUserDetails
                    .Where(u => u.UsrPassWord.Contains($"@{currentYear}"))
                    .OrderByDescending(u => u.UsrPassWord)
                    .FirstOrDefaultAsync();

                int Count_number = 1;

                if (lastUser != null)
                {
                    string lastPassword = lastUser.UsrPassWord;
                    string numberPart = new string(lastPassword.Skip(1).TakeWhile(char.IsDigit).ToArray());

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        Count_number = lastNumber + 1;
                    }
                }

                string plainPassword = $"T{nextNumber}@{currentYear}";
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
                User.UsrPassWord = hashedPassword;
                User.UsrDutyStatus = "A";
                User.UsrDelFlag = "A";
                User.UsrStatus = "U";
                User.UsrType = "C";
                User.UsrIsLogin = "Y";
                Customer.McrStatus = "A";
                
                Customer.McrTstatus = "A";
                

                await _dbcontext.SadUserDetails.AddAsync(User);
                await _customerRegistrationContext.MmcsCustomerRegistrations.AddAsync(Customer);
                await _dbcontext.SaveChangesAsync();
                await _customerRegistrationContext.SaveChangesAsync();

                return new
                {
                    statuscode = 201,
                    message = "User successfully added."
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    statuscode = 500,
                    message = $"An error occurred: {ex.Message}",
                    data = new object()
                };
            }
        }


        public async Task<object> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                // Find user by email
                var user = await _dbcontext.SadUserDetails.FirstOrDefaultAsync(u => u.UsrEmail == email);

                if (user == null)
                {
                    return new
                    {
                        statuscode = 404,
                        message = "Invalid Email"
                    };
                }

                // Verify the hashed password using BCrypt
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.UsrPassWord);

                if (!isPasswordValid)
                {
                    return new
                    {
                        statuscode = 401,
                        message = "Invalid password."
                    };
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                return new
                {
                    statuscode = 200,
                    message = "Login successful.",
                    token = token
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    statuscode = 500,
                    message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public string GenerateJwtToken(SadUserDetail user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["JwtSettings:Secret"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryInHours = int.Parse(_configuration["JwtSettings:ExpiryInHours"]);

            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.UsrId.ToString()),
                new Claim(ClaimTypes.Email, user.UsrEmail)
            }),
                Expires = DateTime.UtcNow.AddHours(expiryInHours),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        
        }
    }






