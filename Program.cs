using System;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using TracePca.Data;
using TracePca.Data.CustomerRegistration;
using TracePca.Interface;
using TracePca.Interface.AssetMaserInterface;
using TracePca.Interface.Audit;
using TracePca.Interface.DatabaseConnection;
using TracePca.Interface.DigitalFiling;
using TracePca.Interface.DigitalFilling;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Interface.LedgerReview;
using TracePca.Interface.Master;
using TracePca.Interface.Middleware;
using TracePca.Interface.ProfileSetting;
using TracePca.Service;
using TracePca.Service.AssetService;
using TracePca.Service.Audit;
using TracePca.Service.Communication_with_client;
using TracePca.Service.DigitalFiling;
using TracePca.Service.FIN_statement;
using TracePca.Service.FixedAssetsService;
using TracePca.Service.LedgerReview;
using TracePca.Service.Master;
using TracePca.Service.Miidleware;
using TracePca.Service.ProfileSetting;
// Change this in CustomerContextMiddleware.cs


//using TracePca.Interface.AssetMaserInterface;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
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

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("SessionDb"); // or "CommonSessionDb"
    options.SchemaName = "dbo";
    options.TableName = "SessionCache";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(90);
    options.Cookie.Name = ".AspNetCore.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;


    // Always allow cross-site cookies
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requires HTTPS
});



//builder.Services.AddSession(options =>
//{
//    options.Cookie.Name = ".AspNetCore.Session";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;

//    if (environment.IsDevelopment())
//    {
//        // Local development settings (no HTTPS)
//        options.Cookie.SameSite = SameSiteMode.Lax;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
//    }
//    else
//    {
//        // Production settings (with HTTPS)
//        options.Cookie.SameSite = SameSiteMode.None;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    }
//});






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

builder.Services.AddScoped<ScheduleMappingInterface, ScheduleMappingService>();
builder.Services.AddScoped<ScheduleFormatInterface, ScheduleFormatService>();
builder.Services.AddScoped<JournalEntryInterface, JournalEntryService>();
builder.Services.AddScoped<ScheduleNoteInterface, ScheduleNoteService>();
builder.Services.AddScoped<ScheduleReportInterface, ScheduleReportService>();
builder.Services.AddScoped<ScheduleExcelUploadInterface, ScheduleExcelUploadService>();
builder.Services.AddScoped<ScheduleMastersInterface, ScheduleMastersService>();


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


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(

             "http://localhost:3000", // React app for local development
              "http://localhost:4000",
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
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));



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
                var token = claimsIdentity?.FindFirst("access_token")?.Value;

                if (string.IsNullOrEmpty(token))
                {
                    context.Fail("Token is missing.");
                    return;
                }

               
                using var scope = context.HttpContext.RequestServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IDbConnection>();

               
                var isRevoked = await db.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM UserTokens WHERE AccessToken = @AccessToken AND IsRevoked = 1",
                    new { AccessToken = token });

                if (isRevoked > 0)
                {
                    context.Fail("Token has been revoked.");
                }
            }
        };
    });

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
app.UseMiddleware<TracePca.Middleware.SessionTimeout>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//});


//app.UseCors("AllowReactApp");

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
//    RequestPath = ""
//});
//app.UseSession();

//app.UseMiddleware<TracePca.Middleware.CustomerContextMiddleware>();



//app.UseAuthorization();

//app.MapControllers();

//app.UseStaticFiles();

//app.Run();