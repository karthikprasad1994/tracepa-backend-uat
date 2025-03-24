using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TracePca.Models.CustomerRegistration;

namespace TracePca.Data.CustomerRegistration;

public partial class CustomerRegistrationContext : DbContext
{
    private readonly string _connectionString;
   
    public CustomerRegistrationContext()
    {
    }

    public CustomerRegistrationContext(DbContextOptions<CustomerRegistrationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appsetting> Appsettings { get; set; }

    public virtual DbSet<MmcsCustomerModule> MmcsCustomerModules { get; set; }

    public virtual DbSet<MmcsCustomerProspect> MmcsCustomerProspects { get; set; }

    public virtual DbSet<MmcsCustomerRegistration> MmcsCustomerRegistrations { get; set; }

    public virtual DbSet<MmcsCustomerRegistrationLog> MmcsCustomerRegistrationLogs { get; set; }

    public virtual DbSet<MmcsEmailConfig> MmcsEmailConfigs { get; set; }

    public virtual DbSet<MmcsEmailLog> MmcsEmailLogs { get; set; }

    public virtual DbSet<MmcsModule> MmcsModules { get; set; }

    public virtual DbSet<MmcsProduct> MmcsProducts { get; set; }

    public virtual DbSet<MmcsProductDemoDetail> MmcsProductDemoDetails { get; set; }

    public virtual DbSet<MmcsProductDetail> MmcsProductDetails { get; set; }

    public virtual DbSet<MmcsplUserLogin> MmcsplUserLogins { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appsetting>(entity =>
        {
            entity.HasKey(e => e.PkId).HasName("PK__Appsetti__1543595E2BE08256");

            entity.ToTable("Appsetting");

            entity.Property(e => e.PkId).HasColumnName("pk_id");
            entity.Property(e => e.AccessCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("access_code");
            entity.Property(e => e.ConnectionPath)
                .IsUnicode(false)
                .HasColumnName("connection_path");
        });

        modelBuilder.Entity<MmcsCustomerModule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_CustomerModules");

            entity.Property(e => e.McmId).HasColumnName("MCM_ID");
            entity.Property(e => e.McmMcrId).HasColumnName("MCM_MCR_ID");
            entity.Property(e => e.McmModuleId).HasColumnName("MCM_ModuleID");
        });

        modelBuilder.Entity<MmcsCustomerProspect>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_CustomerProspect");

            entity.Property(e => e.McpContactEmail)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCP_ContactEmail");
            entity.Property(e => e.McpContactName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCP_ContactName");
            entity.Property(e => e.McpContactPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCP_ContactPhone");
            entity.Property(e => e.McpCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MCP_CreatedOn");
            entity.Property(e => e.McpNumOffices).HasColumnName("MCP_NumOffices");
            entity.Property(e => e.McpPkid).HasColumnName("MCP_PKID");
            entity.Property(e => e.McpProspectName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCP_ProspectName");
            entity.Property(e => e.McpProspectSize).HasColumnName("MCP_ProspectSize");
            entity.Property(e => e.McpProspecttype)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCP_Prospecttype");
            entity.Property(e => e.McpRefferredBy)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCP_RefferredBy");
            entity.Property(e => e.McpRefferredOn)
                .HasColumnType("datetime")
                .HasColumnName("MCP_RefferredOn");
            entity.Property(e => e.McpStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MCP_Status");
            entity.Property(e => e.McpTypeofBusiness)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCP_TypeofBusiness");
            entity.Property(e => e.McpUrl)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("MCP_URL");
        });

        modelBuilder.Entity<MmcsCustomerRegistration>(entity =>
        {
            entity
    .HasKey(e => e.McrId);

            entity
                .ToTable("MMCS_CustomerRegistration");


            entity.Property(e => e.McrAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCR_Address");
            entity.Property(e => e.McrBillingFrequency).HasColumnName("MCR_BillingFrequency");
            entity.Property(e => e.McrCity)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_City");
            entity.Property(e => e.McrContactPersonEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MCR_ContactPersonEmail");
            entity.Property(e => e.McrContactPersonName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_ContactPersonName");
            entity.Property(e => e.McrContactPersonPhoneNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCR_ContactPersonPhoneNo");
            entity.Property(e => e.McrCreatedDate).HasColumnName("MCR_CreatedDate");
            entity.Property(e => e.McrCustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerCode");
            entity.Property(e => e.McrCustomerEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerEmail");
            entity.Property(e => e.McrCustomerName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerName");
            entity.Property(e => e.McrCustomerTelephoneNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerTelephoneNo");
            entity.Property(e => e.McrDataSize).HasColumnName("MCR_DataSize");
            entity.Property(e => e.McrFromDate).HasColumnName("MCR_FromDate");
            entity.Property(e => e.McrGstno)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCR_GSTNo");
            entity.Property(e => e.McrId).HasColumnName("MCR_ID");
            entity.Property(e => e.McrMpId).HasColumnName("MCR_MP_ID");
            entity.Property(e => e.McrNumberOfCustomers).HasColumnName("MCR_NumberOfCustomers");
            entity.Property(e => e.McrNumberOfUsers).HasColumnName("MCR_NumberOfUsers");
            entity.Property(e => e.McrProductKey)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MCR_ProductKey");
            entity.Property(e => e.McrRnwlFee).HasColumnName("MCR_RnwlFee");
            entity.Property(e => e.McrState)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_State");
            entity.Property(e => e.McrStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MCR_Status");
            entity.Property(e => e.McrToDate).HasColumnName("MCR_ToDate");
            entity.Property(e => e.McrTstatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MCR_TStatus");
        });

        modelBuilder.Entity<MmcsCustomerRegistrationLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_CustomerRegistration_Log");

            entity.Property(e => e.LogDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.McrAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MCR_Address");
            entity.Property(e => e.McrBillingFrequency).HasColumnName("MCR_BillingFrequency");
            entity.Property(e => e.McrCity)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_City");
            entity.Property(e => e.McrContactPersonEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MCR_ContactPersonEmail");
            entity.Property(e => e.McrContactPersonName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_ContactPersonName");
            entity.Property(e => e.McrContactPersonPhoneNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCR_ContactPersonPhoneNo");
            entity.Property(e => e.McrCustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerCode");
            entity.Property(e => e.McrCustomerEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerEmail");
            entity.Property(e => e.McrCustomerName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerName");
            entity.Property(e => e.McrCustomerTelephoneNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCR_CustomerTelephoneNo");
            entity.Property(e => e.McrDataSize).HasColumnName("MCR_DataSize");
            entity.Property(e => e.McrFromDate)
                .HasColumnType("datetime")
                .HasColumnName("MCR_FromDate");
            entity.Property(e => e.McrGstno)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCR_GSTNo");
            entity.Property(e => e.McrId).HasColumnName("MCR_ID");
            entity.Property(e => e.McrMpId).HasColumnName("MCR_MP_ID");
            entity.Property(e => e.McrNumberOfCustomers).HasColumnName("MCR_NumberOfCustomers");
            entity.Property(e => e.McrNumberOfUsers).HasColumnName("MCR_NumberOfUsers");
            entity.Property(e => e.McrProductKey)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MCR_ProductKey");
            entity.Property(e => e.McrState)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("MCR_State");
            entity.Property(e => e.McrToDate)
                .HasColumnType("datetime")
                .HasColumnName("MCR_ToDate");
            entity.Property(e => e.NMcrAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nMCR_Address");
            entity.Property(e => e.NMcrBillingFrequency).HasColumnName("nMCR_BillingFrequency");
            entity.Property(e => e.NMcrCity)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nMCR_City");
            entity.Property(e => e.NMcrContactPersonEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nMCR_ContactPersonEmail");
            entity.Property(e => e.NMcrContactPersonName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nMCR_ContactPersonName");
            entity.Property(e => e.NMcrContactPersonPhoneNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nMCR_ContactPersonPhoneNo");
            entity.Property(e => e.NMcrCustomerCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nMCR_CustomerCode");
            entity.Property(e => e.NMcrCustomerEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nMCR_CustomerEmail");
            entity.Property(e => e.NMcrCustomerName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nMCR_CustomerName");
            entity.Property(e => e.NMcrCustomerTelephoneNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nMCR_CustomerTelephoneNo");
            entity.Property(e => e.NMcrDataSize).HasColumnName("nMCR_DataSize");
            entity.Property(e => e.NMcrFromDate)
                .HasColumnType("datetime")
                .HasColumnName("nMCR_FromDate");
            entity.Property(e => e.NMcrGstno)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nMCR_GSTNo");
            entity.Property(e => e.NMcrMpId).HasColumnName("nMCR_MP_ID");
            entity.Property(e => e.NMcrNumberOfCustomers).HasColumnName("nMCR_NumberOfCustomers");
            entity.Property(e => e.NMcrNumberOfUsers).HasColumnName("nMCR_NumberOfUsers");
            entity.Property(e => e.NMcrProductKey)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nMCR_ProductKey");
            entity.Property(e => e.NMcrState)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nMCR_State");
            entity.Property(e => e.NMcrToDate)
                .HasColumnType("datetime")
                .HasColumnName("nMCR_ToDate");
        });

        modelBuilder.Entity<MmcsEmailConfig>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_EmailConfig");

            entity.Property(e => e.EcCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("EC_CreatedOn");
            entity.Property(e => e.EcCreatedby)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EC_Createdby");
            entity.Property(e => e.EcId).HasColumnName("EC_ID");
            entity.Property(e => e.EcPassword)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EC_Password");
            entity.Property(e => e.EcPortno)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EC_Portno");
            entity.Property(e => e.EcReferenceCcEmailId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EC_ReferenceCcEmailId");
            entity.Property(e => e.EcSenderEmailId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EC_SenderEmailId");
            entity.Property(e => e.EcSmtpaddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EC_SMTPAddress");
        });

        modelBuilder.Entity<MmcsEmailLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_EMailLog");

            entity.Property(e => e.AutomailStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("automail_status");
            entity.Property(e => e.Ccemail)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ccemail");
            entity.Property(e => e.CustId).HasColumnName("Cust_Id");
            entity.Property(e => e.CustName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Cust_name");
            entity.Property(e => e.EmailStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("email_status");
            entity.Property(e => e.Mailbody).IsUnicode(false);
            entity.Property(e => e.Mailsubject)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SenderId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("sender_id");
            entity.Property(e => e.SentBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("sent_by");
            entity.Property(e => e.SentOn)
                .HasColumnType("datetime")
                .HasColumnName("sent_on");
            entity.Property(e => e.Toemail)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("toemail");
        });

        modelBuilder.Entity<MmcsModule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_Modules");

            entity.Property(e => e.MmId).HasColumnName("MM_ID");
            entity.Property(e => e.MmMpId).HasColumnName("MM_MP_ID");
            entity.Property(e => e.MpModuleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MP_ModuleName");
        });

        modelBuilder.Entity<MmcsProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_Product");

            entity.Property(e => e.MpId).HasColumnName("MP_ID");
            entity.Property(e => e.MpProductName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MP_ProductName");
        });

        modelBuilder.Entity<MmcsProductDemoDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_ProductDemoDetails");

            entity.Property(e => e.MpddCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MPDD_CreatedOn");
            entity.Property(e => e.MpddFeedback)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MPDD_Feedback");
            entity.Property(e => e.MpddMcpPkid).HasColumnName("MPDD_MCP_PKID");
            entity.Property(e => e.MpddMpdPkid).HasColumnName("MPDD_MPD_PKID");
            entity.Property(e => e.MpddNameofPeopleandEmails)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MPDD_NameofPeopleandEmails");
            entity.Property(e => e.MpddNextStep)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MPDD_NextStep");
            entity.Property(e => e.MpddPkid).HasColumnName("MPDD_PKID");
            entity.Property(e => e.MpddProLink)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("MPDD_ProLink");
            entity.Property(e => e.MpddSummaryRateAbc).HasColumnName("MPDD_SummaryRateABC");
            entity.Property(e => e.MpddWhogavethedemo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("MPDD_Whogavethedemo");
        });

        modelBuilder.Entity<MmcsProductDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCS_ProductDetails");

            entity.Property(e => e.MpdCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MPD_CreatedOn");
            entity.Property(e => e.MpdDateofDemoDetailsShared)
                .HasColumnType("datetime")
                .HasColumnName("MPD_DateofDemoDetailsShared");
            entity.Property(e => e.MpdDetailsSent)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("MPD_DetailsSent");
            entity.Property(e => e.MpdMcpPkid).HasColumnName("MPD_MCP_PKID");
            entity.Property(e => e.MpdPkid).HasColumnName("MPD_PKID");
            entity.Property(e => e.MpdProduct).HasColumnName("MPD_Product");
        });

        modelBuilder.Entity<MmcsplUserLogin>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCSPL_UserLogin");

            entity.Property(e => e.MulCreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MUL_CreatedBy");
            entity.Property(e => e.MulCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MUL_CreatedOn");
            entity.Property(e => e.MulFullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MUL_FullName");
            entity.Property(e => e.MulIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MUL_IPAddress");
            entity.Property(e => e.MulLastLogindate)
                .HasColumnType("datetime")
                .HasColumnName("MUL_LastLogindate");
            entity.Property(e => e.MulLoginName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MUL_LoginName");
            entity.Property(e => e.MulNoOfLogins).HasColumnName("MUL_NoOfLogins");
            entity.Property(e => e.MulPassword)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MUL_Password");
            entity.Property(e => e.MulPkid).HasColumnName("MUL_PKID");
            entity.Property(e => e.MulUnSuccessfullAttempts).HasColumnName("MUL_UnSuccessfullAttempts");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Persons__3214EC27BE01C349");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
