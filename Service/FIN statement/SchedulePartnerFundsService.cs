using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.SchedulePartnerFundsDto;

namespace TracePca.Service.FIN_statement
{
    public class SchedulePartnerFundsService : SchedulePartnerFundsInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SchedulePartnerFundsService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetAllPartnershipFirms
        public async Task<IEnumerable<PartnershipFirmRowDto>> LoadAllPartnershipFirmsAsync(PartnershipFirmRequestDto request)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
    SELECT SSP_PartnerName, SSP_ShareOfProfit,
        ISNULL(SSP_CapitalAmount, 0) AS SSP_CapitalAmount,
        ISNULL(FYC.APF_OpeningBalance, 0) AS FYC_APF_OpeningBalance,
        ISNULL(FYP.APF_OpeningBalance, 0) AS FYP_APF_OpeningBalance,
        ISNULL(FYC.APF_UnsecuredLoanTreatedAsCapital, 0) AS FYC_APF_UnsecuredLoanTreatedAsCapital,
        ISNULL(FYP.APF_UnsecuredLoanTreatedAsCapital, 0) AS FYP_APF_UnsecuredLoanTreatedAsCapital,
        ISNULL(FYC.APF_InterestOnCapital, 0) AS FYC_APF_InterestOnCapital,
        ISNULL(FYP.APF_InterestOnCapital, 0) AS FYP_APF_InterestOnCapital,
        ISNULL(FYC.APF_PartnersSalary, 0) AS FYC_APF_PartnersSalary,
        ISNULL(FYP.APF_PartnersSalary, 0) AS FYP_APF_PartnersSalary,
        ISNULL(FYC.APF_ShareOfprofit, 0) AS FYC_APF_ShareOfprofit,
        ISNULL(FYP.APF_ShareOfprofit, 0) AS FYP_APF_ShareOfprofit,
        ISNULL(FYC.APF_CapitalAmount, 0) AS FYC_APF_CapitalAmount,
        ISNULL(FYP.APF_CapitalAmount, 0) AS FYP_APF_CapitalAmount,
        ISNULL(FYC.APF_AddOthers, 0) AS FYC_APF_AddOthers,
        ISNULL(FYP.APF_AddOthers, 0) AS FYP_APF_AddOthers,
        ISNULL(FYC.APF_TransferToFixedCapital, 0) AS FYC_APF_TransferToFixedCapital,
        ISNULL(FYP.APF_TransferToFixedCapital, 0) AS FYP_APF_TransferToFixedCapital,
        ISNULL(FYC.APF_Drawings, 0) AS FYC_APF_Drawings,
        ISNULL(FYP.APF_Drawings, 0) AS FYP_APF_Drawings,
        ISNULL(FYC.APF_LessOthers, 0) AS FYC_APF_LessOthers,
        ISNULL(FYP.APF_LessOthers, 0) AS FYP_APF_LessOthers
    FROM SAD_Statutory_PartnerDetails
    LEFT JOIN ACC_Partnership_Firms FYC ON FYC.APF_YearID = @FinancialYearID AND FYC.APF_Cust_ID = @CustomerID AND FYC.APF_Partner_ID = SSP_Id AND FYC.APF_CompID = 1
    LEFT JOIN ACC_Partnership_Firms FYP ON FYP.APF_YearID = @PreviousFinancialYearID AND FYP.APF_Cust_ID = @CustomerID AND FYP.APF_Partner_ID = SSP_Id AND FYP.APF_CompID = @AcID
    WHERE SSP_CustID = @CustomerID
    AND SSP_Id IN (SELECT DISTINCT(APF_Partner_ID) FROM ACC_Partnership_Firms WHERE APF_YearID = @FinancialYearID OR APF_YearID = @PreviousFinancialYearID)";

            var dtTab = (await connection.QueryAsync<dynamic>(query, new
            {
                FinancialYearID = request.iFinancialYearID,
                PreviousFinancialYearID = request.iFinancialYearID - 1,
                CustomerID = request.iCustomerID,
                AcID = 1
            })).ToList();

