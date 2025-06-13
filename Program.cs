using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using TracePca.Data;
using TracePca.Data.CustomerRegistration;
using TracePca.Interface;
using TracePca.Interface.AssetMaserInterface;
using TracePca.Interface.Audit;
using TracePca.Interface.DigitalFiling;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Interface.ProfileSetting;
using TracePca.Service;
using TracePca.Service.AssetService;
using TracePca.Service.Audit;
using TracePca.Service.Communication_with_client;
using TracePca.Service.DigitalFiling;
using TracePca.Service.FIN_statement;
using TracePca.Service.FixedAssetsService;
using TracePca.Service.ProfileSetting;
using QuestPDF.Infrastructure;
//using TracePca.Interface.AssetMaserInterface;


var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
builder.Services.AddScoped<EngagementPlanInterface, EngagementPlanService>();
builder.Services.AddScoped<AuditCompletionInterface, AuditCompletionService>();
builder.Services.AddScoped<ConductAuditInterface, ConductAuditService>();

builder.Services.AddScoped<ScheduleMappingInterface, ScheduleMappingService>();
builder.Services.AddScoped<ScheduleFormatInterface, ScheduleFormatService>();
builder.Services.AddScoped<JournalEntryInterface, JournalEntryService>();

builder.Services.AddScoped<ProfileSettingInterface, ProfileSettingService>();
builder.Services.AddScoped<SubCabinetsInterface, SubCabinetsService>();
builder.Services.AddScoped<FoldersInterface, FoldersService>();
builder.Services.AddScoped<AuditAndDashboardInterface, DashboardAndSchedule>();

builder.Services.AddScoped<AuditInterface, Communication>();

builder.Services.AddScoped<AuditSummaryInterface, TracePca.Service.Audit.AuditSummary>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
             "http://localhost:3000", // React app for local development


           
             "https://tracelites.multimedia.interactivedns.com"
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
builder.Services.AddDbContext<Trdmyus1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection1")));
builder.Services.AddDbContext<Trdmyus1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection2")));



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