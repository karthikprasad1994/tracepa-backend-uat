using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetTransactionAdditionDto;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetTransactionAdditionService : AssetTransactionAdditionInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public AssetTransactionAdditionService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        ////LoadCustomer
        //public async Task<IEnumerable<CustDto>> LoadCustomerAsync(int CompId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var query = @"
        //    SELECT 
        //        CUST_ID AS Cust_Id,
        //        CUST_NAME AS Cust_Name
        //    FROM SAD_CUSTOMER_MASTER
        //    WHERE CUST_DELFLG <> 'D' 
        //      AND CUST_CompID = @CompId";

        //    return await connection.QueryAsync<CustDto>(query, new { CompId });
        //}

        ////LoadStatus
        //public async Task<IEnumerable<StatusDto>> LoadStatusAsync(int compId, string Name)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var query = @"
        //    SELECT 
        //        CUST_STATUS AS Status
        //    FROM SAD_CUSTOMER_MASTER
        //    WHERE CUST_CompID = @CompId
        //    AND CUST_NAME = @Name";

        //    return await connection.QueryAsync<StatusDto>(query, new { CompId = compId, Name = Name });

        //}

        ////FinancialYear
        //public async Task<IEnumerable<YearDto>> GetYearsAsync(int compId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var query = @"
        //     SELECT 
        //         YMS_YEARID,
        //       YMS_ID
        //       FROM YEAR_MASTER
        //       WHERE 
        //       YMS_FROMDATE < DATEADD(YEAR, 1, GETDATE())
        //       AND YMS_CompId = @CompId
        //       ORDER BY LEFT(YMS_ID, 4) DESC";

        //    return await connection.QueryAsync<YearDto>(query, new { CompId = compId });
        //}


        ////AddDetails
        //public async Task<List<AssetAdditionDetailsDto>> AddAssetDetailsAsync(AddAssetDetailsRequest request)
        //{
        //    // Validation
        //    if (request.TransactionTypeId == 0)
        //        throw new Exception("Select Transaction Type.");
        //    if (request.AssetClassId == 0)
        //        throw new Exception("Select Asset Class.");
        //    if (request.AssetId == 0)
        //        throw new Exception("Select Asset.");

        //    // Dynamic DB
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode missing in session.");

        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    // New row object
        //    var newRow = new AssetAdditionDetailsDto
        //    {
        //        TypeId = 2,
        //        Type = "Addition",
        //        PKID = 0,
        //        MasID = 0,
        //        SupplierName = request.SupplierName,
        //        Particulars = request.Particulars,
        //        DocNo = request.DocNo,
        //        DocDate = request.DocDate,
        //        BasicCost = request.BasicCost,
        //        TaxAmount = request.TaxAmount,
        //        Total = request.Total,
        //        AssetValue = request.AssetValue
        //    };

        //    using var conn = new SqlConnection(connectionString);
        //    await conn.OpenAsync();

        //    var param = new DynamicParameters();
        //    param.Add("@SupplierName", request.SupplierName);
        //    param.Add("@Particulars", request.Particulars);
        //    param.Add("@DocNo", request.DocNo);
        //    param.Add("@DocDate", request.DocDate);
        //    param.Add("@BasicCost", request.BasicCost);
        //    param.Add("@TaxAmount", request.TaxAmount);
        //    param.Add("@Total", request.Total);
        //    param.Add("@AssetValue", request.AssetValue);
        //    param.Add("@TypeId", 2);

        //    await conn.ExecuteAsync("sp_Insert_AssetAdditionDetails", param, commandType: CommandType.StoredProcedure);

        //    return new List<AssetAdditionDetailsDto> { newRow };
        //}


        //LoadAsset
        public async Task<IEnumerable<AssettTypeDto>> LoadFxdAssetTypeAsync(int compId, int custId)
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
        SELECT 
            AM_ID AS AssetTypeId,
            AM_Description AS AssetTypeName
        FROM Acc_AssetMaster
        WHERE AM_LevelCode = 2
          AND AM_DelFlag = 'A'
          AND AM_CompID = @CompId
          AND AM_CustId = @CustId";

            return await connection.QueryAsync<AssettTypeDto>(query, new
            {
                CompId = compId,
                CustId = custId
            });
        }



        //SaveTransactionAddition
        public async Task<int> SaveTransactionAssetAndAuditAsync( ClsAssetTransactionAdditionDto asset, TransactionAuditDto audit)
        {
            // 🔹 Get DB name from session
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
                // ------------------------------------------------------
                // 1️⃣ SAVE ASSET TRANSACTION
                // ------------------------------------------------------
                var p = new DynamicParameters();

                p.Add("@AFAA_ID", asset.AFAA_ID);
                p.Add("@AFAA_AssetTrType", asset.AFAA_AssetTrType);
                p.Add("@AFAA_CurrencyType", asset.AFAA_CurrencyType);
                p.Add("@AFAA_CurrencyAmnt", asset.AFAA_CurrencyAmnt);
                p.Add("@AFAA_Location", asset.AFAA_Location);
                p.Add("@AFAA_Division", asset.AFAA_Division);
                p.Add("@AFAA_Department", asset.AFAA_Department);
                p.Add("@AFAA_Bay", asset.AFAA_Bay);
                p.Add("@AFAA_ActualLocn", asset.AFAA_ActualLocn);
                p.Add("@AFAA_SupplierName", asset.AFAA_SupplierName);
                p.Add("@AFAA_SupplierCode", asset.AFAA_SupplierCode);
                p.Add("@AFAA_TrType", asset.AFAA_TrType);
                p.Add("@AFAA_AssetType", asset.AFAA_AssetType);
                p.Add("@AFAA_AssetNo", asset.AFAA_AssetNo);
                p.Add("@AFAA_AssetRefNo", asset.AFAA_AssetRefNo);
                p.Add("@AFAA_Description", asset.AFAA_Description);
                p.Add("@AFAA_ItemCode", asset.AFAA_ItemCode);
                p.Add("@AFAA_ItemDescription", asset.AFAA_ItemDescription);
                p.Add("@AFAA_Quantity", asset.AFAA_Quantity);
                p.Add("@AFAA_CommissionDate", asset.AFAA_CommissionDate);
                p.Add("@AFAA_PurchaseDate", asset.AFAA_PurchaseDate);
                p.Add("@AFAA_AssetAge", asset.AFAA_AssetAge);
                p.Add("@AFAA_AssetAmount", asset.AFAA_AssetAmount);
                p.Add("@AFAA_FYAmount", asset.AFAA_FYAmount);
                p.Add("@AFAA_DepreAmount", asset.AFAA_DepreAmount);
                p.Add("@AFAA_AssetDelID", asset.AFAA_AssetDelID);
                p.Add("@AFAA_AssetDelDate", asset.AFAA_AssetDelDate);
                p.Add("@AFAA_AssetDeletionDate", asset.AFAA_AssetDeletionDate);
                p.Add("@AFAA_Assetvalue", asset.AFAA_Assetvalue);
                p.Add("@AFAA_AssetDesc", asset.AFAA_AssetDesc);
                p.Add("@AFAA_CreatedBy", asset.AFAA_CreatedBy);
                p.Add("@AFAA_CreatedOn", asset.AFAA_CreatedOn);
                p.Add("@AFAA_UpdatedBy", asset.AFAA_UpdatedBy);
                p.Add("@AFAA_UpdatedOn", asset.AFAA_UpdatedOn);
                p.Add("@AFAA_Status", asset.AFAA_Status);
                p.Add("@AFAA_Delflag", asset.AFAA_Delflag);
                p.Add("@AFAA_YearID", asset.AFAA_YearID);
                p.Add("@AFAA_CompID", asset.AFAA_CompID);
                p.Add("@AFAA_Operation", asset.AFAA_Operation);
                p.Add("@AFAA_IPAddress", asset.AFAA_IPAddress);
                p.Add("@AFAA_AddnType", asset.AFAA_AddnType);
                p.Add("@AFAA_DelnType", asset.AFAA_DelnType);
                p.Add("@AFAA_Depreciation", asset.AFAA_Depreciation);
                p.Add("@AFAA_AddtnDate", asset.AFAA_AddtnDate);
                p.Add("@AFAA_ApprovedBy", asset.AFAA_ApprovedBy);
                p.Add("@AFAA_ApprovedOn", asset.AFAA_ApprovedOn);
                p.Add("@AFAA_ItemType", asset.AFAA_ItemType);
                p.Add("@AFAA_CustId", asset.AFAA_CustId);

                // OUTPUT parameters (SP returns values here)
                p.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await conn.ExecuteAsync("spAcc_FixedAssetAdditionDel",
                    p, transaction, commandType: CommandType.StoredProcedure);

                // Returned Auto-ID from SP  
                int savedAssetId = p.Get<int>("@iOper");

                // ------------------------------------------------------
                // 2️⃣ SAVE AUDIT LOG  
                // CONDITION ADDED:  @ALFO_MasterID = savedAssetId (@AFAA_ID)
                // ------------------------------------------------------
                var auditParams = new DynamicParameters();

                auditParams.Add("@ALFO_UserID", audit.UserId);
                auditParams.Add("@ALFO_Module", audit.Module);
                auditParams.Add("@ALFO_Form", audit.Form);
                auditParams.Add("@ALFO_Event", audit.Event);

                // 🔥 Condition applied here
                auditParams.Add("@ALFO_MasterID", savedAssetId);  // Same as @AFAA_ID

                auditParams.Add("@ALFO_MasterName", audit.MasterName);
                auditParams.Add("@ALFO_SubMasterID", audit.SubMasterID);
                auditParams.Add("@ALFO_SubMasterName", audit.SubMasterName);
                auditParams.Add("@ALFO_IPAddress", audit.IPAddress);
                auditParams.Add("@ALFO_CompID", audit.CompID);

                await conn.ExecuteAsync("spAudit_Log_Form_Operations",
                    auditParams, transaction, commandType: CommandType.StoredProcedure);

                // ------------------------------------------------------
                // FINAL COMMIT
                // ------------------------------------------------------
                transaction.Commit();
                return savedAssetId;   // Return the final ID
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //LoadVoucherNo(transactionno)
        public async Task<IEnumerable<AssetTransactionDto>> ExistingTransactionNoAsync(int compId, int yearId, int custId)
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
        SELECT 
            AFAA_ID  AS AssetId,
            AFAA_AssetNo AS AssetNo
        FROM Acc_FixedAssetAdditionDel
        WHERE AFAA_CompID = @CompId
             ";

            if (custId != 0)
            {
                query += " AND AFAA_CustId = @CustId ";
            }

            query += " ORDER BY AFAA_ID ASC";

            return await connection.QueryAsync<AssetTransactionDto>(query,new{CompId = compId, CustId = custId});
        }

        //ExcelUpload
        public AssetAdditionResult GetAssetAdditionExcelTemplate()
        {
            var filePath = "C:\\Users\\Intel\\Desktop\\tracepa-dotnet-core - Copy\\SampleExcels\\AssetAdditionExcelUpload.xls";

            if (!File.Exists(filePath))
                return new AssetAdditionResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "AssetAdditionExcelUpload.xls";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new AssetAdditionResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //SaveDetails
        public async Task<(int UpdateOrSave, int Oper)> SaveFixedAssetAsync(ClsAssetOpeningBalExcelUpload header,ClsAssetTransactionAddition details,AuditLogDto audit)
        {
            // 1. Get database from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            string connectionString = _configuration.GetConnectionString(dbName);

            using var con = new SqlConnection(connectionString);
            await con.OpenAsync();

            using var tran = con.BeginTransaction();

            try
            {
                  // 1. SAVE / UPDATE ASSET HEADER (MANDATORY)

                var headerParams = new DynamicParameters();

                headerParams.Add("@AFAA_ID", header.AFAA_ID);
                headerParams.Add("@AFAA_AssetTrType", 0);
                headerParams.Add("@AFAA_CurrencyType", 0);
                headerParams.Add("@AFAA_CurrencyAmnt", 0);
                headerParams.Add("@AFAA_Location", header.AFAA_Location);
                headerParams.Add("@AFAA_Division", header.AFAA_Division);
                headerParams.Add("@AFAA_Department", header.AFAA_Department);
                headerParams.Add("@AFAA_Bay", header.AFAA_Bay);
                headerParams.Add("@AFAA_ActualLocn", "");
                headerParams.Add("@AFAA_SupplierName", 0);
                headerParams.Add("@AFAA_SupplierCode", 0);
                headerParams.Add("@AFAA_TrType", header.AFAA_TrType);   // 🔑 CONDITION DRIVER
                headerParams.Add("@AFAA_AssetType", header.AFAA_AssetType);
                headerParams.Add("@AFAA_AssetNo", header.AFAA_AssetNo);
                headerParams.Add("@AFAA_AssetRefNo", "");
                headerParams.Add("@AFAA_Description", "");
                headerParams.Add("@AFAA_ItemCode", header.AFAA_ItemType);
                headerParams.Add("@AFAA_ItemDescription", "");
                headerParams.Add("@AFAA_Quantity", 0);
                headerParams.Add("@AFAA_CommissionDate", new DateTime(1900, 1, 1));
                headerParams.Add("@AFAA_PurchaseDate", header.AFAA_PurchaseDate);
                headerParams.Add("@AFAA_AssetAge", 0);
                headerParams.Add("@AFAA_AssetAmount", header.AFAA_AssetAmount);
                headerParams.Add("@AFAA_FYAmount", header.AFAA_FYAmount);
                headerParams.Add("@AFAA_DepreAmount", header.AFAA_DepreAmount);
                headerParams.Add("@AFAA_AssetDelID", 0);
                headerParams.Add("@AFAA_AssetDelDate", new DateTime(1900, 1, 1));
                headerParams.Add("@AFAA_AssetDeletionDate", new DateTime(1900, 1, 1));
                headerParams.Add("@AFAA_Assetvalue", header.AFAA_Assetvalue);
                headerParams.Add("@AFAA_AssetDesc", "");
                headerParams.Add("@AFAA_CreatedBy", header.AFAA_CreatedBy);
                headerParams.Add("@AFAA_CreatedOn", header.AFAA_CreatedOn);
                headerParams.Add("@AFAA_UpdatedBy", header.AFAA_UpdatedBy);
                headerParams.Add("@AFAA_UpdatedOn", header.AFAA_UpdatedOn);
                headerParams.Add("@AFAA_Status", header.AFAA_Status);
                headerParams.Add("@AFAA_Delflag", header.AFAA_Delflag);
                headerParams.Add("@AFAA_YearID", header.AFAA_YearID);
                headerParams.Add("@AFAA_CompID", header.AFAA_CompID);
                headerParams.Add("@AFAA_Operation", header.AFAA_Operation);
                headerParams.Add("@AFAA_IPAddress", header.AFAA_IPAddress);
                headerParams.Add("@AFAA_AddnType", "");
                headerParams.Add("@AFAA_DelnType", "");
                headerParams.Add("@AFAA_Depreciation", 0);
                headerParams.Add("@AFAA_AddtnDate", new DateTime(1900, 1, 1));
                headerParams.Add("@AFAA_ApprovedBy", header.AFAA_ApprovedBy);
                headerParams.Add("@AFAA_ApprovedOn", header.AFAA_ApprovedOn);
                headerParams.Add("@AFAA_ItemType", header.AFAA_ItemType);
                headerParams.Add("@AFAA_CustId", header.AFAA_CustId);

                headerParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                headerParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await con.ExecuteAsync(
                    "spAcc_FixedAssetAdditionDel",
                    headerParams,
                    tran,
                    commandType: CommandType.StoredProcedure
                );

                int headerUpdateOrSave = headerParams.Get<int>("@iUpdateOrSave");
                int headerOper = headerParams.Get<int>("@iOper");

                  // 2. SAVE ASSET DETAILS (ONLY IF TrType = 2)
               
                int detailUpdateOrSave = 0;
                int detailOper = 0;

                if (header.AFAA_TrType == 2)
                {
                    if (details.FAAD_MasID == 0)
                        details.FAAD_MasID = headerOper;

                    var detailParams = new DynamicParameters();

                    detailParams.Add("@FAAD_PKID", details.FAAD_PKID);
                    detailParams.Add("@FAAD_MasID", details.FAAD_MasID);
                    detailParams.Add("@FAAD_Location", details.FAAD_Location);
                    detailParams.Add("@FAAD_Division", details.FAAD_Division);
                    detailParams.Add("@FAAD_Department", details.FAAD_Department);
                    detailParams.Add("@FAAD_Bay", details.FAAD_Bay);
                    detailParams.Add("@FAAD_Particulars", details.FAAD_Particulars);
                    detailParams.Add("@FAAD_DocNo", details.FAAD_DocNo);
                    detailParams.Add("@FAAD_DocDate", details.FAAD_DocDate);
                    detailParams.Add("@FAAD_chkCost", details.FAAD_chkCost);
                    detailParams.Add("@FAAD_BasicCost", details.FAAD_BasicCost);
                    detailParams.Add("@FAAD_TaxAmount", details.FAAD_TaxAmount);
                    detailParams.Add("@FAAD_Total", details.FAAD_Total);
                    detailParams.Add("@FAAD_AssetValue", details.FAAD_AssetValue);
                    detailParams.Add("@FAAD_CreatedBy", details.FAAD_CreatedBy);
                    detailParams.Add("@FAAD_CreatedOn", details.FAAD_CreatedOn);
                    detailParams.Add("@FAAD_UpdatedBy", details.FAAD_UpdatedBy);
                    detailParams.Add("@FAAD_UpdatedOn", details.FAAD_UpdatedOn);
                    detailParams.Add("@FAAD_IPAddress", details.FAAD_IPAddress);
                    detailParams.Add("@FAAD_CompID", details.FAAD_CompID);
                    detailParams.Add("@FAAD_Status", details.FAAD_Status);
                    detailParams.Add("@FAAD_AssetType", details.FAAD_AssetType);
                    detailParams.Add("@FAAD_ItemType", details.FAAD_ItemType);
                    detailParams.Add("@FAAD_SupplierName", details.FAAD_SupplierName);
                    detailParams.Add("@FAAD_CustId", details.FAAD_CustId);
                    detailParams.Add("@FAAD_Delflag", details.sFAAD_Delflag);
                    detailParams.Add("@FAAD_YearID", details.FAAD_YearID);
                    detailParams.Add("@FAAD_InitDep", details.iFAAD_InitDep);
                    detailParams.Add("@FAAD_OtherTrType", details.iFAAD_OtherTrType);
                    detailParams.Add("@FAAD_OtherTrAmount", details.sFAAD_OtherTrAmount);

                    detailParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    detailParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await con.ExecuteAsync(
                        "spAcc_FixedAssetAdditionDetails",
                        detailParams,
                        tran,
                        commandType: CommandType.StoredProcedure
                    );

                    detailUpdateOrSave = detailParams.Get<int>("@iUpdateOrSave");
                    detailOper = detailParams.Get<int>("@iOper");
                }

                  // 3. AUDIT LOG (ALWAYS)

                var auditParams = new DynamicParameters();

                auditParams.Add("@ALFO_UserID", audit.ALFO_UserID);
                auditParams.Add("@ALFO_Module", audit.ALFO_Module);
                auditParams.Add("@ALFO_Form", audit.ALFO_Form);
                auditParams.Add("@ALFO_Event", audit.ALFO_Event);
                auditParams.Add("@ALFO_MasterID", headerOper);
                auditParams.Add("@ALFO_MasterName", audit.ALFO_MasterName);
                auditParams.Add("@ALFO_SubMasterID", audit.ALFO_SubMasterID);
                auditParams.Add("@ALFO_SubMasterName", audit.ALFO_SubMasterName);
                auditParams.Add("@ALFO_IPAddress", audit.ALFO_IPAddress);
                auditParams.Add("@ALFO_CompID", audit.ALFO_CompID);

                await con.ExecuteAsync(
                    "spAudit_Log_Form_Operations",
                    auditParams,
                    tran,
                    commandType: CommandType.StoredProcedure
                );

                tran.Commit();

                return (detailUpdateOrSave == 0 ? headerUpdateOrSave : detailUpdateOrSave,
                        detailOper == 0 ? headerOper : detailOper);
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }



    }

}












