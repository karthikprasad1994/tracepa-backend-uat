using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.FixedAssetReportDto;


namespace TracePca.Service.FixedAssetsService
{
    public class FixedAssetReportService : FixedAssetReportInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        //Depreciation
        public FixedAssetReportService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //Report(Ok)
        //    public async Task<List<FixedAssetReportRow>> GetFixedAssetReportAsync(
        //int reportType,      // 1 = Company Act, 2 = IT Act
        //int compId,
        //int custId,
        //int yearId,
        //string financialYear,
        //string locationIds,
        //int methodType,
        //IDbConnection db)
        //    {
        //        string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //        if (string.IsNullOrWhiteSpace(dbName))
        //            throw new Exception("CustomerCode is missing in session.");

        //        string connectionString = _configuration.GetConnectionString(dbName);

        //        using var connection = new SqlConnection(connectionString);
        //        await connection.OpenAsync();

        //        var result = new List<FixedAssetReportRow>();

        //        decimal subOpening = 0, subAddGT = 0, subAddLT = 0,
        //                subDel = 0, subDep = 0, subClose = 0;

        //        string sql;

        //        if (reportType == 1)
        //        {
        //            // ================= COMPANY ACT =================
        //            sql = @"
        //    SELECT 
        //        AM.AM_Description AS AssetClass,
        //        SUM(ADEP_OpeningWDV) AS OpeningWDV,
        //        SUM(ADEP_AdditionGT180) AS AddGT180,
        //        SUM(ADEP_AdditionLT180) AS AddLT180,
        //        SUM(ADEP_Deletion) AS Deletion,
        //        MAX(ADEP_DepRate) AS DepRate
        //    FROM Acc_AssetDepCompanyAct ADEP
        //    JOIN Acc_AssetMaster AM ON AM.AM_ID = ADEP.ADEP_AssetClassID
        //    WHERE ADEP_YearID = @YearId
        //      AND ADEP_CustId = @CustId
        //      AND ADEP_CompId = @CompId
        //    GROUP BY AM.AM_Description";
        //        }
        //        else if (reportType == 2)
        //        {
        //            // ================= IT ACT =================
        //            sql = @"
        //    SELECT 
        //        AM.AM_Description AS AssetClass,
        //        SUM(ADITAct_WrittenDownValue) AS OpeningWDV,
        //        SUM(ADITAct_AftQtrAmount) AS AddGT180,
        //        SUM(ADITAct_BfrQtrAmount) AS AddLT180,
        //        SUM(ADITAct_DelAmount) AS Deletion,
        //        MAX(ADITAct_RateofDep) AS DepRate
        //    FROM Acc_AssetDepITAct A
        //    JOIN Acc_AssetMaster AM ON AM.AM_ID = A.ADITAct_AssetClassID
        //    WHERE A.ADITAct_YearID = @YearId
        //      AND A.ADITAct_CustId = @CustId
        //      AND A.ADITAct_CompID = @CompId
        //    GROUP BY AM.AM_Description";
        //        }
        //        else
        //        {
        //            throw new Exception("Invalid report type");
        //        }

        //        var rawRows = await db.QueryAsync(sql, new
        //        {
        //            YearId = yearId,
        //            CustId = custId,
        //            CompId = compId
        //        });

        //        foreach (var r in rawRows)
        //        {
        //            decimal opening = r.OpeningWDV ?? 0;
        //            decimal addGT = r.AddGT180 ?? 0;
        //            decimal addLT = r.AddLT180 ?? 0;
        //            decimal deletion = r.Deletion ?? 0;
        //            decimal rate = r.DepRate ?? 0;

        //            decimal total = opening + addGT + addLT - deletion;

        //            decimal depAmount;

        //            if (reportType == 1)
        //            {
        //                // Company Act depreciation
        //                depAmount = (opening + addGT + (addLT / 2)) * rate / 100;
        //            }
        //            else
        //            {
        //                // IT Act depreciation
        //                depAmount =
        //                    ((opening + addGT - deletion) * rate / 100) +
        //                    ((addLT * rate / 100) / 2);
        //            }

        //            decimal closing = total - depAmount;

        //            result.Add(new FixedAssetReportRowDto
        //            {
        //                AssetClass = r.AssetClass,
        //                OpeningWDV = opening,
        //                AdditionsGT180 = addGT,
        //                AdditionsLT180 = addLT,
        //                Deletions = deletion,
        //                Total = total,
        //                DepRate = rate,
        //                DepForPeriod = depAmount,
        //                ClosingWDV = closing,
        //                IsSubTotal = false
        //            });

        //            subOpening += opening;
        //            subAddGT += addGT;
        //            subAddLT += addLT;
        //            subDel += deletion;
        //            subDep += depAmount;
        //            subClose += closing;
        //        }

        //        // ================= SUB TOTAL ROW =================
        //        result.Add(new FixedAssetReportRow
        //        {
        //            AssetClass = "Sub Total",
        //            OpeningWDV = subOpening,
        //            AdditionsGT180 = subAddGT,
        //            AdditionsLT180 = subAddLT,
        //            Deletions = subDel,
        //            DepForPeriod = subDep,
        //            ClosingWDV = subClose,
        //            IsSubTotal = true
        //        });

        //        return result;
        //    }

        //-----------------------

