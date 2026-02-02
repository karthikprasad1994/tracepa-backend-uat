using System.Data;
using System.Drawing;
using System.Globalization;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Ocsp;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.DepreciationComputationDto;
using static TracePca.Service.FixedAssetsService.DepreciationComputationService;



namespace TracePca.Service.FixedAssetsService
{
    public class DepreciationComputationService : DepreciationComputationInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        //Depreciation
        public DepreciationComputationService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //SaveDepreciation

        public async Task<bool> SaveDepreciationAsync(
     int depBasis,
     int yearId,
     int compId,
     int custId,
     int method,
     int userId,
     string ipAddress,
     List<DepreciationComputationnDto> normalList,
     List<DepreciationITActDto> itActList,
     AuditLoggDto audit)
        {
            // 1️⃣ SESSION / DB VALIDATION
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2️⃣ CONNECTION STRING
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                int masterId = 0; // ⭐ WILL HOLD MAIN TABLE ID

                // ================= NORMAL DEPRECIATION =================
                if (depBasis == 1 && normalList != null && normalList.Count > 0)
                {
                    foreach (var item in normalList)
                    {
                        var param = new DynamicParameters();

                        param.Add("@ADep_ID", item.ADep_ID);
                        param.Add("@ADep_AssetID", item.AssetId);
                        param.Add("@ADep_Item", item.Item);
                        param.Add("@ADep_RateofDep", item.RateOfDep);
                        param.Add("@ADep_OPBForYR", item.OpeningBalance);
                        param.Add("@ADep_DepreciationforFY", item.DepreciationForFY);
                        param.Add("@ADep_WrittenDownValue", item.WDV);
                        param.Add("@ADep_ClosingDate", DateTime.Today);

                        param.Add("@ADep_CreatedBy", userId);
                        param.Add("@ADep_CreatedOn", DateTime.Now);
                        param.Add("@ADep_UpdatedBy", userId);
                        param.Add("@ADep_UpdatedOn", DateTime.Now);
                        param.Add("@ADep_ApprovedBy", userId);
                        param.Add("@ADep_ApprovedOn", DateTime.Now);

                        param.Add("@ADep_DelFlag", item.DelFlag);
                        param.Add("@ADep_Status", item.Status);
                        param.Add("@ADep_YearID", yearId);
                        param.Add("@ADep_CompID", compId);
                        param.Add("@ADep_CustId", custId);

                        param.Add("@ADep_Location", item.LocationId);
                        param.Add("@ADep_Division", item.DivisionId);
                        param.Add("@ADep_Department", item.DepartmentId);
                        param.Add("@ADep_Bay", item.BayId);
                        param.Add("@ADep_TransType", item.TransType);
                        param.Add("@ADep_Method", method);

                        param.Add("@ADep_Opeartion", item.Operation);
                        param.Add("@ADep_IPAddress", ipAddress);

                        param.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        param.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync(
                            "spAcc_AsetDepreciation",
                            param,
                            transaction,
                            commandType: CommandType.StoredProcedure);

                        // ⭐ CAPTURE MAIN TABLE ID
                        masterId = param.Get<int>("@iOper");
                    }
                }

                // ================= IT ACT DEPRECIATION =================
                if (depBasis == 2 && itActList != null && itActList.Count > 0)
                {
                    foreach (var item in itActList)
                    {
                        var param = new DynamicParameters();

                        param.Add("@ADITAct_ID", item.Id);
                        param.Add("@ADITAct_AssetClassID", item.AssetClassId);
                        param.Add("@ADITAct_RateofDep", item.Rate);
                        param.Add("@ADITAct_OPBForYR", item.OpeningBalance);
                        param.Add("@ADITAct_DepreciationforFY", item.Depreciation);
                        param.Add("@ADITAct_WrittenDownValue", item.WDV);

                        param.Add("@ADITAct_BfrQtrAmount", item.BeforeQuarterAmount);
                        param.Add("@ADITAct_BfrQtrDep", item.BeforeQuarterDep);
                        param.Add("@ADITAct_AftQtrAmount", item.AfterQuarterAmount);
                        param.Add("@ADITAct_AftQtrDep", item.AfterQuarterDep);
                        param.Add("@ADITAct_DelAmount", item.DeletionAmount);


                        param.Add("@ADITAct_CreatedBy", userId);
                        param.Add("@ADITAct_CreatedOn", DateTime.Now);
                        param.Add("@ADITAct_UpdatedBy", userId);
                        param.Add("@ADITAct_UpdatedOn", DateTime.Now);
                        param.Add("@ADITAct_ApprovedBy", userId);
                        param.Add("@ADITAct_ApprovedOn", DateTime.Now);

                        param.Add("@ADITAct_DelFlag", item.DelFlag);
                        param.Add("@ADITAct_Status", item.Status);
                        param.Add("@ADITAct_YearID", yearId);
                        param.Add("@ADITAct_CompID", compId);
                        param.Add("@ADITAct_CustId", custId);

                        param.Add("@ADITAct_Opeartion", item.Operation);
                        param.Add("@ADITAct_IPAddress", ipAddress);

                        param.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        param.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync(
                            "spAcc_AssetDepITAct",
                            param,
                            transaction,
                            commandType: CommandType.StoredProcedure);

                        // ⭐ CAPTURE MAIN TABLE ID
                        masterId = param.Get<int>("@iOper");
                    }
                }

                // ================= AUDIT LOG =================
                var auditParam = new DynamicParameters();
                auditParam.Add("@ALFO_UserID", audit.UserId);
                auditParam.Add("@ALFO_Module", audit.Module);
                auditParam.Add("@ALFO_Form", audit.Form);
                auditParam.Add("@ALFO_Event", audit.Event);

                // ⭐ FIX: USE MAIN TABLE ID
                auditParam.Add("@ALFO_MasterID", masterId);

                auditParam.Add("@ALFO_MasterName", audit.MasterName);
                auditParam.Add("@ALFO_SubMasterID", audit.SubMasterId);
                auditParam.Add("@ALFO_SubMasterName", audit.SubMasterName);
                auditParam.Add("@ALFO_IPAddress", audit.IPAddress);
                auditParam.Add("@ALFO_CompID", compId);

                await connection.ExecuteAsync(
                    "spAudit_Log_Form_Operations",
                    auditParam,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //Report(DownloadExcel)
        public AssetDepreciationResult GetAssetDepreciationExcelTemplate()
        {
            var filePath = "C:\\Users\\Intel\\Desktop\\tracepa-dotnet-core - Copy\\SampleExcels\\AssetDepreciation.xls";

            if (!File.Exists(filePath))
                return new AssetDepreciationResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "AssetDepreciation.xls";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new AssetDepreciationResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //Go

        //public DepreciationResultDto CalculateDepreciation(
        //    string accessCode,
        //    int compId,
        //    int yearId,
        //    int custId,
        //    int depBasis,
        //    int method,
        //    int durationType,
        //    int halfYear,
        //    int quarter,
        //    int month,
        //    string financialYear,
        //    DateTime? fromDate,
        //    DateTime? toDate
        //)
        //{
        //    #region VALIDATION (FROM ORIGINAL CODE)

        //    if (custId == 0)
        //        throw new Exception("Select Customer Name");

        //    if (depBasis == 0)
        //        throw new Exception("Select Depreciation Basis");

        //    if (method == 0 && depBasis == 1)
        //        throw new Exception("Select Method of Depreciation");

        //    #endregion

        //    #region FINANCIAL YEAR CALCULATION

        //    var fy = financialYear.Split('-');
        //    int startYear = Convert.ToInt32(fy[0]);
        //    int endYear = Convert.ToInt32(fy[1]);

        //    DateTime fyStart = new DateTime(startYear, 4, 1);
        //    DateTime fyEnd = new DateTime(endYear, 3, 31);
        //    int yearlyDays = (fyEnd - fyStart).Days + 1;

        //    #endregion

        //    DateTime periodFrom = fyStart;
        //    DateTime periodTo = fyEnd;
        //    int noOfDays = yearlyDays;

        //    #region DURATION LOGIC (AS-IS)

        //    if (durationType == 1) // Half-Yearly
        //    {
        //        if (halfYear == 1)
        //        {
        //            periodFrom = new DateTime(startYear, 4, 1);
        //            periodTo = new DateTime(startYear, 9, 30);
        //        }
        //        else
        //        {
        //            periodFrom = new DateTime(startYear, 10, 1);
        //            periodTo = new DateTime(endYear, 3, 31);
        //        }
        //        noOfDays = (periodTo - periodFrom).Days;
        //    }
        //    else if (durationType == 2) // Quarterly
        //    {
        //        if (quarter == 1)
        //        {
        //            periodFrom = new DateTime(startYear, 4, 1);
        //            periodTo = new DateTime(startYear, 6, 30);
        //        }
        //        else if (quarter == 2)
        //        {
        //            periodFrom = new DateTime(startYear, 7, 1);
        //            periodTo = new DateTime(startYear, 9, 30);
        //        }
        //        else if (quarter == 3)
        //        {
        //            periodFrom = new DateTime(startYear, 10, 1);
        //            periodTo = new DateTime(startYear, 12, 31);
        //        }
        //        else
        //        {
        //            periodFrom = new DateTime(endYear, 1, 1);
        //            periodTo = new DateTime(endYear, 3, 31);
        //        }
        //        noOfDays = (periodTo - periodFrom).Days;
        //    }
        //    else if (durationType == 3) // Monthly
        //    {
        //        int year = month <= 3 ? endYear : startYear;
        //        periodFrom = new DateTime(year, month, 1);
        //        periodTo = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        //        noOfDays = (periodTo - periodFrom).Days + 1;
        //    }
        //    else if (durationType == 4) // Customized
        //    {
        //        periodFrom = fromDate.Value;
        //        periodTo = toDate.Value;
        //        noOfDays = (periodTo - periodFrom).Days;
        //    }

        //    #endregion

        //    DataTable compTable = new();
        //    DataTable itActTable = new();

        //    #region COMPANY BOOKS

        //    if (depBasis == 1)
        //    {
        //        if (method == 1)
        //        {
        //            compTable = LoadDepreciationCompSLM(
        //                accessCode, compId, yearId, custId,
        //                noOfDays, yearlyDays, durationType,
        //                periodFrom, periodTo);
        //        }
        //        else if (method == 2)
        //        {
        //            compTable = LoadDepreciationCompWDV(
        //                accessCode, compId, yearId, custId,
        //                noOfDays, yearlyDays, durationType,
        //                periodFrom, periodTo);
        //        }
        //    }

        //    #endregion

        //    #region IT ACT

        //    if (depBasis == 2)
        //    {
        //        itActTable = LoadDepreciationITAct(
        //            accessCode, compId, yearId, custId, fyEnd);
        //    }

        //    #endregion

        //    #region COUNTS

        //    int openingCount = GetCount(accessCode, compId, yearId, custId, 1);
        //    int additionCount = GetCount(accessCode, compId, yearId, custId, 2);

        //    #endregion

        //    return new DepreciationResultDto
        //    {
        //        CompanyBookData = compTable,
        //        ITActData = itActTable,
        //        OpeningBalanceCount = openingCount,
        //        AdditionCount = additionCount
        //    };
        //}

        //#region DAPPER FUNCTIONS

        //private DataTable LoadDepreciationCompSLM(
        //    string accessCode, int compId, int yearId, int custId,
        //    int noOfDays, int yearlyDays, int durationType,
        //    DateTime fromDate, DateTime toDate)
        //{
        //    using var reader = _connection.ExecuteReader(
        //        "sp_Depreciation_Comp_SLM",
        //        new
        //        {
        //            AccessCode = accessCode,
        //            CompID = compId,
        //            YearID = yearId,
        //            CustID = custId,
        //            NoOfDays = noOfDays,
        //            YearlyDays = yearlyDays,
        //            Duration = durationType,
        //            FromDate = fromDate,
        //            ToDate = toDate
        //        },
        //        commandType: CommandType.StoredProcedure);

        //    DataTable dt = new();
        //    dt.Load(reader);
        //    return dt;
        //}

        //private DataTable LoadDepreciationCompWDV(
        //    string accessCode, int compId, int yearId, int custId,
        //    int noOfDays, int yearlyDays, int durationType,
        //    DateTime fromDate, DateTime toDate)
        //{
        //    using var reader = _connection.ExecuteReader(
        //        "sp_Depreciation_Comp_WDV",
        //        new
        //        {
        //            AccessCode = accessCode,
        //            CompID = compId,
        //            YearID = yearId,
        //            CustID = custId,
        //            NoOfDays = noOfDays,
        //            YearlyDays = yearlyDays,
        //            Duration = durationType,
        //            FromDate = fromDate,
        //            ToDate = toDate
        //        },
        //        commandType: CommandType.StoredProcedure);

        //    DataTable dt = new();
        //    dt.Load(reader);
        //    return dt;
        //}

        //private DataTable LoadDepreciationITAct(
        //    string accessCode, int compId, int yearId, int custId, DateTime fyEnd)
        //{
        //    using var reader = _connection.ExecuteReader(
        //        "sp_Depreciation_ITAct",
        //        new
        //        {
        //            AccessCode = accessCode,
        //            CompID = compId,
        //            YearID = yearId,
        //            CustID = custId,
        //            FYEndDate = fyEnd
        //        },
        //        commandType: CommandType.StoredProcedure);

        //    DataTable dt = new();
        //    dt.Load(reader);
        //    return dt;
        //}

        //private int GetCount(
        //    string accessCode, int compId, int yearId, int custId, int type)
        //{
        //    return _connection.ExecuteScalar<int>(
        //        "sp_Get_Depreciation_Count",
        //        new
        //        {
        //            AccessCode = accessCode,
        //            CompID = compId,
        //            YearID = yearId,
        //            CustID = custId,
        //            Type = type
        //        },
        //        commandType: CommandType.StoredProcedure);
        //}

        //  #endregion


        //-----------
        //itcorrect
        //        public async Task<List<DepreciationnITActDto>> LoadDepreciationITActAsync(
        //            int compId, int yearId, int custId, DateTime endDate)
        //        {
        //            var result = new List<DepreciationnITActDto>();
        //            int previousYearId = yearId != 0 ? yearId - 1 : 0;

        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrWhiteSpace(dbName))
        //                throw new Exception("CustomerCode is missing in session.");

        //            string connectionString = _configuration.GetConnectionString(dbName);

        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            using var transaction = connection.BeginTransaction();

        //            try
        //            {
        //                // 1️⃣ Check if previous year data exists
        //                string countSql = @"SELECT COUNT(ADITAct_ID) 
        //                            FROM Acc_AssetDepITAct 
        //                            WHERE ADITAct_CustId=@CustId 
        //                            AND ADITAct_YearID=@PrevYearId 
        //                            AND ADITAct_CompID=@CompId";
        //                int count = await connection.ExecuteScalarAsync<int>(
        //                    countSql, new { CustId = custId, PrevYearId = previousYearId, CompId = compId }, transaction);

        //                // 2️⃣ Get Asset Master
        //                string assetSql = @"SELECT AM_ID, AM_Description, AM_WDVITAct, AM_ITRate 
        //                            FROM Acc_AssetMaster 
        //                            WHERE AM_CompID=@CompId AND AM_LevelCode=2 AND AM_CustId=@CustId";
        //                var assets = (await connection.QueryAsync<AssetMasterDto>(
        //                    assetSql, new { CompId = compId, CustId = custId }, transaction)).AsList();

        //                foreach (var asset in assets)
        //                {
        //                    var dto = new DepreciationnITActDto
        //                    {
        //                        AssetClassID = asset.AM_ID,
        //                        ClassofAsset = asset.AM_Description,
        //                        RateofDep = asset.AM_ITRate
        //                    };

        //                    // 3️⃣ WDV Opening
        //                    double openingValue = asset.AM_WDVITAct;
        //                    if (count > 0)
        //                    {
        //                        string prevWdvSql = @"SELECT ISNULL(SUM(ADITAct_WrittenDownValue),0) 
        //                                      FROM Acc_AssetDepITAct 
        //                                      WHERE ADITAct_AssetClassID=@AssetId AND ADITAct_CustId=@CustId 
        //                                      AND ADITAct_YearID=@PrevYearId AND ADITAct_CompID=@CompId";
        //                        openingValue = await connection.ExecuteScalarAsync<double>(
        //                            prevWdvSql, new { AssetId = asset.AM_ID, CustId = custId, PrevYearId = previousYearId, CompId = compId }, transaction);
        //                        if (openingValue == 0) openingValue = asset.AM_WDVITAct;
        //                    }

        //                    // 4️⃣ Deletion Amount
        //                    string delSql = @"SELECT ISNULL(SUM(AFAD_SalesPrice),0) 
        //                              FROM Acc_FixedAssetDeletion 
        //                              WHERE AFAD_AssetClass=@AssetId AND AFAD_CompID=@CompId 
        //                              AND AFAD_CustomerName=@CustId AND AFAD_YearID=@YearId";
        //                    double delAmount = await connection.ExecuteScalarAsync<double>(
        //                        delSql, new { AssetId = asset.AM_ID, CompId = compId, CustId = custId, YearId = yearId }, transaction);

        //                    dto.WDVOpeningValue = Math.Round(openingValue - delAmount, 0);
        //                    dto.DelAmount = Math.Round(delAmount, 0);

        //                    // 5️⃣ Previous Init Dep
        //                    //string prevInitDepSql = @"SELECT ISNULL(SUM(ADITAct_InitAmt),0) 
        //                    //                  FROM Acc_AssetDepITAct 
        //                    //                  WHERE ADITAct_AssetClassID=@AssetId AND ADITAct_CustId=@CustId 
        //                    //                  AND ADITAct_YearID=@PrevYearId AND ADITAct_CompID=@CompId";
        //                    //double prevInitDepAmt = await connection.ExecuteScalarAsync<double>(
        //                    //    prevInitDepSql, new { AssetId = asset.AM_ID, CustId = custId, PrevYearId = previousYearId, CompId = compId }, transaction);
        //                    //dto.PrevInitDepAmt = prevInitDepAmt;

        //                    double prevInitDepAmt = 0;


        //                    // 6️⃣ Depreciation for period
        //                    double depForPeriod = (dto.WDVOpeningValue * dto.RateofDep) / 100;
        //                    dto.WDVOpeningDepreciation = depForPeriod;

        //                    // 7️⃣ Asset Additions
        //                    string addSql = @"SELECT b.FAAD_ItemType, a.AFAM_CommissionDate, ISNULL(SUM(b.FAAD_AssetValue),0) AS FAAD_AssetValue, ISNULL(FAAD_InitDep,0) AS FAAD_InitDep
        //                              FROM Acc_FixedAssetMaster a
        //                              LEFT JOIN Acc_FixedAssetAdditionDetails b ON a.AFAM_ID=b.FAAD_ItemType
        //                              WHERE b.FAAD_ItemType<>'' AND FAAD_YearID=@YearId AND FAAD_Delflag<>'D' 
        //                              AND FAAD_CustId=@CustId AND FAAD_Status<>'D' AND FAAD_CompID=@CompId AND FAAD_AssetType=@AssetId
        //                              GROUP BY FAAD_ItemType, AFAM_CommissionDate, FAAD_InitDep";

        //                    var addDetails = (await connection.QueryAsync<FixedAssetAdditionDto>(
        //                        addSql, new { YearId = yearId, CustId = custId, CompId = compId, AssetId = asset.AM_ID }, transaction)).AsList();

        //                    double less180 = 0, more180 = 0, lTotalDep = 0, mTotalDep = 0, initDepTot = 0, nextYearCarry = 0;

        //                    foreach (var add in addDetails)
        //                    {
        //                        int noOfDays = (endDate - add.AFAM_CommissionDate).Days;
        //                        if (noOfDays <= 180)
        //                        {
        //                            less180 += add.FAAD_AssetValue;
        //                            if (add.FAAD_InitDep == 1)
        //                            {
        //                                double initDep = (add.FAAD_AssetValue * 10) / 100;
        //                                dto.InitDepAmt = initDep;
        //                                initDepTot += initDep;
        //                                nextYearCarry += initDep;
        //                            }
        //                            lTotalDep += (add.FAAD_AssetValue * (dto.RateofDep / 2)) / 100;
        //                        }
        //                        else
        //                        {
        //                            more180 += add.FAAD_AssetValue;
        //                            if (add.FAAD_InitDep == 1)
        //                            {
        //                                double initDep = (add.FAAD_AssetValue * 20) / 100;
        //                                dto.InitDepAmt = initDep;
        //                                initDepTot += initDep;
        //                            }
        //                            mTotalDep += (add.FAAD_AssetValue * dto.RateofDep) / 100;
        //                        }
        //                    }

        //                    dto.BfrQtrAmount = less180;
        //                    dto.BfrQtrDep = lTotalDep;
        //                    dto.AftQtrAmount = more180;
        //                    dto.AftQtrDep = mTotalDep;
        //                    dto.Depfortheperiod = Math.Round(depForPeriod + lTotalDep + mTotalDep + initDepTot + prevInitDepAmt, 0);
        //                    dto.AdditionDuringtheYear = Math.Round(less180 + more180, 0);
        //                    dto.WDVClosingValue = Math.Round(dto.WDVOpeningValue + less180 + more180 - dto.Depfortheperiod, 0);
        //                    dto.NextYrCarry = nextYearCarry;

        //                    result.Add(dto);
        //                }

        //                transaction.Commit();
        //                return result;
        //            }
        //            catch
        //            {
        //                transaction.Rollback();
        //                throw;
        //            }
        //        }


        //        //--------------------new it correct

        //        public async Task<DepreciationResultDto> CalculateDepreciationAsync(DepreciationRequesttDto request)
        //        {
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrWhiteSpace(dbName))
        //                throw new Exception("CustomerCode missing in session");

        //            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
        //            await connection.OpenAsync();

        //            if (request.CustId == 0)
        //                throw new ArgumentException("Select Customer Name");

        //            if (request.DepBasis != 1 && request.DepBasis != 2)
        //                throw new ArgumentException("Select valid Depreciation Basis");

        //            var fySplit = request.FinancialYear.Split('-');
        //            int startYear = int.Parse(fySplit[0]);
        //            int endYear = int.Parse(fySplit[1]);
        //            DateTime fyStart = new(startYear, 4, 1);
        //            DateTime fyEnd = new(endYear, 3, 31);

        //            var result = new DepreciationResultDto
        //            {
        //                ITActData = new List<ITActDepreciationDto>(),
        //                CompanyActData = new List<CompanyActDepreciationDto>()
        //            };

        //            // ========================== IT ACT ==========================
        //            if (request.DepBasis == 2)
        //            {
        //                result.DepreciationBasis = "IT_ACT";
        //                int prevYearId = request.YearId - 1;

        //                var assetClasses = await connection.QueryAsync(
        //                    @"SELECT AM_ID, AM_Description, AM_ITRate, AM_WDVITAct
        //                  FROM Acc_AssetMaster
        //                  WHERE AM_CompID = @CompId 
        //                    AND AM_LevelCode = 2 
        //                    AND AM_CustId = @CustId",
        //                    new { request.CompId, request.CustId });

        //                foreach (var asset in assetClasses)
        //                {
        //                    decimal openingWDV = await connection.ExecuteScalarAsync<decimal>(
        //                        @"SELECT ISNULL(SUM(ADITAct_WrittenDownValue),0)
        //                      FROM Acc_AssetDepITAct
        //                      WHERE ADITAct_AssetClassID = @ClassId
        //                        AND ADITAct_YearID = @PrevYear
        //                        AND ADITAct_CompID = @CompId
        //                        AND ADITAct_CustId = @CustId",
        //                        new { ClassId = asset.AM_ID, PrevYear = prevYearId, request.CompId, request.CustId });

        //                    if (openingWDV == 0)
        //                        openingWDV = asset.AM_WDVITAct ?? 0;

        //                    decimal before180 = await connection.ExecuteScalarAsync<decimal>(
        //                        @"SELECT ISNULL(SUM(FAAD_AssetValue),0)
        //                      FROM Acc_FixedAssetAdditionDetails
        //                      WHERE FAAD_YearID = @YearId
        //                        AND FAAD_AssetType = @ClassId
        //                        AND DATEDIFF(DAY, FAAD_DocDate, @FyEnd) > 180",
        //                        new { request.YearId, ClassId = asset.AM_ID, FyEnd = fyEnd });

        //                    decimal after180 = await connection.ExecuteScalarAsync<decimal>(
        //                        @"SELECT ISNULL(SUM(FAAD_AssetValue),0)
        //                      FROM Acc_FixedAssetAdditionDetails
        //                      WHERE FAAD_YearID = @YearId
        //                        AND FAAD_AssetType = @ClassId
        //                        AND DATEDIFF(DAY, FAAD_DocDate, @FyEnd) <= 180",
        //                        new { request.YearId, ClassId = asset.AM_ID, FyEnd = fyEnd });

        //                    decimal rate = asset.AM_ITRate ?? 0;
        //                    decimal depBefore = (openingWDV + before180) * rate / 100;
        //                    decimal depAfter = after180 * rate / 200;
        //                    decimal totalDep = depBefore + depAfter;
        //                    decimal closingWDV = openingWDV + before180 + after180 - totalDep;

        //                    result.ITActData.Add(new ITActDepreciationDto
        //                    {
        //                        AssetClassID = asset.AM_ID,
        //                        ClassOfAsset = asset.AM_Description,
        //                        RateOfDep = rate,
        //                        WDVOpeningValue = openingWDV,
        //                        Before180DaysAddition = before180,
        //                        After180DaysAddition = after180,
        //                        TotalAddition = before180 + after180,
        //                        DepBefore180Days = depBefore,
        //                        DepAfter180Days = depAfter,
        //                        TotalDepreciation = totalDep,
        //                        WDVClosingValue = closingWDV
        //                    });
        //                }
        //            }
        //            // ====================== COMPANY ACT =======================
        //            else if (request.DepBasis == 1)
        //            {
        //                result.DepreciationBasis = "COMPANY_ACT";

        //                // ✅ Use TRY_CAST for all varchar->int fields
        //                var assets = await connection.QueryAsync<CompanyActDepreciationDto>(
        //                    @"
        //       SELECT
        //    FA.AFAM_PurchaseDate AS DateOfPutToUse,
        //    FA.AFAM_AssetCode AS AssetCode,
        //    FA.AFAM_Description AS AssetName,
        //    FA.AFAM_PurchaseAmount AS OriginalCost,
        //    ISNULL(FA.AFAM_AssetAge,0) AS AssetLife,
        //    ISNULL(FA.AFAM_Value,0) AS ResidualPercent,
        //    LOC.LS_Description AS Location,
        //    DIVI.LS_Description AS Division,
        //    DEPT.LS_Description AS Department,
        //    BAY.LS_Description AS Bay
        //FROM Acc_FixedAssetMaster FA

        //LEFT JOIN Acc_AssetLocationSetup LOC
        //    ON LOC.LS_LevelCode = 1
        //   AND LOC.LS_CustId = FA.AFAM_CustId
        //   AND LOC.LS_CompID = FA.AFAM_CompID
        //   AND CAST(LOC.LS_Code AS VARCHAR(50)) = FA.AFAM_Location

        //LEFT JOIN Acc_AssetLocationSetup DIVI
        //    ON DIVI.LS_LevelCode = 2
        //   AND DIVI.LS_CustId = FA.AFAM_CustId
        //   AND DIVI.LS_CompID = FA.AFAM_CompID
        //   AND CAST(DIVI.LS_Code AS VARCHAR(50)) = FA.AFAM_Division

        //LEFT JOIN Acc_AssetLocationSetup DEPT
        //    ON DEPT.LS_LevelCode = 3
        //   AND DEPT.LS_CustId = FA.AFAM_CustId
        //   AND DEPT.LS_CompID = FA.AFAM_CompID
        //   AND CAST(DEPT.LS_Code AS VARCHAR(50)) = FA.AFAM_Department

        //LEFT JOIN Acc_AssetLocationSetup BAY
        //    ON BAY.LS_LevelCode = 4
        //   AND BAY.LS_CustId = FA.AFAM_CustId
        //   AND BAY.LS_CompID = FA.AFAM_CompID
        //   AND CAST(BAY.LS_Code AS VARCHAR(50)) = FA.AFAM_Bay

        //WHERE FA.AFAM_CompID = @CompId
        //  AND FA.AFAM_CustId = @CustId
        //  AND ISNULL(FA.AFAM_DelFlag,0) = 0;
        //        ",
        //                    new { request.CompId, request.CustId });

        //                foreach (var a in assets)
        //                {
        //                    if (a.OriginalCost <= 0) continue;

        //                    decimal residualValue = a.OriginalCost * (a.ResidualPercent / 100);

        //                    DateTime putToUse = a.DateOfPutToUse < request.FromDate
        //                        ? request.FromDate
        //                        : a.DateOfPutToUse.Value;

        //                    int daysUsed = (int)(request.ToDate - putToUse).TotalDays + 1;
        //                    int totalDaysFY = (int)(request.ToDate - request.FromDate).TotalDays + 1;

        //                    decimal depreciation = 0;

        //                    if (request.Method == 1) // SLM
        //                    {
        //                        decimal life = a.AssetLife > 0 ? a.AssetLife : 1;
        //                        decimal yearlyDep = (a.OriginalCost - residualValue) / life;
        //                        depreciation = yearlyDep * daysUsed / totalDaysFY;
        //                    }
        //                    else // WDV
        //                    {
        //                        decimal life = a.AssetLife > 0 ? a.AssetLife : 10;
        //                        decimal rate = 100 / life;
        //                        depreciation = a.OriginalCost * rate / 100 * daysUsed / totalDaysFY;
        //                    }

        //                    depreciation = Math.Round(depreciation, 2);

        //                    result.CompanyActData.Add(new CompanyActDepreciationDto
        //                    {
        //                        DateOfPutToUse = a.DateOfPutToUse,
        //                        AssetCode = a.AssetCode,
        //                        AssetName = a.AssetName,
        //                        Location = a.Location,
        //                        Division = a.Division,
        //                        Department = a.Department,
        //                        Bay = a.Bay,
        //                        OriginalCost = a.OriginalCost,
        //                        ResidualPercent = a.ResidualPercent,
        //                        DepreciationAmount = depreciation,
        //                        ClosingValue = a.OriginalCost - depreciation
        //                    });
        //                }
        //            }

        //            return result;
        //        }

        //        //-------wdv
        //        public async Task<List<dynamic>> CalculateCompanyActWDVAsync(
        //  int compId,
        //  int yearId,
        //  int custId,
        //  int noOfDays,
        //  int totalDays,
        //  int duration,
        //  DateTime startDate,
        //  DateTime endDate,
        //  int method)
        //        {

        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrWhiteSpace(dbName))
        //                throw new Exception("CustomerCode missing in session");