            var result = new List<PartnershipFirmRowDto>();

            int iSlNo = 0;
            decimal dFYCAdd = 0, dFYPAdd = 0, dFYCLess = 0, dFYPLess = 0;
            decimal dFYCTotal = 0, dFYPTotal = 0;

            if (dtTab.Any())
            {
                if (request.sIsReport == "No")
                {
                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "PARTICULARS",
                        FYCData = $"As at 31st March {request.sFY1}",
                        FYPData = $"As at 31st March {request.sFY2}"
                    });
                }

                foreach (var row in dtTab)
                {
                    iSlNo++;

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = iSlNo.ToString(),
                        Particulars = row.SSP_PartnerName.ToString().ToUpper(),
                        FYCData = "",
                        FYPData = ""
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Opening Balance",
                        FYCData = row.FYC_APF_OpeningBalance.ToString(),
                        FYPData = row.FYP_APF_OpeningBalance.ToString()
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Add: Capital Introduced - Unsecured Loan treated as Capital",
                        FYCData = row.FYC_APF_UnsecuredLoanTreatedAsCapital.ToString(),
                        FYPData = row.FYP_APF_UnsecuredLoanTreatedAsCapital.ToString()
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Add: Interest on Capital",
                        FYCData = row.FYC_APF_InterestOnCapital.ToString(),
                        FYPData = row.FYP_APF_InterestOnCapital.ToString()
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Add: Partner's Salary",
                        FYCData = row.FYC_APF_PartnersSalary.ToString(),
                        FYPData = row.FYP_APF_PartnersSalary.ToString()
                    });

                    string shareOfProfitPercent = row.SSP_ShareOfProfit != null ? row.SSP_ShareOfProfit.ToString() : "";
                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = $"Add: Share of profit({shareOfProfitPercent}%)",
                        FYCData = row.FYC_APF_ShareOfprofit.ToString(),
                        FYPData = row.FYP_APF_ShareOfprofit.ToString()
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Add: Others",
                        FYCData = row.FYC_APF_AddOthers.ToString(),
                        FYPData = row.FYP_APF_AddOthers.ToString()
                    });

                    dFYCAdd = Convert.ToDecimal(row.FYC_APF_OpeningBalance ?? 0) +
                               Convert.ToDecimal(row.FYC_APF_UnsecuredLoanTreatedAsCapital ?? 0) +
                               Convert.ToDecimal(row.FYC_APF_InterestOnCapital ?? 0) +
                               Convert.ToDecimal(row.FYC_APF_PartnersSalary ?? 0) +
                               Convert.ToDecimal(row.FYC_APF_ShareOfprofit ?? 0) +
                               Convert.ToDecimal(row.FYC_APF_AddOthers ?? 0);

                    dFYPAdd = Convert.ToDecimal(row.FYP_APF_OpeningBalance ?? 0) +
                               Convert.ToDecimal(row.FYP_APF_UnsecuredLoanTreatedAsCapital ?? 0) +
                               Convert.ToDecimal(row.FYP_APF_InterestOnCapital ?? 0) +
                               Convert.ToDecimal(row.FYP_APF_PartnersSalary ?? 0) +
                               Convert.ToDecimal(row.FYP_APF_ShareOfprofit ?? 0) +
                               Convert.ToDecimal(row.FYP_APF_AddOthers ?? 0);

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Add Total",
                        FYCData = dFYCAdd.ToString("#,##0.00"),
                        FYPData = dFYPAdd.ToString("#,##0.00")
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Less: Transfer to Fixed Capital",
                        FYCData = row.FYC_APF_TransferToFixedCapital.ToString(),
                        FYPData = row.FYP_APF_TransferToFixedCapital.ToString()
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Less: Drawings",
                        FYCData = row.FYC_APF_Drawings.ToString(),
                        FYPData = row.FYP_APF_Drawings.ToString()
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Less: Others",
                        FYCData = row.FYC_APF_LessOthers.ToString(),
                        FYPData = row.FYP_APF_LessOthers.ToString()
                    });

                    dFYCLess = Convert.ToDecimal(row.FYC_APF_TransferToFixedCapital ?? 0) +
                               Convert.ToDecimal(row.FYC_APF_Drawings ?? 0) +
                               Convert.ToDecimal(row.FYC_APF_LessOthers ?? 0);

                    dFYPLess = Convert.ToDecimal(row.FYP_APF_TransferToFixedCapital ?? 0) +
                               Convert.ToDecimal(row.FYP_APF_Drawings ?? 0) +
                               Convert.ToDecimal(row.FYP_APF_LessOthers ?? 0);

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Less Total",
                        FYCData = dFYCLess.ToString("#,##0.00"),
                        FYPData = dFYPLess.ToString("#,##0.00")
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "",
                        FYCData = "",
                        FYPData = ""
                    });

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "Total",
                        FYCData = (dFYCAdd - dFYCLess).ToString("#,##0.00"),
                        FYPData = (dFYPAdd - dFYPLess).ToString("#,##0.00")
                    });

                    dFYCTotal += (dFYCAdd - dFYCLess);
                    dFYPTotal += (dFYPAdd - dFYPLess);

                    dFYCAdd = 0;
                    dFYCLess = 0;
                    dFYPAdd = 0;
                    dFYPLess = 0;

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "",
                        FYCData = "",
                        FYPData = ""
                    });
                }

                result.Add(new PartnershipFirmRowDto
                {
                    SlNo = "",
                    Particulars = "TOTAL - CURRENT A/C CAPITAL",
                    FYCData = dFYCTotal.ToString("#,##0.00"),
                    FYPData = dFYPTotal.ToString("#,##0.00")
                });

                var fixedCapitalQuery = @"
    SELECT ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0) AS DbTotal,
           ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0) AS PrevDbTotal
    FROM Acc_TrailBalance_Upload_Details
    LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ATBUD_Subheading
    LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ATBUD_Description 
        AND d.ATBU_YEARId = @FinancialYearID AND d.ATBU_CustId = @CustomerID AND ATBUD_YEARId = @FinancialYearID
    LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ATBUD_Description
        AND e.ATBU_YEARId = @PreviousFinancialYearID AND e.ATBU_CustId = @CustomerID AND ATBUD_YEARId = @PreviousFinancialYearID
    WHERE ATBUD_Schedule_type = 4 AND ATBUD_Subheading = 160