        //        public async Task<List<FixedAssetReportRowDto>> GetFixedAssetReportAsync(
        //            FixedAssetReportRequestDto request)
        //        {
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrWhiteSpace(dbName))
        //                throw new Exception("CustomerCode is missing in session.");

        //            string connectionString = _configuration.GetConnectionString(dbName);

        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            var result = new List<FixedAssetReportRowDto>();

        //            decimal subOpening = 0, subAddGT = 0, subAddLT = 0,
        //                    subDel = 0, subDep = 0, subClose = 0;

        //            string sql;

        //            if (request.ReportType == 1)
        //            {
        //                // ===== COMPANY ACT =====
        //                sql = @"
        //           SELECT
        //    AM.AM_Description AS AssetClass,

        //    -- COST
        //    SUM(CASE WHEN A.ADep_TransType = 1 THEN A.ADep_OPBForYR ELSE 0 END) AS CostBegYear,
        //    SUM(CASE WHEN A.ADep_TransType = 2 THEN A.ADep_OPBForYR ELSE 0 END) AS Additions,
        //    SUM(CASE WHEN A.ADep_TransType = 3 THEN A.ADep_OPBForYR ELSE 0 END) AS Deletions,

        //    -- TOTAL COST
        //    SUM(A.ADep_OPBForYR) AS TotalCostEndYear,

        //    -- DEPRECIATION
        //    SUM(CASE WHEN A.ADep_TransType = 1 THEN A.ADep_DepreciationforFY ELSE 0 END) AS DepOnOpening,
        //    SUM(CASE WHEN A.ADep_TransType = 2 THEN A.ADep_DepreciationforFY ELSE 0 END) AS DepOnAdditions,
        //    SUM(CASE WHEN A.ADep_TransType = 3 THEN A.ADep_DepreciationforFY ELSE 0 END) AS DepOnDeletions,

        //    -- TOTAL DEPRECIATION FOR YEAR
        //    SUM(A.ADep_DepreciationforFY) AS TotalDepForYear,

        //    -- TOTAL DEPRECIATION TILL DATE
        //    SUM(A.ADep_DepreciationforFY)
        //        + SUM(CASE WHEN A.ADep_TransType = 1 THEN A.ADep_OPBForYR ELSE 0 END)
        //        AS TotalDepTillDate,

        //    -- WDV
        //    SUM(A.ADep_WrittenDownValue) AS WDVEndYear,
        //    SUM(A.ADep_OPBForYR) - SUM(A.ADep_DepreciationforFY) AS WDVBegYear

        //FROM Acc_AssetDepreciation A
        //JOIN Acc_AssetMaster AM
        //    ON AM.AM_ID = A.ADep_AssetID

        //WHERE
        //    A.ADep_YearID = 9        -- ✅ REAL YEAR
        //    AND A.ADep_CustId = 1
        //    AND A.ADep_CompID = 1
        //    AND A.ADep_Method = 2    -- ✅ REAL METHOD
        //    AND A.ADep_DelFlag = 'X'

        //GROUP BY
        //    AM.AM_Description

        //ORDER BY
        //    AM.AM_Description;";
        //            }
        //            else if (request.ReportType == 2)
        //            {
        //                // ===== IT ACT =====
        //                sql = @"
        //                SELECT 
        //                    AM.AM_Description AS AssetClass,
        //                    SUM(ADITAct_WrittenDownValue) AS OpeningWDV,
        //                    SUM(ADITAct_AftQtrAmount) AS AddGT180,
        //                    SUM(ADITAct_BfrQtrAmount) AS AddLT180,
        //                    SUM(ADITAct_DelAmount) AS Deletion,
        //                    MAX(ADITAct_RateofDep) AS DepRate
        //                FROM Acc_AssetDepITAct A
        //                JOIN Acc_AssetMaster AM ON AM.AM_ID = A.ADITAct_AssetClassID
        //                WHERE A.ADITAct_YearID = @YearId
        //                  AND A.ADITAct_CustId = @CustId
        //                  AND A.ADITAct_CompID = @CompId
        //                GROUP BY AM.AM_Description";
        //            }
        //            else
        //            {
        //                throw new Exception("Invalid ReportType");
        //            }

        //            var rows = await connection.QueryAsync(sql, new
        //            {
        //                request.YearId,
        //                request.CustId,
        //                request.CompId
        //            });

        //            foreach (var r in rows)
        //            {
        //                decimal opening = r.OpeningWDV ?? 0;
        //                decimal addGT = r.AddGT180 ?? 0;
        //                decimal addLT = r.AddLT180 ?? 0;
        //                decimal deletion = r.Deletion ?? 0;
        //                decimal rate = r.DepRate ?? 0;

        //                decimal total = opening + addGT + addLT - deletion;

        //                decimal depAmount =
        //                    request.ReportType == 1
        //                        ? (opening + addGT + (addLT / 2)) * rate / 100
        //                        : ((opening + addGT - deletion) * rate / 100)   //ITACT
        //                          + ((addLT * rate / 100) / 2);

        //                decimal closing = total - depAmount;

        //                result.Add(new FixedAssetReportRowDto
        //                {
        //                    AssetClass = r.AssetClass,
        //                    OpeningWDV = opening,
        //                    AdditionsGT180 = addGT,
        //                    AdditionsLT180 = addLT,
        //                    Deletions = deletion,
        //                    Total = total,
        //                    DepRate = rate,
        //                    DepForPeriod = depAmount,
        //                    ClosingWDV = closing,
        //                    IsSubTotal = false
        //                });

        //                subOpening += opening;
        //                subAddGT += addGT;
        //                subAddLT += addLT;
        //                subDel += deletion;
        //                subDep += depAmount;
        //                subClose += closing;
        //            }

        //            result.Add(new FixedAssetReportRowDto
        //            {
        //                AssetClass = "Sub Total",
        //                OpeningWDV = subOpening,
        //                AdditionsGT180 = subAddGT,
        //                AdditionsLT180 = subAddLT,
        //                Deletions = subDel,
        //                DepForPeriod = subDep,
        //                ClosingWDV = subClose,
        //                IsSubTotal = true
        //            });

        //            return result;
        //        }

        //------------


        public async Task<object> GetFixedAssetReportAsync(
     int reportType,
     int compId,
     int custId,
     int yearId,
     int methodType,
     int locationIds,
     string financialYear
 )
        {
            string dbName = _httpContextAccessor.HttpContext?
                .Session.GetString("CustomerCode");

            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session.");

            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ================= COMPANY ACT =================
            if (reportType == 1)
            {
                string companyActSql = @"
        SELECT
            AM.AM_Description AS AssetClass,

            -- COST
            SUM(CASE WHEN A.ADep_TransType = 1 THEN A.ADep_OPBForYR ELSE 0 END) AS CostBegYear,
            SUM(CASE WHEN A.ADep_TransType = 2 THEN A.ADep_OPBForYR ELSE 0 END) AS Additions,
            SUM(CASE WHEN A.ADep_TransType = 3 THEN A.ADep_OPBForYR ELSE 0 END) AS Deletions,
            SUM(A.ADep_OPBForYR) AS TotalCostEndYear,

            -- DEPRECIATION
            SUM(CASE WHEN A.ADep_TransType = 1 THEN A.ADep_DepreciationforFY ELSE 0 END) AS DepOnOpening,
            SUM(CASE WHEN A.ADep_TransType = 2 THEN A.ADep_DepreciationforFY ELSE 0 END) AS DepOnAdditions,
            SUM(CASE WHEN A.ADep_TransType = 3 THEN A.ADep_DepreciationforFY ELSE 0 END) AS DepOnDeletions,
            SUM(A.ADep_DepreciationforFY) AS TotalDepForYear,

            -- TOTAL DEPRECIATION AT END
            SUM(A.ADep_DepreciationforFY)
                + SUM(CASE WHEN A.ADep_TransType = 1 THEN A.ADep_OPBForYR ELSE 0 END)
              AS TotalDepEndYear,

            -- WDV
            SUM(A.ADep_OPBForYR) - SUM(A.ADep_DepreciationforFY) AS WDVBegYear,
            SUM(A.ADep_WrittenDownValue) AS WDVEndYear

        FROM Acc_AssetDepreciation A
        JOIN Acc_AssetMaster AM ON AM.AM_ID = A.ADep_AssetID
        WHERE
            A.ADep_YearID = @YearId
            AND A.ADep_CustId = @CustId
            AND A.ADep_CompID = @CompId
            AND A.ADep_Method = @MethodType
            AND A.ADep_DelFlag = 'X'
        GROUP BY
            AM.AM_Description
        ORDER BY
            AM.AM_Description;
        ";

                var data = await connection.QueryAsync<CompanyActReportRowDto>(
                    companyActSql,
                    new
                    {
                        YearId = yearId,
                        CustId = custId,
                        CompId = compId,
                        MethodType = methodType
                    });

                return data.ToList();
            }

            // ================= IT ACT (UNCHANGED) =================
            string itActSql = @"
    SELECT 
        AM.AM_Description AS AssetClass,
        SUM(ADITAct_WrittenDownValue) AS OpeningWDV,
        SUM(ADITAct_AftQtrAmount) AS AddGT180,
        SUM(ADITAct_BfrQtrAmount) AS AddLT180,
        SUM(ADITAct_DelAmount) AS Deletion,
        MAX(ADITAct_RateofDep) AS DepRate
    FROM Acc_AssetDepITAct A
    JOIN Acc_AssetMaster AM ON AM.AM_ID = A.ADITAct_AssetClassID
    WHERE A.ADITAct_YearID = @YearId
      AND A.ADITAct_CustId = @CustId
      AND A.ADITAct_CompID = @CompId
    GROUP BY AM.AM_Description;
    ";

            var rows = await connection.QueryAsync<dynamic>(
                itActSql,
                new
                {
                    YearId = yearId,
                    CustId = custId,
                    CompId = compId
                });

            var result = new List<FixedAssetReportRowDto>();

            decimal subOpening = 0, subAddGT = 0, subAddLT = 0,
                    subDel = 0, subDep = 0, subClose = 0;

            foreach (var r in rows)
            {
                decimal opening = r.OpeningWDV ?? 0;
                decimal addGT = r.AddGT180 ?? 0;
                decimal addLT = r.AddLT180 ?? 0;
                decimal deletion = r.Deletion ?? 0;
                decimal rate = r.DepRate ?? 0;

                decimal total = opening + addGT + addLT - deletion;

                decimal depAmount =
                    ((opening + addGT - deletion) * rate / 100)
                    + ((addLT * rate / 100) / 2);

                decimal closing = total - depAmount;

                result.Add(new FixedAssetReportRowDto
                {
                    AssetClass = r.AssetClass,
                    OpeningWDV = opening,
                    AdditionsGT180 = addGT,
                    AdditionsLT180 = addLT,
                    Deletions = deletion,
                    Total = total,
                    DepRate = rate,
                    DepForPeriod = depAmount,
                    ClosingWDV = closing,
                    IsSubTotal = false
                });

                subOpening += opening;
                subAddGT += addGT;
                subAddLT += addLT;
                subDel += deletion;
                subDep += depAmount;
                subClose += closing;
            }

            result.Add(new FixedAssetReportRowDto
            {
                AssetClass = "Sub Total",
                OpeningWDV = subOpening,
                AdditionsGT180 = subAddGT,
                AdditionsLT180 = subAddLT,
                Deletions = subDel,
                DepForPeriod = subDep,
                ClosingWDV = subClose,
                IsSubTotal = true
            });

            return result;
        }

        //Report(Go)
        //    public async Task<DataTable> LoadDynCompanyDetailedActAsync(
        //string nameSpace,
        //int compId,
        //int yearId,
        //int custId,
        //string locationIds,
        //string divisionIds,
        //string departmentIds,
        //string bayIds,
        //int assetClassId,
        //int transType,
        //int inAmount,
        //int roundOff)
        //    {
        //        var dt = new DataTable();
        //        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //        #region Columns
        //        dt.Columns.Add("ID");
        //        dt.Columns.Add("AssetClass");
        //        dt.Columns.Add("Asset");
        //        dt.Columns.Add("AssetCode");
        //        dt.Columns.Add("Location");
        //        dt.Columns.Add("Division");
        //        dt.Columns.Add("Department");
        //        dt.Columns.Add("Bay");
        //        dt.Columns.Add("Costasat");
        //        dt.Columns.Add("Additions");
        //        dt.Columns.Add("Deletions");
        //        dt.Columns.Add("DelReason");
        //        dt.Columns.Add("TotalAmount");
        //        dt.Columns.Add("ToDate");
        //        dt.Columns.Add("FromDate");
        //        dt.Columns.Add("DepUptoPY");
        //        dt.Columns.Add("DepOnOpengBal");
        //        dt.Columns.Add("DepOnAdditions");
        //        dt.Columns.Add("DepOnDeletions");
        //        dt.Columns.Add("TotalDepFY");
        //        dt.Columns.Add("TotalDepasOn");
        //        dt.Columns.Add("WDVasOn");
        //        dt.Columns.Add("WDVasOnPY");
        //        dt.Columns.Add("PHUpto");
        //        dt.Columns.Add("HTotalDep");
        //        dt.Columns.Add("HTotalDepreciationason");
        //        dt.Columns.Add("HWDVason");
        //        dt.Columns.Add("PHWDVason");
        //        dt.Columns.Add("Total");
        //        #endregion

        //        int prevYearId = yearId;
        //        int headerPrevYearId = yearId - 1;

        //        double oCost = 0, add = 0, del = 0, total = 0;
        //        double depUpto = 0, depOpen = 0, depAdd = 0, depFY = 0, depAsOn = 0;
        //        double wdv = 0, wdvPY = 0;

        //        double gOCost = 0, gAdd = 0, gDel = 0, gTotal = 0;
        //        double gDepUpto = 0, gDepOpen = 0, gDepAdd = 0, gDepFY = 0, gDepAsOn = 0;
        //        double gWDV = 0, gWDVPY = 0;

        //        string assetSql = @"
        //    SELECT AM_ID, AM_Description
        //    FROM Acc_AssetMaster
        //    WHERE AM_LevelCode = 2
        //      AND AM_CustId = @CustId
        //      AND AM_CompID = @CompId";

        //        if (assetClassId > 0)
        //            assetSql += " AND AM_ID = @AssetClassId";

        //        var assetClasses = await connection.QueryAsync(assetSql, new
        //        {
        //            CustId = custId,
        //            CompId = compId,
        //            AssetClassId = assetClassId
        //        });

        //        foreach (var cls in assetClasses)
        //        {
        //            var assets = await connection.QueryAsync(@"
        //        SELECT *
        //        FROM Acc_FixedAssetMaster
        //        WHERE AFAM_AssetClassID = @ClassId
        //          AND AFAM_YearID = @YearId
        //          AND AFAM_Location IN (" + locationIds + @")
        //          AND (@DivIds = '' OR AFAM_Division IN (" + divisionIds + @"))
        //          AND (@DeptIds = '' OR AFAM_Department IN (" + departmentIds + @"))
        //          AND (@BayIds = '' OR AFAM_Bay IN (" + bayIds + @"))",
        //                new
        //                {
        //                    ClassId = cls.AM_ID,
        //                    YearId = prevYearId,
        //                    DivIds = divisionIds,
        //                    DeptIds = departmentIds,
        //                    BayIds = bayIds
        //                });

        //            bool hasData = false;

        //            foreach (var asset in assets)
        //            {
        //                var item = await connection.QueryAsync(@"
        //            SELECT *
        //            FROM Acc_FixedAssetAdditionDel
        //            WHERE AFAA_AssetID = @AssetId
        //              AND AFAA_YearID = @YearId
        //              AND AFAA_TransType = @TransType",
        //                    new
        //                    {
        //                        AssetId = asset.AFAM_ID,
        //                        YearId = prevYearId,
        //                        TransType = transType
        //                    });

        //                var rowItem = item.FirstOrDefault();
        //                if (rowItem == null) continue;

        //                hasData = true;
        //                var dr = dt.NewRow();

        //                double orgCost = Convert.ToDouble(rowItem.OrgCost);
        //                if (orgCost == 0)
        //                {
        //                    orgCost = await connection.ExecuteScalarAsync<double>(
        //                        @"SELECT ISNULL(SUM(ADep_WrittenDownValue),0)
        //                  FROM Acc_AssetDepreciation
        //                  WHERE ADep_CompID=@CompId
        //                    AND ADep_YearID=@YearId
        //                    AND ADep_CustId=@CustId
        //                    AND ADep_Item=@ItemId",
        //                        new
        //                        {
        //                            CompId = compId,
        //                            YearId = prevYearId,
        //                            CustId = custId,
        //                            ItemId = asset.AFAM_ID
        //                        });
        //                }

        //                Func<double, string> fmt = v =>
        //                {
        //                    if (inAmount > 0) v /= inAmount;
        //                    return roundOff switch
        //                    {
        //                        0 => v.ToString("N0"),
        //                        1 => v.ToString("N1"),
        //                        2 => v.ToString("N2"),
        //                        3 => v.ToString("N3"),
        //                        4 => v.ToString("N4"),
        //                        _ => v.ToString()
        //                    };
        //                };

        //                double addAmt = Convert.ToDouble(rowItem.AddAmt);
        //                double delAmt = Convert.ToDouble(rowItem.DelAmt);
        //                double depPY = Convert.ToDouble(rowItem.AFAA_DepreAmount);
        //                double depFYAmt = Convert.ToDouble(rowItem.ADep_DepreciationforFY);
        //                double delDep = Convert.ToDouble(rowItem.AFAD_DelDeprec);

        //                dr["AssetClass"] = cls.AM_Description;
        //                dr["Asset"] = asset.AFAM_ItemDescription;
        //                dr["AssetCode"] = asset.AFAM_AssetCode;

        //                dr["Costasat"] = fmt(orgCost);
        //                dr["Additions"] = fmt(addAmt);
        //                dr["Deletions"] = fmt(delAmt);

        //                double totAmt = orgCost + addAmt - delAmt;
        //                dr["TotalAmount"] = fmt(totAmt);

        //                dr["DepUptoPY"] = fmt(depPY);

        //                if (rowItem.ADep_TransType == 1)
        //                {
        //                    double open = depFYAmt - delDep;
        //                    dr["DepOnOpengBal"] = fmt(open);
        //                    dr["DepOnAdditions"] = 0;
        //                    depOpen += open;
        //                }
        //                else
        //                {
        //                    dr["DepOnAdditions"] = fmt(depFYAmt);
        //                    dr["DepOnOpengBal"] = 0;
        //                    depAdd += depFYAmt;
        //                }

        //                dr["DepOnDeletions"] = fmt(delDep);

        //                double depYear = depFYAmt - delDep;
        //                dr["TotalDepFY"] = fmt(depYear);

        //                double depAs = depPY + depYear;
        //                dr["TotalDepasOn"] = fmt(depAs);

        //                double wdvAs = totAmt - depAs;
        //                dr["WDVasOn"] = fmt(wdvAs);

        //                double wdvPrev = orgCost - depPY;
        //                dr["WDVasOnPY"] = fmt(wdvPrev);

        //                dt.Rows.Add(dr);

        //                oCost += orgCost; add += addAmt; del += delAmt;
        //                total += totAmt; depUpto += depPY; depFY += depYear;
        //                depAsOn += depAs; wdv += wdvAs; wdvPY += wdvPrev;
        //            }

        //            if (hasData)
        //            {
        //                var tr = dt.NewRow();
        //                tr["AssetClass"] = "<b>Total</b>";
        //                tr["Costasat"] = "<b>" + Math.Round(oCost).ToString("#,##0") + "</b>";
        //                tr["Additions"] = "<b>" + Math.Round(add).ToString("#,##0") + "</b>";
        //                tr["Deletions"] = "<b>" + Math.Round(del).ToString("#,##0") + "</b>";
        //                tr["TotalAmount"] = "<b>" + Math.Round(total).ToString("#,##0") + "</b>";
        //                tr["DepUptoPY"] = "<b>" + Math.Round(depUpto).ToString("#,##0") + "</b>";
        //                tr["TotalDepFY"] = "<b>" + Math.Round(depFY).ToString("#,##0") + "</b>";
        //                tr["TotalDepasOn"] = "<b>" + Math.Round(depAsOn).ToString("#,##0") + "</b>";
        //                tr["WDVasOn"] = "<b>" + Math.Round(wdv).ToString("#,##0") + "</b>";
        //                tr["WDVasOnPY"] = "<b>" + Math.Round(wdvPY).ToString("#,##0") + "</b>";
        //                dt.Rows.Add(tr);

        //                gOCost += oCost; gAdd += add; gDel += del; gTotal += total;
        //                gDepUpto += depUpto; gDepFY += depFY; gDepAsOn += depAsOn;
        //                gWDV += wdv; gWDVPY += wdvPY;

        //                oCost = add = del = total = depUpto = depFY = depAsOn = wdv = wdvPY = 0;
        //            }
        //        }

        //        var gr = dt.NewRow();
        //        gr["AssetClass"] = "<b>Grand Total</b>";
        //        gr["Costasat"] = "<b>" + Math.Round(gOCost).ToString("#,##0") + "</b>";
        //        gr["Additions"] = "<b>" + Math.Round(gAdd).ToString("#,##0") + "</b>";
        //        gr["Deletions"] = "<b>" + Math.Round(gDel).ToString("#,##0") + "</b>";
        //        gr["TotalAmount"] = "<b>" + Math.Round(gTotal).ToString("#,##0") + "</b>";
        //        gr["DepUptoPY"] = "<b>" + Math.Round(gDepUpto).ToString("#,##0") + "</b>";
        //        gr["TotalDepFY"] = "<b>" + Math.Round(gDepFY).ToString("#,##0") + "</b>";
        //        gr["TotalDepasOn"] = "<b>" + Math.Round(gDepAsOn).ToString("#,##0") + "</b>";
        //        gr["WDVasOn"] = "<b>" + Math.Round(gWDV).ToString("#,##0") + "</b>";
        //        gr["WDVasOnPY"] = "<b>" + Math.Round(gWDVPY).ToString("#,##0") + "</b>";
        //        dt.Rows.Add(gr);

        //        return dt;
        //    }

        //    public async Task<IEnumerable<dynamic>> LoadDynComnyDetailedActAsync(
        //string nameSpace,
        //int acId,
        //int yearId,
        //int custId,
        //string locationIds,
        //string divisionIds,
        //string departmentIds,
        //string bayIds,
        //int assetClassId,
        //int transType,
        //int inAmount,
        //int roundOff)
        //    {
        //        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //        await connection.OpenAsync();

        //        var result = new List<dynamic>();

        //        // 1. Get Asset Classes
        //        var assetClassesSql = @"
        //    SELECT AM_ID, AM_Description 
        //    FROM Acc_AssetMaster 
        //    WHERE AM_LevelCode = 2 
        //      AND AM_CustId = @CustId 
        //      AND AM_COmpID = @AcId
        //      " + (assetClassId != 0 ? " AND AM_ID = @AssetClassId " : "");

        //        var assetClasses = await connection.QueryAsync(assetClassesSql, new { CustId = custId, AcId = acId, AssetClassId = assetClassId });

        //        double totalOriginalCost = 0;
        //        double totalAdditions = 0;
        //        double totalDeletions = 0;
        //        double totalAmount = 0;
        //        double totalDepUptoPY = 0;
        //        double totalDepOnOpenBal = 0;
        //        double totalDepOnAdditions = 0;
        //        double totalDepFY = 0;
        //        double totalDepAsOn = 0;
        //        double totalWDV = 0;
        //        double totalWDVPY = 0;

        //        foreach (var ac in assetClasses)
        //        {
        //            // 2. Load Asset IDs for this class
        //            var assetIds = await connection.QueryAsync<dynamic>(
        //                @"SELECT AFAM_ID, AFAM_ItemDescription, AFAM_AssetCode 
        //          FROM Acc_FixedAssetMaster 
        //          WHERE AFAM_AssetClassID = @AssetClassId", new { AssetClassId = ac.AM_ID });

        //            foreach (var asset in assetIds)
        //            {
        //                // 3. Load Asset Item details
        //                var item = await connection.QueryFirstOrDefaultAsync<dynamic>(
        //                    @"SELECT OrgCost, AddAmt, DelAmt, AFAD_AssetDeletion, AFAA_DepreAmount, 
        //                     ADep_DepreciationforFY, AFAD_DelDeprec,
        //                     AFAA_Location, AFAA_Division, AFAA_Department, AFAA_Bay
        //              FROM Acc_AssetAFAD
        //              WHERE AFAM_ID = @AssetId", new { AssetId = asset.AFAM_ID });

        //                // 4. Compute numeric values
        //                double costasat = item.OrgCost != null ? Convert.ToDouble(item.OrgCost) : 0;
        //                double additions = item.AddAmt != null ? Convert.ToDouble(item.AddAmt) : 0;
        //                double deletions = item.DelAmt != null ? Convert.ToDouble(item.DelAmt) : 0;
        //                double total = costasat + additions - deletions;
        //                double depUptoPY = item.AFAA_DepreAmount != null ? Convert.ToDouble(item.AFAA_DepreAmount) : 0;
        //                double depFY = (item.ADep_DepreciationforFY != null ? Convert.ToDouble(item.ADep_DepreciationforFY) : 0)
        //                             - (item.AFAD_DelDeprec != null ? Convert.ToDouble(item.AFAD_DelDeprec) : 0);
        //                double depAsOn = depUptoPY + depFY;
        //                double wdv = total - depAsOn;
        //                double wdvPY = costasat - depUptoPY;

        //                // 5. Add row
        //                result.Add(new
        //                {
        //                    AssetClass = ac.AM_Description,
        //                    Asset = asset.AFAM_ItemDescription,
        //                    AssetCode = asset.AFAM_AssetCode,
        //                    Costasat = Math.Round(costasat, roundOff),
        //                    Additions = Math.Round(additions, roundOff),
        //                    Deletions = Math.Round(deletions, roundOff),
        //                    TotalAmount = Math.Round(total, roundOff),
        //                    DepUptoPY = Math.Round(depUptoPY, roundOff),
        //                    DepOnOpengBal = Math.Round(depFY, roundOff), // simplified
        //                    DepOnAdditions = 0,
        //                    DepOnDeletions = 0,
        //                    TotalDepFY = Math.Round(depFY, roundOff),
        //                    TotalDepasOn = Math.Round(depAsOn, roundOff),
        //                    WDVasOn = Math.Round(wdv, roundOff),
        //                    WDVasOnPY = Math.Round(wdvPY, roundOff)
        //                });

        //                // 6. Accumulate totals
        //                totalOriginalCost += costasat;
        //                totalAdditions += additions;
        //                totalDeletions += deletions;
        //                totalAmount += total;
        //                totalDepUptoPY += depUptoPY;
        //                totalDepOnOpenBal += depFY;
        //                totalDepFY += depFY;
        //                totalDepAsOn += depAsOn;
        //                totalWDV += wdv;
        //                totalWDVPY += wdvPY;
        //            }
        //        }

        //        // 7. Add Grand Total
        //        result.Add(new
        //        {
        //            AssetClass = "Grand Total",
        //            Asset = "",
        //            AssetCode = "",
        //            Costasat = Math.Round(totalOriginalCost, roundOff),
        //            Additions = Math.Round(totalAdditions, roundOff),
        //            Deletions = Math.Round(totalDeletions, roundOff),
        //            TotalAmount = Math.Round(totalAmount, roundOff),
        //            DepUptoPY = Math.Round(totalDepUptoPY, roundOff),
        //            DepOnOpengBal = Math.Round(totalDepOnOpenBal, roundOff),
        //            DepOnAdditions = Math.Round(totalDepOnAdditions, roundOff),
        //           // DepOnDeletions = Math.Round(totalDepOnDeletions, roundOff),
        //            TotalDepFY = Math.Round(totalDepFY, roundOff),
        //            TotalDepasOn = Math.Round(totalDepAsOn, roundOff),
        //            WDVasOn = Math.Round(totalWDV, roundOff),
        //            WDVasOnPY = Math.Round(totalWDVPY, roundOff)
        //        });

        //        return result;
        //    }

        public async Task<List<CompanyDetailedActDto>> LoadDynComnyDetailedActAsync(
    string nameSpace,
    int acId,
    int yearId,
    int custId,
    string locationIds,
    string divisionIds,
    string departmentIds,
    string bayIds,
    int assetClassId,
    int transType,
    int inAmount,
    int roundOff)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var result = new List<CompanyDetailedActDto>();

            // Get asset classes
            var assetClassesSql = @"
        SELECT AM_ID, AM_Description 
        FROM Acc_AssetMaster 
        WHERE AM_LevelCode = 2 
          AND AM_CustId = @CustId 
          AND AM_CompID = @AcId" +
                  (assetClassId != 0 ? " AND AM_ID = @AssetClassId " : "");

            var assetClasses = await connection.QueryAsync(assetClassesSql,
                new { CustId = custId, AcId = acId, AssetClassId = assetClassId });

            decimal gCost = 0, gAdd = 0, gDel = 0, gTotal = 0;
            decimal gDepUpto = 0, gDepFY = 0, gDepAsOn = 0;
            decimal gWDV = 0, gWDVPY = 0;

            foreach (var ac in assetClasses)
            {
                // Get assets with your filter conditions
                var assetsSql = $@"
            SELECT AFAM_ID, AFAM_ItemDescription, AFAM_AssetCode
            FROM Acc_FixedAssetMaster
            WHERE AFAM_AssetClass = @AssetClassId
              AND AFAM_YearID = @YearId
              AND AFAM_Location IN ({locationIds})
              AND (@Div IS NULL OR AFAM_Division IN ({divisionIds}))
              AND (@Dept IS NULL OR AFAM_Department IN ({departmentIds}))
              AND (@Bay IS NULL OR AFAM_Bay IN ({bayIds}))";

                var assets = await connection.QueryAsync(assetsSql, new
                {
                    AssetClassId = ac.AM_ID,
                    YearId = yearId,
                    Div = string.IsNullOrEmpty(divisionIds) ? null : divisionIds,
                    Dept = string.IsNullOrEmpty(departmentIds) ? null : departmentIds,
                    Bay = string.IsNullOrEmpty(bayIds) ? null : bayIds
                });

                foreach (var asset in assets)
                {
                    // Get addition, deletion, depreciation
                    var item = await connection.QueryFirstOrDefaultAsync(@"
                SELECT
                    ISNULL(FM.AFAM_Cost,0) AS OrgCost,
                    ISNULL(ADDN.AddAmt,0) AS AddAmt,
                    ISNULL(DELN.DelAmt,0) AS DelAmt,
                    ISNULL(PREV.AFAA_DepreAmount,0) AS DepUptoPY,
                    ISNULL(CUR.ADep_DepreciationforFY,0) AS DepFY
                FROM Acc_FixedAssetMaster FM
                LEFT JOIN (
                    SELECT AFAD_AssetID, SUM(AFAD_AdditionAmount) AddAmt
                    FROM Acc_FixedAssetAdditionDetails
                    GROUP BY AFAD_AssetID
                ) ADDN ON ADDN.AFAD_AssetID = FM.AFAM_ID
                LEFT JOIN (
                    SELECT AFD_AssetID, SUM(AFD_DeletionAmount) DelAmt
                    FROM Acc_FixedAssetDeletion
                    GROUP BY AFD_AssetID
                ) DELN ON DELN.AFD_AssetID = FM.AFAM_ID
                LEFT JOIN Acc_AssetDepreciationPrev PREV
                    ON PREV.AFAA_AssetID = FM.AFAM_ID
                   AND PREV.AFAA_YearID < @YearId
                LEFT JOIN Acc_AssetDepreciation CUR
                    ON CUR.ADep_AssetID = FM.AFAM_ID
                   AND CUR.ADep_YearID = @YearId
                WHERE FM.AFAM_ID = @AssetId
            ", new { AssetId = asset.AFAM_ID, YearId = yearId });

                    decimal costasat = item?.OrgCost ?? 0;
                    decimal additions = item?.AddAmt ?? 0;
                    decimal deletions = item?.DelAmt ?? 0;
                    decimal total = costasat + additions - deletions;

                    decimal depUptoPY = item?.DepUptoPY ?? 0;
                    decimal depFY = item?.DepFY ?? 0;

                    decimal depAsOn = depUptoPY + depFY;
                    decimal wdv = total - depAsOn;
                    decimal wdvPY = costasat - depUptoPY;

                    result.Add(new CompanyDetailedActDto
                    {
                        AssetClass = ac.AM_Description,
                        Asset = asset.AFAM_ItemDescription,
                        AssetCode = asset.AFAM_AssetCode,

                        Costasat = Math.Round(costasat, roundOff),
                        Additions = Math.Round(additions, roundOff),
                        Deletions = Math.Round(deletions, roundOff),
                        TotalAmount = Math.Round(total, roundOff),

                        DepUptoPY = Math.Round(depUptoPY, roundOff),
                        DepOnOpengBal = Math.Round(depFY, roundOff),
                        DepOnAdditions = 0,
                        DepOnDeletions = 0,

                        TotalDepFY = Math.Round(depFY, roundOff),
                        TotalDepasOn = Math.Round(depAsOn, roundOff),

                        WDVasOn = Math.Round(wdv, roundOff),
                        WDVasOnPY = Math.Round(wdvPY, roundOff)
                    });

                    gCost += costasat;
                    gAdd += additions;
                    gDel += deletions;
                    gTotal += total;
                    gDepUpto += depUptoPY;
                    gDepFY += depFY;
                    gDepAsOn += depAsOn;
                    gWDV += wdv;
                    gWDVPY += wdvPY;
                }
            }

            // Grand Total Row
            result.Add(new CompanyDetailedActDto
            {
                AssetClass = "Grand Total",
                Asset = "",
                AssetCode = "",

                Costasat = Math.Round(gCost, roundOff),
                Additions = Math.Round(gAdd, roundOff),
                Deletions = Math.Round(gDel, roundOff),
                TotalAmount = Math.Round(gTotal, roundOff),

                DepUptoPY = Math.Round(gDepUpto, roundOff),
                DepOnOpengBal = Math.Round(gDepFY, roundOff),

                TotalDepFY = Math.Round(gDepFY, roundOff),
                TotalDepasOn = Math.Round(gDepAsOn, roundOff),

                WDVasOn = Math.Round(gWDV, roundOff),
                WDVasOnPY = Math.Round(gWDVPY, roundOff)
            });

            return result;
        }



        //----

        //public async Task<DataTable> LoadDynComDetailedReportAsync(
        //    string sNameSpace,
        //    int iACID,
        //    int iYearId,
        //    int iCustId,
        //    string iLocationId,
        //    string iDivId,
        //    string iDeptId,
        //    string iBayId,
        //    int iAsstCls,
        //    int iTransType,
        //    int iInAmt,
        //    int iRoundOff)
        //{
        //    using IDbConnection connection = new SqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    var dt = new DataTable();

        //    dt.Columns.AddRange(new DataColumn[]
        //    {
        //    new("ID"), new("AssetClass"), new("Asset"), new("AssetCode"),
        //    new("Location"), new("Division"), new("Department"), new("Bay"),
        //    new("Costasat"), new("Additions"), new("Deletions"), new("DelReason"),
        //    new("TotalAmount"), new("ToDate"), new("FromDate"),
        //    new("DepUptoPY"), new("DepOnOpengBal"), new("DepOnAdditions"),
        //    new("DepOnDeletions"), new("TotalDepFY"), new("TotalDepasOn"),
        //    new("WDVasOn"), new("WDVasOnPY"), new("PHUpto"),
        //    new("HTotalDep"), new("HTotalDepreciationason"),
        //    new("HWDVason"), new("PHWDVason")
        //    });

        //    double Format(double v) => iInAmt > 0 ? v / iInAmt : v;
        //    double Round(double v) => Math.Round(v, iRoundOff);

        //    string GetLoc(int id)
        //    {
        //        if (id <= 0) return string.Empty;

        //        return connection.ExecuteScalar<string>(
        //            @"SELECT ISNULL(LS_Description,'')
        //          FROM Acc_AssetLocationSetup
        //          WHERE LS_ID=@Id AND LS_CustId=@CustId",
        //            new { Id = id, CustId = iCustId }) ?? string.Empty;
        //    }

        //    var year = await connection.QueryFirstOrDefaultAsync<YearDto>(
        //        @"SELECT * FROM Year_Master WHERE YMS_YEARID=@YearId",
        //        new { YearId = iYearId });

        //    if (year == null)
        //        throw new Exception("Year not configured");

        //    var classes = await connection.QueryAsync<AssetClassDto>(
        //        @"SELECT AM_ID, AM_Description
        //      FROM Acc_AssetMaster
        //      WHERE AM_LevelCode = 2
        //      AND AM_CompID = @CompId
        //      AND AM_CustId = @CustId",
        //        new { CompId = iACID, CustId = iCustId });

        //    foreach (var cls in classes)
        //    {
        //        var assets = await connection.QueryAsync<AssetDto>(
        //            @"SELECT AFAM_ID, AFAM_ItemDescription, AFAM_AssetCode
        //          FROM Acc_AssetAFAM
        //          WHERE AFAM_AssetClassId = @ClsId",
        //            new { ClsId = cls.AM_ID });

        //        foreach (var asset in assets)
        //        {
        //            var item = await connection.QueryFirstOrDefaultAsync<AssetItemDto>(
        //                @"SELECT *
        //              FROM Acc_AssetItem
        //              WHERE AFAM_ID = @AssetId
        //              AND TransType = @TransType",
        //                new { AssetId = asset.AFAM_ID, TransType = iTransType });

        //            if (item == null) continue;

        //            var row = dt.NewRow();

        //            double orgCost = item.OrgCost ?? 0;
        //            double addAmt = item.AddAmt ?? 0;
        //            double delAmt = item.DelAmt ?? 0;
        //            double depPY = item.AFAA_DepreAmount ?? 0;
        //            double depFY = item.ADep_DepreciationforFY ?? 0;

        //            row["ID"] = 0;
        //            row["AssetClass"] = cls.AM_Description;
        //            row["Asset"] = asset.AFAM_ItemDescription;
        //            row["AssetCode"] = asset.AFAM_AssetCode;

        //            row["Costasat"] = Round(Format(orgCost));
        //            row["Additions"] = Round(Format(addAmt));
        //            row["Deletions"] = Round(Format(delAmt));
        //            row["TotalAmount"] = Round(Format(orgCost + addAmt - delAmt));

        //            row["DepUptoPY"] = Round(Format(depPY));
        //            row["TotalDepFY"] = Round(Format(depFY));
        //            row["TotalDepasOn"] = Round(Format(depPY + depFY));

        //            row["WDVasOn"] = Round(Format((orgCost + addAmt - delAmt) - (depPY + depFY)));
        //            row["WDVasOnPY"] = Round(Format(orgCost - depPY));

        //            row["FromDate"] = $"Cost As at {year.YMSFROMDATE:d}";
        //            row["ToDate"] = $"Total As at {year.YMSTODATE:d}";
        //            row["PHUpto"] = $"Up To {year.YMS_TODATE:d}";
        //            row["HTotalDep"] = $"Total Dep. {year.YMSID}";
        //            row["HTotalDepreciationason"] = $"Total Depreciation As On {year.YMSTODATE:d}";
        //            row["HWDVason"] = $"WDV As On {year.YMSTODATE:d}";
        //            row["PHWDVason"] = $"WDV As On {year.YMS_TODATE:d}";

        //            row["Location"] = GetLoc(item.AFAA_Location ?? 0);
        //            row["Division"] = GetLoc(item.AFAA_Division ?? 0);
        //            row["Department"] = GetLoc(item.AFAA_Department ?? 0);
        //            row["Bay"] = GetLoc(item.AFAA_Bay ?? 0);

        //            dt.Rows.Add(row);
        //        }
        //    }

        //    return dt;
        //}
    }

}



