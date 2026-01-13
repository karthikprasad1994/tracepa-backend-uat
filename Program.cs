using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using System;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using TracePca.Data;
using TracePca.Data.CustomerRegistration;
using TracePca.Interface;
using TracePca.Interface.Audit;
using TracePca.Interface.ClientPortal;
using TracePca.Interface.CustomerUserMaster;
using TracePca.Interface.Dashboard;
using TracePca.Interface.DatabaseConnection;
using TracePca.Interface.DigitalFiling;
using TracePca.Interface.DigitalFilling;
using TracePca.Interface.EmployeeMaster;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Interface.LedgerReview;
using TracePca.Interface.Master;
using TracePca.Interface.Middleware;
using TracePca.Interface.Permission;
using TracePca.Interface.ProfileSetting;
using TracePca.Interface.SuperMaster;
using TracePca.Interface.TaskManagement;
using TracePca.Service;
using TracePca.Service.Audit;
using TracePca.Service.ClientPortal;
using TracePca.Service.Communication_with_client;
using TracePca.Service.CustomerMaster;
using TracePca.Service.CustomerUserMaster;
using TracePca.Service.Dashboard;
using TracePca.Service.DigitalFiling;
using TracePca.Service.EmployeeMaster;
using TracePca.Service.FIN_statement;
using TracePca.Service.FixedAssetsService;
using TracePca.Service.LedgerReview;
using TracePca.Service.Master;
using TracePca.Service.Miidleware;
using TracePca.Service.ProfileSetting;
using TracePca.Service.SuperMaster;
using TracePca.Service.TaskManagement;
using TracePca.Utility;
using System.Globalization;
using QuestPDF.Infrastructure;
using JournalEntryUploadAPI.Services;



var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.PropertyNamingPolicy = null; // ? Keep original case
//    });
//var environment = builder.Environment;
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()


.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddDistributedMemoryCache();

//builder.Services.AddDistributedSqlServerCache(options =>
//{
//    options.ConnectionString = builder.Configuration.GetConnectionString("SessionDb"); // or "CommonSessionDb"
//    options.SchemaName = "dbo";
//    options.TableName = "SessionCache";
//});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.Name = ".AspNetCore.Session";
    options.Cookie.MaxAge = TimeSpan.FromHours(24);

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;


    // Always allow cross-site cookies
    options.Cookie.SameSite = SameSiteMode.None;
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Requires HTTPS
});

// 🔥 FORCE DASH INSTEAD OF SLASH
var culture = new CultureInfo("en-GB");

