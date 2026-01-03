using System.Data;
using System.Globalization;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetTransactionDeletionnDto;


namespace TracePca.Service.FixedAssetsService
{
    public class AssetTransactionDeletionService : AssetTransactionDeletionInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public AssetTransactionDeletionService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //Deletee
        public async Task<(int UpdateOrSave, int Oper)> SaveFixedAssetDeletionnAsync(
    AssetDeletionDto dto, AaudittDto audit)
        {
            // 1️⃣ SESSION / DB VALIDATION
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // 2️⃣ MANDATORY BUSINESS CHECKS
            if (dto.AFAD_CustomerName <= 0)
                throw new Exception("Customer is required.");
            if (dto.AFAD_AssetClass <= 0)
                throw new Exception("Asset Class is required.");
            if (dto.AFAD_Asset <= 0)
                throw new Exception("Asset is required.");
            if (dto.AFAD_AssetDeletion <= 0)
                throw new Exception("Asset Deletion Type is required.");
            if (dto.AFAD_CompID <= 0 || dto.AFAD_YearID <= 0)
                throw new Exception("Company and Financial Year are required.");

            // 3️⃣ SAVE vs UPDATE
            dto.AFAD_ID = dto.AFAD_ID > 0 ? dto.AFAD_ID : 0;

            // 4️⃣ DEFAULT STATUS FLAGS
            dto.AFAD_Status ??= "C";   // Created
            dto.AFAD_Delflag ??= "W";  // Waiting

            // 5️⃣ DATE SAFETY
            DateTime defaultDate = new DateTime(1900, 1, 1);
            dto.AFAD_DeletionDate ??= defaultDate;
            dto.AFAD_DateofInitiate ??= defaultDate;
            dto.AFAD_DateofReceived ??= defaultDate;
            dto.AFAD_InsClaimedDate ??= defaultDate;
            dto.AFAD_InsRefDate ??= defaultDate;
            dto.AFAD_CreatedOn ??= DateTime.Now;
            dto.AFAD_ApprovedOn ??= DateTime.Now;
            dto.AFAD_DeletedOn ??= DateTime.Now;

            // 6️⃣ NUMERIC SAFETY
            dto.AFAD_Amount ??= 0;
            dto.AFAD_Quantity ??= 0;
            dto.AFAD_CostofTransport ??= 0;
            dto.AFAD_InstallationCost ??= 0;
            dto.AFAD_SalesPrice ??= 0;
            dto.AFAD_DelDeprec ??= 0;
            dto.AFAD_WDVValue ??= 0;
            dto.AFAD_PorLAmount ??= 0;
            dto.AFAD_ContAssetValue ??= 0;
            dto.AFAD_ContDep ??= 0;
            dto.AFAD_ContWDV ??= 0;
            dto.AFAD_InsAmtClaimed ??= 0;
            dto.AFAD_InsAmtRecvd ??= 0;

            // 7️⃣ STRING LENGTH SAFETY (SQL Truncation Prevention)
            dto.AFAD_TransNo = dto.AFAD_TransNo?.Substring(0, Math.Min(dto.AFAD_TransNo.Length, 500));
            dto.AFAD_AssetDelDesc = dto.AFAD_AssetDelDesc?.Substring(0, Math.Min(dto.AFAD_AssetDelDesc.Length, 500));
            dto.AFAD_PorLStatus = dto.AFAD_PorLStatus?.Substring(0, Math.Min(dto.AFAD_PorLStatus.Length, 50));
            dto.AFAD_InsClaimedNo = dto.AFAD_InsClaimedNo?.Substring(0, Math.Min(dto.AFAD_InsClaimedNo.Length, 500));
            dto.AFAD_InsRefNo = dto.AFAD_InsRefNo?.Substring(0, Math.Min(dto.AFAD_InsRefNo.Length, 500));
            dto.AFAD_Remarks = dto.AFAD_Remarks?.Substring(0, Math.Min(dto.AFAD_Remarks.Length, 500));
            dto.AFAD_Status = dto.AFAD_Status?.Substring(0, Math.Min(dto.AFAD_Status.Length, 25));
            dto.AFAD_Delflag = dto.AFAD_Delflag?.Substring(0, 1);
            dto.AFAD_IPAddress = dto.AFAD_IPAddress?.Substring(0, Math.Min(dto.AFAD_IPAddress.Length, 100));

            // 8️⃣ DATABASE EXECUTION
            string connectionString = _configuration.GetConnectionString(dbName);

            using var con = new SqlConnection(connectionString);
            await con.OpenAsync();
            using var tran = con.BeginTransaction();

            try
            {
                var p = new DynamicParameters();

                #region Fixed Asset Deletion Params
                p.Add("@AFAD_ID", dto.AFAD_ID);
                p.Add("@AFAD_CustomerName", dto.AFAD_CustomerName);
                p.Add("@AFAD_TransNo", dto.AFAD_TransNo);
                p.Add("@AFAD_Location", dto.AFAD_Location);
                p.Add("@AFAD_Division", dto.AFAD_Division);
                p.Add("@AFAD_Department", dto.AFAD_Department);
                p.Add("@AFAD_Bay", dto.AFAD_Bay);
                p.Add("@AFAD_AssetClass", dto.AFAD_AssetClass);
                p.Add("@AFAD_Asset", dto.AFAD_Asset);
                p.Add("@AFAD_AssetDeletion", dto.AFAD_AssetDeletion);
                p.Add("@AFAD_AssetDeletionType", dto.AFAD_AssetDeletionType);
                p.Add("@AFAD_DeletionDate", dto.AFAD_DeletionDate);
                p.Add("@AFAD_Amount", dto.AFAD_Amount);
                p.Add("@AFAD_Quantity", dto.AFAD_Quantity);
                p.Add("@AFAD_Paymenttype", dto.AFAD_Paymenttype);
                p.Add("@AFAD_CostofTransport", dto.AFAD_CostofTransport);
                p.Add("@AFAD_InstallationCost", dto.AFAD_InstallationCost);
                p.Add("@AFAD_DateofInitiate", dto.AFAD_DateofInitiate);
                p.Add("@AFAD_DateofReceived", dto.AFAD_DateofReceived);
                p.Add("@AFAD_ToLocation", dto.AFAD_ToLocation);
                p.Add("@AFAD_ToDivision", dto.AFAD_ToDivision);
                p.Add("@AFAD_ToDepartment", dto.AFAD_ToDepartment);
                p.Add("@AFAD_ToBay", dto.AFAD_ToBay);
                p.Add("@AFAD_AssetDelDesc", dto.AFAD_AssetDelDesc);
                p.Add("@AFAD_PorLStatus", dto.AFAD_PorLStatus);
                p.Add("@AFAD_PorLAmount", dto.AFAD_PorLAmount);
                p.Add("@AFAD_SalesPrice", dto.AFAD_SalesPrice);
                p.Add("@AFAD_DelDeprec", dto.AFAD_DelDeprec);
                p.Add("@AFAD_WDVValue", dto.AFAD_WDVValue);
                p.Add("@AFAD_ContAssetValue", dto.AFAD_ContAssetValue);
                p.Add("@AFAD_ContDep", dto.AFAD_ContDep);
                p.Add("@AFAD_ContWDV", dto.AFAD_ContWDV);
                p.Add("@AFAD_InsClaimedNo", dto.AFAD_InsClaimedNo);
                p.Add("@AFAD_InsAmtClaimed", dto.AFAD_InsAmtClaimed);
                p.Add("@AFAD_InsClaimedDate", dto.AFAD_InsClaimedDate);
                p.Add("@AFAD_InsAmtRecvd", dto.AFAD_InsAmtRecvd);
                p.Add("@AFAD_InsRefNo", dto.AFAD_InsRefNo);
                p.Add("@AFAD_InsRefDate", dto.AFAD_InsRefDate);
                p.Add("@AFAD_Remarks", dto.AFAD_Remarks);
                p.Add("@AFAD_CreatedBy", dto.AFAD_CreatedBy);
                p.Add("@AFAD_CreatedOn", dto.AFAD_CreatedOn);
                p.Add("@AFAD_ApprovedBy", dto.AFAD_ApprovedBy);
                p.Add("@AFAD_ApprovedOn", dto.AFAD_ApprovedOn);
                p.Add("@AFAD_Status", dto.AFAD_Status);
                p.Add("@AFAD_Delflag", dto.AFAD_Delflag);
                p.Add("@AFAD_YearID", dto.AFAD_YearID);
                p.Add("@AFAD_CompID", dto.AFAD_CompID);
                p.Add("@AFAD_Deletedby", dto.AFAD_Deletedby);
                p.Add("@AFAD_DeletedOn", dto.AFAD_DeletedOn);
                p.Add("@AFAD_IPAddress", dto.AFAD_IPAddress);

                p.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);
                #endregion

                await con.ExecuteAsync("spAcc_FixedAssetDeletion", p, tran, commandType: CommandType.StoredProcedure);

                int updateOrSave = p.Get<int>("@iUpdateOrSave");
                int oper = p.Get<int>("@iOper");

                // 9️⃣ AUDIT LOG (Only on success)
                if (updateOrSave == 2 || updateOrSave == 3)
                {
                    var auditParams = new DynamicParameters();
                    auditParams.Add("@ALFO_UserID", audit.ALFO_UserID);
                    auditParams.Add("@ALFO_Module", audit.ALFO_Module);
                    auditParams.Add("@ALFO_Form", audit.ALFO_Form);
                    auditParams.Add("@ALFO_Event", audit.ALFO_Event);
                    auditParams.Add("@ALFO_MasterID", oper);
                    auditParams.Add("@ALFO_MasterName", audit.ALFO_MasterName);
                    auditParams.Add("@ALFO_SubMasterID", audit.ALFO_SubMasterID);
                    auditParams.Add("@ALFO_SubMasterName", audit.ALFO_SubMasterName);
                    auditParams.Add("@ALFO_IPAddress", audit.ALFO_IPAddress);
                    auditParams.Add("@ALFO_CompID", audit.ALFO_CompID);

                    await con.ExecuteAsync("spAudit_Log_Form_Operations", auditParams, tran, commandType: CommandType.StoredProcedure);
                }

                tran.Commit();
                return (updateOrSave, oper);
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }


    }
}




