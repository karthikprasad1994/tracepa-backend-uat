using System.Data;
using Dapper;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetCreationDto;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetCreationService : AssetCreationInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public AssetCreationService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //DownloadExcel
        public AssetMasterResult GetAssetMasterExcelTemplate()
        {
            var filePath = "C:\\Users\\Intel\\Desktop\\tracepa-dotnet-core - Copy\\SampleExcels\\AssetMaster-Upload.xlsx";

            if (!File.Exists(filePath))
                return new AssetMasterResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "AssetMaster-Ipload.xlsx";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new AssetMasterResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //LoadAssetClass
        public async Task<IEnumerable<AssetTypeDto>> LoadAssetTypeAsync(int compId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string query = @"
        SELECT 
            AM_ID AS AssetTypeId,
            AM_Description AS AssetTypeName
        FROM Acc_AssetMaster
        WHERE 
            AM_LevelCode = 2
            AND AM_DelFlag = 'A'
            AND AM_CompID = @CompId
            AND AM_CustID = @CustId
        ORDER BY AM_ID DESC";

            return await connection.QueryAsync<AssetTypeDto>(query, new
            {
                CompId = compId,
                CustId = custId
            });
        }

        //New
        public async Task<int> AddNewAssetAsync(NewDto asset)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        INSERT INTO AssetMaster
        (
            AssetName, 
            AssetCode, 
            CreatedBy, 
            CompId, 
            CustId
        )
        VALUES
        (
            @AssetName,
            @AssetCode,
            @CreatedBy,
            @CompId,
            @CustId
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

            var parameters = new
            {
                AssetName = asset.AssetName,
                AssetCode = asset.AssetCode,
                CreatedBy = asset.CreatedBy,
                CompId = asset.CompId,
                CustId = asset.CustId
            };

            return await connection.ExecuteScalarAsync<int>(query, parameters);
        }

        //Search
        public async Task<IEnumerable<AssetRegisterDto>> LoadAssetRegisterAsync(int compId, int assetTypeId, int yearId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string query = @"
        SELECT 
            F.AFAM_ID AS Id,
            F.AFAM_AssetType AS AssetId,
            F.AFAM_AssetCode AS AssetCode,
            AM.AM_Description AS AssetDescription,
            F.AFAM_ItemCode AS ItemCode,
            F.AFAM_ItemDescription AS ItemDescription,
            F.AFAM_CommissionDate AS CommissionDate,
            F.AFAM_Quantity AS Qty,
            F.AFAM_AssetAge AS AssetAge,
            F.AFAM_Status AS CurrentStatus,
            F.AFAM_TRStatus AS TRStatus,
            LS1.LS_Description AS Location,
            LS2.LS_Description AS Division,
            LS3.LS_Description AS Department,
            LS4.LS_Description AS Bay
        FROM Acc_FixedAssetMaster F
        LEFT JOIN Acc_AssetMaster AM 
            ON AM.AM_ID = F.AFAM_AssetType 
            AND AM.AM_CompID = @CompId 
            AND AM.AM_CustId = @CustId
        LEFT JOIN Acc_AssetLocationSetup LS1 
            ON LS1.LS_ID = F.AFAM_Location 
            AND LS1.LS_CompID = @CompId 
            AND LS1.LS_CustId = @CustId
        LEFT JOIN Acc_AssetLocationSetup LS2 
            ON LS2.LS_ID = F.AFAM_Division 
            AND LS2.LS_CompID = @CompId 
            AND LS2.LS_CustId = @CustId 
            AND LS2.LS_LevelCode = 1
        LEFT JOIN Acc_AssetLocationSetup LS3 
            ON LS3.LS_ID = F.AFAM_Department 
            AND LS3.LS_CompID = @CompId 
            AND LS3.LS_CustId = @CustId 
            AND LS3.LS_LevelCode = 2
        LEFT JOIN Acc_AssetLocationSetup LS4 
            ON LS4.LS_ID = F.AFAM_Bay 
            AND LS4.LS_CompID = @CompId 
            AND LS4.LS_CustId = @CustId 
            AND LS4.LS_LevelCode = 3
        WHERE 
            F.AFAM_CompID = @CompId
            AND F.AFAM_CustID = @CustId
            AND F.AFAM_YearID = @YearId
            AND F.AFAM_DelFlag <> 'T'
            AND F.AFAM_Status <> 'T'
            " + (assetTypeId != 0 ? " AND F.AFAM_AssetType = @AssetTypeId " : "") + @"
        ORDER BY F.AFAM_ID";

            var rawResults = await connection.QueryAsync<AssetRegisterRaw>(query, new
            {
                CompId = compId,
                CustId = custId,
                YearId = yearId,
                AssetTypeId = assetTypeId
            });

            int counter = 1;

            // final result mapping
            return rawResults.Select(r => new AssetRegisterDto
            {
                SLNo = counter++,
                ID = r.Id,
                AssetID = r.AssetId,
                AssetCode = r.AssetCode ?? "0",
                AssetDescription = r.AssetDescription ?? "",
                ItemCode = r.ItemCode ?? "0",
                ItemDescription = r.ItemDescription ?? "",
                DateCommission =
                    r.CommissionDate?.ToString("dd/MM/yyyy") == "01/01/1900"
                    ? ""
                    : r.CommissionDate?.ToString("dd/MM/yyyy"),
                Qty = r.Qty ?? 0,
                AssetAge = r.AssetAge ?? 0,
                CurrentStatus = r.CurrentStatus == "W" ? "Waiting For Approval" : "Approved",
                TRStatus = r.TRStatus ?? "",
                Location = r.Location ?? "",
                Division = r.Division ?? "",
                Department = r.Department ?? "",
                Bay = r.Bay ?? ""
            });
        }

        //UnitsofMeasurement
        public async Task<IEnumerable<UnitMeasureDto>> LoadUnitsOfMeasureAsync(int compId)
        {
            // Step 1: Get DB from Session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // Step 2: Get Dynamic Connection String
            string connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT cmm_ID, cmm_Desc 
            FROM Content_Management_Master
            WHERE CMM_CompID = @CompId 
              AND CMM_Category = 'UM'
            ORDER BY cmm_ID";

                return await connection.QueryAsync<UnitMeasureDto>(
                    query,
                    new { CompId = compId }
                );
            }
        }

        //LoadSuplierName
        public async Task<IEnumerable<SupplierDto>> LoadExistingSupplierAsync(int compId, int supplierId)
        {
            // Step 1: Get DB Name from Session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // Step 2: Get Dynamic Connection String
            string connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                string sql = "";

                if (supplierId > 0)
                {
                    sql = @"
                SELECT SUP_Name, SUP_ID 
                FROM SAD_SUPPLIER_MASTER
                WHERE SUP_CompID = @CompId
                  AND SUP_ID = @SupplierId
                ORDER BY SUP_ID";
                }
                else
                {
                    sql = @"
                SELECT SUP_Name, SUP_ID 
                FROM SAD_SUPPLIER_MASTER
                WHERE SUP_CompID = @CompId
                ORDER BY SUP_ID";
                }

                return await connection.QueryAsync<SupplierDto>(sql, new
                {
                    CompId = compId,
                    SupplierId = supplierId
                });
            }
        }

        //EditSuplierName
        public async Task<IEnumerable<EditSupplierDetailsDto>> EditSupplierDetailsAsync(int compId, int supplierId)
        {
            // Step 1: Get DB Name from Session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // Step 2: Dynamic connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT 
                SUP_Name,
                SUP_ContactPerson,
                SUP_Address,
                SUP_PhoneNo,
                SUP_Fax,
                SUP_Email,
                SUP_Website
            FROM SAD_SUPPLIER_MASTER
            WHERE SUP_CompID = @CompId
              AND SUP_ID = @SupplierId";

                return await connection.QueryAsync<EditSupplierDetailsDto>(sql, new
                {
                    CompId = compId,
                    SupplierId = supplierId
                });
            }
        }

        //SaveSuplierDetails
        public async Task<(int iUpdateOrSave, int iOper)> SaveSupplierDetailsAsync(SaveSupplierDto model)
        {
            // Step 1: Get DB Name from Session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // Step 2: Get dynamic connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("@SUP_ID", model.SUP_ID);
                parameters.Add("@SUP_Name", model.SUP_Name);
                parameters.Add("@SUP_Code", model.SUP_Code);
                parameters.Add("@SUP_ContactPerson", model.SUP_ContactPerson);
                parameters.Add("@SUP_Address", model.SUP_Address);
                parameters.Add("@SUP_PhoneNo", model.SUP_PhoneNo);
                parameters.Add("@SUP_Fax", model.SUP_Fax);
                parameters.Add("@SUP_Email", model.SUP_Email);
                parameters.Add("@SUP_Website", model.SUP_Website);
                parameters.Add("@SUP_CRBY", model.SUP_CRBY);
                parameters.Add("@SUP_CRON", model.SUP_CRON);
                parameters.Add("@SUP_STATUS", model.SUP_STATUS);
                parameters.Add("@SUP_IPAddress", model.SUP_IPAddress);
                parameters.Add("@SUP_CompID", model.SUP_CompID);

                // Output parameters
                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute stored procedure
                await connection.ExecuteAsync(
                    "spSAD_SUPPLIER_MASTER",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Fetch output values
                int iUpdateOrSave = parameters.Get<int>("@iUpdateOrSave");
                int iOper = parameters.Get<int>("@iOper");

                return (iUpdateOrSave, iOper);

            }
        }

        //SaveAsset      
        //public async Task<(int iUpdateOrSave, int iOper)> SaveFixedAssetAsync(SaveFixedAssetDto m)
        //{
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session.");

        //    string connectionString = _configuration.GetConnectionString(dbName);

        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        var p = new DynamicParameters();

        //        // ALL FIXED ASSET INPUT PARAMETERS  
        //        p.Add("@AFAM_ID", m.AFAM_ID);
        //        p.Add("@AFAM_AssetType", m.AFAM_AssetType);
        //        p.Add("@AFAM_AssetCode", m.AFAM_AssetCode);
        //        p.Add("@AFAM_Description", m.AFAM_Description);
        //        p.Add("@AFAM_ItemCode", m.AFAM_ItemCode);
        //        p.Add("@AFAM_ItemDescription", m.AFAM_ItemDescription);
        //        p.Add("@AFAM_CommissionDate", m.AFAM_CommissionDate);
        //        p.Add("@AFAM_PurchaseDate", m.AFAM_PurchaseDate);
        //        p.Add("@AFAM_Quantity", m.AFAM_Quantity);
        //        p.Add("@AFAM_Unit", m.AFAM_Unit);
        //        p.Add("@AFAM_AssetAge", m.AFAM_AssetAge);
        //        p.Add("@AFAM_PurchaseAmount", m.AFAM_PurchaseAmount);
        //        p.Add("@AFAM_PolicyNo", m.AFAM_PolicyNo);
        //        p.Add("@AFAM_Amount", m.AFAM_Amount);
        //        p.Add("@AFAM_BrokerName", m.AFAM_BrokerName);
        //        p.Add("@AFAM_CompanyName", m.AFAM_CompanyName);
        //        p.Add("@AFAM_Date", m.AFAM_Date);
        //        p.Add("@AFAM_ToDate", m.AFAM_ToDate);
        //        p.Add("@AFAM_Location", m.AFAM_Location);
        //        p.Add("@AFAM_Division", m.AFAM_Division);
        //        p.Add("@AFAM_Department", m.AFAM_Department);
        //        p.Add("@AFAM_Bay", m.AFAM_Bay);
        //        p.Add("@AFAM_EmployeeName", m.AFAM_EmployeeName);
        //        p.Add("@AFAM_EmployeeCode", m.AFAM_EmployeeCode);
        //        p.Add("@AFAM_Code", m.AFAM_Code);
        //        p.Add("@AFAM_SuplierName", m.AFAM_SuplierName);
        //        p.Add("@AFAM_ContactPerson", m.AFAM_ContactPerson);
        //        p.Add("@AFAM_Address", m.AFAM_Address);
        //        p.Add("@AFAM_Phone", m.AFAM_Phone);
        //        p.Add("@AFAM_Fax", m.AFAM_Fax);
        //        p.Add("@AFAM_EmailID", m.AFAM_EmailID);
        //        p.Add("@AFAM_Website", m.AFAM_Website);
        //        p.Add("@AFAM_CreatedBy", m.AFAM_CreatedBy);
        //        p.Add("@AFAM_CreatedOn", m.AFAM_CreatedOn);
        //        p.Add("@AFAM_UpdatedBy", m.AFAM_UpdatedBy);
        //        p.Add("@AFAM_UpdatedOn", m.AFAM_UpdatedOn);
        //        p.Add("@AFAM_DelFlag", m.AFAM_DelFlag);
        //        p.Add("@AFAM_Status", m.AFAM_Status);
        //        p.Add("@AFAM_YearID", m.AFAM_YearID);
        //        p.Add("@AFAM_CompID", m.AFAM_CompID);
        //        p.Add("@AFAM_Opeartion", m.AFAM_Opeartion);
        //        p.Add("@AFAM_IPAddress", m.AFAM_IPAddress);
        //        p.Add("@AFAM_WrntyDesc", m.AFAM_WrntyDesc);
        //        p.Add("@AFAM_ContactPrsn", m.AFAM_ContactPrsn);
        //        p.Add("@AFAM_AMCFrmDate", m.AFAM_AMCFrmDate);
        //        p.Add("@AFAM_AMCTo", m.AFAM_AMCTo);
        //        p.Add("@AFAM_Contprsn", m.AFAM_Contprsn);
        //        p.Add("@AFAM_PhoneNo", m.AFAM_PhoneNo);
        //        p.Add("@AFAM_AMCCompanyName", m.AFAM_AMCCompanyName);
        //        p.Add("@AFAM_AssetDeletion", m.AFAM_AssetDeletion);
        //        p.Add("@AFAM_DlnDate", m.AFAM_DlnDate);
        //        p.Add("@AFAM_DateOfDeletion", m.AFAM_DateOfDeletion);
        //        p.Add("@AFAM_Value", m.AFAM_Value);
        //        p.Add("@AFAM_Remark", m.AFAM_Remark);
        //        p.Add("@AFAM_EMPCode", m.AFAM_EMPCode);
        //        p.Add("@AFAM_LToWhom", m.AFAM_LToWhom);
        //        p.Add("@AFAM_LAmount", m.AFAM_LAmount);
        //        p.Add("@AFAM_LAggriNo", m.AFAM_LAggriNo);
        //        p.Add("@AFAM_LDate", m.AFAM_LDate);
        //        p.Add("@AFAM_LCurrencyType", m.AFAM_LCurrencyType);
        //        p.Add("@AFAM_LExchDate", m.AFAM_LExchDate);
        //        p.Add("@AFAM_CustId", m.AFAM_CustId);

        //        // OUTPUT
        //        p.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //        p.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);


        //        await connection.ExecuteAsync("spAcc_FixedAssetMaster", p, commandType: CommandType.StoredProcedure);

        //        return (p.Get<int>("@iUpdateOrSave"), p.Get<int>("@iOper"));
        //    }
        //}

        //// -----------------------------
        //// Save GRACe Form Audit Log
        //// -----------------------------
        //public async Task SaveGRACeFormOperationsAsync(GRACeFormOperationDto model)
        //{
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session.");

        //    string connectionString = _configuration.GetConnectionString(dbName);

        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        var param = new DynamicParameters();

        //        param.Add("@ALFO_UserID", model.UserId);
        //        param.Add("@ALFO_Module", model.Module);
        //        param.Add("@ALFO_Form", model.Form);
        //        param.Add("@ALFO_Event", model.EventName);
        //        param.Add("@ALFO_MasterID", model.MasterId);
        //        param.Add("@ALFO_MasterName", model.MasterName);
        //        param.Add("@ALFO_SubMasterID", model.SubMasterId);
        //        param.Add("@ALFO_SubMasterName", model.SubMasterName);
        //        param.Add("@ALFO_IPAddress", model.IPAddress);
        //        param.Add("@ALFO_CompID", model.CompId);

        //        await connection.ExecuteAsync("spAudit_Log_Form_Operations", param, commandType: CommandType.StoredProcedure);
        //    }
        //}

        //public async Task<(int iUpdateOrSave, int iOper)> SaveFixedAssetWithAuditAsync( SaveFixedAssetDto asset,  GRACeFormOperationDto audit)
        //{
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session.");

        //    string connectionString = _configuration.GetConnectionString(dbName);

        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        await connection.OpenAsync();
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                // --------------------------
        //                // 1️⃣ SAVE FIXED ASSET
        //                // --------------------------
        //                var p = new DynamicParameters();

        //                // All fixed asset params
        //                p.Add("@AFAM_ID", asset.AFAM_ID);
        //                p.Add("@AFAM_AssetType", asset.AFAM_AssetType);
        //                p.Add("@AFAM_AssetCode", asset.AFAM_AssetCode);
        //                p.Add("@AFAM_Description", asset.AFAM_Description);
        //                p.Add("@AFAM_ItemCode", asset.AFAM_ItemCode);
        //                p.Add("@AFAM_ItemDescription", asset.AFAM_ItemDescription);
        //                p.Add("@AFAM_CommissionDate", asset.AFAM_CommissionDate);
        //                p.Add("@AFAM_PurchaseDate", asset.AFAM_PurchaseDate);
        //                p.Add("@AFAM_Quantity", asset.AFAM_Quantity);
        //                p.Add("@AFAM_Unit", asset.AFAM_Unit);
        //                p.Add("@AFAM_AssetAge", asset.AFAM_AssetAge);
        //                p.Add("@AFAM_PurchaseAmount", asset.AFAM_PurchaseAmount);
        //                p.Add("@AFAM_PolicyNo", asset.AFAM_PolicyNo);
        //                p.Add("@AFAM_Amount", asset.AFAM_Amount);
        //                p.Add("@AFAM_BrokerName", asset.AFAM_BrokerName);
        //                p.Add("@AFAM_CompanyName", asset.AFAM_CompanyName);
        //                p.Add("@AFAM_Date", asset.AFAM_Date);
        //                p.Add("@AFAM_ToDate", asset.AFAM_ToDate);
        //                p.Add("@AFAM_Location", asset.AFAM_Location);
        //                p.Add("@AFAM_Division", asset.AFAM_Division);
        //                p.Add("@AFAM_Department", asset.AFAM_Department);
        //                p.Add("@AFAM_Bay", asset.AFAM_Bay);
        //                p.Add("@AFAM_EmployeeName", asset.AFAM_EmployeeName);
        //                p.Add("@AFAM_EmployeeCode", asset.AFAM_EmployeeCode);
        //                p.Add("@AFAM_Code", asset.AFAM_Code);
        //                p.Add("@AFAM_SuplierName", asset.AFAM_SuplierName);
        //                p.Add("@AFAM_ContactPerson", asset.AFAM_ContactPerson);
        //                p.Add("@AFAM_Address", asset.AFAM_Address);
        //                p.Add("@AFAM_Phone", asset.AFAM_Phone);
        //                p.Add("@AFAM_Fax", asset.AFAM_Fax);
        //                p.Add("@AFAM_EmailID", asset.AFAM_EmailID);
        //                p.Add("@AFAM_Website", asset.AFAM_Website);
        //                p.Add("@AFAM_CreatedBy", asset.AFAM_CreatedBy);
        //                p.Add("@AFAM_CreatedOn", asset.AFAM_CreatedOn);
        //                p.Add("@AFAM_UpdatedBy", asset.AFAM_UpdatedBy);
        //                p.Add("@AFAM_UpdatedOn", asset.AFAM_UpdatedOn);
        //                p.Add("@AFAM_DelFlag", asset.AFAM_DelFlag);
        //                p.Add("@AFAM_Status", asset.AFAM_Status);
        //                p.Add("@AFAM_YearID", asset.AFAM_YearID);
        //                p.Add("@AFAM_CompID", asset.AFAM_CompID);
        //                p.Add("@AFAM_Opeartion", asset.AFAM_Opeartion);
        //                p.Add("@AFAM_IPAddress", asset.AFAM_IPAddress);
        //                p.Add("@AFAM_WrntyDesc", asset.AFAM_WrntyDesc);
        //                p.Add("@AFAM_ContactPrsn", asset.AFAM_ContactPrsn);
        //                p.Add("@AFAM_AMCFrmDate", asset.AFAM_AMCFrmDate);
        //                p.Add("@AFAM_AMCTo", asset.AFAM_AMCTo);
        //                p.Add("@AFAM_Contprsn", asset.AFAM_Contprsn);
        //                p.Add("@AFAM_PhoneNo", asset.AFAM_PhoneNo);
        //                p.Add("@AFAM_AMCCompanyName", asset.AFAM_AMCCompanyName);
        //                p.Add("@AFAM_AssetDeletion", asset.AFAM_AssetDeletion);
        //                p.Add("@AFAM_DlnDate", asset.AFAM_DlnDate);
        //                p.Add("@AFAM_DateOfDeletion", asset.AFAM_DateOfDeletion);
        //                p.Add("@AFAM_Value", asset.AFAM_Value);
        //                p.Add("@AFAM_Remark", asset.AFAM_Remark);
        //                p.Add("@AFAM_EMPCode", asset.AFAM_EMPCode);
        //                p.Add("@AFAM_LToWhom", asset.AFAM_LToWhom);
        //                p.Add("@AFAM_LAmount", asset.AFAM_LAmount);
        //                p.Add("@AFAM_LAggriNo", asset.AFAM_LAggriNo);
        //                p.Add("@AFAM_LDate", asset.AFAM_LDate);
        //                p.Add("@AFAM_LCurrencyType", asset.AFAM_LCurrencyType);
        //                p.Add("@AFAM_LExchDate", asset.AFAM_LExchDate);
        //                p.Add("@AFAM_CustId", asset.AFAM_CustId);

        //                p.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //                p.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                await connection.ExecuteAsync(
        //                    "spAcc_FixedAssetMaster",
        //                    p,
        //                    transaction,
        //                    commandType: CommandType.StoredProcedure
        //                );

        //                int iUpdateOrSave = p.Get<int>("@iUpdateOrSave");
        //                int iOper = p.Get<int>("@iOper");

        //                // If asset failed → rollback
        //                if (iOper != 1)
        //                {
        //                    transaction.Rollback();
        //                    return (iUpdateOrSave, iOper);
        //                }


        //                // --------------------------
        //                // 2️⃣ SAVE AUDIT LOG
        //                // --------------------------
        //                var auditParam = new DynamicParameters();

        //                auditParam.Add("@ALFO_UserID", audit.UserId);
        //                auditParam.Add("@ALFO_Module", audit.Module);
        //                auditParam.Add("@ALFO_Form", audit.Form);
        //                auditParam.Add("@ALFO_Event", audit.EventName);
        //                auditParam.Add("@ALFO_MasterID", audit.MasterId);
        //                auditParam.Add("@ALFO_MasterName", audit.MasterName);
        //                auditParam.Add("@ALFO_SubMasterID", audit.SubMasterId);
        //                auditParam.Add("@ALFO_SubMasterName", audit.SubMasterName);
        //                auditParam.Add("@ALFO_IPAddress", audit.IPAddress);
        //                auditParam.Add("@ALFO_CompID", audit.CompId);

        //                await connection.ExecuteAsync(
        //                    "spAudit_Log_Form_Operations",
        //                    auditParam,
        //                    transaction,
        //                    commandType: CommandType.StoredProcedure
        //                );

        //                // Commit both saves
        //                transaction.Commit();

        //                return (iUpdateOrSave, iOper);
        //            }
        //            catch (Exception)
        //            {
        //                transaction.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        // }

        //--------------------------

        //public async Task<int[]> SaveFixedAssetWithAuditAsync(SaveFixedAssetDto asset, GRACeFormOperationDto audit)
        //{
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    using var transaction = connection.BeginTransaction();

        //    int updateOrSave = 0, oper = 0;

        //    try
        //    {
        //        // 1️⃣ SAVE FIXED ASSET
        //        using (var cmdAsset = new SqlCommand("spAcc_FixedAssetMaster", connection, transaction))
        //        {
        //            cmdAsset.CommandType = CommandType.StoredProcedure;

        //            object DbDate(DateTime? dt) => dt ?? (object)DBNull.Value;

        //            cmdAsset.Parameters.AddWithValue("@AFAM_ID", asset.AFAM_ID);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_AssetType", asset.AFAM_AssetType ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_AssetCode", asset.AFAM_AssetCode ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Description", asset.AFAM_Description ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_ItemCode", asset.AFAM_ItemCode ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_ItemDescription", asset.AFAM_ItemDescription ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_CommissionDate", DbDate(asset.AFAM_CommissionDate));
        //            cmdAsset.Parameters.AddWithValue("@AFAM_PurchaseDate", DbDate(asset.AFAM_PurchaseDate));
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Quantity", asset.AFAM_Quantity);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Unit", asset.AFAM_Unit);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_AssetAge", asset.AFAM_AssetAge);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_PurchaseAmount", asset.AFAM_PurchaseAmount);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_PolicyNo", asset.AFAM_PolicyNo ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Amount", asset.AFAM_Amount);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_BrokerName", asset.AFAM_BrokerName ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_CompanyName", asset.AFAM_CompanyName ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Date", DbDate(asset.AFAM_Date));
        //            cmdAsset.Parameters.AddWithValue("@AFAM_ToDate", DbDate(asset.AFAM_ToDate));
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Location", asset.AFAM_Location);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Division", asset.AFAM_Division);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Department", asset.AFAM_Department);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Bay", asset.AFAM_Bay);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_EmployeeName", asset.AFAM_EmployeeName ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_EmployeeCode", asset.AFAM_EmployeeCode ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Code", asset.AFAM_Code ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_SuplierName", asset.AFAM_SuplierName ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_ContactPerson", asset.AFAM_ContactPerson ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Address", asset.AFAM_Address ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Phone", asset.AFAM_Phone ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Fax", asset.AFAM_Fax ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_EmailID", asset.AFAM_EmailID ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Website", asset.AFAM_Website ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_CreatedBy", asset.AFAM_CreatedBy);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_CreatedOn", DbDate(asset.AFAM_CreatedOn));
        //            cmdAsset.Parameters.AddWithValue("@AFAM_UpdatedBy", asset.AFAM_UpdatedBy);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_UpdatedOn", DbDate(asset.AFAM_UpdatedOn));
        //            cmdAsset.Parameters.AddWithValue("@AFAM_DelFlag", asset.AFAM_DelFlag ?? "X");
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Status", asset.AFAM_Status ?? "W");
        //            cmdAsset.Parameters.AddWithValue("@AFAM_YearID", asset.AFAM_YearID);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_CompID", asset.AFAM_CompID);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Opeartion", asset.AFAM_Opeartion ?? "C");
        //            cmdAsset.Parameters.AddWithValue("@AFAM_IPAddress", asset.AFAM_IPAddress ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_WrntyDesc", asset.AFAM_WrntyDesc ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_ContactPrsn", asset.AFAM_ContactPrsn ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Contprsn", asset.AFAM_Contprsn ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_PhoneNo", asset.AFAM_PhoneNo ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_AMCCompanyName", asset.AFAM_AMCCompanyName ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_AssetDeletion", asset.AFAM_AssetDeletion);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Value", asset.AFAM_Value);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_Remark", asset.AFAM_Remark ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_EMPCode", asset.AFAM_EMPCode ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_LToWhom", asset.AFAM_LToWhom ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_LAmount", asset.AFAM_LAmount);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_LAggriNo", asset.AFAM_LAggriNo ?? string.Empty);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_LCurrencyType", asset.AFAM_LCurrencyType);
        //            cmdAsset.Parameters.AddWithValue("@AFAM_CustId", asset.AFAM_CustId);

        //            cmdAsset.Parameters.AddWithValue("@AFAM_AMCFrmDate",
        //                 asset.AFAM_AMCFrmDate.HasValue ? asset.AFAM_AMCFrmDate.Value : (object)DBNull.Value);

        //            cmdAsset.Parameters.AddWithValue("@AFAM_AMCTo",
        //                asset.AFAM_AMCTo.HasValue ? asset.AFAM_AMCTo.Value : (object)DBNull.Value);

        //            cmdAsset.Parameters.AddWithValue("@AFAM_DateOfDeletion",
        //                asset.AFAM_DateOfDeletion.HasValue ? asset.AFAM_DateOfDeletion.Value : (object)DBNull.Value);

        //            cmdAsset.Parameters.AddWithValue("@AFAM_LDate",
        //                asset.AFAM_LDate.HasValue ? asset.AFAM_LDate.Value : (object)DBNull.Value);

        //            cmdAsset.Parameters.AddWithValue("@AFAM_DlnDate",
        //                asset.AFAM_DlnDate.HasValue ? asset.AFAM_DlnDate.Value : (object)DBNull.Value);

        //            cmdAsset.Parameters.AddWithValue("@AFAM_LExchDate",
        //               asset.AFAM_LExchDate.HasValue ? asset.AFAM_LExchDate.Value : (object)DBNull.Value);

        //            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

        //            cmdAsset.Parameters.Add(updateOrSaveParam);
        //            cmdAsset.Parameters.Add(operParam);

        //            await cmdAsset.ExecuteNonQueryAsync();

        //            updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
        //            oper = (int)(operParam.Value ?? 0);

        //            if (oper != 1)
        //            {
        //                transaction.Rollback();
        //                return new int[] { updateOrSave, oper };
        //            }
        //        }

        //        // 2️⃣ SAVE AUDIT LOG
        //        using (var cmdAudit = new SqlCommand("spAudit_Log_Form_Operations", connection, transaction))
        //        {
        //            cmdAudit.CommandType = CommandType.StoredProcedure;

        //            cmdAudit.Parameters.AddWithValue("@ALFO_UserID", audit.UserId);
        //            cmdAudit.Parameters.AddWithValue("@ALFO_Module", audit.Module ?? string.Empty);
        //            cmdAudit.Parameters.AddWithValue("@ALFO_Form", audit.Form ?? string.Empty);
        //            cmdAudit.Parameters.AddWithValue("@ALFO_Event", audit.EventName ?? string.Empty);

        //            cmdAudit.Parameters.AddWithValue("@ALFO_MasterID", asset.AFAM_ID == 0 ? updateOrSave : asset.AFAM_ID);

        //            cmdAudit.Parameters.AddWithValue("@ALFO_MasterName", audit.MasterName ?? string.Empty);
        //            cmdAudit.Parameters.AddWithValue("@ALFO_SubMasterID", audit.SubMasterId);
        //            cmdAudit.Parameters.AddWithValue("@ALFO_SubMasterName", audit.SubMasterName ?? string.Empty);
        //            cmdAudit.Parameters.AddWithValue("@ALFO_IPAddress", audit.IPAddress ?? string.Empty);
        //            cmdAudit.Parameters.AddWithValue("@ALFO_CompID", audit.CompId);

        //            await cmdAudit.ExecuteNonQueryAsync();
        //        }

        //        transaction.Commit();
        //        return new int[] { updateOrSave, oper };
        //    }
        //    catch (Exception ex)
        //    {
        //        try { transaction.Rollback(); } catch { }
        //        throw new Exception("ERROR in spAcc_FixedAssetMaster OR Transaction: " + ex.Message, ex);
        //    }
        //}

        //--------------------------------------



        public async Task<int> SaveFixedAssetAsync(FixedAssetDto asset, AuditDto audit)
        {
            // 🔹 Get CustomerCode (DB Name) from Session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 🔹 Dynamic Connection String
            string connectionString = _configuration.GetConnectionString(dbName);

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();

            try
            {
                // 1️⃣ SAVE FIXED ASSET
                var p = new DynamicParameters();

                p.Add("@AFAM_ID", asset.AFAM_ID);
                p.Add("@AFAM_AssetType", asset.AFAM_AssetType);
                p.Add("@AFAM_AssetCode", asset.AFAM_AssetCode);
                p.Add("@AFAM_Description", asset.AFAM_Description);
                p.Add("@AFAM_ItemCode", asset.AFAM_ItemCode);
                p.Add("@AFAM_ItemDescription", asset.AFAM_ItemDescription);
                p.Add("@AFAM_CommissionDate", asset.AFAM_CommissionDate);
                p.Add("@AFAM_PurchaseDate", asset.AFAM_PurchaseDate);
                p.Add("@AFAM_Quantity", asset.AFAM_Quantity);
                p.Add("@AFAM_Unit", asset.AFAM_Unit);
                p.Add("@AFAM_AssetAge", asset.AFAM_AssetAge);
                p.Add("@AFAM_PurchaseAmount", asset.AFAM_PurchaseAmount);
                p.Add("@AFAM_PolicyNo", asset.AFAM_PolicyNo);
                p.Add("@AFAM_Amount", asset.AFAM_Amount);
                p.Add("@AFAM_BrokerName", asset.AFAM_BrokerName);
                p.Add("@AFAM_CompanyName", asset.AFAM_CompanyName);
                p.Add("@AFAM_Date", asset.AFAM_Date);
                p.Add("@AFAM_ToDate", asset.AFAM_ToDate);
                p.Add("@AFAM_Location", asset.AFAM_Location);
                p.Add("@AFAM_Division", asset.AFAM_Division);
                p.Add("@AFAM_Department", asset.AFAM_Department);
                p.Add("@AFAM_Bay", asset.AFAM_Bay);
                p.Add("@AFAM_EmployeeName", asset.AFAM_EmployeeName);
                p.Add("@AFAM_EmployeeCode", asset.AFAM_EmployeeCode);
                p.Add("@AFAM_Code", asset.AFAM_Code);
                p.Add("@AFAM_SuplierName", asset.AFAM_SuplierName);
                p.Add("@AFAM_ContactPerson", asset.AFAM_ContactPerson);
                p.Add("@AFAM_Address", asset.AFAM_Address);
                p.Add("@AFAM_Phone", asset.AFAM_Phone);
                p.Add("@AFAM_Fax", asset.AFAM_Fax);
                p.Add("@AFAM_EmailID", asset.AFAM_EmailID);
                p.Add("@AFAM_Website", asset.AFAM_Website);
                p.Add("@AFAM_CreatedBy", asset.AFAM_CreatedBy);
                p.Add("@AFAM_CreatedOn", asset.AFAM_CreatedOn);
                p.Add("@AFAM_UpdatedBy", asset.AFAM_UpdatedBy);
                p.Add("@AFAM_UpdatedOn", asset.AFAM_UpdatedOn);
                p.Add("@AFAM_DelFlag", asset.AFAM_DelFlag);
                p.Add("@AFAM_Status", asset.AFAM_Status);
                p.Add("@AFAM_YearID", asset.AFAM_YearID);
                p.Add("@AFAM_CompID", asset.AFAM_CompID);
                p.Add("@AFAM_Opeartion", asset.AFAM_Opeartion);
                p.Add("@AFAM_IPAddress", asset.AFAM_IPAddress);
                p.Add("@AFAM_WrntyDesc", asset.AFAM_WrntyDesc);
                p.Add("@AFAM_ContactPrsn", asset.AFAM_ContactPrsn);
                p.Add("@AFAM_AMCFrmDate", asset.AFAM_AMCFrmDate);
                p.Add("@AFAM_AMCTo", asset.AFAM_AMCTo);
                p.Add("@AFAM_Contprsn", asset.AFAM_Contprsn);
                p.Add("@AFAM_PhoneNo", asset.AFAM_PhoneNo);
                p.Add("@AFAM_AMCCompanyName", asset.AFAM_AMCCompanyName);
                p.Add("@AFAM_AssetDeletion", asset.AFAM_AssetDeletion);
                p.Add("@AFAM_DlnDate", asset.AFAM_DlnDate);
                p.Add("@AFAM_DateOfDeletion", asset.AFAM_DateOfDeletion);
                p.Add("@AFAM_Value", asset.AFAM_Value);
                p.Add("@AFAM_Remark", asset.AFAM_Remark);
                p.Add("@AFAM_EMPCode", asset.AFAM_EMPCode);
                p.Add("@AFAM_LToWhom", asset.AFAM_LToWhom);
                p.Add("@AFAM_LAmount", asset.AFAM_LAmount);
                p.Add("@AFAM_LAggriNo", asset.AFAM_LAggriNo);
                p.Add("@AFAM_LDate", asset.AFAM_LDate);
                p.Add("@AFAM_LCurrencyType", asset.AFAM_LCurrencyType);
                p.Add("@AFAM_LExchDate", asset.AFAM_LExchDate);
                p.Add("@AFAM_CustId", asset.AFAM_CustId);

                // OUTPUT
                p.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await conn.ExecuteAsync(
                    "spAcc_FixedAssetMaster",
                    p,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );

                int savedAssetID = p.Get<int>("@iOper");

                // CONDITION: AFAM_ID = ALFO_MasterID (UPDATE CASE)
                // INSERT CASE → savedAssetID
                int masterIdToSave = asset.AFAM_ID > 0 ? asset.AFAM_ID : savedAssetID;

                // 2️⃣ SAVE AUDIT LOG
                var auditParams = new DynamicParameters();

                auditParams.Add("@ALFO_UserID", audit.UserID);
                auditParams.Add("@ALFO_Module", audit.Module);
                auditParams.Add("@ALFO_Form", audit.Form);
                auditParams.Add("@ALFO_Event", audit.Event);
                auditParams.Add("@ALFO_MasterID", masterIdToSave);   // FIX APPLIED HERE
                auditParams.Add("@ALFO_MasterName", audit.MasterName);
                auditParams.Add("@ALFO_SubMasterID", audit.SubMasterID);
                auditParams.Add("@ALFO_SubMasterName", audit.SubMasterName);
                auditParams.Add("@ALFO_IPAddress", audit.IPAddress);
                auditParams.Add("@ALFO_CompID", audit.CompID);

                await conn.ExecuteAsync(
                    "spAudit_Log_Form_Operations",
                    auditParams,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );

                // COMMIT
                transaction.Commit();

                return masterIdToSave;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //LoadImportedforeigncurrencytype
    }
    }
















