using System.Data;
using ClosedXML.Excel;
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

        //Generate
        public async Task<string> GenerateAssetCodeAsync(
int compId,
int custId,
int locationId,
int divisionId,
int departmentId,
int bayId,
string assetCode)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            if (string.IsNullOrWhiteSpace(assetCode))
                throw new Exception("Asset Code is required.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Local function → replaces LoadLevelCode
            async Task<string> LoadLevelCodeAsync(int levelId)
            {
                const string sql = @"
            SELECT LS_DescCode
            FROM Acc_AssetLocationSetup
            WHERE LS_ID = @LevelId
              AND LS_CustId = @CustId
              AND LS_CompID = @CompId";

                return await connection.QueryFirstOrDefaultAsync<string>(
                    sql,
                    new
                    {
                        LevelId = levelId,
                        CustId = custId,
                        CompId = compId
                    });
            }

            string slocation = string.Empty;
            string sDivision = string.Empty;
            string sDeptmnt = string.Empty;
            string sBay = string.Empty;

            // ✅ Same logic as SelectedIndex > 0
            if (locationId > 0)
                slocation = await LoadLevelCodeAsync(locationId);

            if (divisionId > 0)
                sDivision = "/" + await LoadLevelCodeAsync(divisionId);

            if (departmentId > 0)
                sDeptmnt = "/" + await LoadLevelCodeAsync(departmentId);

            if (bayId > 0)
                sBay = "/" + await LoadLevelCodeAsync(bayId);

            // Final asset code (same as VB)
            string finalCode =
                $"{slocation}{sDivision}{sDeptmnt}{sBay}/{assetCode}";

            return finalCode;
        }

        //excelupload
        public class AssetUploadException : Exception
        {
            public Dictionary<string, List<string>> Errors { get; }

            public AssetUploadException(Dictionary<string, List<string>> errors)
                : base("Error processing asset master")
            {
                Errors = errors;
            }
        }
        public async Task<List<string>> UploadAssetExcelAsync(
     int compId,
     int custId,
     int yearId,
     int userId,
     IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            // 1️⃣ Read Excel
            List<UploadAssetCreationExcelDto> assets;
            List<string> headerErrors;

            using (var stream = file.OpenReadStream())
            {
                assets = ParseExcelToAssets(stream, out headerErrors);
            }

            if (!assets.Any())
                throw new Exception("Excel file contains no data rows.");

            // 2️⃣ Validate rows
            var validationErrors = ValidateAssets(assets);

            // 3️⃣ Duplicate checks
            var duplicateErrors = new List<UploadAssetCreationExcelDto>();

            void AddDuplicateErrors(
                Func<UploadAssetCreationExcelDto, string> keySelector,
                string fieldName)
            {
                duplicateErrors.AddRange(
                    assets.GroupBy(keySelector)
                          .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                          .Select(g => new UploadAssetCreationExcelDto
                          {
                              AssetClass = g.First().AssetClass,
                              AssetDescription = g.First().AssetDescription,
                              ErrorMessage = $"Duplicate {fieldName} found: {g.Key}"
                          })
                );
            }

            AddDuplicateErrors(a => $"{a.AssetClass}-{a.AssetDescription}", "Asset");

            // 4️⃣ Group all errors
            var finalErrors = new Dictionary<string, List<string>>();

            if (headerErrors.Any())
                finalErrors["Missing column"] = headerErrors;

            if (validationErrors.Any())
            {
                var grouped = validationErrors
                    .GroupBy(e => new { e.AssetClass, e.AssetDescription })
                    .Select(g =>
                        $"{g.Key.AssetClass} - {g.Key.AssetDescription}: " +
                        $"{string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Missing / Invalid values"] = grouped;
            }

            if (duplicateErrors.Any())
            {
                var grouped = duplicateErrors
                    .GroupBy(e => new { e.AssetClass, e.AssetDescription })
                    .Select(g =>
                        $"{g.Key.AssetClass} - {g.Key.AssetDescription}: " +
                        $"{string.Join(", ", g.Select(e => e.ErrorMessage))}")
                    .ToList();

                finalErrors["Duplication"] = grouped;
            }

            // 5️⃣ Throw if any validation issue
            if (finalErrors.Any())
                throw new AssetUploadException(finalErrors);

            // 6️⃣ Process row by row
            var result = new List<string>();

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");


            var connectionString = _configuration.GetConnectionString(dbName);
            using var con = new SqlConnection(connectionString);
            await con.OpenAsync();

            int rowNo = 2;

            // 🔥 Get starting running number only once
            int runningNo = await con.ExecuteScalarAsync<int>(
                @"SELECT ISNULL(MAX(AFAM_ID),0) + 1 
      FROM Acc_FixedAssetMaster
      WHERE AFAM_CompID=@CompID AND AFAM_YearID=@YearID",
                new { CompID = compId, YearID = yearId }
            );

            foreach (var asset in assets)
            {
                using var tran = con.BeginTransaction();

                try
                {
                    // Asset Type
                    string assetTypeIdsql = @"
SELECT AM_ID
FROM Acc_AssetMaster
WHERE AM_Description = @Desc
  AND AM_CompID = @CompID
  AND AM_CustId = @CustID
  AND AM_LevelCode = 2";

                    int? assetTypeId = await con.ExecuteScalarAsync<int?>(
                        assetTypeIdsql,
                        new { Desc = asset.AssetClass, CompID = compId, CustID = custId },
                        tran);

                    if (!assetTypeId.HasValue)
                    {
                        // 🔑 Generate new AM_ID
                        int newAssetTypeId = await con.ExecuteScalarAsync<int>(
                            @"SELECT ISNULL(MAX(AM_ID), 0) + 1 FROM Acc_AssetMaster",
                            transaction: tran);

                        string insertSql = @"
INSERT INTO Acc_AssetMaster
(AM_ID, AM_Description, AM_LevelCode, AM_CompID, AM_CustId, AM_Status)
VALUES (@AM_ID, @Desc, 2, @CompID, @CustID, 'A');
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        assetTypeId = await con.ExecuteScalarAsync<int>(
                            insertSql,
                            new { AM_ID = newAssetTypeId, Desc = asset.AssetClass, CompID = compId, CustID = custId },
                            tran);

                        assetTypeId = newAssetTypeId;
                    }

                    // Unit of Measurement
                    string unitSql = @"
SELECT CMM_ID
FROM Content_Management_Master
WHERE CMM_Desc = @Desc
  AND CMM_CompID = @CompID
  AND CMM_Category = 'UM'";

                    int? unitId = await con.ExecuteScalarAsync<int?>(
                        unitSql,
                        new { Desc = asset.UnitOfMeasurement, CompID = compId },
                        tran);

                    if (!unitId.HasValue)
                    {
                        int newCmmId = await con.ExecuteScalarAsync<int>(
                            @"SELECT ISNULL(MAX(CMM_ID),0) + 1 FROM Content_Management_Master",
                            transaction: tran);

                        string cmmCode = asset.UnitOfMeasurement
                            .Replace(" ", "")
                            .ToUpper();

                        string insertUnitSql = @"
INSERT INTO Content_Management_Master
(CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMM_CompID, CMM_Status, CMM_DelFlag)
VALUES
(@CMM_ID, @CMM_Code, @Desc, 'UM', @CompID, 'A', 'W');";

                        await con.ExecuteAsync(
                            insertUnitSql,
                            new
                            {
                                CMM_ID = newCmmId,
                                CMM_Code = cmmCode,
                                Desc = asset.UnitOfMeasurement,
                                CompID = compId
                            },
                            tran);

                        unitId = newCmmId;
                    }

                    //location
                    string locationSql = @"
                          SELECT LS_ID 
                          FROM Acc_AssetLocationSetup 
                          WHERE LS_Description = @Val
                          AND LS_CompID = @CompID
                          AND LS_CustId = @CustID";

                    int? locId = await con.ExecuteScalarAsync<int?>(
                        locationSql, new { Val = asset.Location, CompID = compId, CustID = custId }, tran);

                    if (!locId.HasValue)
                    {
                        // 🔑 Generate new LS_ID
                        int newlocId = await con.ExecuteScalarAsync<int>(
                            @"SELECT ISNULL(MAX(LS_ID), 0) + 1 FROM Acc_AssetLocationSetup",
                            transaction: tran);

                        // Insert new Location
                        string insertLocationSql = @"
                              INSERT INTO Acc_AssetLocationSetup
                               (LS_ID, LS_Description, LS_CompID, LS_CustId, LS_Status)
                                VALUES (@LS_ID, @Val, @CompID, @CustID, 'A');
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        locId = await con.ExecuteScalarAsync<int>(
                       insertLocationSql, new { LS_ID = newlocId, Val = asset.Location, CompID = compId, CustID = custId }, tran);
                        locId = newlocId;
                    }

                    //    // Division
                    //        string divisionSql = @"
                    //SELECT LS_ID
                    //FROM Acc_AssetLocationSetup
                    //WHERE LS_Description = @Val
                    //  AND LS_CompID = @CompID
                    //  AND LS_CustId = @CustID";

                    //        int? divisionId = await con.ExecuteScalarAsync<int?>(
                    //            divisionSql,
                    //            new { Val = asset.Division, CompID = compId, CustID = custId },
                    //            tran);

                    //        if (!divisionId.HasValue)
                    //        {
                    //            string insertDivisionSql = @"
                    //    INSERT INTO Acc_AssetLocationSetup
                    //    (LS_Description, LS_CompID, LS_CustId, LS_Status)
                    //    VALUES (@Val, @CompID, @CustID, 'A');
                    //    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    //        divisionId = await con.ExecuteScalarAsync<int>(
                    //                insertDivisionSql,
                    //                new { Val = asset.Division, CompID = compId, CustID = custId },
                    //                tran);
                    //        }

                    //    // Department

                    //        string departmentSql = @"
                    //SELECT LS_ID
                    //FROM Acc_AssetLocationSetup
                    //WHERE LS_Description = @Val
                    //  AND LS_CompID = @CompID
                    //  AND LS_CustId = @CustID";

                    //        int? departmentId = await con.ExecuteScalarAsync<int?>(
                    //            departmentSql,
                    //            new { Val = asset.Department, CompID = compId, CustID = custId },
                    //            tran);

                    //        if (!departmentId.HasValue)
                    //        {
                    //            string insertDepartmentSql = @"
                    //    INSERT INTO Acc_AssetLocationSetup
                    //    (LS_Description, LS_CompID, LS_CustId, LS_Status)
                    //    VALUES (@Val, @CompID, @CustID, 'A');
                    //    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    //        departmentId = await con.ExecuteScalarAsync<int>(
                    //                insertDepartmentSql,
                    //                new { Val = asset.Department, CompID = compId, CustID = custId },
                    //                tran);
                    //        }

                    //    // Bay

                    //        string baySql = @"
                    //SELECT LS_ID
                    //FROM Acc_AssetLocationSetup
                    //WHERE LS_Description = @Val
                    //  AND LS_CompID = @CompID
                    //  AND LS_CustId = @CustID";

                    //        int? bayId = await con.ExecuteScalarAsync<int?>(
                    //            baySql,
                    //            new { Val = asset.Bay, CompID = compId, CustID = custId },
                    //            tran);

                    //        if (!bayId.HasValue)
                    //        {
                    //            string insertBaySql = @"
                    //    INSERT INTO Acc_AssetLocationSetup
                    //    (LS_Description, LS_CompID, LS_CustId, LS_Status)
                    //    VALUES (@Val, @CompID, @CustID, 'A');
                    //    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    //        bayId = await con.ExecuteScalarAsync<int>(
                    //                insertBaySql,
                    //                new { Val = asset.Bay, CompID = compId, CustID = custId },
                    //                tran);
                    //        }

                    // Generate Transaction No
                    //    int maxId = await con.ExecuteScalarAsync<int>(
                    //@"SELECT ISNULL(MAX(AFAM_ID)+1,1)
                    //  FROM Acc_FixedAssetMaster
                    //  WHERE AFAM_CompID=@CompID
                    //    AND AFAM_YearID=@YearID",
                    //new { CompID = compId, YearID = yearId },
                    //tran);

                    //    string assetTransNo = "FAR000-" + maxId;

                    // Excel gives prefix like FAR000-
                    string prefix = asset.AssetCode?.Trim();

                    if (string.IsNullOrWhiteSpace(prefix))
                        prefix = "FAR000-"; // fallback

                    // Ensure dash exists
                    if (!prefix.EndsWith("-"))
                        prefix += "-";

                    // 🔥 Final Generated Code
                    string generatedAssetCode = prefix + runningNo;


                    // Save Fixed Asset
                    using var cmd = new SqlCommand(
                        "spAcc_FixedAssetMaster",
                        con,
                        tran);

                    cmd.CommandType = CommandType.StoredProcedure;
                    // Mandatory Keys
                    cmd.Parameters.AddWithValue("@AFAM_ID", 0);
                    cmd.Parameters.AddWithValue("@AFAM_AssetType", assetTypeId?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@AFAM_AssetCode", "");
                    cmd.Parameters.AddWithValue("@AFAM_Description", asset.AssetDescription ?? "");
                    cmd.Parameters.AddWithValue("@AFAM_ItemCode", "");
                    cmd.Parameters.AddWithValue("@AFAM_ItemDescription", "");
                    cmd.Parameters.AddWithValue("@AFAM_CommissionDate", SqlDbType.DateTime).Value = asset.PutToUseDate ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@AFAM_PurchaseDate", new DateTime(1900, 1, 1));
                    cmd.Parameters.AddWithValue("@AFAM_Quantity", SqlDbType.Int).Value = asset.Quantity ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@AFAM_Unit", unitId ?? 0);
                    cmd.Parameters.AddWithValue("@AFAM_AssetAge", asset.UsefulLife);
                    cmd.Parameters.AddWithValue("@AFAM_PurchaseAmount", 0);
                    cmd.Parameters.AddWithValue("@AFAM_PolicyNo", "");
                    cmd.Parameters.AddWithValue("@AFAM_Amount", 0);
                    cmd.Parameters.AddWithValue("@AFAM_BrokerName", "");
                    cmd.Parameters.AddWithValue("@AFAM_CompanyName", "");
                    cmd.Parameters.AddWithValue("@AFAM_Date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AFAM_ToDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AFAM_Location", locId);
                    cmd.Parameters.AddWithValue("@AFAM_Division", 0);
                    cmd.Parameters.AddWithValue("@AFAM_Department", 0);
                    cmd.Parameters.AddWithValue("@AFAM_Bay", 0);
                    cmd.Parameters.AddWithValue("@AFAM_EmployeeName", "");
                    cmd.Parameters.AddWithValue("@AFAM_EmployeeCode", "");
                    cmd.Parameters.AddWithValue("@AFAM_Code", "");
                    cmd.Parameters.AddWithValue("@AFAM_SuplierName", "");
                    cmd.Parameters.AddWithValue("@AFAM_ContactPerson", "");
                    cmd.Parameters.AddWithValue("@AFAM_Address", "");
                    cmd.Parameters.AddWithValue("@AFAM_Phone", "");
                    cmd.Parameters.AddWithValue("@AFAM_Fax", "");
                    cmd.Parameters.AddWithValue("@AFAM_EmailID", "");
                    cmd.Parameters.AddWithValue("@AFAM_Website", "");
                    cmd.Parameters.AddWithValue("@AFAM_CreatedBy", userId);
                    cmd.Parameters.AddWithValue("@AFAM_CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AFAM_UpdatedBy", userId);
                    cmd.Parameters.AddWithValue("@AFAM_UpdatedOn", DateTime.Now);

                    //// IMPORTANT: These must ALWAYS be sent
                    if (asset.PutToUseDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@AFAM_DelFlag", "A");
                        cmd.Parameters.AddWithValue("@AFAM_Status", "A");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@AFAM_DelFlag", "X");
                        cmd.Parameters.AddWithValue("@AFAM_Status", "W");
                    }

                    cmd.Parameters.AddWithValue("@AFAM_YearID", yearId);
                    cmd.Parameters.AddWithValue("@AFAM_CompID", compId);
                    cmd.Parameters.AddWithValue("@AFAM_Opeartion", "C");
                    cmd.Parameters.AddWithValue("@AFAM_IPAddress", "0.0.0.0");

                    // Optional / Extra Fields
                    cmd.Parameters.AddWithValue("@AFAM_WrntyDesc", "");
                    cmd.Parameters.AddWithValue("@AFAM_ContactPrsn", "");
                    cmd.Parameters.AddWithValue("@AFAM_AMCFrmDate", asset.PutToUseDate ?? new DateTime(1900, 1, 1));
                    cmd.Parameters.AddWithValue("@AFAM_AMCTo", new DateTime(1900, 1, 1));
                    cmd.Parameters.AddWithValue("@AFAM_Contprsn", "");
                    cmd.Parameters.AddWithValue("@AFAM_PhoneNo", "");
                    cmd.Parameters.AddWithValue("@AFAM_AMCCompanyName", "");
                    cmd.Parameters.AddWithValue("@AFAM_AssetDeletion", 0);
                    cmd.Parameters.AddWithValue("@AFAM_DlnDate", new DateTime(1900, 1, 1));
                    cmd.Parameters.AddWithValue("@AFAM_DateOfDeletion", new DateTime(1900, 1, 1));
                    cmd.Parameters.AddWithValue("@AFAM_Value", 0);
                    cmd.Parameters.AddWithValue("@AFAM_Remark", "");
                    cmd.Parameters.AddWithValue("@AFAM_EMPCode", "");
                    cmd.Parameters.AddWithValue("@AFAM_LToWhom", "");
                    cmd.Parameters.AddWithValue("@AFAM_LAmount", 0);
                    cmd.Parameters.AddWithValue("@AFAM_LAggriNo", "");
                    cmd.Parameters.AddWithValue("@AFAM_LDate", new DateTime(1900, 1, 1));
                    cmd.Parameters.AddWithValue("@AFAM_LCurrencyType", 0);
                    cmd.Parameters.AddWithValue("@AFAM_LExchDate", new DateTime(1900, 1, 1));
                    cmd.Parameters.AddWithValue("@AFAM_CustId", custId);
                    //cmd.Parameters.AddWithValue("@AFAM_Attribute1", "");
                    //cmd.Parameters.AddWithValue("@AFAM_Attribute2", "");
                    //cmd.Parameters.AddWithValue("@AFAM_Attribute3", "");
                    // OUTPUT parameters
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
                    int newAfamId = Convert.ToInt32(operParam.Value);
                    string generatedCode = $"FAR00{newAfamId}";
                    await con.ExecuteAsync(@"
UPDATE Acc_FixedAssetMaster
SET AFAM_AssetCode = @Code,
    AFAM_Code = @Code
WHERE AFAM_ID = @Id",
new { Code = generatedCode, Id = newAfamId }, tran);

                    tran.Commit();
                    result.Add($"Row {rowNo}: Success");
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }

                rowNo++;
            }

            return result;
        }
        private List<UploadAssetCreationExcelDto> ParseExcelToAssets(Stream fileStream, out List<string> headerErrors)
        {
            using var workbook = new XLWorkbook(fileStream);
            var ws = workbook.Worksheet(1);

            headerErrors = new List<string>();

            // Expected headers (match your template)
            string[] expectedHeaders = new[]
            {
    "SlNo","Location","Division","Department","Bay",
    "AssetClass","AssetCode","Asset Description",
    "Quantity","Date of Put To Use","Units Of Measurement","Useful Life of Asset"
};

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                var actual = ws.Cell(1, col).GetString().Trim();
                var expected = expectedHeaders[col - 1];

                if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                {
                    headerErrors.Add($"Expected header '{expected}' at column {col}, but found '{actual}'");
                }
            }

            var assets = new List<UploadAssetCreationExcelDto>();
            var rows = ws.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                var quantityStr = row.Cell(9).GetString().Trim();
                var dateStr = row.Cell(10).GetString().Trim();
                var usefulLifeStr = row.Cell(12).GetString().Trim();


                int q;
                decimal ul;
                DateTime d;

                assets.Add(new UploadAssetCreationExcelDto
                {
                    Location = row.Cell(2).GetString().Trim(),
                    Division = row.Cell(3).GetString().Trim(),
                    Department = row.Cell(4).GetString().Trim(),
                    Bay = row.Cell(5).GetString().Trim(),
                    AssetClass = row.Cell(6).GetString().Trim(),
                    AssetCode = row.Cell(7).GetString().Trim(),
                    AssetDescription = row.Cell(8).GetString().Trim(),

                    QuantityRaw = quantityStr,
                    Quantity = int.TryParse(quantityStr, out q) ? q : (int?)null,

                    PutToUseDateRaw = dateStr,
                    PutToUseDate = DateTime.TryParse(dateStr, out d) ? d : (DateTime?)null,

                    UnitOfMeasurement = row.Cell(11).GetString().Trim(),

                    UsefulLifeRaw = usefulLifeStr,
                    UsefulLife = decimal.TryParse(usefulLifeStr, out ul) ? ul : (decimal?)null,
                });
            }

            return assets;
        }
        private List<UploadAssetCreationExcelDto> ValidateAssets(List<UploadAssetCreationExcelDto> assets)
        {
            var errors = new List<UploadAssetCreationExcelDto>();
            int rowNo = 2;

            foreach (var a in assets)
            {
                void AddError(string msg)
                {
                    errors.Add(new UploadAssetCreationExcelDto
                    {
                        AssetClass = a.AssetClass,
                        AssetDescription = a.AssetDescription,
                        ErrorMessage = $"Row {rowNo}: {msg}"
                    });
                }

                // Mandatory field checks
                if (string.IsNullOrWhiteSpace(a.Location))
                    AddError("Location is mandatory");

                if (string.IsNullOrWhiteSpace(a.AssetClass))
                    AddError("Asset Class is mandatory");

                if (string.IsNullOrWhiteSpace(a.AssetDescription))
                    AddError("Asset Description is mandatory");

                if (string.IsNullOrWhiteSpace(a.UnitOfMeasurement))
                    AddError("Units of Measurement is mandatory");


                if (string.IsNullOrWhiteSpace(a.QuantityRaw))
                    AddError("Quantity is mandatory");
                else if (!a.Quantity.HasValue)
                    AddError("Quantity must be numeric");
                else if (a.Quantity <= 0)
                    AddError("Quantity must be greater than 0");

                if (string.IsNullOrWhiteSpace(a.PutToUseDateRaw))
                    AddError("Put To Use Date is mandatory");
                else if (!a.PutToUseDate.HasValue)
                    AddError("Put To Use Date must be a valid date");

                if (string.IsNullOrWhiteSpace(a.UsefulLifeRaw))
                    AddError("Useful Life is mandatory");
                else if (!a.UsefulLife.HasValue)
                    AddError("Useful Life must be numeric");
                else if (a.UsefulLife <= 0)
                    AddError("Useful Life must be greater than 0");

                //if (string.IsNullOrWhiteSpace(a.AssetCode))
                //    AddError("Asset Code is mandatory");
                //else if (a.AssetCode.All(char.IsDigit))
                //    AddError("Asset Code must contain alphabets");

                // Text validations
                if (!string.IsNullOrWhiteSpace(a.Location) && a.Location.All(char.IsDigit))
                    AddError("Location must contain text, not only numbers");

                if (!string.IsNullOrWhiteSpace(a.AssetClass) && a.AssetClass.All(char.IsDigit))
                    AddError("Asset Class must contain text, not only numbers");

                if (!string.IsNullOrWhiteSpace(a.AssetDescription) && a.AssetDescription.All(char.IsDigit))
                    AddError("Asset Description must contain text, not only numbers");

                if (!string.IsNullOrWhiteSpace(a.UnitOfMeasurement) && a.UnitOfMeasurement.All(char.IsDigit))
                    AddError("Unit Of Measurement must contain text, not only numbers");

                // Optional text columns
                if (!string.IsNullOrWhiteSpace(a.Division) && a.Division.All(char.IsDigit))
                    AddError("Division must be text");

                if (!string.IsNullOrWhiteSpace(a.Department) && a.Department.All(char.IsDigit))
                    AddError("Department must be text");

                if (!string.IsNullOrWhiteSpace(a.Bay) && a.Bay.All(char.IsDigit))
                    AddError("Bay must be text");

                rowNo++;
            }
            // Duplicate validation
            var duplicates = assets
                .GroupBy(x => new { x.AssetClass, x.AssetDescription })
                .Where(g => g.Count() > 1);

            foreach (var d in duplicates)
            {
                errors.Add(new UploadAssetCreationExcelDto
                {
                    AssetClass = d.Key.AssetClass,
                    AssetDescription = d.Key.AssetDescription,
                    ErrorMessage = "Duplicate asset found"
                });
            }

            // var codeDuplicates = assets
            //.GroupBy(x => x.AssetCode)
            //.Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1);

            // foreach (var d in codeDuplicates)
            // {
            //     errors.Add(new UploadAssetCreationExcelDto
            //     {
            //         AssetClass = d.First().AssetClass,
            //         AssetDescription = d.First().AssetDescription,
            //         ErrorMessage = $"Duplicate Asset Code found: {d.Key}"
            //     });
            // }

            return errors;
        }
    }
    }
