culture.DateTimeFormat.DateSeparator = "-";
culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("CustomerCode", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Customer-Code",
        Type = SecuritySchemeType.ApiKey,
        Description = "Enter the customer code (e.g. harsha123)",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CustomerCode"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<LoginInterface, Login>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<AssetMasterInterface, AssetMasterService>();
builder.Services.AddScoped<AssetCreationInterface, AssetCreationService>();
builder.Services.AddScoped<AssetSetupInterface, AssetSetupService>();
builder.Services.AddScoped<AssetTransactionAdditionInterface, AssetTransactionAdditionService>();
builder.Services.AddScoped<EngagementPlanInterface, EngagementPlanService>();
builder.Services.AddScoped<AuditCompletionInterface, AuditCompletionService>();
builder.Services.AddScoped<ExcelInformationInterfaces, ExcelInformationService>();


builder.Services.AddScoped<ScheduleMappingInterface, ScheduleMappingService>();
builder.Services.AddScoped<ScheduleFormatInterface, ScheduleFormatService>();
builder.Services.AddScoped<JournalEntryInterface, JournalEntryService>();
builder.Services.AddScoped<ScheduleNoteInterface, ScheduleNoteService>();
builder.Services.AddScoped<ScheduleReportInterface, ScheduleReportService>();
builder.Services.AddScoped<ScheduleExcelUploadInterface, ScheduleExcelUploadService>();
builder.Services.AddScoped<ScheduleMastersInterface, ScheduleMastersService>();
builder.Services.AddScoped<ScheduleAccountingRatioInterface, ScheduleAccountingRatioService>();
builder.Services.AddScoped<LedgerMaterialityInterface, LedgerMaterialityService>();
builder.Services.AddScoped<ILedgerDifferenceInterface, LedgerDifferenceService>();
builder.Services.AddScoped<SchedulePartnerFundsInterface, SchedulePartnerFundsService>();
builder.Services.AddScoped<AbnormalitiesInterface, AbnormalitiesService>();
builder.Services.AddScoped<SelectedPartiesInterface, SelectedPartiesService>();
builder.Services.AddScoped<CashFlowInterface, CashFlowService>();
builder.Services.AddScoped<FlaggedTransactionInterface, FlaggedTransactionService>();
builder.Services.AddScoped<AgingAnalysisInterface, AgingAnalysisService>();
builder.Services.AddScoped<SamplingInterface, SamplingService>();
builder.Services.AddScoped<IClientPortalInterface, ClientPortalService>();
builder.Services.AddScoped<IDBHelper, DBHelper>();

// Register your custom DbConnectionFactory
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

// Register your service
builder.Services.AddScoped<DashboardInterface, DashboardService>();


builder.Services.AddScoped<ProfileSettingInterface, ProfileSettingService>();
builder.Services.AddScoped<SubCabinetsInterface, SubCabinetsService>();
builder.Services.AddScoped<FoldersInterface, FoldersService>();
builder.Services.AddScoped<AuditAndDashboardInterface, DashboardAndSchedule>();

builder.Services.AddScoped<AuditInterface, Communication>();

builder.Services.AddScoped<AuditSummaryInterface, TracePca.Service.Audit.AuditSummary>();

builder.Services.AddScoped<ReportanIssueInterface, ReportanIssueService>();

builder.Services.AddScoped<ConductAuditInterface, TracePca.Service.Audit.ConductAuditService>();

builder.Services.AddScoped<ContentManagementMasterInterface, ContentManagementMasterService>();

builder.Services.AddScoped<AuditSummaryInterface, TracePca.Service.Audit.AuditSummary>();
builder.Services.AddScoped<CabinetInterface, TracePca.Service.DigitalFilling.Cabinet>();
builder.Services.AddScoped<LedgerReviewInterface, LedgerReviewService>();
builder.Services.AddScoped<DbConnectionProvider, DbConnectionProvider>();
builder.Services.AddScoped<ICustomerContext, CustomerContext>();
builder.Services.AddScoped<ErrorLoggerInterface, ErrorLoggerService>();
builder.Services.AddScoped<ApplicationMetricInterface, ApplicationMetric>();
builder.Services.AddScoped<ErrorLogInterface, TracePca.Service.Master.ErrorLog>();
builder.Services.AddScoped<PerformanceInterface, PerformanceService>();
builder.Services.AddScoped<EmailInterface, EmailService>();
builder.Services.AddScoped<EmployeeMasterInterface, EmployeeMaster>();
builder.Services.AddScoped<CustomerMasterInterface, CustomerMaster>();
builder.Services.AddScoped<CustomerUserMasterInterface, CustomerUserMaster>();
builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();
builder.Services.AddScoped<ReportTemplateInterface, ReportTemplateService>();
builder.Services.AddScoped<IBulkJournalEntryService, BulkJournalEntryService>();


builder.Services.AddScoped<ApiPerformanceTracker>();
builder.Services.AddScoped<PermissionInterface, TracePca.Service.Permission.Permission>();

builder.Services.AddScoped<TaskDashboardInterface, TaskDashboardService>();
builder.Services.AddScoped<TaskScheduleInterface, TaskScheduleService>();
builder.Services.AddScoped<CompanyDetailsInterface, CompanyDetailsService>();
builder.Services.AddScoped<TaskInvoiceAndReportInterface, TaskInvoiceAndReportService>();
builder.Services.AddScoped<JournalEntryInterface, JournalEntryService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    //AllowReactApp
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(

             "http://localhost:3000", // React app for local development
              "http://localhost:4000",
              "http://localhost:5173",
              "https://tracelites.multimedia.interactivedns.com",
              "https://clients.tracelites.multimedia.interactivedns.com",
              "https://edictin.multimedia.interactivedns.com",
              "http://app.tracepa.mmcspl.com"
            )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "493423119535-jughepjld5acrqn90lk33onq2u1sveju.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = "/signin-google"; // must match Google Cloud redirect URI
    options.SaveTokens = true; // save access token in auth cookie
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
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

// Configure Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtSettings["Issuer"],
//            ValidAudience = jwtSettings["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
//        };
//    });
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
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ClockSkew = TimeSpan.Zero
        };


        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                // Get the access token from the JWT claims
                var accessToken = claimsIdentity?.FindFirst("AccessToken")?.Value;

                if (string.IsNullOrEmpty(accessToken))
                {
                    context.Fail("Token is missing.");
                    return;
                }

                using var scope = context.HttpContext.RequestServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IDbConnection>();

                // Check if the access token has been revoked
                var isRevoked = await db.ExecuteScalarAsync<int>(
                     "SELECT COUNT(1) FROM UserTokens WHERE AccessToken = @AccessToken AND IsRevoked = 1",
                    new { AccessToken = accessToken });

                if (isRevoked > 0)
                {
                    context.Fail("Token has been revoked.");
                }
            }
        };
    });
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddHttpClient();


var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseMiddleware<TracePca.Middleware.CustomerContextMiddleware>();
app.UseMiddleware<TracePca.Middleware.ErrorLogMiddleware>();
app.UseMiddleware<TracePca.Middleware.ResponseTime>();
//app.MapControllers();

//app.UseMiddleware<TracePca.Middleware.SessionTimeout>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();