        //            var connectionString = _configuration.GetConnectionString(dbName);
        //            if (string.IsNullOrWhiteSpace(connectionString))
        //                throw new Exception($"Connection string not found for key: {dbName}");

        //            using var con = new SqlConnection(connectionString);
        //            await con.OpenAsync();

        //            List<dynamic> result = new();

        //            var assetRows = await con.QueryAsync<dynamic>(
        //                @"SELECT DISTINCT 
        //              AFAA_TrType,
        //              AFAA_Id,
        //              AFAA_ItemType,
        //              AFAA_AssetType,
        //              AFAA_AssetAmount,
        //              AFAA_FYAmount,
        //              AFAA_Location,
        //              AFAA_Division,
        //              AFAA_Department,
        //              AFAA_Bay
        //          FROM Acc_FixedAssetAdditionDel
        //          WHERE AFAA_CompID = @CompID
        //            AND AFAA_CustId = @CustID
        //            AND AFAA_YearID <= @YearID
        //            AND AFAA_Delflag = 'A'
        //          ORDER BY AFAA_ItemType",
        //                new { CompID = compId, CustID = custId, YearID = yearId });

        //            foreach (var row in assetRows)
        //            {
        //                /* -------------------- ASSET MASTER -------------------- */
        //                var asset = await con.QueryFirstOrDefaultAsync<dynamic>(
        //                    @"SELECT 
        //                 AFAM_AssetCode,
        //                 AFAM_AssetAge,
        //                 AFAM_PurchaseDate
        //              FROM Acc_FixedAssetMaster
        //              WHERE AFAM_ID = @ID
        //                AND AFAM_CompID = @CompID
        //                AND AFAM_CustId = @CustID",
        //                    new { ID = row.AFAA_ItemType, CompID = compId, CustID = custId });