";

                var fixedCapital = await connection.QueryFirstOrDefaultAsync<dynamic>(fixedCapitalQuery, new
                {
                    FinancialYearID = request.iFinancialYearID,
                    PreviousFinancialYearID = request.iFinancialYearID - 1,
                    CustomerID = request.iCustomerID
                });

                result.Add(new PartnershipFirmRowDto
                {
                    SlNo = "",
                    Particulars = "PARTNER'S FIXED CAPITAL",
                    FYCData = "",
                    FYPData = ""
                });

                if (request.sIsReport == "No")
                {
                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = "",
                        Particulars = "PARTICULARS",
                        FYCData = $"As at 31st March {request.sFY1}",
                        FYPData = $"As at 31st March {request.sFY2}"
                    });
                }

                // Add individual partner fixed capital
                decimal totalCapitalFYC = 0, totalCapitalFYP = 0;
                int slNo = 0;
                foreach (var row in dtTab)
                {
                    slNo++;
                    decimal fyc = Convert.ToDecimal(row.FYC_APF_CapitalAmount ?? 0);
                    decimal fyp = Convert.ToDecimal(row.FYP_APF_CapitalAmount ?? 0);
                    totalCapitalFYC += fyc;
                    totalCapitalFYP += fyp;

                    result.Add(new PartnershipFirmRowDto
                    {
                        SlNo = slNo.ToString(),
                        Particulars = row.SSP_PartnerName.ToString().ToUpper(),
                        FYCData = fyc.ToString("#,##0.00"),
                        FYPData = fyp.ToString("#,##0.00")
                    });
                }

                // Total fixed capital of partners
                result.Add(new PartnershipFirmRowDto
                {
                    SlNo = "",
                    Particulars = "Total",
                    FYCData = totalCapitalFYC.ToString("#,##0.00"),
                    FYPData = totalCapitalFYP.ToString("#,##0.00")
                });

                // Total capital including current A/C
                decimal grandTotalFYC = totalCapitalFYC + dFYCTotal;
                decimal grandTotalFYP = totalCapitalFYP + dFYPTotal;

                result.Add(new PartnershipFirmRowDto
                {
                    SlNo = "",
                    Particulars = "Total Capital",
                    FYCData = grandTotalFYC.ToString("#,##0.00"),
                    FYPData = grandTotalFYP.ToString("#,##0.00")
                });
            }

            return result;
        }

        //GetPartnernName
        public async Task<IEnumerable<PartnerDto>> LoadCustPartnerAsync(int custId, int compId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            SSP_Id AS Id, 
            SSP_PartnerName AS Name
        FROM SAD_Statutory_PartnerDetails
        WHERE SSP_CustID = @CustId 
          AND SSP_CompID = @CompId";

            return await connection.QueryAsync<PartnerDto>(query, new { CustId = custId, CompId = compId });
        }

        //SavePartnershipFirms
        public async Task<int[]> SavePartnershipFirmsAsync(SavePartnershipFirmDto objPF)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var cmd = new SqlCommand("spACC_Partnership_Firms", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input params
                        cmd.Parameters.AddWithValue("@APF_ID", objPF.iAPF_ID);
                        cmd.Parameters.AddWithValue("@APF_YearID", objPF.iAPF_YearID);
                        cmd.Parameters.AddWithValue("@APF_Cust_ID", objPF.iAPF_Cust_ID);
                        cmd.Parameters.AddWithValue("@APF_Branch_ID", objPF.iAPF_Branch_ID);
                        cmd.Parameters.AddWithValue("@APF_Partner_ID", objPF.iAPF_Partner_ID);
                        cmd.Parameters.AddWithValue("@APF_OpeningBalance", objPF.dAPF_OpeningBalance);
                        cmd.Parameters.AddWithValue("@APF_UnsecuredLoanTreatedAsCapital", objPF.dAPF_UnsecuredLoanTreatedAsCapital);
                        cmd.Parameters.AddWithValue("@APF_InterestOnCapital", objPF.dAPF_InterestOnCapital);
                        cmd.Parameters.AddWithValue("@APF_PartnersSalary", objPF.dAPF_PartnersSalary);
                        cmd.Parameters.AddWithValue("@APF_ShareOfprofit", objPF.dAPF_ShareOfprofit);
                        cmd.Parameters.AddWithValue("@APF_TransferToFixedCapital", objPF.dAPF_TransferToFixedCapital);
                        cmd.Parameters.AddWithValue("@APF_Drawings", objPF.dAPF_Drawings);
                        cmd.Parameters.AddWithValue("@APF_AddOthers", objPF.dAPF_AddOthers);
                        cmd.Parameters.AddWithValue("@APF_LessOthers", objPF.dAPF_LessOthers);
                        cmd.Parameters.AddWithValue("@APF_CapitalAmount", objPF.sAPF_CapitalAmount ?? string.Empty);
                        cmd.Parameters.AddWithValue("@APF_CrBy", objPF.iAPF_CrBy);
                        cmd.Parameters.AddWithValue("@APF_UpdateBy", objPF.iAPF_UpdateBy);
                        cmd.Parameters.AddWithValue("@APF_IPAddress", objPF.sAPF_IPAddress ?? string.Empty);
                        cmd.Parameters.AddWithValue("@APF_CompID", objPF.iAPF_CompID);

                        // Output params
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

                        // Execute
                        await cmd.ExecuteNonQueryAsync();

                        // Commit
                        transaction.Commit();

                        return new int[]
                        {
                    (int)(updateOrSaveParam.Value ?? 0),
                    (int)(operParam.Value ?? 0)
                        };
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        //UpdatePartnershipFirms
        public async Task<int[]> SaveOrUpdatePartnershipFirmAsync(UpdatePartnershipFirmDto dto)
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
                var parameters = new DynamicParameters();
                parameters.Add("@APF_ID", dto.APF_ID);  // <= If >0 → Update, else → Insert
                parameters.Add("@APF_YearID", dto.APF_YearID);
                parameters.Add("@APF_Cust_ID", dto.APF_Cust_ID);
                parameters.Add("@APF_Branch_ID", dto.APF_Branch_ID);
                parameters.Add("@APF_Partner_ID", dto.APF_Partner_ID);
                parameters.Add("@APF_OpeningBalance", dto.APF_OpeningBalance);
                parameters.Add("@APF_UnsecuredLoanTreatedAsCapital", dto.APF_UnsecuredLoanTreatedAsCapital);
                parameters.Add("@APF_InterestOnCapital", dto.APF_InterestOnCapital);
                parameters.Add("@APF_PartnersSalary", dto.APF_PartnersSalary);
                parameters.Add("@APF_ShareOfProfit", dto.APF_ShareOfProfit);
                parameters.Add("@APF_TransferToFixedCapital", dto.APF_TransferToFixedCapital);
                parameters.Add("@APF_Drawings", dto.APF_Drawings);
                parameters.Add("@APF_AddOthers", dto.APF_AddOthers);
                parameters.Add("@APF_LessOthers", dto.APF_LessOthers);
                parameters.Add("@APF_CapitalAmount", dto.APF_CapitalAmount ?? "");
                parameters.Add("@APF_CrBy", dto.APF_CrBy);
                parameters.Add("@APF_UpdateBy", dto.APF_UpdateBy);
                parameters.Add("@APF_IPAddress", dto.APF_IPAddress ?? "");
                parameters.Add("@APF_CompID", dto.APF_CompID);

                // Output parameters from SP
                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "spACC_Partnership_Firms",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction
                );

                transaction.Commit();

                return new int[]
                {
            parameters.Get<int>("@iUpdateOrSave"),
            parameters.Get<int>("@iOper")
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //GetSelectedPartnershipFirms
        public async Task<IEnumerable<SelectedPartnershipFirmRowDto>> LoadSelectedPartnershipFirmAsync(int partnershipFirmId, int compId, int yearId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"SELECT * 
      FROM ACC_Partnership_Firms 
      WHERE APF_Partner_ID = @PartnershipFirmId AND APF_YearID= @YearId
        AND APF_CompID = @CompId";

            return await connection.QueryAsync<SelectedPartnershipFirmRowDto>(query, new { PartnershipFirmId = partnershipFirmId, CompId = compId, YearId = yearId });

        }

        //UpdateAndCalculate
        public async Task<decimal> UpdateAndCalculateAsync(PartnershipFirmCalculationDto dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        UPDATE ACC_Partnership_Firms
        SET 
            APF_OpeningBalance = @OpeningBalance,
            APF_UnsecuredLoanTreatedAsCapital = @UnsecuredLoanTreatedAsCapital,
            APF_InterestOnCapital = @InterestOnCapital,
            APF_PartnersSalary = @PartnersSalary,
            APF_ShareOfProfit = @ShareOfProfit,
            APF_AddOthers = @AddOthers
        WHERE APF_ID = @PartnershipFirmId AND APF_CompID = @CompId";

            await connection.ExecuteAsync(query, dto);

            // Calculate total
            decimal total = dto.OpeningBalance
                             + dto.UnsecuredLoanTreatedAsCapital
                             + dto.InterestOnCapital
                             + dto.PartnersSalary
                             + dto.ShareOfProfit
                             + dto.AddOthers;
            return total;
        }

        //GetCustomerPartnerDetails
        public async Task<IEnumerable<PartnerDetailsDto>> GetCustomerPartnerDetailsAsync(int compId, int custId, int custPartnerPkId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            SSP_Id      AS PartnerPkID,
            SSP_PartnerName AS Name,
            SSP_DOJ    AS DOJ,
            SSP_PAN    AS PAN,
            ISNULL(SSP_ShareOfProfit,0) AS ShareOfProfit,
            ISNULL(SSP_CapitalAmount,0) AS CapitalAmount,
            SSP_DelFlag AS Status
        FROM SAD_Statutory_PartnerDetails
        WHERE SSP_CustID = @CustId 
          AND SSP_CompID = @CompId
          AND (@CustPartnerPkId = 0 OR SSP_Id = @CustPartnerPkId)";

            return await connection.QueryAsync<PartnerDetailsDto>(query, new
            {
                CustId = custId,
                CompId = compId,
                CustPartnerPkId = custPartnerPkId
            });
        }

        //SaveCustomerStatutoryPartner
        public async Task<int[]> SaveOrUpdateCustomerStatutoryPartnerAsync(SaveCustomerStatutoryPartnerDto partnerDto)
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
                if (partnerDto.SSP_Id > 0)
                {
                    // 🔹 Do direct UPDATE instead of calling SP
                    var query = @"
                UPDATE SAD_Statutory_PartnerDetails
                SET SSP_CustID = @SSP_CustID,
                    SSP_PartnerName = @SSP_PartnerName,
                    SSP_PAN = @SSP_PAN,
                    SSP_ShareOfProfit = @SSP_ShareOfProfit,
                    SSP_CapitalAmount = @SSP_CapitalAmount,
                    SSP_CRBY = @SSP_CRBY,
                    SSP_UpdatedOn = @SSP_UpdatedOn,
                    SSP_UpdatedBy = @SSP_UpdatedBy,
                    SSP_DelFlag = @SSP_DelFlag,
                    SSP_STATUS = @SSP_STATUS,
                    SSP_IPAddress = @SSP_IPAddress,
                    SSP_CompID = @SSP_CompID
                WHERE SSP_Id = @SSP_Id";

                    await connection.ExecuteAsync(query, partnerDto, transaction);
                    transaction.Commit();

                    // return [2 = updated, 1 = success]
                    return new int[] { 2, 1 };
                }
                else
                {
                    // 🔹 Use existing SP for INSERT
                    var parameters = new DynamicParameters();
                    parameters.Add("@SSP_Id", partnerDto.SSP_Id);
                    parameters.Add("@SSP_CustID", partnerDto.SSP_CustID);
                    parameters.Add("@SSP_PartnerName", partnerDto.SSP_PartnerName ?? "");
                    parameters.Add("@SSP_DOJ", partnerDto.SSP_DOJ);
                    parameters.Add("@SSP_PAN", partnerDto.SSP_PAN ?? "");
                    parameters.Add("@SSP_ShareOfProfit", partnerDto.SSP_ShareOfProfit);
                    parameters.Add("@SSP_CapitalAmount", partnerDto.SSP_CapitalAmount);
                    parameters.Add("@SSP_CRON", partnerDto.SSP_CRON);
                    parameters.Add("@SSP_CRBY", partnerDto.SSP_CRBY);
                    parameters.Add("@SSP_UpdatedOn", partnerDto.SSP_UpdatedOn);
                    parameters.Add("@SSP_UpdatedBy", partnerDto.SSP_UpdatedBy);
                    parameters.Add("@SSP_DelFlag", partnerDto.SSP_DelFlag ?? "");
                    parameters.Add("@SSP_STATUS", partnerDto.SSP_STATUS ?? "");
                    parameters.Add("@SSP_IPAddress", partnerDto.SSP_IPAddress ?? "");
                    parameters.Add("@SSP_CompID", partnerDto.SSP_CompID);

                    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spSAD_Statutory_PartnerDetails",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        transaction: transaction
                    );

                    transaction.Commit();

                    return new int[]
                    {
                parameters.Get<int>("@iUpdateOrSave"),
                parameters.Get<int>("@iOper")
                    };
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}


