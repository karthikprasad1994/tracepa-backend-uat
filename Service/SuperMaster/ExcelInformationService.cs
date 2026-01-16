using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Packaging.Ionic.Zip;
using OpenAI;
using System.Data;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Dto.SuperMaster;
using TracePca.Interface.SuperMaster;
using static TracePca.Dto.SuperMaster.ExcelInformationDto;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.VisualBasic;
using static Dropbox.Api.TeamLog.FedExtraDetails;
using static Microsoft.FSharp.Core.ByRefKinds;


namespace TracePca.Service.SuperMaster
{
    public class ExcelInformationService : ExcelInformationInterfaces
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelInformationService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }
      
        //SaveEmployeeMaster
        public async Task<List<int[]>> SuperMasterSaveEmployeeDetailsAsync(int CompId, List<SuperMasterSaveEmployeeMasterDto> employees)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var results = new List<int[]>();

                foreach (var objEmp in employees)
                {
                    // ✅ Check Designation ID validity
                    string checkQuery = @"
                SELECT COUNT(1)
                FROM SAD_GRPDESGN_General_Master
                WHERE Mas_ID = @DesignationID AND Mas_CompID = @CompId";

                    int exists = await connection.ExecuteScalarAsync<int>(
                        checkQuery,
                        new { DesignationID = objEmp.iUsrDesignation, CompId = CompId },
                        transaction: transaction
                    );

                    if (exists == 0)
                        throw new Exception($"Invalid designation ID for employee: {objEmp.sUsrFullName}");

                    int updateOrSave, oper;

                    using var command = new SqlCommand("spEmployeeMaster", connection, transaction);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters (same as your existing code)
                    command.Parameters.AddWithValue("@Usr_ID", objEmp.iUserID);
                    command.Parameters.AddWithValue("@Usr_Node", objEmp.iUsrNode);
                    command.Parameters.AddWithValue("@Usr_Code", objEmp.sUsrCode ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_FullName", objEmp.sUsrFullName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_LoginName", objEmp.sUsrLoginName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Password", objEmp.sUsrPassword ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Email", objEmp.sUsrEmail ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Category", objEmp.iUsrSentMail);
                    command.Parameters.AddWithValue("@Usr_Suggetions", objEmp.iUsrSuggetions);
                    command.Parameters.AddWithValue("@usr_partner", objEmp.iUsrPartner);
                    command.Parameters.AddWithValue("@Usr_LevelGrp", objEmp.iUsrLevelGrp);
                    command.Parameters.AddWithValue("@Usr_DutyStatus", objEmp.sUsrDutyStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_PhoneNo", objEmp.sUsrPhoneNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_MobileNo", objEmp.sUsrMobileNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OfficePhone", objEmp.sUsrOfficePhone ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OffPhExtn", objEmp.sUsrOffPhExtn ?? string.Empty);

                    // ✅ Designation as ID
                    command.Parameters.AddWithValue("@Usr_Designation", objEmp.iUsrDesignation);

                    command.Parameters.AddWithValue("@Usr_CompanyID", objEmp.iUsrCompanyID);
                    command.Parameters.AddWithValue("@Usr_OrgnID", objEmp.iUsrOrgID);
                    command.Parameters.AddWithValue("@Usr_GrpOrUserLvlPerm", objEmp.iUsrGrpOrUserLvlPerm);
                    command.Parameters.AddWithValue("@Usr_Role", objEmp.iUsrRole);
                    command.Parameters.AddWithValue("@Usr_MasterModule", objEmp.iUsrMasterModule);
                    command.Parameters.AddWithValue("@Usr_AuditModule", objEmp.iUsrAuditModule);
                    command.Parameters.AddWithValue("@Usr_RiskModule", objEmp.iUsrRiskModule);
                    command.Parameters.AddWithValue("@Usr_ComplianceModule", objEmp.iUsrComplianceModule);
                    command.Parameters.AddWithValue("@Usr_BCMModule", objEmp.iUsrBCMmodule);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeModule", objEmp.iUsrDigitalOfficeModule);
                    command.Parameters.AddWithValue("@Usr_MasterRole", objEmp.iUsrMasterRole);
                    command.Parameters.AddWithValue("@Usr_AuditRole", objEmp.iUsrAuditRole);
                    command.Parameters.AddWithValue("@Usr_RiskRole", objEmp.iUsrRiskRole);
                    command.Parameters.AddWithValue("@Usr_ComplianceRole", objEmp.iUsrComplianceRole);
                    command.Parameters.AddWithValue("@Usr_BCMRole", objEmp.iUsrBCMRole);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeRole", objEmp.iUsrDigitalOfficeRole);
                    command.Parameters.AddWithValue("@Usr_CreatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_UpdatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_DelFlag", objEmp.sUsrFlag ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Status", objEmp.sUsrStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_IPAddress", objEmp.Usr_IPAdress ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_CompId", objEmp.iUsrCompID);
                    command.Parameters.AddWithValue("@Usr_Type", objEmp.sUsrType ?? string.Empty);
                    command.Parameters.AddWithValue("@usr_IsSuperuser", objEmp.iusr_IsSuperuser);
                    command.Parameters.AddWithValue("@USR_DeptID", objEmp.iUSR_DeptID);
                    command.Parameters.AddWithValue("@USR_MemberType", objEmp.iUSR_MemberType);
                    command.Parameters.AddWithValue("@USR_Levelcode", objEmp.iUSR_Levelcode);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    command.Parameters.Add(updateOrSaveParam);
                    command.Parameters.Add(operParam);

                    await command.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0);

                    results.Add(new[] { updateOrSave, oper });
                }

                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //SaveClientDetails
        public async Task<List<int[]>> SuperMasterSaveCustomerDetailsAsync(int CompId, List<SuperMasterSaveCustomerDto> customers)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var results = new List<int[]>();

                foreach (var objCust in customers)
                {

                    //Checks OrgType Fisrt
                    if (!string.IsNullOrWhiteSpace(objCust.OrgTypeName) && objCust.CUST_ORGTYPEID == 0)
                    {
                        int orgTypeId = await connection.ExecuteScalarAsync<int?>(
                            @"SELECT TOP 1 Cmm_ID 
          FROM Content_Management_Master 
          WHERE cmm_Category = 'ORG' 
            AND Cmm_CompID = @CompId 
            AND UPPER(cmm_Desc) = UPPER(@OrgType)",
                            new { CompId, OrgType = objCust.OrgTypeName }, transaction
                        ) ?? 0;

                        if (orgTypeId == 0)
                        {
                            // Get next cmm_ID before inserting
                            int nextCmmId = await connection.ExecuteScalarAsync<int>(
                                "SELECT ISNULL(MAX(cmm_ID) + 1, 1) FROM Content_Management_Master",
                                transaction: transaction
                            );

                            // Insert new OrgType
                            orgTypeId = await connection.ExecuteScalarAsync<int>(
                                @"INSERT INTO Content_Management_Master 
              (cmm_ID, cmm_Code, cmm_Category, Cmm_CompID, cmm_Desc, cmm_DelFlag)
              VALUES (@Cmm_ID, @cmm_Code, 'ORG', @CompId, @OrgType, 'A');
              SELECT @Cmm_ID;",
                                new
                                {
                                    Cmm_ID = nextCmmId,
                                    cmm_Code = objCust.Cmm_Code,
                                    CompId,
                                    OrgType = objCust.OrgTypeName
                                },
                                transaction
                            );
                        }
                        else
                        {
                            // Ensure it's active
                            await connection.ExecuteAsync(
                                @"UPDATE Content_Management_Master
              SET cmm_DelFlag = 'A'
              WHERE Cmm_ID = @OrgTypeId",
                                new { OrgTypeId = orgTypeId }, transaction
                            );
                        }

                        objCust.CUST_ORGTYPEID = orgTypeId;
                    }


                    //Ensure Customer Code exists or activate it
                    var custRecord = await connection.QueryFirstOrDefaultAsync<(int Id, string DelFlag)>(
                        @"SELECT Cust_ID AS Id, CUST_DELFLG AS DelFlag
      FROM SAD_CUSTOMER_MASTER
      WHERE CUST_CODE = @CustCode",
                        new { CustCode = objCust.CUST_CODE }, transaction
                    );

                    if (custRecord.Id == 0) // Not found
                    {
                        objCust.CUST_CODE = await connection.ExecuteScalarAsync<string>(
                            @"SELECT 'CUST' + CAST(COALESCE(MAX(Cust_ID), 0) + 1 AS VARCHAR)
          FROM SAD_CUSTOMER_MASTER",
                            transaction: transaction
                        );
                    }
                    else
                    {
                        if (!string.Equals(custRecord.DelFlag, "A", StringComparison.OrdinalIgnoreCase))
                        {
                            await connection.ExecuteAsync(
                                @"UPDATE SAD_CUSTOMER_MASTER
              SET CUST_DELFLG = 'A'
              WHERE Cust_ID = @Id",
                                new { custRecord.Id }, transaction
                            );
                        }
                    }

                    //Save Customer Master Details via stored procedure
                    int updateOrSave, customerId;
                    using (var cmdCust = new SqlCommand("spSAD_CUSTOMER_MASTER", connection, transaction))
                    {
                        cmdCust.CommandType = CommandType.StoredProcedure;
                        cmdCust.Parameters.AddWithValue("@CUST_ID", objCust.CUST_ID);
                        cmdCust.Parameters.AddWithValue("@CUST_NAME", objCust.CUST_NAME ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_CODE", objCust.CUST_CODE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_WEBSITE", objCust.CUST_WEBSITE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_EMAIL", objCust.CUST_EMAIL ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_GROUPNAME", objCust.CUST_GROUPNAME ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_GROUPINDIVIDUAL", objCust.CUST_GROUPINDIVIDUAL);
                        cmdCust.Parameters.AddWithValue("@CUST_ORGTYPEID", objCust.CUST_ORGTYPEID);
                        cmdCust.Parameters.AddWithValue("@CUST_INDTYPEID", objCust.CUST_INDTYPEID);
                        cmdCust.Parameters.AddWithValue("@CUST_MGMTTYPEID", objCust.CUST_MGMTTYPEID);
                        cmdCust.Parameters.AddWithValue("@CUST_CommitmentDate", objCust.CUST_CommitmentDate);
                        cmdCust.Parameters.AddWithValue("@CUSt_BranchId", objCust.CUSt_BranchId ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_ADDRESS", objCust.CUST_COMM_ADDRESS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_CITY", objCust.CUST_COMM_CITY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_PIN", objCust.CUST_COMM_PIN ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_STATE", objCust.CUST_COMM_STATE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_COUNTRY", objCust.CUST_COMM_COUNTRY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_FAX", objCust.CUST_COMM_FAX ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_TEL", objCust.CUST_COMM_TEL ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COMM_Email", objCust.CUST_COMM_Email ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_ADDRESS", objCust.CUST_ADDRESS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_CITY", objCust.CUST_CITY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_PIN", objCust.CUST_PIN ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_STATE", objCust.CUST_STATE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_COUNTRY", objCust.CUST_COUNTRY ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_FAX", objCust.CUST_FAX ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_TELPHONE", objCust.CUST_TELPHONE ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_ConEmailID", objCust.CUST_ConEmailID ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_LOCATIONID", objCust.CUST_LOCATIONID ?? "0");
                        cmdCust.Parameters.AddWithValue("@CUST_TASKS", objCust.CUST_TASKS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_ORGID", objCust.CUST_ORGID);
                        cmdCust.Parameters.AddWithValue("@CUST_CRBY", objCust.CUST_CRBY);
                        cmdCust.Parameters.AddWithValue("@CUST_UpdatedBy", objCust.CUST_UpdatedBy);
                        cmdCust.Parameters.AddWithValue("@CUST_BOARDOFDIRECTORS", objCust.CUST_BOARDOFDIRECTORS ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_DEPMETHOD", objCust.CUST_DEPMETHOD);
                        cmdCust.Parameters.AddWithValue("@CUST_IPAddress", objCust.CUST_IPAddress ?? string.Empty);
                        cmdCust.Parameters.AddWithValue("@CUST_CompID", objCust.CUST_CompID);
                        cmdCust.Parameters.AddWithValue("@CUST_Amount_Type", objCust.CUST_Amount_Type);
                        cmdCust.Parameters.AddWithValue("@CUST_RoundOff", objCust.CUST_RoundOff);
                        cmdCust.Parameters.AddWithValue("@Cust_DurtnId", objCust.Cust_DurtnId);
                        cmdCust.Parameters.AddWithValue("@Cust_FY", objCust.Cust_FY);


                        var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdCust.Parameters.Add(updateOrSaveParam);
                        cmdCust.Parameters.Add(operParam);

                        await cmdCust.ExecuteNonQueryAsync();

                        updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        customerId = (int)(operParam.Value ?? 0);
                    }

                    //Step 6: Save Locations
                    if (!string.IsNullOrWhiteSpace(objCust.LocationName))
                    {
                        int locationId;
                        using (var cmdLoc = new SqlCommand("spSAD_CUST_LOCATION", connection, transaction))
                        {
                            cmdLoc.CommandType = CommandType.StoredProcedure;
                            cmdLoc.Parameters.AddWithValue("@Mas_Id", objCust.Mas_Id);
                            cmdLoc.Parameters.AddWithValue("@Mas_code", objCust.Mas_code ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Description", objCust.LocationName ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_DelFlag", objCust.DelFlag ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CustID", customerId);
                            cmdLoc.Parameters.AddWithValue("@Mas_Loc_Address", objCust.Address ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Person", objCust.ContactPerson ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_MobileNo", objCust.Mobile ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_LandLineNo", objCust.Landline ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Email", objCust.Email ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@mas_Designation", objCust.Designation ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CRBY", objCust.CUST_CRBY);
                            cmdLoc.Parameters.AddWithValue("@Mas_UpdatedBy", objCust.CUST_UpdatedBy);
                            cmdLoc.Parameters.AddWithValue("@Mas_STATUS", "A");
                            cmdLoc.Parameters.AddWithValue("@Mas_IPAddress", objCust.CUST_IPAddress ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CompID", objCust.CUST_CompID);

                            var updateOrSaveLoc = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operLoc = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdLoc.Parameters.Add(updateOrSaveLoc);
                            cmdLoc.Parameters.Add(operLoc);

                            await cmdLoc.ExecuteNonQueryAsync();
                            locationId = (int)(operLoc.Value ?? 0);
                        }

                        //Save Statutory Refs for this location
                        async Task SaveStatutoryRef(string desc, string value)
                        {
                            if (string.IsNullOrWhiteSpace(value))
                                return;

                            using var cmdStat = new SqlCommand("spSAD_CUST_Accounting_Template", connection, transaction);
                            cmdStat.CommandType = CommandType.StoredProcedure;
                            cmdStat.Parameters.AddWithValue("@Cust_PKID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_ID", customerId);
                            cmdStat.Parameters.AddWithValue("@Cust_Desc", desc);
                            cmdStat.Parameters.AddWithValue("@Cust_Value", value);
                            cmdStat.Parameters.AddWithValue("@Cust_Delflag", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_Status", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_AttchID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_CrBy", objCust.CUST_CRBY);
                            cmdStat.Parameters.AddWithValue("@Cust_UpdatedBy", objCust.CUST_UpdatedBy);
                            cmdStat.Parameters.AddWithValue("@Cust_IPAddress", objCust.CUST_IPAddress ?? string.Empty);
                            cmdStat.Parameters.AddWithValue("@Cust_Compid", objCust.CUST_CompID);
                            cmdStat.Parameters.AddWithValue("@Cust_LocationId", locationId);

                            var updateOrSaveStat = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operStat = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdStat.Parameters.Add(updateOrSaveStat);
                            cmdStat.Parameters.Add(operStat);

                            await cmdStat.ExecuteNonQueryAsync();
                        }

                        await SaveStatutoryRef("CIN", objCust.CIN);
                        await SaveStatutoryRef("TAN", objCust.TAN);
                        await SaveStatutoryRef("GST", objCust.GST);
                    }

                    results.Add(new[] { updateOrSave, customerId });
                }
                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // SaveClientUser 
        public async Task<List<int[]>> SuperMasterSaveClientUserAsync(int CompId, List<SaveClientUserDto> clientUser)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var results = new List<int[]>();

                foreach (var objEmp in clientUser)
                {
                    // ✅ 1. Vendor check or create
                    const string checkVendorQuery = @"
                SELECT CUST_ID
                FROM SAD_CUSTOMER_MASTER
                WHERE CUST_EMAIL = @EmailId
                  AND CUST_CompID = @CompanyId";

                    var vendorId = await connection.ExecuteScalarAsync<int>(
                        checkVendorQuery,
                        new { EmailId = objEmp.EmailId, CompanyId = CompId },
                        transaction: transaction
                    );

                    if (vendorId == 0)
                    {
                        const string insertVendorQuery = @"
                    INSERT INTO SAD_CUSTOMER_MASTER (CUST_NAME, CUST_EMAIL, CUST_CompID)
                    OUTPUT INSERTED.CUST_ID
                    VALUES (@VendorName, @EmailId, @CompanyId)";

                        vendorId = await connection.ExecuteScalarAsync<int>(
                            insertVendorQuery,
                            new
                            {
                                VendorName = objEmp.VendorName ?? objEmp.EmailId,
                                EmailId = objEmp.EmailId,
                                CompanyId = CompId
                            },
                            transaction: transaction
                        );
                    }
                    objEmp.iUsrCompanyID = vendorId;

                    // ✅ 2. Designation check or create
                    const string checkDesignationQuery = @"
                SELECT Mas_ID
                FROM SAD_GRPDESGN_General_Master
                WHERE Mas_ID = @DesignationID
                  AND Mas_CompID = @CompId";

                    var designationId = await connection.ExecuteScalarAsync<int>(
                        checkDesignationQuery,
                        new { DesignationID = objEmp.iUsrDesignation, CompId = CompId },
                        transaction: transaction
                    );

                    if (designationId == 0)
                    {
                        const string insertDesignationQuery = @"
                    INSERT INTO SAD_GRPDESGN_General_Master (Mas_Description, Mas_CompID)
                    OUTPUT INSERTED.Mas_ID
                    VALUES (@DesignationName, @CompId)";

                        designationId = await connection.ExecuteScalarAsync<int>(
                            insertDesignationQuery,
                            new
                            {
                                DesignationName = objEmp.DesignationName ?? "New Designation",
                                CompId = CompId
                            },
                            transaction: transaction
                        );
                    }
                    objEmp.iUsrDesignation = designationId;

                    // ✅ 3. Save/Update Employee using stored procedure
                    int updateOrSave, oper;
                    using var command = new SqlCommand("spEmployeeMaster", connection, transaction);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Usr_ID", objEmp.iUserID);
                    command.Parameters.AddWithValue("@Usr_Node", objEmp.iUsrNode);
                    command.Parameters.AddWithValue("@Usr_Code", objEmp.sUsrCode ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_FullName", objEmp.sUsrFullName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_LoginName", objEmp.sUsrLoginName ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Password", objEmp.sUsrPassword ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Email", objEmp.sUsrEmail ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Category", objEmp.iUsrSentMail);
                    command.Parameters.AddWithValue("@Usr_Suggetions", objEmp.iUsrSuggetions);
                    command.Parameters.AddWithValue("@usr_partner", objEmp.iUsrPartner);
                    command.Parameters.AddWithValue("@Usr_LevelGrp", objEmp.iUsrLevelGrp);
                    command.Parameters.AddWithValue("@Usr_DutyStatus", objEmp.sUsrDutyStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_PhoneNo", objEmp.sUsrPhoneNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_MobileNo", objEmp.sUsrMobileNo ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OfficePhone", objEmp.sUsrOfficePhone ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_OffPhExtn", objEmp.sUsrOffPhExtn ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Designation", objEmp.iUsrDesignation);
                    command.Parameters.AddWithValue("@Usr_CompanyID", objEmp.iUsrCompanyID);
                    command.Parameters.AddWithValue("@Usr_OrgnID", objEmp.iUsrOrgID);
                    command.Parameters.AddWithValue("@Usr_GrpOrUserLvlPerm", objEmp.iUsrGrpOrUserLvlPerm);
                    command.Parameters.AddWithValue("@Usr_Role", objEmp.iUsrDesignation);
                    command.Parameters.AddWithValue("@Usr_MasterModule", objEmp.iUsrMasterModule);
                    command.Parameters.AddWithValue("@Usr_AuditModule", objEmp.iUsrAuditModule);
                    command.Parameters.AddWithValue("@Usr_RiskModule", objEmp.iUsrRiskModule);
                    command.Parameters.AddWithValue("@Usr_ComplianceModule", objEmp.iUsrComplianceModule);
                    command.Parameters.AddWithValue("@Usr_BCMModule", objEmp.iUsrBCMmodule);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeModule", objEmp.iUsrDigitalOfficeModule);
                    command.Parameters.AddWithValue("@Usr_MasterRole", objEmp.iUsrMasterRole);
                    command.Parameters.AddWithValue("@Usr_AuditRole", objEmp.iUsrAuditRole);
                    command.Parameters.AddWithValue("@Usr_RiskRole", objEmp.iUsrRiskRole);
                    command.Parameters.AddWithValue("@Usr_ComplianceRole", objEmp.iUsrComplianceRole);
                    command.Parameters.AddWithValue("@Usr_BCMRole", objEmp.iUsrBCMRole);
                    command.Parameters.AddWithValue("@Usr_DigitalOfficeRole", objEmp.iUsrDigitalOfficeRole);
                    command.Parameters.AddWithValue("@Usr_CreatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_UpdatedBy", objEmp.iUsrCreatedBy);
                    command.Parameters.AddWithValue("@Usr_DelFlag", objEmp.sUsrFlag ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_Status", objEmp.sUsrStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_IPAddress", objEmp.Usr_IPAdress ?? string.Empty);
                    command.Parameters.AddWithValue("@Usr_CompId", objEmp.iUsrCompID);
                    command.Parameters.AddWithValue("@Usr_Type", objEmp.sUsrType ?? string.Empty);
                    command.Parameters.AddWithValue("@usr_IsSuperuser", objEmp.iusr_IsSuperuser);
                    command.Parameters.AddWithValue("@USR_DeptID", objEmp.iUSR_DeptID);
                    command.Parameters.AddWithValue("@USR_MemberType", objEmp.iUSR_MemberType);
                    command.Parameters.AddWithValue("@USR_Levelcode", objEmp.iUSR_Levelcode);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(updateOrSaveParam);
                    command.Parameters.Add(operParam);

                    await command.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0);

                    results.Add(new[] { updateOrSave, oper });
                }

                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //DownloadEmployeeMaster
        public EmployeeMasterResult GetEmployeeMasterExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SampleExcels\\EmployeeMaster Template.xlsx";

            if (!File.Exists(filePath))
                return new EmployeeMasterResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "EmployeeMaster Template.xlsx"; 
            var contentType = "application/vnd.ms-excel"; 

            return new EmployeeMasterResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadClientDetails
        public ClientDetailsResult GetClientDetailsExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SampleExcels\\ClientDetails Template.xlsx";

            if (!File.Exists(filePath))
                return new ClientDetailsResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ClientDetails Template.xlsx";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new ClientDetailsResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadClientuser
        public ClientUserResult GetClientUserExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SampleExcels\\ClientUser Template.xlsx";

            if (!File.Exists(filePath))
                return new ClientUserResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ClientUser Template.xlsx";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new ClientUserResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadExcelTemplateFiles
        public ExcelTemplateResult GetExcelTemplate(string templateName)
        {
            var templates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Employee Master", @"C:\Users\SSD\Desktop\Current BackEnd\tracepa-corebackend\SampleExcels\EmployeeMaster Excel Format.xlsx" },
            { "Client Details", @"C:\Users\SSD\Desktop\Current BackEnd\tracepa-corebackend\SampleExcels\ClientDetails Format.xlsx" },
            { "Client User", @"C:\Users\SSD\Desktop\Current BackEnd\tracepa-corebackend\SampleExcels\ClientUser Format.xlsx" },
            { "Audit Type & Checkpoints", @"C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\SampleExcels\AuditChecklistMaster.xlsx" },
            { "Task & SubTasks", @"C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\SampleExcels\TaskMaster.xlsx" }
        };

            if (!templates.ContainsKey(templateName))
                return null;

            var filePath = templates[templateName];

            if (!File.Exists(filePath))
                return null;

            var bytes = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            // Set content type dynamically based on extension
            var contentType = Path.GetExtension(filePath).ToLower() switch
            {
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                _ => "application/octet-stream"
            };

            return new ExcelTemplateResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        public class AuditTypeAndCheckpointsUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }
            public AuditTypeAndCheckpointsUploadException(Dictionary<string, List<string>> errors)
            {
                Errors = errors;
            }
        }

        public async Task<List<string>> UploadAuditTypeAndCheckpointsAsync(int compId, int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);

            AuditTypeAndCheckpointParseResult parseResult;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var stream = file.OpenReadStream();
                parseResult = await ParseAuditTypeAndCheckpointExcelAndResolveAuditTypeAsync(stream, compId, userId, connection);
            }

            var rows = parseResult.Rows;
            var headerErrors = parseResult.HeaderErrors;
            var missingErrors = ValidateAuditChecklist(rows);

            var duplicateErrors = rows
                .GroupBy(r => new { r.ACM_AuditTypeID, r.ACM_Heading, r.ACM_Checkpoint })
                .Where(g => g.Count() > 1)
                .Select(g => $"Duplicate: Heading '{g.Key.ACM_Heading}' - Checkpoint '{g.Key.ACM_Checkpoint}'")
                .ToList();

            var finalErrors = new Dictionary<string, List<string>>();
            if (headerErrors.Any()) finalErrors["Missing column"] = headerErrors;
            if (missingErrors.Any()) finalErrors["Missing values"] = missingErrors;
            if (duplicateErrors.Any()) finalErrors["Duplication"] = duplicateErrors;

            if (finalErrors.Any())
                throw new AuditTypeAndCheckpointsUploadException(finalErrors);

            var failed = new List<string>();

            foreach (var dto in rows)
            {
                dto.ACM_CompId = compId;
                dto.ACM_CRBY = userId;
                dto.ACM_IPAddress = "127.0.0.1";

                if (!await SaveOrUpdateAuditTypeAndCheckpointAsync(dto))
                    failed.Add(dto.ACM_Checkpoint);
            }

            if (failed.Any())
            {
                return new List<string> { "Failed to save the following Audit Type And Checkpoints:" }
                    .Concat(failed).ToList();
            }

            return new List<string> { "Successfully saved Audit Type And Checkpoints details" };
        }

        private async Task<AuditTypeAndCheckpointParseResult> ParseAuditTypeAndCheckpointExcelAndResolveAuditTypeAsync(Stream stream, int compId, int userId, SqlConnection connection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            var sheet = package.Workbook.Worksheets[0];
            int rowCount = sheet.Dimension.Rows;                            

            var result = new AuditTypeAndCheckpointParseResult();
            string[] expectedHeaders = { "Audit Type", "Heading", "Checkpoint", "Assertions" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
                if (!string.Equals(sheet.Cells[1, col].Text?.Trim(), expectedHeaders[col - 1], StringComparison.OrdinalIgnoreCase))
                    result.HeaderErrors.Add($"Expected header '{expectedHeaders[col - 1]}' at column {col}, but found '{sheet.Cells[1, col].Text}'");

            int? lastAuditTypeId = null;

            for (int row = 2; row <= rowCount; row++)
            {
                string typeName = sheet.Cells[row, 1].Text?.Trim();
                int? typeId = null;

                if (!string.IsNullOrWhiteSpace(typeName))
                {
                    typeId = await connection.ExecuteScalarAsync<int?>(
                        "SELECT cmm_ID FROM content_management_master WHERE UPPER(cmm_desc)=UPPER(@type) AND cmm_compID=@cid AND cmm_category='AT'",
                        new { type = typeName, cid = compId });

                    if (!typeId.HasValue)
                        typeId = await InsertNewAuditTypeAsync(typeName, compId, userId, connection, null);

                    lastAuditTypeId = typeId;
                }
                else
                {
                    if (lastAuditTypeId == null)
                        result.HeaderErrors.Add($"Audit Type missing at row {row}.");

                    typeId = lastAuditTypeId;
                }

                result.Rows.Add(new AuditTypeChecklistMasterDTO
                {
                    ACM_AuditTypeID = typeId ?? 0,
                    ACM_Heading = sheet.Cells[row, 2].Text,
                    ACM_Checkpoint = sheet.Cells[row, 3].Text,
                    ACM_Assertions = sheet.Cells[row, 4].Text
                });
            }
            return result;
        }

        private async Task<int> InsertNewAuditTypeAsync(string taskName, int compId, int userId, SqlConnection connection, SqlTransaction tx)
        {
            int maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(cmm_ID), 0) + 1 FROM Content_Management_Master WHERE CMM_CompID = @CompId", new { CompId = compId }, tx);

            string code = $"AT_{maxId}";

            int newId = await connection.ExecuteScalarAsync<int>(
                @"DECLARE @NewId INT = (SELECT ISNULL(MAX(CMM_ID),0) + 1 FROM Content_Management_Master);
                 INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMM_Delflag, CMM_Status, CMM_CrBy, CMM_CrOn, CMM_CompID) 
                 VALUES (@NewId, @CMM_Code, @CMM_Desc, 'AT', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_CompID);
                 SELECT @NewId;",
                new
                {
                    CMM_Code = code,
                    CMM_Desc = taskName,
                    CMM_CompID = compId,
                    CMM_CrBy = userId
                }, tx);

            return newId;
        }

        private List<string> ValidateAuditChecklist(List<AuditTypeChecklistMasterDTO> rows)
        {
            var errors = new List<string>();

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int excelRow = i + 2;

                if (r.ACM_AuditTypeID == 0)
                    errors.Add($"Row {excelRow}: Audit Type missing for checkpoint '{r.ACM_Checkpoint}'");

                if (string.IsNullOrWhiteSpace(r.ACM_Heading))
                    errors.Add($"Row {excelRow}: Heading missing for checkpoint '{r.ACM_Checkpoint}'");

                if (string.IsNullOrWhiteSpace(r.ACM_Checkpoint))
                    errors.Add($"Row {excelRow}: Checkpoint missing for heading '{r.ACM_Heading}'");
            }
            return errors;
        }

        public async Task<bool> SaveOrUpdateAuditTypeAndCheckpointAsync(AuditTypeChecklistMasterDTO dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                int exists = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM AuditType_Checklist_Master WHERE ACM_AuditTypeID = @ACM_AuditTypeID AND ACM_Checkpoint = @ACM_Checkpoint AND ACM_CompId = @ACM_CompId",
                    new
                    {
                        dto.ACM_AuditTypeID,
                        dto.ACM_Checkpoint,
                        dto.ACM_CompId
                    }, tx);

                if (exists > 0)
                {
                    await tx.CommitAsync();
                    return true;
                }

                int maxId = await connection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX(ACM_ID),0)+1 FROM AuditType_Checklist_Master WHERE ACM_CompId=@CompId",
                    new { CompId = dto.ACM_CompId }, tx);

                dto.ACM_Code = $"ACM_{maxId}";

                dto.ACM_ID = await connection.ExecuteScalarAsync<int>(
                    @"DECLARE @NewId INT = (SELECT ISNULL(MAX(ACM_ID),0)+1 FROM AuditType_Checklist_Master);
                      INSERT INTO AuditType_Checklist_Master (ACM_ID, ACM_Code, ACM_AuditTypeID, ACM_Heading, ACM_Checkpoint, ACM_Assertions, ACM_DELFLG, ACM_STATUS, ACM_CRBY, ACM_CRON, ACM_IPAddress, ACM_CompId)
                      VALUES (@NewId, @ACM_Code, @ACM_AuditTypeID, @ACM_Heading, @ACM_Checkpoint, @ACM_Assertions, 'A', 'A', @ACM_CRBY, GETDATE(), @ACM_IPAddress, @ACM_CompId);
                      SELECT @NewId;",
                    dto, tx);

                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                return false;
            }
        }

        public class TaskAndSubTasksUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }
            public TaskAndSubTasksUploadException(Dictionary<string, List<string>> errors)
            {
                Errors = errors;
            }
        }

        public async Task<List<string>> UploadTaskAndSubTasksAsync(int compId, int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);
            TaskAndSubtasksParseResult parseResult;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var stream = file.OpenReadStream();
                parseResult = await ParseTaskAndSubtasksExcelAndResolveTaskAsync(stream, compId, userId, connection);
            }

            var rows = parseResult.Rows;
            var headerErrors = parseResult.HeaderErrors;
            var missingErrors = ValidateTaskAndSubtasklist(rows);

            var duplicateErrors = rows
                .GroupBy(r => new { r.ACM_AssignmentTaskID, r.ACM_Checkpoint })
                .Where(g => g.Count() > 1)
                .Select(g => $"Duplicate: Task '{g.Key.ACM_AssignmentTaskID}' - Subtask '{g.Key.ACM_Checkpoint}'")
                .ToList();

            var finalErrors = new Dictionary<string, List<string>>();
            if (headerErrors.Any()) finalErrors["Missing column"] = headerErrors;
            if (missingErrors.Any()) finalErrors["Missing values"] = missingErrors;
            if (duplicateErrors.Any()) finalErrors["Duplication"] = duplicateErrors;

            if (finalErrors.Any())
                throw new TaskAndSubTasksUploadException(finalErrors);

            var results = new List<string>();
            var failedCheckpoints = new List<string>();

            foreach (var dto in rows)
            {
                dto.ACM_CompId = compId;
                dto.ACM_CRBY = userId;
                dto.ACM_IPAddress = "127.0.0.1";

                if (!await SaveOrUpdateTaskAndSubTasklistAsync(dto))
                    failedCheckpoints.Add(dto.ACM_Checkpoint);
            }

            if (failedCheckpoints.Any())
            {
                results.Add("Failed to save the following Task And Subtask:");
                results.AddRange(failedCheckpoints);
                return results;
            }

            results.Add("Successfully saved Task And Subtask details");
            return results;
        }

        private async Task<TaskAndSubtasksParseResult> ParseTaskAndSubtasksExcelAndResolveTaskAsync(Stream stream, int compId, int userId, SqlConnection connection)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            var sheet = package.Workbook.Worksheets[0];
            int rowCount = sheet.Dimension.Rows;

            var result = new TaskAndSubtasksParseResult();

            string[] expectedHeaders = { "Task", "Subtask", "Billing Type" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
                if (!string.Equals(sheet.Cells[1, col].Text?.Trim(), expectedHeaders[col - 1], StringComparison.OrdinalIgnoreCase))
                    result.HeaderErrors.Add($"Expected header '{expectedHeaders[col - 1]}' at column {col}, found '{sheet.Cells[1, col].Text}'");

            int? lastTaskId = null;
            for (int row = 2; row <= rowCount; row++)
            {
                string taskName = sheet.Cells[row, 1].Text?.Trim();
                int? taskId = null;

                if (!string.IsNullOrWhiteSpace(taskName))
                {
                    taskId = await connection.ExecuteScalarAsync<int?>(
                        "SELECT cmm_ID FROM content_management_master WHERE UPPER(cmm_desc)=UPPER(@task) AND cmm_compID=@cid AND cmm_category='ASGT'",
                        new { task = taskName, cid = compId });

                    if (!taskId.HasValue)
                        taskId = await InsertNewTaskAsync(taskName, compId, userId, connection, null);

                    lastTaskId = taskId;
                }
                else
                {
                    if (lastTaskId == null)
                        result.HeaderErrors.Add($"Task missing at row {row}, and no previous task found.");

                    taskId = lastTaskId;
                }

                string billingText = sheet.Cells[row, 3].Text?.Trim().ToLower();
                int billingType = billingText switch
                {
                    "billable" => 1,
                    "non billable" => 0,
                    "" => 0,
                    null => 0,
                    _ => 0
                };

                result.Rows.Add(new AssignmentTaskChecklistMasterDTO
                {
                    ACM_AssignmentTaskID = taskId ?? 0,
                    ACM_Heading = "",
                    ACM_Checkpoint = sheet.Cells[row, 2].Text,
                    ACM_BillingType = billingType
                });
            }
            return result;
        }

        private async Task<int> InsertNewTaskAsync(string taskName, int compId,int userId, SqlConnection connection, SqlTransaction tx)
        {
            int maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(cmm_ID), 0) + 1 FROM Content_Management_Master WHERE CMM_CompID = @CompId", new { CompId = compId }, tx);

            string code = $"ASGT_{maxId}";

            int newId = await connection.ExecuteScalarAsync<int>(
                @"DECLARE @NewId INT = (SELECT ISNULL(MAX(CMM_ID),0) + 1 FROM Content_Management_Master);
                 INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMM_Delflag, CMM_Status, CMM_CrBy, CMM_CrOn, CMM_CompID) 
                 VALUES (@NewId, @CMM_Code, @CMM_Desc, 'ASGT', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_CompID);
                 SELECT @NewId;",
                new
                {
                    CMM_Code = code,
                    CMM_Desc = taskName,
                    CMM_CompID = compId,
                    CMM_CrBy = userId
                }, tx);

            return newId;
        }


        private List<string> ValidateTaskAndSubtasklist(List<AssignmentTaskChecklistMasterDTO> rows)
        {
            var errors = new List<string>();
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int excelRow = i + 2;

                if (r.ACM_AssignmentTaskID == 0)
                    errors.Add($"Row {excelRow}: Task missing for Subtask '{r.ACM_Checkpoint}'");

                if (string.IsNullOrWhiteSpace(r.ACM_Checkpoint))
                    errors.Add($"Row {excelRow}: Subtask missing for the Task");
            }
            return errors;
        }


        public async Task<bool> SaveOrUpdateTaskAndSubTasklistAsync(AssignmentTaskChecklistMasterDTO dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                int exists = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM AssignmentTask_Checklist_Master WHERE ACM_AssignmentTaskID = @ACM_AssignmentTaskID AND ACM_Checkpoint = @ACM_Checkpoint AND ACM_CompId = @ACM_CompId",
                    new
                    {
                        dto.ACM_AssignmentTaskID,
                        dto.ACM_Checkpoint,
                        dto.ACM_CompId
                    }, tx);

                if (exists > 0)
                {
                    await tx.CommitAsync();
                    return true;
                }

                int maxId = await connection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX(ACM_ID),0) + 1 FROM AssignmentTask_Checklist_Master WHERE ACM_CompId=@CompId",
                    new { CompId = dto.ACM_CompId }, tx);

                dto.ACM_Code = $"ATCM_{maxId}";

                dto.ACM_ID = await connection.ExecuteScalarAsync<int>(
                    @"DECLARE @NewId INT = (SELECT ISNULL(MAX(ACM_ID),0) + 1 FROM AssignmentTask_Checklist_Master);
                    INSERT INTO AssignmentTask_Checklist_Master (ACM_ID, ACM_Code, ACM_AssignmentTaskID, ACM_Heading, ACM_Checkpoint, ACM_BillingType,
                    ACM_DELFLG, ACM_STATUS, ACM_CRBY, ACM_CRON, ACM_IPAddress, ACM_CompId) VALUES(@NewId, @ACM_Code, @ACM_AssignmentTaskID, @ACM_Heading, @ACM_Checkpoint, @ACM_BillingType, 'A', 'A', @ACM_CRBY, GETDATE(), @ACM_IPAddress, @ACM_CompId);
                    SELECT @NewId;",
                    dto, tx);

                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                return false;
            }
        }

        //UploadEmployeeMasters
        public class EmployeeUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public EmployeeUploadException(Dictionary<string, List<string>> errors)
                : base("Error processing employee master")
            {
                Errors = errors;
            }
        }
        public async Task<List<string>> UploadEmployeeDetailsAsync(int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            var connectionString = _configuration.GetConnectionString(dbName);

            List<UploadEmployeeMasterDto> employees;
            List<string> headerErrors;
            bool hasDataRows;

            using (var stream = file.OpenReadStream())
            {
                employees = ParseExcelToEmployees(stream, out headerErrors, out hasDataRows);
            }

            // Excel contains only headers
            if (!hasDataRows)
            {
                throw new EmployeeUploadException(new Dictionary<string, List<string>>
                {
                    ["File Error"] = new List<string>
            {
                "Excel file contains only headers with no data. Please add employee records."
            }
                });
            }

            var validationErrors = ValidateEmployees(employees);


            var duplicateErrors = new List<UploadEmployeeMasterDto>();

            void CheckDuplicates(Func<UploadEmployeeMasterDto, string> selector, string field)
            {
                duplicateErrors.AddRange(
                    employees
                        .GroupBy(selector)
                        .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                        .Select(g => new UploadEmployeeMasterDto
                        {
                            EmpCode = g.First().EmpCode,
                            EmployeeName = g.First().EmployeeName,
                            ErrorMessage = $"Duplicate {field}: {g.Key}"
                        })
                );
            }

            CheckDuplicates(e => e.LoginName, "LoginName");
            CheckDuplicates(e => e.Email, "Email");
            CheckDuplicates(e => e.MobileNo, "MobileNo");

            var finalErrors = new Dictionary<string, List<string>>();

            if (headerErrors.Any())
                finalErrors["Header Errors"] = headerErrors;

            if (validationErrors.Any())
            {
                finalErrors["Validation Errors"] = validationErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeName })
                    .Select(g =>
                        $"{g.Key.EmployeeName} ({g.Key.EmpCode}): " +
                        string.Join(", ", g.Select(x => x.ErrorMessage)))
                    .ToList();
            }

            if (duplicateErrors.Any())
            {
                finalErrors["Duplicate Errors"] = duplicateErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeName })
                    .Select(g =>
                        $"{g.Key.EmployeeName} ({g.Key.EmpCode}): " +
                        string.Join(", ", g.Select(x => x.ErrorMessage)))
                    .ToList();
            }

            if (finalErrors.Any())
                throw new EmployeeUploadException(finalErrors);


            var results = new List<string>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {

                for (int i = 0; i < employees.Count; i++)
                {
                    employees[i].EmpCode = $"TEMP{i + 1}";
                }
                foreach (var emp in employees)
                {

                    int usrId = 0;
                    int usrNode = 0;
                    int usrCategory = 0;
                    int usrSuggestions = 0;
                    int usrPartner = 0;
                    int usrLevelGrp = 0;
                    string usrDutyStatus = "A";
                    string usrDelFlag = "A";
                    string usrStatus = "N";
                    string usrType = "C";
                    int usrIsSuperuser = 0;
                    int usrDeptId = 0;
                    int usrMemberType = 0;
                    int usrLevelCode = 0;
                    int usrDesignation = 0;
                    int usrOrgnId = 0;
                    string usrOffPhExtn = "";
                    int usrMasterModule = 0;
                    int usrAuditModule = 0;
                    int usrRiskModule = 0;
                    int usrComplianceModule = 0;
                    int usrBcmModule = 0;
                    int usrDigitalOfficeModule = 0;
                    int usrMasterRole = 0;
                    int usrAuditRole = 0;
                    int usrRiskRole = 0;
                    int usrComplianceRole = 0;
                    int usrBcmRole = 0;
                    int usrDigitalOfficeRole = 0;

                    int createdBy = emp.CreatedBy ?? 1;
                    int updatedBy = emp.UpdatedBy ?? 1;
                    string ipAddress = emp.IPAddress ?? "";

                    int grpOrUserLvlPerm = emp.Permission?.ToLower() == "group" ? 1 : 2;

                    string roleSql = @"
                SELECT Mas_ID 
                FROM SAD_GrpOrLvl_General_Master
                WHERE UPPER(Mas_Description) = UPPER(@Role)
                  AND Mas_CompID = @CompId";

                    int? roleId = await connection.ExecuteScalarAsync<int?>(
                        roleSql, new { Role = emp.Role, CompId = compId }, transaction);

                    //if (!roleId.HasValue)
                    //{
                    //    string insertRole = @"
                    //INSERT INTO SAD_GrpOrLvl_General_Master (Mas_Description, Mas_CompID)
                    //VALUES (@Role, @CompId);
                    //SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    //    roleId = await connection.ExecuteScalarAsync<int>(
                    //        insertRole, new { Role = emp.Role, CompId = compId }, transaction);
                    //}
                    var parameters = new DynamicParameters();

                    parameters.Add("@Usr_ID", usrId);
                    parameters.Add("@Usr_Node", usrNode);
                    parameters.Add("@Usr_Category", usrCategory);
                    parameters.Add("@Usr_Suggetions", usrSuggestions);
                    parameters.Add("@usr_partner", usrPartner);
                    parameters.Add("@Usr_LevelGrp", usrLevelGrp);
                    parameters.Add("@Usr_DutyStatus", usrDutyStatus);
                    parameters.Add("@Usr_OffPhExtn", usrOffPhExtn);
                    parameters.Add("@Usr_Designation", usrDesignation);
                    parameters.Add("@Usr_OrgnID", usrOrgnId);
                    parameters.Add("@Usr_GrpOrUserLvlPerm", grpOrUserLvlPerm);

                    parameters.Add("@Usr_MasterModule", usrMasterModule);
                    parameters.Add("@Usr_AuditModule", usrAuditModule);
                    parameters.Add("@Usr_RiskModule", usrRiskModule);
                    parameters.Add("@Usr_ComplianceModule", usrComplianceModule);
                    parameters.Add("@Usr_BCMModule", usrBcmModule);
                    parameters.Add("@Usr_DigitalOfficeModule", usrDigitalOfficeModule);

                    parameters.Add("@Usr_MasterRole", usrMasterRole);
                    parameters.Add("@Usr_AuditRole", usrAuditRole);
                    parameters.Add("@Usr_RiskRole", usrRiskRole);
                    parameters.Add("@Usr_ComplianceRole", usrComplianceRole);
                    parameters.Add("@Usr_BCMRole", usrBcmRole);
                    parameters.Add("@Usr_DigitalOfficeRole", usrDigitalOfficeRole);

                    parameters.Add("@Usr_DelFlag", usrDelFlag);
                    parameters.Add("@Usr_Status", usrStatus);
                    parameters.Add("@Usr_Type", usrType);
                    parameters.Add("@usr_IsSuperuser", usrIsSuperuser);
                    parameters.Add("@USR_DeptID", usrDeptId);
                    parameters.Add("@USR_MemberType", usrMemberType);
                    parameters.Add("@USR_Levelcode", usrLevelCode);

                    parameters.Add("@Usr_CreatedBy", createdBy);
                    parameters.Add("@Usr_UpdatedBy", updatedBy);
                    parameters.Add("@Usr_IPAddress", ipAddress);
                    parameters.Add("@Usr_CompId", compId);

                    parameters.Add("@Usr_Code", emp.EmpCode);
                    parameters.Add("@Usr_FullName", emp.EmployeeName);
                    parameters.Add("@Usr_LoginName", emp.LoginName);
                    parameters.Add("@Usr_Password",
                        string.IsNullOrWhiteSpace(emp.Password) ? null : emp.Password);
                    parameters.Add("@Usr_Email", emp.Email);
                    parameters.Add("@Usr_PhoneNo", emp.MobileNo);
                    parameters.Add("@Usr_MobileNo", emp.MobileNo);
                    parameters.Add("@Usr_OfficePhone", emp.MobileNo);
                    parameters.Add("@Usr_CompanyID", compId);
                    parameters.Add("@Usr_Role", roleId.Value);

                    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spEmployeeMaster",
                        parameters,
                        transaction,
                        commandType: CommandType.StoredProcedure);

                    int updateOrSave = parameters.Get<int>("@iUpdateOrSave");
                    int oper = parameters.Get<int>("@iOper");

                    emp.EmpCode = $"EMP00{oper.ToString().PadLeft(3, '0')}"; // Format: EMP00430, EMP00431, etc.

                    // 🔹 UPDATE the employee code in the database
                    string updateEmpCodeSql = @"
        UPDATE Sad_UserDetails 
        SET Usr_Code = @EmpCode 
        WHERE Usr_ID = @UsrId AND Usr_CompanyID = @CompId";

                    await connection.ExecuteAsync(updateEmpCodeSql,
                        new
                        {
                            EmpCode = emp.EmpCode,
                            UsrId = oper,
                            CompId = compId
                        },
                        transaction);

                    string action = updateOrSave == 2 ? "Updated" : "Inserted";
                    results.Add($"{action} employee: {emp.EmpCode} - {emp.EmployeeName} (Usr_ID={oper})");
                }
                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private List<UploadEmployeeMasterDto> ParseExcelToEmployees(
            Stream stream,
            out List<string> headerErrors,
            out bool hasDataRows)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);
            var sheet = package.Workbook.Worksheets[0];

            string[] expectedHeaders =
            {
                "Employee Name",
                "Email",
                "Mobile No",
                "Login Name",
                "Role",
                "Permission"
            };

            headerErrors = new List<string>();
            hasDataRows = false;

            for (int i = 0; i < expectedHeaders.Length; i++)
            {
                var actual = sheet.Cells[1, i + 1].Text.Trim();
                if (!actual.Equals(expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    headerErrors.Add(
                        $"Expected '{expectedHeaders[i]}' but found '{actual}' at column {i + 1}");
                }
            }

            var list = new List<UploadEmployeeMasterDto>();

            if (sheet.Dimension == null || sheet.Dimension.Rows <= 1)
            {
                return list;
            }

            for (int row = 2; row <= sheet.Dimension.Rows; row++)
            {
                string empName = sheet.Cells[row, 1].Text?.Trim();
                string email = sheet.Cells[row, 2].Text?.Trim();
                string mobile = sheet.Cells[row, 3].Text?.Trim();
                string loginName = sheet.Cells[row, 4].Text?.Trim();
                string role = sheet.Cells[row, 5].Text?.Trim();
                string permission = sheet.Cells[row, 6].Text?.Trim();

                bool isRowEmpty =
                    string.IsNullOrWhiteSpace(empName) &&
                    string.IsNullOrWhiteSpace(email) &&
                    string.IsNullOrWhiteSpace(mobile) &&
                    string.IsNullOrWhiteSpace(loginName) &&
                    string.IsNullOrWhiteSpace(role) &&
                    string.IsNullOrWhiteSpace(permission);

                if (isRowEmpty)
                {
                    continue;
                }

                bool hasAnyValue =
                    !string.IsNullOrWhiteSpace(empName) ||
                    !string.IsNullOrWhiteSpace(email) ||
                    !string.IsNullOrWhiteSpace(mobile) ||
                    !string.IsNullOrWhiteSpace(loginName) ||
                    !string.IsNullOrWhiteSpace(role) ||
                    !string.IsNullOrWhiteSpace(permission);

                if (hasAnyValue)
                {
                    hasDataRows = true;
                }

                list.Add(new UploadEmployeeMasterDto
                {
                    EmpCode = string.Empty,
                    EmployeeName = empName,
                    Email = email,
                    MobileNo = mobile,
                    LoginName = loginName,
                    Role = role,
                    Permission = permission
                });
            }

            return list;
        }

        private List<UploadEmployeeMasterDto> ValidateEmployees(List<UploadEmployeeMasterDto> employees)
        {
            var errors = new List<UploadEmployeeMasterDto>();

            foreach (var emp in employees)
            {
                bool isRowEmpty =
                    string.IsNullOrWhiteSpace(emp.EmployeeName) &&
                    string.IsNullOrWhiteSpace(emp.Email) &&
                    string.IsNullOrWhiteSpace(emp.MobileNo) &&
                    string.IsNullOrWhiteSpace(emp.LoginName) &&
                    string.IsNullOrWhiteSpace(emp.Role) &&
                    string.IsNullOrWhiteSpace(emp.Permission);

                if (isRowEmpty)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(emp.EmployeeName))
                {
                    errors.Add(new UploadEmployeeMasterDto
                    {
                        EmpCode = emp.EmpCode,
                        EmployeeName = emp.EmployeeName,
                        ErrorMessage = "Employee Name is required"
                    });
                }

                if (string.IsNullOrWhiteSpace(emp.Email))
                {
                    errors.Add(new UploadEmployeeMasterDto
                    {
                        EmpCode = emp.EmpCode,
                        EmployeeName = emp.EmployeeName,
                        ErrorMessage = "Email is required"
                    });
                }
                else
                {
                    string trimmedEmail = emp.Email.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(
                        trimmedEmail,
                        @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadEmployeeMasterDto
                        {
                            EmpCode = emp.EmpCode,
                            EmployeeName = emp.EmployeeName,
                            ErrorMessage = "Invalid Gmail format. Must be like 'username@gmail.com'"
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(emp.MobileNo))
                {
                    errors.Add(new UploadEmployeeMasterDto
                    {
                        EmpCode = emp.EmpCode,
                        EmployeeName = emp.EmployeeName,
                        ErrorMessage = "Mobile No is required"
                    });
                }
                else
                {
                    string phone = emp.MobileNo.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$"))
                    {
                        errors.Add(new UploadEmployeeMasterDto
                        {
                            EmpCode = emp.EmpCode,
                            EmployeeName = emp.EmployeeName,
                            ErrorMessage = "Mobile No must be exactly 10 digits"
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(emp.LoginName))
                {
                    errors.Add(new UploadEmployeeMasterDto
                    {
                        EmpCode = emp.EmpCode,
                        EmployeeName = emp.EmployeeName,
                        ErrorMessage = "Login Name is required"
                    });
                }
                else
                {
                    string trimmedLogin = emp.LoginName.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(
                        trimmedLogin,
                        @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadEmployeeMasterDto
                        {
                            EmpCode = emp.EmpCode,
                            EmployeeName = emp.EmployeeName,
                            ErrorMessage = "Invalid Login Name format. Must be like 'username@gmail.com'"
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(emp.Role))
                {
                    errors.Add(new UploadEmployeeMasterDto
                    {
                        EmpCode = emp.EmpCode,
                        EmployeeName = emp.EmployeeName,
                        ErrorMessage = "Role is required"
                    });
                }

                if (string.IsNullOrWhiteSpace(emp.Permission))
                {
                    errors.Add(new UploadEmployeeMasterDto
                    {
                        EmpCode = emp.EmpCode,
                        EmployeeName = emp.EmployeeName,
                        ErrorMessage = "Permission is required"
                    });
                }
            }
            return errors;
        }

        //ChangedUploadClientUser
        public class ClientUserUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public ClientUserUploadException(Dictionary<string, List<string>> errors)
                : base("Error processing client user master")
            {
                Errors = errors;
            }
        }

        public async Task<List<string>> UploadClientUsersAsync(int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            string connectionString = _configuration.GetConnectionString(dbName);

            List<UploadClientUserDto> users;
            List<string> headerErrors;
            bool hasDataRows;

            using (var stream = file.OpenReadStream())
            {
                users = ParseClientUserExcel(stream, out headerErrors, out hasDataRows);
            }

            // Check if Excel has only headers with no data
            if (!hasDataRows)
            {
                var emptyExcelErrors = new Dictionary<string, List<string>>
                {
                    ["File Error"] = new List<string> { "Excel file contains only headers with no data. Please add client user records." }
                };
                throw new ClientUserUploadException(emptyExcelErrors);
            }

            var validationErrors = ValidateClientUsers(users);

            // ================= DUPLICATE CHECK =================
            var duplicateErrors = new List<UploadClientUserDto>();

            void CheckDuplicates(Func<UploadClientUserDto, string> selector, string field)
            {
                duplicateErrors.AddRange(
                    users.GroupBy(selector)
                         .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                         .Select(g => new UploadClientUserDto
                         {
                             EmpCode = string.Empty, // Empty for parentheses
                             EmployeeFullName = g.First().EmployeeFullName,
                             ErrorMessage = $"Duplicate {field}: {g.Key}"
                         })
                );
            }

            // Only check duplicates for these fields
            CheckDuplicates(x => x.Email, "Email");
            CheckDuplicates(x => x.Email, "OfficePhoneNo");
            CheckDuplicates(x => x.MobileNo, "MobileNo");
            CheckDuplicates(x => x.LoginName, "LoginName");

            // ================= FINAL ERROR COLLECTION =================
            var finalErrors = new Dictionary<string, List<string>>();

            if (headerErrors.Any())
                finalErrors["Header Errors"] = headerErrors;

            if (validationErrors.Any())
            {
                finalErrors["Validation Errors"] = validationErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeFullName })
                    .Select(g => $"{g.Key.EmployeeFullName} ({g.Key.EmpCode}): {string.Join(", ", g.Select(x => x.ErrorMessage))}")
                    .ToList();
            }

            if (duplicateErrors.Any())
            {
                finalErrors["Duplicate Errors"] = duplicateErrors
                    .GroupBy(e => new { e.EmpCode, e.EmployeeFullName })
                    .Select(g => $"{g.Key.EmployeeFullName} ({g.Key.EmpCode}): {string.Join(", ", g.Select(x => x.ErrorMessage))}")
                    .ToList();
            }

            if (finalErrors.Any())
                throw new ClientUserUploadException(finalErrors);

            var results = new List<string>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 🔒 HARD CODED
                const string roleName = "Customer";
                const int permissionType = 2; // Role based

                int roleId = await connection.ExecuteScalarAsync<int>(
                    @"SELECT Mas_ID FROM SAD_GrpOrLvl_General_Master
      WHERE UPPER(Mas_Description)=UPPER(@Role) AND Mas_CompID=@CompId",
                    new { Role = roleName, CompId = compId },
                    transaction);

                foreach (var u in users)
                {
                    // 🔹 Resolve CustomerId
                    int? customerId = await connection.ExecuteScalarAsync<int?>(
                        @"SELECT CUST_ID
                  FROM SAD_CUSTOMER_MASTER
                  WHERE UPPER(CUST_NAME) = UPPER(@Name)",
                        new { Name = u.Customer },
                        transaction);

                    if (!customerId.HasValue)
                    {
                        throw new ClientUserUploadException(
                            new Dictionary<string, List<string>>
                            {
                        {
                           "Customer Errors",
                           new List<string>
                           {
                              $"Customer '{u.Customer}' does not exist in master"
                           }
                        }
                            });
                    }

                    var p = new DynamicParameters();

                    // ================= SP PARAMETERS =================
                    p.Add("@Usr_ID", 0);
                    p.Add("@Usr_Node", 0);
                    p.Add("@Usr_Code", "TEMP"); // Temporary code, will be updated
                    p.Add("@Usr_FullName", u.EmployeeFullName);
                    p.Add("@Usr_LoginName", u.LoginName);
                    p.Add("@Usr_Password", null); // Password is not in Excel, always null
                    p.Add("@Usr_Email", u.Email);
                    p.Add("@Usr_Category", 0);
                    p.Add("@Usr_Suggetions", 0);
                    p.Add("@usr_partner", 0);
                    p.Add("@Usr_LevelGrp", 0);
                    p.Add("@Usr_DutyStatus", "A");
                    p.Add("@Usr_PhoneNo", u.MobileNo);
                    p.Add("@Usr_MobileNo", u.MobileNo);
                    p.Add("@Usr_OfficePhone", u.OfficePhoneNo);
                    p.Add("@Usr_OffPhExtn", "");
                    p.Add("@Usr_Designation", 0);
                    p.Add("@Usr_CompanyID", customerId.Value);
                    p.Add("@Usr_OrgnID", 0);
                    p.Add("@Usr_GrpOrUserLvlPerm", permissionType);
                    p.Add("@Usr_Role", roleId);

                    // Modules
                    p.Add("@Usr_MasterModule", 0);
                    p.Add("@Usr_AuditModule", 0);
                    p.Add("@Usr_RiskModule", 0);
                    p.Add("@Usr_ComplianceModule", 0);
                    p.Add("@Usr_BCMModule", 0);
                    p.Add("@Usr_DigitalOfficeModule", 0);

                    // Roles
                    p.Add("@Usr_MasterRole", 0);
                    p.Add("@Usr_AuditRole", 0);
                    p.Add("@Usr_RiskRole", 0);
                    p.Add("@Usr_ComplianceRole", 0);
                    p.Add("@Usr_BCMRole", 0);
                    p.Add("@Usr_DigitalOfficeRole", 0);

                    // Metadata
                    p.Add("@Usr_CreatedBy", 1);
                    p.Add("@Usr_UpdatedBy", 1);
                    p.Add("@Usr_DelFlag", "A");
                    p.Add("@Usr_Status", "N");
                    p.Add("@Usr_IPAddress", "");
                    p.Add("@Usr_CompId", compId);
                    p.Add("@Usr_Type", "C");
                    p.Add("@usr_IsSuperuser", 0);
                    p.Add("@USR_DeptID", 0);
                    p.Add("@USR_MemberType", 0);
                    p.Add("@USR_Levelcode", 0);

                    p.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spEmployeeMaster",
                        p,
                        transaction,
                        commandType: CommandType.StoredProcedure);

                    int updateOrSave = p.Get<int>("@iUpdateOrSave");
                    int oper = p.Get<int>("@iOper"); // This is the generated Usr_ID

                    // 🔹 Generate employee code based on the generated Usr_ID (format: EMP00430)
                    string generatedEmpCode = $"EMP00{oper.ToString().PadLeft(3, '0')}";

                    // 🔹 Update the employee code in the database
                    string updateEmpCodeSql = @"
                UPDATE Sad_UserDetails 
                SET Usr_Code = @EmpCode 
                WHERE Usr_ID = @UsrId AND Usr_CompId = @CompId";

                    await connection.ExecuteAsync(updateEmpCodeSql,
                        new
                        {
                            EmpCode = generatedEmpCode,
                            UsrId = oper,
                            CompId = compId
                        },
                        transaction);

                    // Update the user object with the generated code
                    u.EmpCode = generatedEmpCode;

                    string action = updateOrSave == 2 ? "Updated" : "Inserted";
                    results.Add($"{action} client user: {u.EmpCode} - {u.EmployeeFullName} (Usr_ID={oper})");
                }

                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private List<UploadClientUserDto> ParseClientUserExcel(
            Stream stream,
            out List<string> headerErrors,
            out bool hasDataRows)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);
            var sheet = package.Workbook.Worksheets[0];

            // Updated headers to match the new Excel format (6 columns)
            string[] expectedHeaders =
            {
        "Customer",           // Column A
        "Email",              // Column B
        "OfficePhoneNo",      // Column C
        "EmployeeFullName",   // Column D
        "MobileNo",           // Column E
        "LoginName"           // Column F
    };

            headerErrors = new List<string>();
            hasDataRows = false;

            for (int i = 0; i < expectedHeaders.Length; i++)
            {
                var actual = sheet.Cells[1, i + 1].Text.Trim();
                if (!actual.Equals(expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    headerErrors.Add(
                        $"Expected '{expectedHeaders[i]}' but found '{actual}' at column {i + 1}");
                }
            }

            var list = new List<UploadClientUserDto>();

            // Check if sheet has any rows beyond header
            if (sheet.Dimension == null || sheet.Dimension.Rows <= 1)
            {
                // Only header row exists, no data rows
                return list;
            }

            for (int row = 2; row <= sheet.Dimension.Rows; row++)
            {
                // 🔹 Read all cells based on NEW Excel format (6 columns)
                string customer = sheet.Cells[row, 1].Text?.Trim();        // Customer
                string email = sheet.Cells[row, 2].Text?.Trim();           // Email
                string officePhoneNo = sheet.Cells[row, 3].Text?.Trim();   // OfficePhoneNo
                string employeeFullName = sheet.Cells[row, 4].Text?.Trim(); // EmployeeFullName
                string mobileNo = sheet.Cells[row, 5].Text?.Trim();        // MobileNo
                string loginName = sheet.Cells[row, 6].Text?.Trim();       // LoginName

                // ✅ SKIP COMPLETELY EMPTY ROWS (ALL 6 fields empty)
                bool isRowEmpty = string.IsNullOrWhiteSpace(customer) &&
                                 string.IsNullOrWhiteSpace(email) &&
                                 string.IsNullOrWhiteSpace(officePhoneNo) &&
                                 string.IsNullOrWhiteSpace(employeeFullName) &&
                                 string.IsNullOrWhiteSpace(mobileNo) &&
                                 string.IsNullOrWhiteSpace(loginName);

                if (isRowEmpty)
                {
                    continue; // Skip empty rows completely
                }

                // Check if this row has at least one non-empty value
                bool hasAnyValue = !string.IsNullOrWhiteSpace(customer) ||
                                  !string.IsNullOrWhiteSpace(email) ||
                                  !string.IsNullOrWhiteSpace(officePhoneNo) ||
                                  !string.IsNullOrWhiteSpace(employeeFullName) ||
                                  !string.IsNullOrWhiteSpace(mobileNo) ||
                                  !string.IsNullOrWhiteSpace(loginName);

                if (hasAnyValue)
                {
                    hasDataRows = true; // We found at least one row with data
                }

                list.Add(new UploadClientUserDto
                {
                    Customer = customer,
                    Email = email,
                    OfficePhoneNo = officePhoneNo,
                    EmployeeFullName = employeeFullName,
                    MobileNo = mobileNo,
                    LoginName = loginName,
                    // EmpCode will be auto-generated later
                    // Password is not in Excel, so it will be null
                    EmpCode = string.Empty,
                    Password = null
                });
            }

            return list;
        }

        private List<UploadClientUserDto> ValidateClientUsers(List<UploadClientUserDto> users)
        {
            var errors = new List<UploadClientUserDto>();

            foreach (var u in users)
            {
                // Skip validation for rows that are completely empty
                bool isRowEmpty = string.IsNullOrWhiteSpace(u.Customer) &&
                                 string.IsNullOrWhiteSpace(u.Email) &&
                                 string.IsNullOrWhiteSpace(u.OfficePhoneNo) &&
                                 string.IsNullOrWhiteSpace(u.EmployeeFullName) &&
                                 string.IsNullOrWhiteSpace(u.MobileNo) &&
                                 string.IsNullOrWhiteSpace(u.LoginName);

                if (isRowEmpty)
                {
                    continue; // Skip validation for empty rows
                }

                // 🔹 EmpCode validation not needed (auto-generated)
                // 🔹 Password validation not needed (not in Excel)

                if (string.IsNullOrWhiteSpace(u.Customer))
                    errors.Add(new UploadClientUserDto
                    {
                        EmpCode = string.Empty,
                        EmployeeFullName = u.EmployeeFullName,
                        ErrorMessage = "Customer is required"
                    });

                if (string.IsNullOrWhiteSpace(u.Email))
                {
                    errors.Add(new UploadClientUserDto
                    {
                        EmpCode = string.Empty,
                        EmployeeFullName = u.EmployeeFullName,
                        ErrorMessage = "Email is required"
                    });
                }
                else
                {
                    string trimmedEmail = u.Email.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedEmail, @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadClientUserDto
                        {
                            EmpCode = string.Empty,
                            EmployeeFullName = u.EmployeeFullName,
                            ErrorMessage = "Invalid Gmail format. Must be like 'username@gmail.com'"
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(u.OfficePhoneNo))
                {
                    errors.Add(new UploadClientUserDto
                    {
                        EmpCode = string.Empty,
                        EmployeeFullName = u.EmployeeFullName,
                        ErrorMessage = "OfficePhoneNo is required"
                    });
                }
                else
                {
                    string phone = u.OfficePhoneNo.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$"))
                    {
                        errors.Add(new UploadClientUserDto
                        {
                            EmpCode = string.Empty,
                            EmployeeFullName = u.EmployeeFullName,
                            ErrorMessage = "OfficePhoneNo must be exactly 10 digits"
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(u.EmployeeFullName))
                    errors.Add(new UploadClientUserDto
                    {
                        EmpCode = string.Empty,
                        EmployeeFullName = u.EmployeeFullName,
                        ErrorMessage = "Employee Name is required"
                    });

                if (string.IsNullOrWhiteSpace(u.MobileNo))
                {
                    errors.Add(new UploadClientUserDto
                    {
                        EmpCode = string.Empty,
                        EmployeeFullName = u.EmployeeFullName,
                        ErrorMessage = "MobileNo is required"
                    });
                }
                else
                {
                    string phone = u.MobileNo.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$"))
                    {
                        errors.Add(new UploadClientUserDto
                        {
                            EmpCode = string.Empty,
                            EmployeeFullName = u.EmployeeFullName,
                            ErrorMessage = "MobileNo must be exactly 10 digits"
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(u.LoginName))
                {
                    errors.Add(new UploadClientUserDto
                    {
                        EmpCode = string.Empty,
                        EmployeeFullName = u.EmployeeFullName,
                        ErrorMessage = "Login Name is required"
                    });
                }
                else
                {
                    string trimmedEmail = u.LoginName.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedEmail, @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadClientUserDto
                        {
                            EmpCode = string.Empty,
                            EmployeeFullName = u.EmployeeFullName,
                            ErrorMessage = "Invalid Login Name format. Must be like 'username@gmail.com'"
                        });
                    }
                }
            }
            return errors;
        }

        //ChangedUploadClientDetails
        public class ClientDetailsUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public ClientDetailsUploadException(Dictionary<string, List<string>> errors)
                : base("Error processing client details")
            {
                Errors = errors;
            }
        }

        public async Task<List<string>> UploadClientDetailsAsync(int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            // ✅ Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Parse Excel with hasDataRows check
            List<UploadClientDetailsDto> clients;
            List<string> headerErrors;
            bool hasDataRows;

            using (var stream = file.OpenReadStream())
            {
                clients = ParseExcelToClients(stream, out headerErrors, out hasDataRows);
            }

            // ✅ Check if Excel has only headers with no data
            if (!hasDataRows)
            {
                var emptyExcelErrors = new Dictionary<string, List<string>>
                {
                    ["File Error"] = new List<string> { "Excel file contains only headers with no data. Please add client records." }
                };
                throw new ClientDetailsUploadException(emptyExcelErrors);
            }

            // ✅ Validate missing/invalid values
            var validationErrors = ValidateClients(clients);

            // ✅ Duplicate errors (matching employee upload pattern)
            var duplicateErrors = new List<UploadClientDetailsDto>();

            void CheckDuplicates(Func<UploadClientDetailsDto, string> selector, string field)
            {
                duplicateErrors.AddRange(
                    clients.GroupBy(selector)
                        .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                            .Select(g => new UploadClientDetailsDto
                            {
                                CUST_CODE = string.Empty, // Empty for parentheses
                                CUST_NAME = g.First().CUST_NAME,
                                ErrorMessage = $"Duplicate {field}: {g.Key}"
                            })
                );
            }

            // Only check duplicates for Email (CUST_CODE is auto-generated)
            CheckDuplicates(c => c.CUST_EMAIL, "CUST_EMAIL");

            // ✅ Group errors in same format as employee upload
            var finalErrors = new Dictionary<string, List<string>>();

            if (headerErrors.Any())
                finalErrors["Header Errors"] = headerErrors;

            if (validationErrors.Any())
            {
                finalErrors["Validation Errors"] = validationErrors
                    .GroupBy(e => new { e.CUST_CODE, e.CUST_NAME })
                    .Select(g => $"{g.Key.CUST_NAME} ({g.Key.CUST_CODE}): {string.Join(", ", g.Select(x => x.ErrorMessage))}")
                    .ToList();
            }

            if (duplicateErrors.Any())
            {
                finalErrors["Duplicate Errors"] = duplicateErrors
                    .GroupBy(e => new { e.CUST_CODE, e.CUST_NAME })
                    .Select(g => $"{g.Key.CUST_NAME} ({g.Key.CUST_CODE}): {string.Join(", ", g.Select(x => x.ErrorMessage))}")
                    .ToList();
            }

            if (finalErrors.Any())
                throw new ClientDetailsUploadException(finalErrors);

            // ✅ No validation errors → proceed with DB transaction
            var results = new List<string>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var client in clients)
                {
                    // Set CompID from parameter
                    client.CUST_CompID = compId;

                    // ✅ Ensure Industry Type exists
                    string indTypeSql = @"
                SELECT Cmm_ID 
                FROM Content_Management_Master 
                WHERE cmm_Category = 'IND' 
                   AND CMM_CompID = @CompId 
                   AND UPPER(cmm_Desc) = UPPER(@Name) 
                   AND cmm_DelFlag = 'A'";

                    int? indTypeId = await connection.ExecuteScalarAsync<int?>(
                        indTypeSql, new { Name = client.CUST_INDTYPEID, CompId = compId }, transaction);

                    // ✅ Ensure Org Type exists
                    string orgTypeSql = @"
                SELECT Cmm_ID 
                FROM Content_Management_Master 
                WHERE cmm_Category = 'ORG' 
                  AND Cmm_CompID = @CompId 
                  AND UPPER(cmm_Desc) = UPPER(@Name) 
                  AND cmm_DelFlag = 'A'";

                    int? orgTypeId = await connection.ExecuteScalarAsync<int?>(
                        orgTypeSql, new { Name = client.CUST_ORGTYPEID, CompId = compId }, transaction);

                    // ✅ Ensure Management Type exists
                    string mngTypeSql = @"
                SELECT Cmm_ID 
                FROM Content_Management_Master 
                WHERE cmm_Category = 'MNG' 
                  AND Cmm_CompID = @CompId 
                  AND UPPER(cmm_Desc) = UPPER(@Name) 
                  AND cmm_DelFlag = 'A'";

                    int? mngTypeId = await connection.ExecuteScalarAsync<int?>(
                        mngTypeSql, new { Name = client.CUST_MGMTTYPEID, CompId = compId }, transaction);

                    // ✅ Ensure Task Type exists
                    string atTypeSql = @"
                SELECT Cmm_ID 
                FROM Content_Management_Master 
                WHERE cmm_Category = 'AT' 
                  AND Cmm_CompID = @CompId 
                  AND UPPER(cmm_Desc) = UPPER(@Name) 
                  AND cmm_DelFlag = 'A'";

                    int? atTypeId = await connection.ExecuteScalarAsync<int?>(
                        atTypeSql, new { Name = client.CUST_TASKS, CompId = compId }, transaction);

                    // ✅ Call stored procedure
                    // ✅ Call stored procedure
                    using var cmd = new SqlCommand("spSAD_CUSTOMER_MASTER", connection, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Set CUST_ID to 0 (will be auto-generated by SP)
                    client.CUST_ID = 0;

                    // Set temporary CUST_CODE (will be updated after SP execution)
                    string tempCustCode = "TEMP";

                    // 🔹 FIXED: Use correct parameter names according to SP
                    cmd.Parameters.AddWithValue("@CUST_ID", client.CUST_ID);
                    cmd.Parameters.AddWithValue("@CUST_NAME", client.CUST_NAME ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_CODE", tempCustCode); // Temporary code
                    cmd.Parameters.AddWithValue("@CUST_WEBSITE", client.CUST_WEBSITE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_EMAIL", client.CUST_EMAIL ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_GROUPNAME", client.CUST_GROUPNAME ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_GROUPINDIVIDUAL", client.CUST_GROUPINDIVIDUAL ?? 0);
                    cmd.Parameters.AddWithValue("@CUST_ORGTYPEID", orgTypeId ?? 0);
                    cmd.Parameters.AddWithValue("@CUST_INDTYPEID", indTypeId ?? 0);
                    cmd.Parameters.AddWithValue("@CUST_MGMTTYPEID", mngTypeId ?? 0);

                    // 🔹 FIX: This should be @CUST_CommitmentDate (not @CUST_CRON)
                    cmd.Parameters.AddWithValue("@CUST_CommitmentDate", client.CUST_CRON ?? DateTime.Now);

                    cmd.Parameters.AddWithValue("@CUSt_BranchId", client.CUSt_BranchId ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_ADDRESS", client.CUST_COMM_ADDRESS ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_CITY", client.CUST_COMM_CITY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_PIN", client.CUST_COMM_PIN ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_STATE", client.CUST_COMM_STATE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_COUNTRY", client.CUST_COMM_COUNTRY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_FAX", client.CUST_COMM_FAX ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_TEL", client.CUST_COMM_TEL ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COMM_Email", client.CUST_COMM_Email ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_ADDRESS", client.CUST_ADDRESS ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_CITY", client.CUST_CITY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_PIN", client.CUST_PIN ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_STATE", client.CUST_STATE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_COUNTRY", client.CUST_COUNTRY ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_FAX", client.CUST_FAX ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_TELPHONE", client.CUST_TELPHONE ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_ConEmailID", client.CUST_ConEmailID ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_LOCATIONID", client.CUST_LOCATIONID ?? "0");
                    cmd.Parameters.AddWithValue("@CUST_TASKS", atTypeId ?? 0); // This should be int, not string
                    cmd.Parameters.AddWithValue("@CUST_ORGID", orgTypeId ?? 0);
                    cmd.Parameters.AddWithValue("@CUST_CRBY", client.CUST_CRBY ?? 1);
                    cmd.Parameters.AddWithValue("@CUST_UpdatedBy", client.CUST_UpdatedBy ?? 1);
                    cmd.Parameters.AddWithValue("@CUST_BOARDOFDIRECTORS", string.Empty); // Empty since not in Excel
                    cmd.Parameters.AddWithValue("@CUST_DEPMETHOD", client.CUST_DEPMETHOD ?? 0);
                    cmd.Parameters.AddWithValue("@CUST_IPAddress", client.CUST_IPAddress ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CUST_CompID", compId);
                    cmd.Parameters.AddWithValue("@CUST_Amount_Type", client.CUST_Amount_Type ?? 0);
                    cmd.Parameters.AddWithValue("@CUST_RoundOff", client.CUST_RoundOff ?? 0);
                    cmd.Parameters.AddWithValue("@Cust_DurtnId", client.Cust_DurtnId ?? 0);
                    cmd.Parameters.AddWithValue("@Cust_FY", client.Cust_FY ?? string.Empty);

                    // 🔹 REMOVE THIS LINE - @CUST_CRON is not a parameter in SP
                    // cmd.Parameters.AddWithValue("@CUST_CRON", client.CUST_CRON ?? DateTime.Now);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(updateOrSaveParam);
                    cmd.Parameters.Add(operParam);

                    await cmd.ExecuteNonQueryAsync();

                    int updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    int oper = (int)(operParam.Value ?? 0); // This is the generated CUST_ID

                    // 🔹 Generate customer code based on the generated CUST_ID (format: CUST430)
                    string generatedCustCode = $"CUST{oper}";

                    // 🔹 Update the customer code in the database
                    string updateCustCodeSql = @"
                UPDATE SAD_CUSTOMER_MASTER 
                SET CUST_CODE = @CustCode 
                WHERE Cust_ID = @CustId AND CUST_CompID = @CompId";

                    await connection.ExecuteAsync(updateCustCodeSql,
                        new
                        {
                            CustCode = generatedCustCode,
                            CustId = oper,
                            CompId = compId
                        },
                        transaction);

                    // Update the client object with the generated code
                    client.CUST_CODE = generatedCustCode;
                    client.CUST_ID = oper;

                    string action = updateOrSave == 2 ? "Updated" : "Inserted";
                    results.Add($"{action} client: {client.CUST_CODE} - {client.CUST_NAME} (Client_ID={oper})");

                    // ✅ Save Locations if provided
                    if (!string.IsNullOrWhiteSpace(client.LocationName))
                    {
                        int locationId;
                        using (var cmdLoc = new SqlCommand("spSAD_CUST_LOCATION", connection, transaction))
                        {
                            cmdLoc.CommandType = CommandType.StoredProcedure;
                            cmdLoc.Parameters.AddWithValue("@Mas_Id", client.Mas_Id);
                            cmdLoc.Parameters.AddWithValue("@Mas_code", client.Mas_code ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Description", client.LocationName ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_DelFlag", client.DelFlag ?? "A");
                            cmdLoc.Parameters.AddWithValue("@Mas_CustID", oper); // Use the generated CUST_ID
                            cmdLoc.Parameters.AddWithValue("@Mas_Loc_Address", client.Address ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Person", client.ContactPerson ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_MobileNo", client.Mobile ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_LandLineNo", client.Landline ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_Contact_Email", client.Email ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@mas_Designation", client.Designation ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CRBY", client.CUST_CRBY ?? 1);
                            cmdLoc.Parameters.AddWithValue("@Mas_UpdatedBy", client.CUST_UpdatedBy ?? 1);
                            cmd.Parameters.AddWithValue("@Mas_STATUS", "A");
                            cmdLoc.Parameters.AddWithValue("@Mas_IPAddress", client.CUST_IPAddress ?? string.Empty);
                            cmdLoc.Parameters.AddWithValue("@Mas_CompID", compId);

                            var updateOrSaveLoc = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operLoc = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdLoc.Parameters.Add(updateOrSaveLoc);
                            cmdLoc.Parameters.Add(operLoc);

                            await cmdLoc.ExecuteNonQueryAsync();
                            locationId = (int)(operLoc.Value ?? 0);
                        }

                        // ✅ Save Statutory Refs
                        async Task SaveStatutoryRef(string desc, string value)
                        {
                            if (string.IsNullOrWhiteSpace(value))
                                return;

                            using var cmdStat = new SqlCommand("spSAD_CUST_Accounting_Template", connection, transaction);
                            cmdStat.CommandType = CommandType.StoredProcedure;
                            cmdStat.Parameters.AddWithValue("@Cust_PKID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_ID", oper); // Use the generated CUST_ID
                            cmdStat.Parameters.AddWithValue("@Cust_Desc", desc);
                            cmdStat.Parameters.AddWithValue("@Cust_Value", value);
                            cmdStat.Parameters.AddWithValue("@Cust_Delflag", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_Status", "A");
                            cmdStat.Parameters.AddWithValue("@Cust_AttchID", 0);
                            cmdStat.Parameters.AddWithValue("@Cust_CrBy", client.CUST_CRBY ?? 1);
                            cmdStat.Parameters.AddWithValue("@Cust_UpdatedBy", client.CUST_UpdatedBy ?? 1);
                            cmdStat.Parameters.AddWithValue("@Cust_IPAddress", client.CUST_IPAddress ?? string.Empty);
                            cmdStat.Parameters.AddWithValue("@Cust_Compid", compId);
                            cmdStat.Parameters.AddWithValue("@Cust_LocationId", locationId);

                            var updateOrSaveStat = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operStat = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmdStat.Parameters.Add(updateOrSaveStat);
                            cmdStat.Parameters.Add(operStat);

                            await cmdStat.ExecuteNonQueryAsync();
                        }

                        await SaveStatutoryRef("CIN", client.CIN ?? string.Empty);
                        await SaveStatutoryRef("TAN", client.TAN ?? string.Empty);
                        await SaveStatutoryRef("GST", client.GST ?? string.Empty);
                    }
                }

                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private List<UploadClientDetailsDto> ParseExcelToClients(Stream fileStream, out List<string> headerErrors, out bool hasDataRows)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];

            // ✅ Updated headers - Removed "Customer Code" and "Board of Directors/Partners"
            string[] expectedHeaders =
            {
        "Customer Name",                   
        "Customer E-mail",                  
        "Customer Financial Year",         
        "Customer CommitmentDate",          
        "Organisation Type",               
        "IndustryType",                    
        "CIN No",                          
        "Management",                     
        "Professional Service Offered",    
        "Address"                           
    };

            headerErrors = new List<string>();
            hasDataRows = false;

            // ✅ Header validation
            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                string actualHeader = worksheet.Cells[1, col].Text?.Trim();
                string expectedHeader = expectedHeaders[col - 1];

                if (!string.Equals(actualHeader, expectedHeader, StringComparison.OrdinalIgnoreCase))
                {
                    headerErrors.Add($"Expected header '{expectedHeader}' at column {col}, but found '{actualHeader}'");
                }
            }

            var clients = new List<UploadClientDetailsDto>();

            if (worksheet.Dimension == null || worksheet.Dimension.Rows <= 1)
            {
                return clients;
            }

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                   
                    string custName = worksheet.Cells[row, 1].Text?.Trim();           
                    string custEmail = worksheet.Cells[row, 2].Text?.Trim();        
                    string custFY = worksheet.Cells[row, 3].Text?.Trim();            

                    DateTime? commitmentDate = null;
                    string dateString = worksheet.Cells[row, 4].Text?.Trim();

                    if (!string.IsNullOrWhiteSpace(dateString))
                    {
                        if (DateTime.TryParse(dateString, out DateTime parsedDate))
                        {
                            commitmentDate = parsedDate;
                        }
                    }

                    string orgTypeId = worksheet.Cells[row, 5].Text?.Trim();         
                    string indTypeId = worksheet.Cells[row, 6].Text?.Trim();        
                    string cinNo = worksheet.Cells[row, 7].Text?.Trim();             
                    string mgmtTypeId = worksheet.Cells[row, 8].Text?.Trim();       
                    string tasks = worksheet.Cells[row, 9].Text?.Trim();           
                    string address = worksheet.Cells[row, 10].Text?.Trim();       

                    // ✅ SKIP COMPLETELY EMPTY ROWS
                    bool isRowEmpty = string.IsNullOrWhiteSpace(custName) &&
                                     string.IsNullOrWhiteSpace(custEmail) &&
                                     string.IsNullOrWhiteSpace(custFY) &&
                                     !commitmentDate.HasValue &&
                                     string.IsNullOrWhiteSpace(orgTypeId) &&
                                     string.IsNullOrWhiteSpace(indTypeId) &&
                                     string.IsNullOrWhiteSpace(cinNo) &&
                                     string.IsNullOrWhiteSpace(mgmtTypeId) &&
                                     string.IsNullOrWhiteSpace(tasks) &&
                                     string.IsNullOrWhiteSpace(address);

                    if (isRowEmpty)
                    {
                        continue;
                    }

                    bool hasAnyValue = !string.IsNullOrWhiteSpace(custName) ||
                                      !string.IsNullOrWhiteSpace(custEmail) ||
                                      !string.IsNullOrWhiteSpace(custFY) ||
                                      commitmentDate.HasValue ||
                                      !string.IsNullOrWhiteSpace(orgTypeId) ||
                                      !string.IsNullOrWhiteSpace(indTypeId) ||
                                      !string.IsNullOrWhiteSpace(cinNo) ||
                                      !string.IsNullOrWhiteSpace(mgmtTypeId) ||
                                      !string.IsNullOrWhiteSpace(tasks) ||
                                      !string.IsNullOrWhiteSpace(address);

                    if (hasAnyValue)
                    {
                        hasDataRows = true;
                    }

                    clients.Add(new UploadClientDetailsDto
                    {
                        CUST_NAME = custName,
                        CUST_EMAIL = custEmail,
                        Cust_FY = custFY,
                        CUST_CommitmentDate = commitmentDate,
                        CUST_ORGTYPEID = orgTypeId,
                        CUST_INDTYPEID = indTypeId,
                        CUSt_BranchId = cinNo, 
                        CUST_MGMTTYPEID = mgmtTypeId,
                        CUST_TASKS = tasks,
                        CUST_ADDRESS = address,
                        CUST_CODE = string.Empty
                    });
                }
                catch (Exception ex)
                {
                    clients.Add(new UploadClientDetailsDto
                    {
                        CUST_NAME = $"Error in row {row}",
                        CUST_CODE = $"ROW{row}",
                        ErrorMessage = $"Error parsing row: {ex.Message}"
                    });
                    hasDataRows = true;
                }
            }

            return clients;
        }
        private List<UploadClientDetailsDto> ValidateClients(List<UploadClientDetailsDto> clients)
        {
            var errors = new List<UploadClientDetailsDto>();

            foreach (var client in clients)
            {
                if (!string.IsNullOrWhiteSpace(client.ErrorMessage))
                {
                    errors.Add(client);
                    continue;
                }

                // Skip validation for rows that are completely empty
                bool isRowEmpty = string.IsNullOrWhiteSpace(client.CUST_NAME) &&
                                 string.IsNullOrWhiteSpace(client.CUST_EMAIL) &&
                                 string.IsNullOrWhiteSpace(client.Cust_FY) &&
                                 !client.CUST_CRON.HasValue &&
                                 string.IsNullOrWhiteSpace(client.CUST_ORGTYPEID) &&
                                 string.IsNullOrWhiteSpace(client.CUST_INDTYPEID) &&
                                 string.IsNullOrWhiteSpace(client.CUSt_BranchId) &&
                                 string.IsNullOrWhiteSpace(client.CUST_MGMTTYPEID) &&
                                 string.IsNullOrWhiteSpace(client.CUST_TASKS) &&
                                 string.IsNullOrWhiteSpace(client.CUST_ADDRESS);

                if (isRowEmpty)
                {
                    continue; 
                }

                if (string.IsNullOrWhiteSpace(client.CUST_NAME))
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Customer Name is required"
                    });

                if (string.IsNullOrWhiteSpace(client.CUST_EMAIL))
                {
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Customer E-mail is required"
                    });
                }
                else
                {
                    string trimmedEmail = client.CUST_EMAIL.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedEmail, @"^[a-z0-9](\.?[a-z0-9]){1,}@gmail\.com$"))
                    {
                        errors.Add(new UploadClientDetailsDto
                        {
                            CUST_CODE = string.Empty,
                            CUST_NAME = client.CUST_NAME,
                            ErrorMessage = "Invalid Gmail format. Must be like 'username@gmail.com'"
                        });
                    }
                }

                if (string.IsNullOrWhiteSpace(client.Cust_FY))
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Financial Year is required"
                    });
                if (!client.CUST_CommitmentDate.HasValue)
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Commitment Date is required"
                    });

                if (string.IsNullOrWhiteSpace(client.CUST_ORGTYPEID))
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Organization Type is required"
                    });

                if (string.IsNullOrWhiteSpace(client.CUST_INDTYPEID))
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Industry Type is required"
                    });

                if (string.IsNullOrWhiteSpace(client.CUSt_BranchId)) // CIN No
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "CIN No is required"
                    });

                if (string.IsNullOrWhiteSpace(client.CUST_MGMTTYPEID))
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Management Type is required"
                    });

                if (string.IsNullOrWhiteSpace(client.CUST_TASKS))
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Professional Service is required"
                    });

                if (string.IsNullOrWhiteSpace(client.CUST_ADDRESS))
                    errors.Add(new UploadClientDetailsDto
                    {
                        CUST_CODE = string.Empty,
                        CUST_NAME = client.CUST_NAME,
                        ErrorMessage = "Address is required"
                    });
            }

            return errors;
        }
    }
}