        //                if (asset == null) continue;

        //                int assetAge = asset.AFAM_AssetAge != null ? Convert.ToInt32(asset.AFAM_AssetAge) : 0;
        //                DateTime purchaseDate = asset.AFAM_PurchaseDate ?? DateTime.MinValue;

        //                /* -------------------- NO OF DAYS -------------------- */
        //                int calcDays = 0;
        //                DateTime expiryDate = purchaseDate.AddYears(assetAge);

        //                if (expiryDate < startDate)
        //                    calcDays = 0;
        //                else if (purchaseDate >= startDate && purchaseDate <= endDate)
        //                {
        //                    if (duration != 3)
        //                        calcDays = (endDate - purchaseDate).Days;
        //                }
        //                else
        //                {
        //                    if (row.AFAA_TrType == 1)
        //                        calcDays = noOfDays;
        //                }

        //                //if (calcDays == 0 && duration != 3)
        //                //    continue;

        //                /* -------------------- RESIDUAL / SALVAGE -------------------- */
        //                decimal residualPercent = await con.ExecuteScalarAsync<decimal>(
        //                @"SELECT ISNULL(AM_ResidualValue,0)
        //              FROM Acc_AssetMaster
        //              WHERE AM_ID = @ID AND AM_CustId = @CustID",
        //                    new { ID = row.AFAA_AssetType, CustID = custId });

