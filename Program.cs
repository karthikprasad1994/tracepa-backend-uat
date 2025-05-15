using OfficeOpenXml;

using Microsoft.EntityFrameworkCore;
using System.Text;
using TracePca.Data;
using TracePca.Data.CustomerRegistration;
using TracePca.Interface;
using TracePca.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Service.FixedAssetsService;
using TracePca.Service.AssetService;
using TracePca.Interface.AssetMaserInterface;
using TracePca.Interface.Audit;
using TracePca.Service.Communication_with_client;
//using TracePca.Interface.AssetMaserInterface;


var builder = WebApplication.CreateBuilder(args);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
 });
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<LoginInterface, Login>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<AssetInterface, Asset>();
builder.Services.AddScoped<AssetRegisterInterface, AssetRegister>();
builder.Services.AddScoped<LocationSetUpInterface, LocationSeUp>();
builder.Services.AddScoped<AssetTransactionAdditionInterface, AssetTransactionAddition>();


builder.Services.AddScoped<AssetAdditionDashboardInterface, AssetAdditionDashboard>();
builder.Services.AddScoped<EngagementPlanInterface, Engagement>();
builder.Services.AddScoped<AuditInterface, Communication>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
             "http://localhost:4000" // React app for local development
           // "https://customerregistration.multimedia.interactivedns.com"
            )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddDbContext<CustomerRegistrationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomerRegistrationConnection")));
builder.Services.AddDbContext<DynamicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<Trdmyus1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });
var app = builder.Build();

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();