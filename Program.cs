using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
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
using TracePca.Service.ProfileSetting;

var builder = WebApplication.CreateBuilder(args);

// Initial setup
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Session setup
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AspNetCore.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

#if DEBUG
    // Local development settings (no HTTPS)
    options.Cookie.SameSite = SameSiteMode.Lax; // Use Lax for localhost:3000 -> 7090
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
#else
    // Production settings (with HTTPS)
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
#endif
});

// CORS setup
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
             "https://localhost:3000",
            "http://localhost:4000",
            "https://tracelites.multimedia.interactivedns.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // ✅ Important for cookies
    });
});

// Swagger setup
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("CustomerCode", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Customer-Code",
        Type = SecuritySchemeType.ApiKey,
        Description = "Enter the customer code (e.g. harsha123)"
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

// Dependency injection
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
builder.Services.AddScoped<CabinetInterface, TracePca.Service.DigitalFilling.Cabinet>();
builder.Services.AddScoped<LedgerReviewInterface, LedgerReviewService>();
builder.Services.AddScoped<DbConnectionProvider, DbConnectionProvider>();
builder.Services.AddScoped<ICustomerContext, CustomerContext>();

// Database contexts
builder.Services.AddDbContext<CustomerRegistrationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomerRegistrationConnection")));

builder.Services.AddDbContext<DynamicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));

// Only register once or conditionally
builder.Services.AddDbContext<Trdmyus1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

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

// Middleware ordering
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
app.UseAuthentication();     // ✅
app.UseAuthorization();

app.UseSession();            // ✅ Before Authentication


app.UseMiddleware<TracePca.Middleware.CustomerContextMiddleware>();

app.MapControllers();

app.Run();