        //                decimal originalCost = Convert.ToDecimal(row.AFAA_AssetAmount ?? 0);
        //                decimal salvageValue = Math.Round((residualPercent * originalCost) / 100, 2);

        //                /* -------------------- WDV RATE -------------------- */
        //                decimal depreciationRate = 0;
        //                if (originalCost > 0 && salvageValue > 0 && assetAge > 0)
        //                {
        //                    depreciationRate = Math.Round(
        //                        (decimal)((1 - Math.Pow((double)(salvageValue / originalCost), 1.0 / assetAge)) * 100), 2);
        //                }

        //                /* -------------------- OPENING WDV -------------------- */
        //                decimal openingWDV = await con.ExecuteScalarAsync<decimal>(
        //                @"SELECT ISNULL(ADep_WrittenDownValue,0)
        //              FROM Acc_AssetDepreciation
        //              WHERE ADep_CompID = @CompID
        //                AND ADep_YearID = @YearID
        //                AND ADep_AssetID = @AssetID
        //                AND ADep_CustId = @CustID
        //                AND ADep_Method = @Method",
        //                    new
        //                    {
        //                        CompID = compId,
        //                        YearID = yearId,
        //                        AssetType = row.AFAA_AssetType,
        //                        AssetID = row.AFAA_ItemType,
        //                        CustID = custId,
        //                        Method = method
        //                    });

        //                if (openingWDV == 0)
        //                    openingWDV = originalCost;

        //                /* -------------------- DEPRECIATION -------------------- */
        //                decimal depreciationFY =
        //                    Math.Round((openingWDV * depreciationRate / 100) * calcDays / totalDays, 2);

        //                decimal wdvAfterDep = openingWDV - depreciationFY;

        //                /* -------------------- SALVAGE CHECK -------------------- */
        //                if (salvageValue >= wdvAfterDep)
        //                {
        //                    depreciationFY = openingWDV - salvageValue;
        //                    wdvAfterDep = salvageValue;
        //                }

        //                /* -------------------- DELETION CHECK -------------------- */
        //                decimal deletionAmount = await con.ExecuteScalarAsync<decimal>(
        //                @"SELECT ISNULL(SUM(AFAD_Amount),0)
        //              FROM Acc_FixedAssetDeletion
        //              WHERE AFAD_CompID = @CompID
        //               ",
        //                    new
        //                    {
        //                        CompID = compId,
        //                        AssetType = row.AFAA_AssetType,
        //                        AssetID = row.AFAA_ItemType,
        //                        CustID = custId
        //                    });

        //                if (deletionAmount > 0)
        //                {
        //                    depreciationFY = deletionAmount;
        //                    wdvAfterDep = 0;
        //                }

        //                /* -------------------- DESCRIPTION -------------------- */
        //                string assetTypeName = await con.ExecuteScalarAsync<string>(
        //                    @"SELECT AM_Description
        //              FROM Acc_AssetMaster
        //              WHERE AM_ID = @ID AND AM_CustId = @CustID",
        //                    new { ID = row.AFAA_AssetType, CustID = custId });

        //                /* -------------------- FINAL ROW -------------------- */
        //                result.Add(new
        //                {
        //                    AssetClassID = row.AFAA_AssetType,
        //                    AssetID = row.AFAA_ItemType,
        //                    AssetType = assetTypeName,
        //                    AssetCode = asset.AFAM_AssetCode,
        //                    PurchaseDate = purchaseDate,
        //                    AssetAge = assetAge,
        //                    OriginalCost = originalCost,
        //                    ResidualPercent = residualPercent,
        //                    SalvageValue = salvageValue,
        //                    DepreciationRate = depreciationRate,
        //                    OpeningWDV = openingWDV,
        //                    DepreciationForFY = depreciationFY,
        //                    ClosingWDV = wdvAfterDep,
        //                    NoOfDays = calcDays,
        //                    TrType = row.AFAA_TrType,
        //                    LocationID = row.AFAA_Location,
        //                    DivisionID = row.AFAA_Division,
        //                    DepartmentID = row.AFAA_Department,
        //                    BayID = row.AFAA_Bay
        //                });
        //            }

        //            return result;
        //        }

        //        //-------sln&wdv
        //        public async Task<List<dynamic>> CalculateCompanyActDepreciationAsync(
        //int compId,
        //int yearId,
        //int custId,
        //int noOfDays,
        //int totalDays,
        //int duration,
        //DateTime startDate,
        //DateTime endDate,
        //int method)
        //        {

        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrWhiteSpace(dbName))
        //                throw new Exception("CustomerCode missing in session");

        //            var connectionString = _configuration.GetConnectionString(dbName);
        //            if (string.IsNullOrWhiteSpace(connectionString))
        //                throw new Exception($"Connection string not found for key: {dbName}");

        //            using var con = new SqlConnection(connectionString);
        //            await con.OpenAsync();

        //            List<dynamic> result = new();

        //            var assetRows = await con.QueryAsync<dynamic>(
        //                @"SELECT DISTINCT 
        //            AFAA_TrType,
        //            AFAA_Id,
        //            AFAA_ItemType,
        //            AFAA_AssetType,
        //            AFAA_AssetAmount,
        //            AFAA_FYAmount,
        //            AFAA_Location,
        //            AFAA_Division,
        //            AFAA_Department,
        //            AFAA_Bay
        //        FROM Acc_FixedAssetAdditionDel
        //        WHERE AFAA_CompID = @CompID
        //          AND AFAA_CustId = @CustID
        //          AND AFAA_YearID <= @YearID
        //          AND AFAA_Delflag = 'A'
        //        ORDER BY AFAA_ItemType",
        //                new { CompID = compId, CustID = custId, YearID = yearId });

        //            foreach (var row in assetRows)
        //            {
        //                /* -------------------- ASSET MASTER -------------------- */
        //                var asset = await con.QueryFirstOrDefaultAsync<dynamic>(
        //                    @"SELECT 
        //               AFAM_AssetCode,
        //               AFAM_AssetAge,
        //               AFAM_PurchaseDate
        //            FROM Acc_FixedAssetMaster
        //            WHERE AFAM_ID = @ID
        //              AND AFAM_CompID = @CompID
        //              AND AFAM_CustId = @CustID",
        //                    new { ID = row.AFAA_ItemType, CompID = compId, CustID = custId });

        //                if (asset == null) continue;

        //                int assetAge = asset.AFAM_AssetAge != null ? Convert.ToInt32(asset.AFAM_AssetAge) : 0;
        //                DateTime purchaseDate = asset.AFAM_PurchaseDate ?? DateTime.MinValue;

        //                /* -------------------- NO OF DAYS -------------------- */
        //                int calcDays = 0;
        //                DateTime expiryDate = purchaseDate.AddYears(assetAge);

        //                if (expiryDate < startDate)
        //                    calcDays = 0;
        //                else if (purchaseDate >= startDate && purchaseDate <= endDate)
        //                {
        //                    if (duration != 3)
        //                        calcDays = (endDate - purchaseDate).Days;
        //                }
        //                else
        //                {
        //                    if (row.AFAA_TrType == 1)
        //                        calcDays = noOfDays;
        //                }

        //                //if (calcDays == 0 && duration != 3)
        //                //    continue;

        //                /* -------------------- RESIDUAL / SALVAGE -------------------- */
        //                decimal residualPercent = await con.ExecuteScalarAsync<decimal>(
        //                @"SELECT ISNULL(AM_ResidualValue,0)
        //            FROM Acc_AssetMaster
        //            WHERE AM_ID = @ID AND AM_CustId = @CustID",
        //                    new { ID = row.AFAA_AssetType, CustID = custId });

        //                decimal originalCost = Convert.ToDecimal(row.AFAA_AssetAmount ?? 0);
        //                decimal salvageValue = Math.Round((residualPercent * originalCost) / 100, 2);

        //                /* -------------------- WDV RATE -------------------- */
        //                decimal depreciationRate = 0;
        //                if (originalCost > 0 && salvageValue > 0 && assetAge > 0)
        //                {
        //                    depreciationRate = Math.Round(
        //                        (decimal)((1 - Math.Pow((double)(salvageValue / originalCost), 1.0 / assetAge)) * 100), 2);
        //                }

        //                /* -------------------- OPENING WDV -------------------- */
        //                decimal openingWDV = await con.ExecuteScalarAsync<decimal>(
        //                @"SELECT ISNULL(ADep_WrittenDownValue,0)
        //            FROM Acc_AssetDepreciation
        //            WHERE ADep_CompID = @CompID
        //              AND ADep_YearID = @YearID
        //              AND ADep_AssetID = @AssetID
        //              AND ADep_CustId = @CustID
        //              AND ADep_Method = @Method",
        //                    new
        //                    {
        //                        CompID = compId,
        //                        YearID = yearId,
        //                        AssetType = row.AFAA_AssetType,
        //                        AssetID = row.AFAA_ItemType,
        //                        CustID = custId,
        //                        Method = method
        //                    });

        //                if (openingWDV == 0)
        //                    openingWDV = originalCost;

        //                /* -------------------- DEPRECIATION -------------------- */
        //                decimal depreciationFY =
        //                    Math.Round((openingWDV * depreciationRate / 100) * calcDays / totalDays, 2);

        //                decimal wdvAfterDep = openingWDV - depreciationFY;

        //                /* -------------------- SALVAGE CHECK -------------------- */
        //                if (salvageValue >= wdvAfterDep)
        //                {
        //                    depreciationFY = openingWDV - salvageValue;
        //                    wdvAfterDep = salvageValue;
        //                }

        //                /* -------------------- DELETION CHECK -------------------- */
        //                decimal deletionAmount = await con.ExecuteScalarAsync<decimal>(
        //                @"SELECT ISNULL(SUM(AFAD_Amount),0)
        //            FROM Acc_FixedAssetDeletion
        //            WHERE AFAD_CompID = @CompID
        //             ",
        //                    new
        //                    {
        //                        CompID = compId,
        //                        AssetType = row.AFAA_AssetType,
        //                        AssetID = row.AFAA_ItemType,
        //                        CustID = custId
        //                    });

        //                if (deletionAmount > 0)
        //                {
        //                    depreciationFY = deletionAmount;
        //                    wdvAfterDep = 0;
        //                }

        //                /* -------------------- DESCRIPTION -------------------- */
        //                string assetTypeName = await con.ExecuteScalarAsync<string>(
        //                    @"SELECT AM_Description
        //            FROM Acc_AssetMaster
        //            WHERE AM_ID = @ID AND AM_CustId = @CustID",
        //                    new { ID = row.AFAA_AssetType, CustID = custId });

        //                /* -------------------- FINAL ROW -------------------- */
        //                result.Add(new
        //                {
        //                    AssetClassID = row.AFAA_AssetType,
        //                    AssetID = row.AFAA_ItemType,
        //                    AssetType = assetTypeName,
        //                    AssetCode = asset.AFAM_AssetCode,
        //                    PurchaseDate = purchaseDate,
        //                    AssetAge = assetAge,
        //                    OriginalCost = originalCost,
        //                    ResidualPercent = residualPercent,
        //                    SalvageValue = salvageValue,
        //                    DepreciationRate = depreciationRate,
        //                    OpeningWDV = openingWDV,
        //                    DepreciationForFY = depreciationFY,
        //                    ClosingWDV = wdvAfterDep,
        //                    NoOfDays = calcDays,
        //                    TrType = row.AFAA_TrType,
        //                    LocationID = row.AFAA_Location,
        //                    DivisionID = row.AFAA_Division,
        //                    DepartmentID = row.AFAA_Department,
        //                    BayID = row.AFAA_Bay
        //                });
        //            }

        //            return result;
        //        }


        //----------final
        public async Task<object> CalculateDepreciationAsync(
    int depBasis,          // 1 = Company Act, 2 = IT Act
    int compId,
    int yearId,
    int custId,
    int noOfDays,
    int totalDays,
    int duration,
    DateTime startDate,
    DateTime endDate,
    int method             // 1 = SLM, 2 = WDV (Company Act only)
)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode missing in session");

            var connectionString = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Connection string not found for key: {dbName}");

            using var con = new SqlConnection(connectionString);
            await con.OpenAsync();

            // ------------------------- IT ACT DEPRECIATION -------------------------
            if (depBasis == 2)
            {
                var result = new List<DepreciatioITActDto>();
                int prevYearId = yearId - 1;

                using var tran = con.BeginTransaction();
                try
                {
                    int prevCount = await con.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(*) FROM Acc_AssetDepITAct
                          WHERE ADITAct_CompID=@CompId
                            AND ADITAct_CustId=@CustId
                            AND ADITAct_YearID=@PrevYearId",
                        new { CompId = compId, CustId = custId, PrevYearId = prevYearId }, tran);

                    var assets = (await con.QueryAsync<AssetMasterDto>(
                        @"SELECT AM_ID, AM_Description, AM_WDVITAct, AM_ITRate
                          FROM Acc_AssetMaster
                          WHERE AM_CompID=@CompId AND AM_LevelCode=2 AND AM_CustId=@CustId",
                        new { CompId = compId, CustId = custId }, tran)).ToList();

                    foreach (var asset in assets)
                    {
                        double openingWDV = asset.AM_WDVITAct;

                        if (prevCount > 0)
                        {
                            openingWDV = await con.ExecuteScalarAsync<double>(
                                @"SELECT ISNULL(SUM(ADITAct_WrittenDownValue),0)
                                  FROM Acc_AssetDepITAct
                                  WHERE ADITAct_AssetClassID=@AssetId
                                    AND ADITAct_YearID=@PrevYearId
                                    AND ADITAct_CompID=@CompId
                                    AND ADITAct_CustId=@CustId",
                                new
                                {
                                    AssetId = asset.AM_ID,
                                    PrevYearId = prevYearId,
                                    CompId = compId,
                                    CustId = custId
                                }, tran);

                            if (openingWDV == 0)
                                openingWDV = asset.AM_WDVITAct;
                        }

                        double deletion = await con.ExecuteScalarAsync<double>(
                            @"SELECT ISNULL(SUM(AFAD_Amount),0)
                              FROM Acc_FixedAssetDeletion
                              WHERE AFAD_AssetClass=@AssetId
                                AND AFAD_YearID=@YearId
                                AND AFAD_CompID=@CompId
                                AND AFAD_CustomerName=@CustId",
                            new
                            {
                                AssetId = asset.AM_ID,
                                YearId = yearId,
                                CompId = compId,
                                CustId = custId
                            }, tran);

                        openingWDV -= deletion;
                        if (openingWDV < 0) openingWDV = 0;

                        double depForYear = 0;
                        if (openingWDV > 0 && asset.AM_ITRate > 0)
                            depForYear = (openingWDV * asset.AM_ITRate) / 100;

                        result.Add(new DepreciatioITActDto
                        {
                            AssetClassID = asset.AM_ID,
                            ClassofAsset = asset.AM_Description,
                            RateofDep = asset.AM_ITRate,
                            WDVOpeningValue = Math.Round(openingWDV, 0),
                            WDVOpeningDepreciation = Math.Round(depForYear, 0),
                            DelAmount = Math.Round(deletion, 0),
                            Depfortheperiod = Math.Round(depForYear, 0),
                            WDVClosingValue = Math.Round(openingWDV - depForYear, 0)
                        });
                    }

                    tran.Commit();
                    return result;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }

            // ------------------------- COMPANY ACT DEPRECIATION -------------------------
            else if (depBasis == 1)
            {
                List<dynamic> result = new();

                var assetRows = await con.QueryAsync<dynamic>(
                    @"SELECT DISTINCT 
                        AFAA_TrType,
                        AFAA_Id,
                        AFAA_ItemType,
                        AFAA_AssetType,
                        AFAA_AssetAmount,
                        AFAA_FYAmount,
                        AFAA_Location,
                        AFAA_Division,
                        AFAA_Department,
                        AFAA_Bay
                      FROM Acc_FixedAssetAdditionDel
                      WHERE AFAA_CompID = @CompID
                        AND AFAA_CustId = @CustID
                        AND AFAA_YearID <= @YearID
                        AND AFAA_Delflag = 'A'
                      ORDER BY AFAA_ItemType",
                    new { CompID = compId, CustID = custId, YearID = yearId });

                foreach (var row in assetRows)
                {
                    var asset = await con.QueryFirstOrDefaultAsync<dynamic>(
                        @"SELECT 
                            AFAM_AssetCode,
                            AFAM_AssetAge,
                            AFAM_PurchaseDate
                          FROM Acc_FixedAssetMaster
                          WHERE AFAM_ID = @ID
                            AND AFAM_CompID = @CompID
                            AND AFAM_CustId = @CustID",
                        new { ID = row.AFAA_ItemType, CompID = compId, CustID = custId });

                    if (asset == null) continue;

                    int assetAge = asset.AFAM_AssetAge != null ? Convert.ToInt32(asset.AFAM_AssetAge) : 0;
                    DateTime purchaseDate = asset.AFAM_PurchaseDate ?? DateTime.MinValue;

                    // Calculate days
                    int calcDays = 0;
                    DateTime expiryDate = purchaseDate.AddYears(assetAge);
                    if (expiryDate < startDate) calcDays = 0;
                    else if (purchaseDate >= startDate && purchaseDate <= endDate)
                    {
                        if (duration != 3)
                            calcDays = (endDate - purchaseDate).Days;
                    }
                    else
                    {
                        if (row.AFAA_TrType == 1)
                            calcDays = noOfDays;
                    }

                    decimal residualPercent = await con.ExecuteScalarAsync<decimal>(
                        @"SELECT ISNULL(AM_ResidualValue,0)
                          FROM Acc_AssetMaster
                          WHERE AM_ID = @ID AND AM_CustId = @CustID",
                        new { ID = row.AFAA_AssetType, CustID = custId });

                    decimal originalCost = Convert.ToDecimal(row.AFAA_AssetAmount ?? 0);
                    decimal salvageValue = Math.Round((residualPercent * originalCost) / 100, 2);

                    decimal depreciationRate = 0;
                    if (originalCost > 0 && salvageValue > 0 && assetAge > 0)
                        depreciationRate = Math.Round((decimal)((1 - Math.Pow((double)(salvageValue / originalCost), 1.0 / assetAge)) * 100), 2);

                    decimal openingWDV = await con.ExecuteScalarAsync<decimal>(
                        @"SELECT ISNULL(ADep_WrittenDownValue,0)
                          FROM Acc_AssetDepreciation
                          WHERE ADep_CompID = @CompID
                            AND ADep_YearID = @YearID
                            AND ADep_AssetID = @AssetID
                            AND ADep_CustId = @CustID
                            AND ADep_Method = @Method",
                        new
                        {
                            CompID = compId,
                            YearID = yearId,
                            AssetType = row.AFAA_AssetType,
                            AssetID = row.AFAA_ItemType,
                            CustID = custId,
                            Method = method
                        });

                    if (openingWDV == 0) openingWDV = originalCost;

                    decimal depreciationFY = Math.Round((openingWDV * depreciationRate / 100) * calcDays / totalDays, 2);
                    decimal wdvAfterDep = openingWDV - depreciationFY;

                    if (salvageValue >= wdvAfterDep)
                    {
                        depreciationFY = openingWDV - salvageValue;
                        wdvAfterDep = salvageValue;
                    }

                    decimal deletionAmount = await con.ExecuteScalarAsync<decimal>(
                        @"SELECT ISNULL(SUM(AFAD_Amount),0)
                          FROM Acc_FixedAssetDeletion
                          WHERE AFAD_CompID = @CompID",
                        new
                        {
                            CompID = compId,
                            AssetType = row.AFAA_AssetType,
                            AssetID = row.AFAA_ItemType,
                            CustID = custId
                        });

                    if (deletionAmount > 0)
                    {
                        depreciationFY = deletionAmount;
                        wdvAfterDep = 0;
                    }

                    string assetTypeName = await con.ExecuteScalarAsync<string>(
                        @"SELECT AM_Description
                          FROM Acc_AssetMaster
                          WHERE AM_ID = @ID AND AM_CustId = @CustID",
                        new { ID = row.AFAA_AssetType, CustID = custId });

                    result.Add(new
                    {
                        AssetClassID = row.AFAA_AssetType,
                        AssetID = row.AFAA_ItemType,
                        AssetType = assetTypeName,
                        AssetCode = asset.AFAM_AssetCode,
                        PurchaseDate = purchaseDate,
                        AssetAge = assetAge,
                        OriginalCost = originalCost,
                        ResidualPercent = residualPercent,
                        SalvageValue = salvageValue,
                        DepreciationRate = depreciationRate,
                        OpeningWDV = openingWDV,
                        DepreciationForFY = depreciationFY,
                        ClosingWDV = wdvAfterDep,
                        NoOfDays = calcDays,
                        TrType = row.AFAA_TrType,
                        LocationID = row.AFAA_Location,
                        DivisionID = row.AFAA_Division,
                        DepartmentID = row.AFAA_Department,
                        BayID = row.AFAA_Bay
                    });
                }

                return result;
            }

            // If invalid depBasis, return empty
            return new List<object>();
        }


        //ITcalculation
        public async Task<List<ITDepreciationResponseDto>> CalculateITActDepreciationAsync(ITDepreciationRequestDto request)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode missing in session");

            var connectionString = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Connection string not found for key: {dbName}");

            using var con = new SqlConnection(connectionString);
            await con.OpenAsync();

            int previousYearId = request.YearId != 0 ? request.YearId - 1 : 0;
            var result = new List<ITDepreciationResponseDto>();

            var assetMasterSql = @"
        SELECT AM_ID, AM_Description, AM_WDVITAct, AM_ITRate
        FROM Acc_AssetMaster
        WHERE AM_CompID=@CompId AND AM_LevelCode=2 AND AM_CustId=@CustId";

            var assets = await con.QueryAsync(assetMasterSql, new { request.CompId, request.CustId });

            foreach (var asset in assets)
            {
                var dto = new ITDepreciationResponseDto
                {
                    AssetClassID = asset.AM_ID,
                    ClassofAsset = asset.AM_Description,
                    RateofDep = Convert.ToDouble(asset.AM_ITRate) // explicit conversion
                };

                // Opening WDV
                var openingSql = @"
            SELECT ISNULL(SUM(ADITAct_WrittenDownValue),0) 
            FROM Acc_AssetDepITAct
            WHERE ADITAct_AssetClassID=@AssetId AND ADITAct_CustId=@CustId AND
                  ADITAct_YearID=@PrevYearId AND ADITAct_CompID=@CompId";

                var wdvOpeningDecimal = await con.ExecuteScalarAsync<decimal>(openingSql, new { AssetId = asset.AM_ID, request.CustId, PrevYearId = previousYearId, request.CompId });
                double wdvOpening = (double)wdvOpeningDecimal; // explicit cast

                if (wdvOpening == 0)
                    wdvOpening = Convert.ToDouble(asset.AM_WDVITAct); // explicit cast

                // Deletion Amount
                var delSql = @"
            SELECT ISNULL(SUM(AFAD_SalesPrice),0)
            FROM Acc_FixedAssetDeletion
            WHERE AFAD_AssetClass=@AssetId AND AFAD_CompID=@CompId AND 
                  AFAD_CustomerName=@CustId AND AFAD_YearID=@YearId";

                var delAmountDecimal = await con.ExecuteScalarAsync<decimal>(delSql, new { AssetId = asset.AM_ID, request.CompId, request.CustId, request.YearId });
                double delAmount = (double)delAmountDecimal; // explicit cast

                dto.WDVOpeningValue = Math.Round(wdvOpening - delAmount);
                dto.DelAmount = Math.Round(delAmount);

                // Base Depreciation
                dto.WDVOpeningDepreciation = (dto.WDVOpeningValue * dto.RateofDep) / 100;

                // Fetch additions
                var addSql = @"
            SELECT b.FAAD_ItemType, a.AFAM_CommissionDate, ISNULL(SUM(b.FAAD_AssetValue),0) AS FAAD_AssetValue, ISNULL(FAAD_InitDep,0) AS FAAD_InitDep
            FROM Acc_FixedAssetMaster a
            LEFT JOIN Acc_FixedAssetAdditionDetails b ON a.AFAM_ID=b.FAAD_ItemType
            WHERE b.FAAD_ItemType<>'' AND FAAD_YearID=@YearId
              AND FAAD_Delflag<>'D' AND FAAD_CustId=@CustId AND FAAD_Status<>'D'
              AND FAAD_CompID=@CompId AND FAAD_AssetType=@AssetId
            GROUP BY FAAD_ItemType, AFAM_CommissionDate, FAAD_InitDep";

                var additions = await con.QueryAsync(addSql, new { request.YearId, request.CustId, request.CompId, AssetId = asset.AM_ID });

                double less180 = 0, more180 = 0, depLess180 = 0, depMore180 = 0;
                double initDepTotal = 0, prevInitDep = 0, nextYrCarry = 0;

                foreach (var add in additions)
                {
                    DateTime commDate = add.AFAM_CommissionDate;
                    int noOfDays = (request.EndDate - commDate).Days;
                    double assetValue = Convert.ToDouble(add.FAAD_AssetValue); // explicit cast
                    int initDepFlag = add.FAAD_InitDep;

                    if (noOfDays <= 180)
                    {
                        less180 += assetValue;
                        if (initDepFlag == 1)
                        {
                            double initDepAmt = assetValue * 0.10;
                            dto.InitDepAmt = initDepAmt;
                            initDepTotal += initDepAmt;
                            nextYrCarry = initDepAmt;
                        }
                        depLess180 += assetValue * (dto.RateofDep / 2) / 100;
                    }
                    else
                    {
                        more180 += assetValue;
                        if (initDepFlag == 1)
                        {
                            double initDepAmt = assetValue * 0.20;
                            dto.InitDepAmt = initDepAmt;
                            initDepTotal += initDepAmt;
                        }
                        depMore180 += assetValue * dto.RateofDep / 100;
                    }
                }

                dto.BfrQtrAmount = less180;
                dto.BfrQtrDep = depLess180;
                dto.AftQtrAmount = more180;
                dto.AftQtrDep = depMore180;
                dto.AdditionDuringtheYear = Math.Round(less180 + more180);
                dto.Depfortheperiod = Math.Round(dto.WDVOpeningDepreciation + depLess180 + depMore180 + initDepTotal + prevInitDep);
                dto.WDVClosingValue = Math.Round(wdvOpening + less180 + more180 - dto.Depfortheperiod);
                dto.NextYrCarry = nextYrCarry;
                dto.PrevInitDepAmt = prevInitDep;

                result.Add(dto);
            }

            return result;
        }

        //companyCalculation
        //        public async Task<List<DepreciationResultDtoo>> CalculateWDVAsync(DepreciationRequest request)
        //        {
        //            // Session & validation
        //            var session = _httpContextAccessor.HttpContext?.Session;
        //            if (session == null)
        //                throw new Exception("Session not available");

        //            string dbName = session.GetString("CustomerCode");
        //            if (string.IsNullOrWhiteSpace(dbName))
        //                throw new Exception("CustomerCode missing in session");

        //            if (request.CompanyId <= 0 || request.CustomerId <= 0 || request.FinancialYearId <= 0)
        //                throw new Exception("Invalid Company / Customer / Year");

        //            var connectionString = _configuration.GetConnectionString(dbName);
        //            if (string.IsNullOrWhiteSpace(connectionString))
        //                throw new Exception("Connection string not found");

        //            var result = new List<DepreciationResultDtoo>();

        //            using (var con = new SqlConnection(connectionString))
        //            {
        //                await con.OpenAsync();

        //                string sql = @"
        //SELECT
        //    AFAA_AssetType      AS AssetClassId,
        //    AFAA_ItemType       AS AssetId,
        //    AFAA_Location       AS Location,
        //    AFAA_Division       AS Division,
        //    AFAA_Department     AS Department,
        //    AFAA_Bay            AS Bay,
        //    AFAA_PurchaseDate   AS PurchaseDate,
        //    ISNULL(AFAA_AssetAmount,0) AS OriginalCost,
        //    ISNULL(AFAA_FYAmount,0)    AS OpeningBalance,
        //    ISNULL(AFAA_AssetAge,0)    AS AssetAge
        //FROM Acc_FixedAssetAdditionDel
        //WHERE AFAA_CompID = @CompId
        //  AND AFAA_CustId = @CustId
        //  AND AFAA_YearID <= @YearId
        //  AND AFAA_Delflag = 'A'";

        //                var assets = (await con.QueryAsync<AssetEntity>(sql, new
        //                {
        //                    CompId = request.CompanyId,
        //                    CustId = request.CustomerId,
        //                    YearId = request.FinancialYearId
        //                })).ToList();

        //                // Calculation
        //                foreach (var asset in assets)
        //                {
        //                    DateTime? purchaseDate = asset.PurchaseDate;
        //                    if (!purchaseDate.HasValue)
        //                        continue;

        //                    // No of days
        //                    DateTime effectiveStart =
        //                        purchaseDate.Value > request.StartDate ? purchaseDate.Value : request.StartDate;

        //                    if (effectiveStart > request.EndDate)
        //                        continue;

        //                    int noOfDays = (request.EndDate - effectiveStart).Days + 1;

        //                    decimal totalDays =
        //                        (decimal)(request.EndDate - request.StartDate).TotalDays + 1;

        //                    if (totalDays <= 0)
        //                        totalDays = 1;

        //                    // WDV RATE (VB SAFE LOGIC)
        //                    decimal rate;
        //                    if (asset.AssetAge <= 0)
        //                    {
        //                        rate = 5; // VB default when AssetAge = 0
        //                    }
        //                    else
        //                    {
        //                        rate = Math.Round(100m / asset.AssetAge, 2);
        //                    }

        //                    decimal depreciation =
        //                        asset.OpeningBalance * rate / 100 * noOfDays / totalDays;

        //                    depreciation = Math.Round(depreciation, 2);

        //                    result.Add(new DepreciationResultDtoo
        //                    {
        //                        AssetClassId = asset.AssetClassId,
        //                        AssetId = asset.AssetId,
        //                        Location = asset.Location,
        //                        Division = asset.Division,
        //                        Department = asset.Department,
        //                        Bay = asset.Bay,
        //                        PurchaseDate = purchaseDate,
        //                        NoOfDays = noOfDays,
        //                        OriginalCost = asset.OriginalCost,
        //                        OpeningBalance = asset.OpeningBalance,
        //                        AssetAge = asset.AssetAge,
        //                        DepreciationRate = rate,
        //                        DepreciationForYear = depreciation,
        //                        WrittenDownValue = asset.OpeningBalance - depreciation
        //                    });
        //                }
        //            }

        //            return result;
        //        }


    }

}








