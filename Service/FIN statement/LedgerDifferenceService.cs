using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;
namespace TracePca.Service.FIN_statement
{
    public class LedgerDifferenceService : ILedgerDifferenceInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LedgerDifferenceService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetDescriptionWiseDetails
        public async Task<IEnumerable<DescriptionWiseDetailsDto>> GetDescriptionWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId)
        {
            {
                // ✅ Step 1: Get DB name from session
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Get the connection string
                var connectionString = _configuration.GetConnectionString(dbName);

                // ✅ Step 3: Use SqlConnection
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                {
                    string sql = string.Empty;

                    if (typeId == 1)
                    {
                        //                        sql = @" Select ATBUD_Description as headingname, ATBUD_Masid as headingId,
                        //                      abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                        //                      abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                        //               CASE   WHEN ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0) = 0
                        //AND ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount), 0) <> 0
                        // THEN  ABS(ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount), 0))
                        // WHEN ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount), 0) = 0
                        // AND ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0) <> 0
                        //THEN ABS(ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0))
                        //WHEN SIGN(ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount), 0))
                        //<> SIGN(ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0))
                        //THEN ABS(ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount), 0))
                        //+ ABS(ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0))
                        //ELSE ABS( ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount), 0)
                        //- ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0))END AS Difference_Amt,
                        //CASE WHEN ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0) = 0 
                        //THEN 0 ELSE 
                        //ABS(ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount), 0)
                        //- ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0)
                        // )  / ABS(ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount), 0))
                        //END AS Difference_Avg,
                        //isnull (d.ATBU_Closing_TotalCredit_Amount,0) As cyCr,isnull (d.ATBU_Closing_TotalDebit_Amount,0) As cydb,
                        //isnull (e.ATBU_Closing_TotalCredit_Amount,0) As pyCr,isnull (e.ATBU_Closing_TotalDebit_Amount,0) As pydb,
                        //                       d.ATBU_STATUS as Status
                        //                  From Acc_TrailBalance_Upload_Details
                        //                      Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                        //                          And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId 
                        //                          And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                        //                          And d.Atbu_Branchid=@BranchId
                        //                      Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                        //                          And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                        //                          And ATBUD_YEARId=@YearId And e.ATBU_Branchid=Atbud_Branchnameid  
                        //                          And e.Atbu_Branchid=@BranchId
                        //                  WHERE ATBUD_CustId = @CustId
                        //                        AND Atbud_Branchnameid = @BranchId
                        //                        AND atbud_yearid = @YearId
                        //                  GROUP BY ATBUD_Description, ATBUD_Masid, d.ATBU_STATUS
                        //,d.ATBU_Closing_TotalCredit_Amount,d.ATBU_Closing_TotalDebit_Amount,
                        //e.ATBU_Closing_TotalCredit_Amount,e.ATBU_Closing_TotalDebit_Amount ";
                        sql = @"  
                               WITH Base AS( SELECT   a.ATBUD_Description AS HeadingName,
        a.ATBUD_Masid       AS HeadingId,
        SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount) AS CY_Net,
        SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount) AS PY_Net,
        SUM(ISNULL(d.ATBU_Closing_TotalCredit_Amount,0)) AS CyCr,
        SUM(ISNULL(d.ATBU_Closing_TotalDebit_Amount,0))  AS CyDb,
        SUM(ISNULL(e.ATBU_Closing_TotalCredit_Amount,0)) AS PyCr,
        SUM(ISNULL(e.ATBU_Closing_TotalDebit_Amount,0))  AS PyDb,
        MAX(d.ATBU_STATUS) AS Status
    FROM Acc_TrailBalance_Upload_Details a
    LEFT JOIN Acc_TrailBalance_Upload d
    ON d.ATBU_Description = a.ATBUD_Description  AND d.ATBU_YearId = @YearId   AND d.ATBU_CustId = @CustId  AND d.ATBU_BranchId = @BranchId
    LEFT JOIN Acc_TrailBalance_Upload e
    ON e.ATBU_Description = a.ATBUD_Description AND e.ATBU_YearId = @PrevYearId  AND e.ATBU_CustId = @CustId    AND e.ATBU_BranchId = @BranchId
    WHERE a.ATBUD_CustId = @CustId      AND a.ATBUD_BranchNameId = @BranchId     AND a.ATBUD_YearId = @YearId
    GROUP BY a.ATBUD_Description, a.ATBUD_Masid)
	SELECT HeadingName, HeadingId,ABS(CY_Net) AS CYamt, ABS(PY_Net) AS PYamt,
    CASE WHEN PY_Net = 0 AND CY_Net <> 0 THEN ABS(CY_Net) WHEN CY_Net = 0 AND PY_Net <> 0 THEN ABS(PY_Net)
        WHEN SIGN(CY_Net) <> SIGN(PY_Net) THEN ABS(CY_Net) + ABS(PY_Net)  ELSE ABS(CY_Net - PY_Net)   END AS Difference_Amt,
   CASE WHEN PY_Net = 0 THEN 0    ELSE    ( CASE
   WHEN SIGN(CY_Net) <> SIGN(PY_Net)  THEN ABS(CY_Net) + ABS(PY_Net)   ELSE ABS(CY_Net - PY_Net)  END ) * 100.0 / ABS(PY_Net)
    END AS Difference_Avg,  CyCr, CyDb, PyCr, PyDb, Status FROM Base;";
                    }
                    else if (typeId == 2)
                    {
                        sql = @" Select a.ASH_ID as headingId, ASH_Name headingname, 
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) as CYamt,
                            abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as PYamt,
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) -
                                abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as Difference_Amt,
                            (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) /
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg,
sum(isnull (d.ATBU_Closing_TotalCredit_Amount,0)) As cyCr,sum(isnull (d.ATBU_Closing_TotalDebit_Amount,0)) As cydb,
sum(isnull(e.ATBU_Closing_TotalCredit_Amount,0)) As pyCr,sum(isnull(e.ATBU_Closing_TotalDebit_Amount,0)) As pydb
                        from Acc_TrailBalance_Upload_Details
                            left join ACC_ScheduleHeading a on a.ASH_ID= ATBUD_Headingid
                            left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                                And d.ATBU_YEARId=@YearId and d.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and d.ATBU_Branchid=Atbud_Branchnameid  
                                And d.Atbu_Branchid=@BranchId
                            left join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                                And e.ATBU_YEARId=@PrevYearId and e.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and e.ATBU_Branchid=Atbud_Branchnameid  
                                And e.Atbu_Branchid=@BranchId
                        WHERE ATBUD_CustId = @CustId
                             AND ATBUD_Headingid <> 0
                             AND Atbud_Branchnameid = @BranchId
                             AND atbud_yearid = @YearId
                        GROUP BY ATBUD_Headingid, ASH_Name, a.ASH_ID
                        ORDER BY ATBUD_Headingid";
                    }
                    else if (typeId == 3)
                    {
                        sql = @" Select a.ASSH_ID as headingId, ASSH_Name headingname, 
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) as CYamt,
                            abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as PYamt,
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) -
                                abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as Difference_Amt,
                            (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) /
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg,
sum(isnull(d.ATBU_Closing_TotalCredit_Amount,0)) As cyCr,sum(isnull(d.ATBU_Closing_TotalDebit_Amount,0)) As cydb,
sum(isnull(e.ATBU_Closing_TotalCredit_Amount,0)) As pyCr,sum(isnull(e.ATBU_Closing_TotalDebit_Amount,0)) As pydb
                        from Acc_TrailBalance_Upload_Details
                            left join ACC_SchedulesubHeading a on a.ASsH_ID= ATBUD_subHeading
                            left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                                And d.ATBU_YEARId=@YearId and d.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and d.ATBU_Branchid=Atbud_Branchnameid  
                                And d.Atbu_Branchid=@BranchId
                            left join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                                And e.ATBU_YEARId=@PrevYearId and e.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and e.ATBU_Branchid=Atbud_Branchnameid  
                                And e.Atbu_Branchid=@BranchId
                        WHERE ATBUD_CustId = @CustId
                            AND ATBUD_Headingid <> 0
                            AND Atbud_Branchnameid = @BranchId
                            AND atbud_yearid = @YearId
                        GROUP BY ATBUD_Headingid, ASSH_Name, a.ASSH_ID";
                    }
                    else if (typeId == 4)
                    {
                        sql = @" Select a.ASI_ID as headingId, ASI_Name headingname, 
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) as CYamt,
                            abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as PYamt,
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) -
                                abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as Difference_Amt,
                            (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) /
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg,
sum(isnull(d.ATBU_Closing_TotalCredit_Amount,0)) As cyCr,sum(isnull(d.ATBU_Closing_TotalDebit_Amount,0)) As cydb,
sum(isnull(e.ATBU_Closing_TotalCredit_Amount,0)) As pyCr,sum(isnull(e.ATBU_Closing_TotalDebit_Amount,0))As pydb
                        from Acc_TrailBalance_Upload_Details
                            left join ACC_ScheduleItems a on a.ASI_ID= ATBUD_itemid
                            left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                                And d.ATBU_YEARId=@YearId and d.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and d.ATBU_Branchid=Atbud_Branchnameid  
                                And d.Atbu_Branchid=@BranchId
                            left join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                                And e.ATBU_YEARId=@PrevYearId and e.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and e.ATBU_Branchid=Atbud_Branchnameid  
                                And e.Atbu_Branchid=@BranchId
                        WHERE ATBUD_CustId = @CustId
                           AND ATBUD_Headingid <> 0
                           AND Atbud_Branchnameid = @BranchId
                           AND ATBUD_ItemId <> 0
                           AND atbud_yearid = @YearId
                        GROUP BY ATBUD_Headingid, ASI_Name, a.ASI_ID";
                    }
                    else if (typeId == 5)
                    {
                        sql = @" Select a.ASSI_ID as headingId, ASSI_Name headingname, 
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) as CYamt,
                            abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as PYamt,
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) -
                                abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as Difference_Amt,
                            (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) /
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg,
sum(isnull(d.ATBU_Closing_TotalCredit_Amount,0)) As cyCr,sum(isnull(d.ATBU_Closing_TotalDebit_Amount,0)) As cydb,
sum(isnull(e.ATBU_Closing_TotalCredit_Amount,0)) As pyCr,sum(isnull(e.ATBU_Closing_TotalDebit_Amount,0)) As pydb
                        from Acc_TrailBalance_Upload_Details
                            left join ACC_ScheduleSUBItems a on a.ASSI_ID= ATBUD_SUBitemid
                            left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                                And d.ATBU_YEARId=@YearId and d.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and d.ATBU_Branchid=Atbud_Branchnameid  
                                And d.Atbu_Branchid=@BranchId
                            left join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                                And e.ATBU_YEARId=@PrevYearId and e.ATBU_CustId=@CustId
                                and ATBUD_YEARId=@YearId and e.ATBU_Branchid=Atbud_Branchnameid  
                                And e.Atbu_Branchid=@BranchId
                        WHERE ATBUD_CustId = @CustId
                             AND ATBUD_Headingid <> 0
                             AND Atbud_Branchnameid = @BranchId
                             AND ATBUD_SubItemId <> 0
                             AND atbud_yearid = @YearId
                        GROUP BY ATBUD_Headingid, ASSI_Name, a.ASSI_ID";
                    }

                    var result = await connection.QueryAsync<DescriptionWiseDetailsDto>(sql, new
                    {
                        CustId = custId,
                        BranchId = branchId,
                        YearId = yearId,
                        PrevYearId = yearId - 1
                    });

                    return result;
                }
            }
        }

        //UpdateDescriptionWiseDetailsStatus
        public async Task<int> UpdateTrailBalanceStatusAsync(List<UpdateDescriptionWiseDetailsStatusDto> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
                return 0; // Nothing to update

            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Build parameterized CASE statement
                var caseStatements = dtoList
                    .Select((d, i) => $"WHEN @Id{i} THEN @Status{i}")
                    .ToList();

                var sql = $@"
          UPDATE Acc_TrailBalance_Upload
          SET ATBU_STATUS = CASE ATBU_ID
              {string.Join(" ", caseStatements)}
          END
          WHERE ATBU_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"Status{i}", dtoList[i].Status);
                }

                var updatedCount = await connection.ExecuteAsync(sql, parameters, transaction);

                transaction.Commit();
                return updatedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //GetAccountDetails
        public async Task<IEnumerable<DescriptionDetailsDto>> GetAccountDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId, int pkId)
        {
            {
                // ✅ Step 1: Get DB name from session
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Get the connection string
                var connectionString = _configuration.GetConnectionString(dbName);

                // ✅ Step 3: Use SqlConnection
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                {
                    string sql = string.Empty;
                    if (typeId == 2)
                    {
                        //int headingIdCA = await GetHeadingId(conn, tran, custId, "2 Current Assets");
                        //int headingIdCL = await GetHeadingId(conn, tran, custId, "4 Current Liabilities");


                        //                        sql = @" Select ATBUD_Description as headingname, ATBUD_Masid as headingId,
                        //                      abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                        //                      abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                        //                      abs(abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) -
                        //                          abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)))   As Difference_Amt,
                        //                      (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) /
                        //                          NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)), 0))   As Difference_Avg,
                        //isnull (d.ATBU_Closing_TotalCredit_Amount,0) As cyCr,isnull (d.ATBU_Closing_TotalDebit_Amount,0) As cydb,
                        //isnull (e.ATBU_Closing_TotalCredit_Amount,0) As pyCr,isnull (e.ATBU_Closing_TotalDebit_Amount,0) As pydb,
                        //                       d.ATBU_STATUS as Status
                        //                  From Acc_TrailBalance_Upload_Details
                        //                      Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                        //                          And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId 
                        //                          And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                        //                          And d.Atbu_Branchid=@BranchId
                        //                      Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                        //                          And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                        //                          And ATBUD_YEARId=@YearId And e.ATBU_Branchid=Atbud_Branchnameid  
                        //                          And e.Atbu_Branchid=@BranchId
                        //                  WHERE ATBUD_CustId = @CustId
                        //                        AND Atbud_Branchnameid = @BranchId
                        //                        AND atbud_yearid = @YearId
                        //                        AND ATBUD_Headingid = @iPkId
                        //                  GROUP BY ATBUD_Description, ATBUD_Masid, d.ATBU_STATUS
                        //,d.ATBU_Closing_TotalCredit_Amount,d.ATBU_Closing_TotalDebit_Amount,
                        //e.ATBU_Closing_TotalCredit_Amount,e.ATBU_Closing_TotalDebit_Amount ";

                        sql = @"  
                               WITH Base AS( SELECT   a.ATBUD_Description AS HeadingName,
        a.ATBUD_Masid       AS HeadingId,
        SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount) AS CY_Net,
        SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount) AS PY_Net,
        SUM(ISNULL(d.ATBU_Closing_TotalCredit_Amount,0)) AS CyCr,
        SUM(ISNULL(d.ATBU_Closing_TotalDebit_Amount,0))  AS CyDb,
        SUM(ISNULL(e.ATBU_Closing_TotalCredit_Amount,0)) AS PyCr,
        SUM(ISNULL(e.ATBU_Closing_TotalDebit_Amount,0))  AS PyDb,
        MAX(d.ATBU_STATUS) AS Status
    FROM Acc_TrailBalance_Upload_Details a
    LEFT JOIN Acc_TrailBalance_Upload d
    ON d.ATBU_Description = a.ATBUD_Description  AND d.ATBU_YearId = @YearId   AND d.ATBU_CustId = @CustId  AND d.ATBU_BranchId = @BranchId
    LEFT JOIN Acc_TrailBalance_Upload e
    ON e.ATBU_Description = a.ATBUD_Description AND e.ATBU_YearId = @PrevYearId  AND e.ATBU_CustId = @CustId    AND e.ATBU_BranchId = @BranchId
    WHERE a.ATBUD_CustId = @CustId      AND a.ATBUD_BranchNameId = @BranchId     AND a.ATBUD_YearId = @YearId and a.ATBUD_Headingid = @iPkId
    GROUP BY a.ATBUD_Description, a.ATBUD_Masid)
	SELECT HeadingName, HeadingId,ABS(CY_Net) AS CYamt, ABS(PY_Net) AS PYamt,
    CASE WHEN PY_Net = 0 AND CY_Net <> 0 THEN ABS(CY_Net) WHEN CY_Net = 0 AND PY_Net <> 0 THEN ABS(PY_Net)
        WHEN SIGN(CY_Net) <> SIGN(PY_Net) THEN ABS(CY_Net) + ABS(PY_Net)  ELSE ABS(CY_Net - PY_Net)   END AS Difference_Amt,
   CASE WHEN PY_Net = 0 THEN 0    ELSE    ( CASE
   WHEN SIGN(CY_Net) <> SIGN(PY_Net)  THEN ABS(CY_Net) + ABS(PY_Net)   ELSE ABS(CY_Net - PY_Net)  END ) * 100.0 / ABS(PY_Net)
    END AS Difference_Avg,  CyCr, CyDb, PyCr, PyDb, Status FROM Base;";
                    }
                    else if (typeId == 3)
                    {
                        sql = @"  
                               WITH Base AS( SELECT   a.ATBUD_Description AS HeadingName,
        a.ATBUD_Masid       AS HeadingId,
        SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount) AS CY_Net,
        SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount) AS PY_Net,
        SUM(ISNULL(d.ATBU_Closing_TotalCredit_Amount,0)) AS CyCr,
        SUM(ISNULL(d.ATBU_Closing_TotalDebit_Amount,0))  AS CyDb,
        SUM(ISNULL(e.ATBU_Closing_TotalCredit_Amount,0)) AS PyCr,
        SUM(ISNULL(e.ATBU_Closing_TotalDebit_Amount,0))  AS PyDb,
        MAX(d.ATBU_STATUS) AS Status
    FROM Acc_TrailBalance_Upload_Details a
    LEFT JOIN Acc_TrailBalance_Upload d
    ON d.ATBU_Description = a.ATBUD_Description  AND d.ATBU_YearId = @YearId   AND d.ATBU_CustId = @CustId  AND d.ATBU_BranchId = @BranchId
    LEFT JOIN Acc_TrailBalance_Upload e
    ON e.ATBU_Description = a.ATBUD_Description AND e.ATBU_YearId = @PrevYearId  AND e.ATBU_CustId = @CustId    AND e.ATBU_BranchId = @BranchId
    WHERE a.ATBUD_CustId = @CustId      AND a.ATBUD_BranchNameId = @BranchId     AND a.ATBUD_YearId = @YearId  AND ATBUD_Subheading = @iPkId
    GROUP BY a.ATBUD_Description, a.ATBUD_Masid)
	SELECT HeadingName, HeadingId,ABS(CY_Net) AS CYamt, ABS(PY_Net) AS PYamt,
    CASE WHEN PY_Net = 0 AND CY_Net <> 0 THEN ABS(CY_Net) WHEN CY_Net = 0 AND PY_Net <> 0 THEN ABS(PY_Net)
        WHEN SIGN(CY_Net) <> SIGN(PY_Net) THEN ABS(CY_Net) + ABS(PY_Net)  ELSE ABS(CY_Net - PY_Net)   END AS Difference_Amt,
   CASE WHEN PY_Net = 0 THEN 0    ELSE    ( CASE
   WHEN SIGN(CY_Net) <> SIGN(PY_Net)  THEN ABS(CY_Net) + ABS(PY_Net)   ELSE ABS(CY_Net - PY_Net)  END ) * 100.0 / ABS(PY_Net)
    END AS Difference_Avg,  CyCr, CyDb, PyCr, PyDb, Status FROM Base;";
                    }
                    else if (typeId == 4)
                    {
                        sql = @"  
                               WITH Base AS( SELECT   a.ATBUD_Description AS HeadingName,
        a.ATBUD_Masid       AS HeadingId,
        SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount) AS CY_Net,
        SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount) AS PY_Net,
        SUM(ISNULL(d.ATBU_Closing_TotalCredit_Amount,0)) AS CyCr,
        SUM(ISNULL(d.ATBU_Closing_TotalDebit_Amount,0))  AS CyDb,
        SUM(ISNULL(e.ATBU_Closing_TotalCredit_Amount,0)) AS PyCr,
        SUM(ISNULL(e.ATBU_Closing_TotalDebit_Amount,0))  AS PyDb,
        MAX(d.ATBU_STATUS) AS Status
    FROM Acc_TrailBalance_Upload_Details a
    LEFT JOIN Acc_TrailBalance_Upload d
    ON d.ATBU_Description = a.ATBUD_Description  AND d.ATBU_YearId = @YearId   AND d.ATBU_CustId = @CustId  AND d.ATBU_BranchId = @BranchId
    LEFT JOIN Acc_TrailBalance_Upload e
    ON e.ATBU_Description = a.ATBUD_Description AND e.ATBU_YearId = @PrevYearId  AND e.ATBU_CustId = @CustId    AND e.ATBU_BranchId = @BranchId
    WHERE a.ATBUD_CustId = @CustId      AND a.ATBUD_BranchNameId = @BranchId     AND a.ATBUD_YearId = @YearId  AND ATBUD_itemid = @iPkId
    GROUP BY a.ATBUD_Description, a.ATBUD_Masid)
	SELECT HeadingName, HeadingId,ABS(CY_Net) AS CYamt, ABS(PY_Net) AS PYamt,
    CASE WHEN PY_Net = 0 AND CY_Net <> 0 THEN ABS(CY_Net) WHEN CY_Net = 0 AND PY_Net <> 0 THEN ABS(PY_Net)
        WHEN SIGN(CY_Net) <> SIGN(PY_Net) THEN ABS(CY_Net) + ABS(PY_Net)  ELSE ABS(CY_Net - PY_Net)   END AS Difference_Amt,
   CASE WHEN PY_Net = 0 THEN 0    ELSE    ( CASE
   WHEN SIGN(CY_Net) <> SIGN(PY_Net)  THEN ABS(CY_Net) + ABS(PY_Net)   ELSE ABS(CY_Net - PY_Net)  END ) * 100.0 / ABS(PY_Net)
    END AS Difference_Avg,  CyCr, CyDb, PyCr, PyDb, Status FROM Base;";
                    }
                    else if (typeId == 5)
                    {
                        sql = @"  
                               WITH Base AS( SELECT   a.ATBUD_Description AS HeadingName,
        a.ATBUD_Masid       AS HeadingId,
        SUM(d.ATBU_Closing_TotalDebit_Amount - d.ATBU_Closing_TotalCredit_Amount) AS CY_Net,
        SUM(e.ATBU_Closing_TotalDebit_Amount - e.ATBU_Closing_TotalCredit_Amount) AS PY_Net,
        SUM(ISNULL(d.ATBU_Closing_TotalCredit_Amount,0)) AS CyCr,
        SUM(ISNULL(d.ATBU_Closing_TotalDebit_Amount,0))  AS CyDb,
        SUM(ISNULL(e.ATBU_Closing_TotalCredit_Amount,0)) AS PyCr,
        SUM(ISNULL(e.ATBU_Closing_TotalDebit_Amount,0))  AS PyDb,
        MAX(d.ATBU_STATUS) AS Status
    FROM Acc_TrailBalance_Upload_Details a
    LEFT JOIN Acc_TrailBalance_Upload d
    ON d.ATBU_Description = a.ATBUD_Description  AND d.ATBU_YearId = @YearId   AND d.ATBU_CustId = @CustId  AND d.ATBU_BranchId = @BranchId
    LEFT JOIN Acc_TrailBalance_Upload e
    ON e.ATBU_Description = a.ATBUD_Description AND e.ATBU_YearId = @PrevYearId  AND e.ATBU_CustId = @CustId    AND e.ATBU_BranchId = @BranchId
    WHERE a.ATBUD_CustId = @CustId      AND a.ATBUD_BranchNameId = @BranchId     AND a.ATBUD_YearId = @YearId    AND ATBUD_SubItemId = @iPkId
    GROUP BY a.ATBUD_Description, a.ATBUD_Masid)
	SELECT HeadingName, HeadingId,ABS(CY_Net) AS CYamt, ABS(PY_Net) AS PYamt,
    CASE WHEN PY_Net = 0 AND CY_Net <> 0 THEN ABS(CY_Net) WHEN CY_Net = 0 AND PY_Net <> 0 THEN ABS(PY_Net)
        WHEN SIGN(CY_Net) <> SIGN(PY_Net) THEN ABS(CY_Net) + ABS(PY_Net)  ELSE ABS(CY_Net - PY_Net)   END AS Difference_Amt,
   CASE WHEN PY_Net = 0 THEN 0    ELSE    ( CASE
   WHEN SIGN(CY_Net) <> SIGN(PY_Net)  THEN ABS(CY_Net) + ABS(PY_Net)   ELSE ABS(CY_Net - PY_Net)  END ) * 100.0 / ABS(PY_Net)
    END AS Difference_Avg,  CyCr, CyDb, PyCr, PyDb, Status FROM Base;";
                    }
                    
                    var result = await connection.QueryAsync<DescriptionDetailsDto>(sql, new
                    {
                        CustId = custId,
                        BranchId = branchId,
                        YearId = yearId,
                        PrevYearId = yearId - 1,
                        iPkId= pkId
                    });
                    return result;
                }
            }
        }

        #region DB Helper Methods

        private async Task<int> GetHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string headName)
        {
            const string sql = @"SELECT ISNULL(ASH_ID,0) FROM ACC_ScheduleHeading WHERE ASH_Name = @HeadName AND ASH_OrgType = @CustId";
            return await conn.ExecuteScalarAsync<int>(sql, new { HeadName = headName, CustId = customerId }, tran);
        }

        private async Task<int> GetSubHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string subHeadName)
        {
            const string sql = @"SELECT ISNULL(ASSH_ID,0) FROM ACC_SchedulesubHeading WHERE ASSH_Name = @SubHeadName AND ASSH_OrgType = @CustId";
            return await conn.ExecuteScalarAsync<int>(sql, new { SubHeadName = subHeadName, CustId = customerId }, tran);
        }

        private async Task<ScheduleAccountingRatioDto.HeadingAmount> GetHeadingAmt(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int headingId)
        {
            if (headingId == 0) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            // Ensure SQL names match your schema — adapted for safety
            var sql = @"
                SELECT 
                  ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                  ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_HeadingId = @HeadingId
            ";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, HeadingId = headingId }, tran);

            if (row == null) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            decimal dc1 = row.Dc1 == null ? 0m : Convert.ToDecimal(row.Dc1);
            decimal dp1 = row.DP1 == null ? 0m : Convert.ToDecimal(row.DP1);

            return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = dc1, DP1 = dp1 };
        }

        private async Task<ScheduleAccountingRatioDto.HeadingAmount> GetSubHeadingAmt(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int subHeadingId)
        {
            if (subHeadingId == 0) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            var sql = @"
                SELECT 
                  ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                  ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_SubHeading = @SubHeadingId
            ";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, SubHeadingId = subHeadingId }, tran);

            if (row == null) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            decimal dc1 = row.Dc1 == null ? 0m : Convert.ToDecimal(row.Dc1);
            decimal dp1 = row.DP1 == null ? 0m : Convert.ToDecimal(row.DP1);

            return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = dc1, DP1 = dp1 };
        }

        private async Task<decimal> GetPandLFinalAmt(SqlConnection conn, SqlTransaction tran, int yearId, int customerId, int branchId)
        {
            const string sql = @"
                SELECT ISNULL(SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount), 0)
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_Description = 'Net Income' AND ATBU_YearId = @YearId AND ATBU_CustId = @CustomerId AND ATBU_BranchId = @BranchId
            ";
            var value = await conn.ExecuteScalarAsync<decimal?>(sql, new { YearId = yearId, CustomerId = customerId, BranchId = branchId }, tran);
            return value ?? 0m;
        }

        #endregion

        //GetCustomerTBGrid
        //    public async Task<CustCOATrialBalanceResult> GetCustCOAMasterDetailsCustomerAsync(int compId, int custId, int yearId, int scheduleTypeId, int unmapped, int branchId)
        //    {
        //        // 1. Get DB from session
        //        string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //        if (string.IsNullOrEmpty(dbName))
        //            throw new Exception("CustomerCode missing in session.");

        //        // 2. Connection string
        //        var connectionString = _configuration.GetConnectionString(dbName);

        //        using var connection = new SqlConnection(connectionString);
        //        await connection.OpenAsync();

        //        // 3. SQL (preserved logic, parameterized)
        //        var sql = @"
        ///* ===================== RESULT SET 1 ===================== */
        //SELECT  
        //    ATBU.ATBU_Id,
        //    ATBU.ATBU_Description AS DescriptionCode,

        //    SUM(CASE WHEN ATBU.ATBU_QuarterId = 1 
        //        THEN ROUND(CAST(ATBU.ATBU_Opening_Debit_Amount AS DECIMAL(19,2)),0) ELSE 0 END) AS OpeningDebit,

        //    SUM(CASE WHEN ATBU.ATBU_QuarterId = 1 
        //        THEN ROUND(CAST(ATBU.ATBU_Opening_Credit_Amount AS DECIMAL(19,2)),0) ELSE 0 END) AS OpeningCredit,

        //    ROUND(ISNULL(Tr.AJTB_Debit,0),0) AS TrDebit,
        //    ROUND(ISNULL(Tr.AJTB_Credit,0),0) AS TrCredit,

        //    ROUND(ISNULL(SUM(Cs.ATBCU_Opening_Debit_Amount),0),0) AS CustOpeningDebit,
        //    ROUND(ISNULL(SUM(Cs.ATBCU_Opening_Credit_Amount),0),0) AS CustOpeningCredit,
        //    ROUND(ISNULL(SUM(Cs.ATBCU_TR_Debit_Amount),0),0) AS CustTrDebit,
        //    ROUND(ISNULL(SUM(Cs.ATBCU_TR_Credit_Amount),0),0) AS CustTrCredit,
        //    ROUND(ISNULL(SUM(Cs.ATBCU_Closing_TotalDebit_Amount),0),0) AS CustClosingDebit,
        //    ROUND(ISNULL(SUM(Cs.ATBCU_Closing_TotalCredit_Amount),0),0) AS CustClosingCredit,

        //    SUM(CASE 
        //        WHEN ATBU.ATBU_QuarterId = 1 THEN ATBU.ATBU_Closing_TotalDebit_Amount
        //        WHEN a.ATBUD_SChedule_Type = 3 AND ATBU.ATBU_YearId = @YearId 
        //        THEN ATBU.ATBU_Closing_TotalDebit_Amount
        //        ELSE 0 END) AS ClosingDebit,

        //    SUM(CASE 
        //        WHEN ATBU.ATBU_QuarterId = 1 THEN ATBU.ATBU_Closing_TotalCredit_Amount
        //        WHEN a.ATBUD_SChedule_Type = 3 AND ATBU.ATBU_YearId = @YearId 
        //        THEN ATBU.ATBU_Closing_TotalCredit_Amount
        //        ELSE 0 END) AS ClosingCredit

        //FROM Acc_TrailBalance_Upload ATBU

        //LEFT JOIN (
        //    SELECT ATBUD_Description, ATBUD_SChedule_Type
        //    FROM Acc_TrailBalance_Upload_Details
        //    WHERE ATBUD_CustId = @CustId
        //      AND ATBUD_CompId = @CompId
        //      AND ATBUD_YearId = @YearId
        //      AND ATBUD_BranchNameId = @BranchId
        //    GROUP BY ATBUD_Description, ATBUD_SChedule_Type
        //) a ON a.ATBUD_Description = ATBU.ATBU_Description

        //LEFT JOIN (
        //    SELECT AJTB_DescName,
        //           SUM(AJTB_Debit) AS AJTB_Debit,
        //           SUM(AJTB_Credit) AS AJTB_Credit
        //    FROM Acc_JETransactions_Details
        //    WHERE AJTB_CustId = @CustId
        //      AND AJTB_BranchId = @BranchId
        //      AND AJTB_YearID = @YearId
        //    GROUP BY AJTB_DescName
        //) Tr ON Tr.AJTB_DescName = ATBU.ATBU_Description

        //LEFT JOIN (
        //    SELECT ATBCU_Description,
        //           SUM(ATBCU_TR_Debit_Amount) AS ATBCU_TR_Debit_Amount,
        //           SUM(ATBCU_TR_Credit_Amount) AS ATBCU_TR_Credit_Amount,
        //           SUM(ATBCU_Opening_Debit_Amount) AS ATBCU_Opening_Debit_Amount,
        //           SUM(ATBCU_Opening_Credit_Amount) AS ATBCU_Opening_Credit_Amount,
        //           SUM(ATBCU_Closing_TotalDebit_Amount) AS ATBCU_Closing_TotalDebit_Amount,
        //           SUM(ATBCU_Closing_TotalCredit_Amount) AS ATBCU_Closing_TotalCredit_Amount
        //    FROM Acc_TrailBalance_CustomerUpload
        //    WHERE ATBCU_CustId = @CustId
        //      AND ATBCU_BranchId = @BranchId
        //      AND ATBCU_YearID = @YearId
        //    GROUP BY ATBCU_Description
        //) Cs ON Cs.ATBCU_Description = ATBU.ATBU_Description

        //WHERE ATBU.ATBU_CustId = @CustId
        //  AND ATBU.ATBU_CompId = @CompId
        //  AND ATBU.ATBU_YearId = @YearId
        //  AND ATBU.ATBU_BranchId = @BranchId

        //GROUP BY ATBU.ATBU_Id, ATBU.ATBU_Description, Tr.AJTB_Debit, Tr.AJTB_Credit
        //ORDER BY ATBU.ATBU_Id;

        ///* ===================== RESULT SET 2 ===================== */
        //SELECT *
        //FROM Acc_TrailBalance_CustomerUpload
        //WHERE ATBCU_MasId = 0
        //  AND ATBCU_YearId = @YearId
        //  AND ATBCU_CustId = @CustId
        //  AND (@BranchId = 0 OR ATBCU_BranchId = @BranchId);

        ///* ===================== RESULT SET 3 ===================== */
        //SELECT
        //    ROUND(SUM(ATBCU_Opening_Debit_Amount),0) AS OpeningDebit,
        //    ROUND(SUM(ATBCU_Opening_Credit_Amount),0) AS OpeningCredit,
        //    ROUND(SUM(ATBCU_TR_Debit_Amount),0) AS TrDebit,
        //    ROUND(SUM(ATBCU_TR_Credit_Amount),0) AS TrCredit,
        //    ROUND(SUM(ATBCU_Closing_TotalDebit_Amount),0) AS ClosingDebit,
        //    ROUND(SUM(ATBCU_Closing_TotalCredit_Amount),0) AS ClosingCredit
        //FROM Acc_TrailBalance_CustomerUpload
        //WHERE ATBCU_CustId = @CustId
        //  AND ATBCU_YearId = @YearId
        //  AND ATBCU_BranchId = @BranchId;

        ///* ===================== RESULT SET 4 ===================== */
        //SELECT *
        //FROM Acc_TrailBalance_Upload a
        //WHERE a.ATBU_CustId = @CustId
        //  AND a.ATBU_CompId = @CompId
        //  AND a.ATBU_YearId = @YearId
        //  AND a.ATBU_BranchId = @BranchId
        //  AND a.ATBU_Description <> 'Net income';
        //";

        //        // 4. Execute as multiple result sets
        //        using var multi = await connection.QueryMultipleAsync(sql, new
        //        {
        //            CompId = compId,
        //            CustId = custId,
        //            YearId = yearId,
        //            BranchId = branchId
        //        });

        //        return new CustCOATrialBalanceResult
        //        {
        //            MainTrailBalance = (await multi.ReadAsync<dynamic>()).ToList(),
        //            UnmappedCustomerUpload = (await multi.ReadAsync<dynamic>()).ToList(),
        //            CustomerTotals = await multi.ReadFirstOrDefaultAsync<dynamic>(),
        //            SystemTotals = (await multi.ReadAsync<dynamic>()).ToList()
        //        };
        //    }

        public async Task<DataSet> GetCustCoaAsync(CustCoaRequestFlaggedDto request)
        {
            var ds = new DataSet();

            // ✅ Get DB from Session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode missing in session.");

            string connectionString = _configuration.GetConnectionString(dbName);

            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = @"
        /* ===========================
           QUERY 1 – SYSTEM DATA
        ============================*/
        SELECT  
            ATBU.ATBU_Description AS DescriptionCode,

            SUM(CASE WHEN ATBU.ATBU_QuarterId = 1 
                THEN ATBU.ATBU_Opening_Debit_Amount ELSE 0 END) AS OpeningDebit,

            SUM(CASE WHEN ATBU.ATBU_QuarterId = 1 
                THEN ATBU.ATBU_Opening_Credit_Amount ELSE 0 END) AS OpeningCredit,

            ROUND(ISNULL(Tr.AJTB_Debit,0),0) AS TrDebit,
            ROUND(ISNULL(Tr.AJTB_Credit,0),0) AS TrCredit,

            SUM(ATBU.ATBU_Closing_TotalDebit_Amount) AS ClosingDebit,
            SUM(ATBU.ATBU_Closing_TotalCredit_Amount) AS ClosingCredit

        FROM Acc_TrailBalance_Upload ATBU
        LEFT JOIN (
            SELECT AJTB_DescName,
                   SUM(AJTB_Debit) AS AJTB_Debit,
                   SUM(AJTB_Credit) AS AJTB_Credit
            FROM Acc_JETransactions_Details
            WHERE AJTB_CustId = @CustId
              AND AJTB_YearID = @YearId
              AND AJTB_BranchId = @BranchId
            GROUP BY AJTB_DescName
        ) Tr ON Tr.AJTB_DescName = ATBU.ATBU_Description

        WHERE ATBU.ATBU_CustId = @CustId
          AND ATBU.ATBU_YearId = @YearId
          AND ATBU.ATBU_BranchId = @BranchId      
        GROUP BY ATBU.ATBU_Description, Tr.AJTB_Debit, Tr.AJTB_Credit;

        /* ===========================
           QUERY 2 – CUSTOMER UPLOAD
        ============================*/
        SELECT 
            ATBCU_ID AS ATBU_Id,
            ATBCU_Description AS DescriptionCode,
            ROUND(ATBCU_Opening_Debit_Amount,0) AS CustOpeningDebit,
            ROUND(ATBCU_Opening_Credit_Amount,0) AS CustOpeningCredit,
            ROUND(SUM(ATBCU_TR_Debit_Amount),0) AS CustTrDebit,
            ROUND(SUM(ATBCU_TR_Credit_Amount),0) AS CustTrCredit,
            ROUND(ATBCU_Closing_TotalDebit_Amount,0) AS CustClosingDebit,
            ROUND(ATBCU_Closing_TotalCredit_Amount,0) AS CustClosingCredit,
            ATBCU_DELFLG AS flagStatus
        FROM Acc_TrailBalance_CustomerUpload
        WHERE ATBCU_CustId = @CustId
          AND ATBCU_YearId = @YearId
          AND ATBCU_BranchId = @BranchId AND ATBCU_DELFLG = 'F'
        GROUP BY 
            ATBCU_ID,
            ATBCU_Description,
            ATBCU_Opening_Debit_Amount,
            ATBCU_Opening_Credit_Amount,
            ATBCU_Closing_TotalDebit_Amount,
            ATBCU_Closing_TotalCredit_Amount,
            ATBCU_DELFLG;
        ";

            // ✅ Parameters
            cmd.Parameters.AddWithValue("@CustId", request.CustId);
            cmd.Parameters.AddWithValue("@YearId", request.YearId);
            cmd.Parameters.AddWithValue("@BranchId", request.BranchId);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            await Task.Run(() => da.Fill(ds));

            ds.Tables[0].TableName = "SystemData";
            ds.Tables[1].TableName = "CustomerUpload";

            return ds;
        }
        public async Task<DataSet> GetCustCOAMasterDetailsCustomerAsync(CustCoaRequestDto r)
        {
            var ds = new DataSet();

            // 1️⃣ Get DB from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            string sql = string.Empty;

            /* ===========================
               QUERY 1 – SYSTEM DATA
            ============================*/
            sql += @"
SELECT  
    ATBU.ATBU_Description AS DescriptionCode,

    SUM(CASE WHEN ATBU.ATBU_QuarterId = 1 
        THEN ATBU.ATBU_Opening_Debit_Amount ELSE 0 END) AS OpeningDebit,

    SUM(CASE WHEN ATBU.ATBU_QuarterId = 1 
        THEN ATBU.ATBU_Opening_Credit_Amount ELSE 0 END) AS OpeningCredit,

    ROUND(ISNULL(Tr.AJTB_Debit,0),0) AS TrDebit,
    ROUND(ISNULL(Tr.AJTB_Credit,0),0) AS TrCredit,

    SUM(ATBU.ATBU_Closing_TotalDebit_Amount) AS ClosingDebit,
    SUM(ATBU.ATBU_Closing_TotalCredit_Amount) AS ClosingCredit

FROM Acc_TrailBalance_Upload ATBU
LEFT JOIN (
    SELECT AJTB_DescName,
           SUM(AJTB_Debit) AS AJTB_Debit,
           SUM(AJTB_Credit) AS AJTB_Credit
    FROM Acc_JETransactions_Details
    WHERE AJTB_CustId = @CustId
      AND AJTB_YearID = @YearId
      AND AJTB_BranchId = @BranchId
    GROUP BY AJTB_DescName
) Tr ON Tr.AJTB_DescName = ATBU.ATBU_Description
WHERE ATBU.ATBU_CustId = @CustId
  AND ATBU.ATBU_YearId = @YearId
  AND ATBU.ATBU_BranchId = @BranchId 
GROUP BY ATBU.ATBU_Description, Tr.AJTB_Debit, Tr.AJTB_Credit;
";

            /* ===========================
               QUERY 2 – CUSTOMER UPLOAD
            ============================*/
            sql += @"
SELECT 
    ATBCU_ID AS ATBU_Id,
    ATBCU_Description AS DescriptionCode,
    ROUND(ATBCU_Opening_Debit_Amount,0) AS CustOpeningDebit,
    ROUND(ATBCU_Opening_Credit_Amount,0) AS CustOpeningCredit,
    ROUND(SUM(ATBCU_TR_Debit_Amount),0) AS CustTrDebit,
    ROUND(SUM(ATBCU_TR_Credit_Amount),0) AS CustTrCredit,
    ROUND(ATBCU_Closing_TotalDebit_Amount,0) AS CustClosingDebit,
    ROUND(ATBCU_Closing_TotalCredit_Amount,0) AS CustClosingCredit,
    ATBCU_DELFLG AS flagStatus
FROM Acc_TrailBalance_CustomerUpload
WHERE ATBCU_CustId = @CustId
  AND ATBCU_YearId = @YearId
  AND ATBCU_BranchId = @BranchId
GROUP BY 
    ATBCU_ID,
    ATBCU_Description,
    ATBCU_Opening_Debit_Amount,
    ATBCU_Opening_Credit_Amount,
    ATBCU_Closing_TotalDebit_Amount,
    ATBCU_Closing_TotalCredit_Amount,
    ATBCU_DELFLG;
";

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@CustId", r.CustomerId);
            cmd.Parameters.AddWithValue("@YearId", r.FinancialYearId);
            cmd.Parameters.AddWithValue("@BranchId", r.BranchId);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            await Task.Run(() => da.Fill(ds));

            /* ===========================
               MATCH + MERGE LOGIC
            ============================*/
            if (ds.Tables.Count < 2)
                return ds;

            DataTable systemDt = ds.Tables[0];
            DataTable custDt = ds.Tables[1];

            // 🔥 FAST LOOKUP
            var custLookup = custDt.AsEnumerable()
                .ToDictionary(rw => rw.Field<string>("DescriptionCode"));

            DataTable resultTable = new DataTable("ComparisonResult");

            // System columns
            resultTable.Columns.Add("ATBU_Id", typeof(int));
            resultTable.Columns.Add("DescriptionCode", typeof(string));
            resultTable.Columns.Add("OpeningDebit", typeof(decimal));
            resultTable.Columns.Add("OpeningCredit", typeof(decimal));
            resultTable.Columns.Add("TrDebit", typeof(decimal));
            resultTable.Columns.Add("TrCredit", typeof(decimal));
            resultTable.Columns.Add("ClosingDebit", typeof(decimal));
            resultTable.Columns.Add("ClosingCredit", typeof(decimal));

            // Customer columns
            resultTable.Columns.Add("CustOpeningDebit", typeof(decimal));
            resultTable.Columns.Add("CustOpeningCredit", typeof(decimal));
            resultTable.Columns.Add("CustTrDebit", typeof(decimal));
            resultTable.Columns.Add("CustTrCredit", typeof(decimal));
            resultTable.Columns.Add("CustClosingDebit", typeof(decimal));
            resultTable.Columns.Add("CustClosingCredit", typeof(decimal));
            resultTable.Columns.Add("flagStatus", typeof(string));

            // Status columns
            resultTable.Columns.Add("OpeningMatchStatus", typeof(string));
            resultTable.Columns.Add("TransactionMatchStatus", typeof(string));
            resultTable.Columns.Add("ClosingMatchStatus", typeof(string));
            resultTable.Columns.Add("OverallMatchStatus", typeof(string));

            // Variance columns
            resultTable.Columns.Add("OpeningDebitVariance", typeof(decimal));
            resultTable.Columns.Add("OpeningCreditVariance", typeof(decimal));
            resultTable.Columns.Add("TrDebitVariance", typeof(decimal));
            resultTable.Columns.Add("TrCreditVariance", typeof(decimal));
            resultTable.Columns.Add("ClosingDebitVariance", typeof(decimal));
            resultTable.Columns.Add("ClosingCreditVariance", typeof(decimal));

            foreach (DataRow sysRow in systemDt.Rows)
            {
                string desc = sysRow["DescriptionCode"]?.ToString();
                var newRow = resultTable.NewRow();

                decimal sysOpeningDebit = GetDecimal(sysRow["OpeningDebit"]);
                decimal sysOpeningCredit = GetDecimal(sysRow["OpeningCredit"]);
                decimal sysTrDebit = GetDecimal(sysRow["TrDebit"]);
                decimal sysTrCredit = GetDecimal(sysRow["TrCredit"]);
                decimal sysClosingDebit = GetDecimal(sysRow["ClosingDebit"]);
                decimal sysClosingCredit = GetDecimal(sysRow["ClosingCredit"]);

                newRow["DescriptionCode"] = desc;
                newRow["OpeningDebit"] = sysOpeningDebit;
                newRow["OpeningCredit"] = sysOpeningCredit;
                newRow["TrDebit"] = sysTrDebit;
                newRow["TrCredit"] = sysTrCredit;
                newRow["ClosingDebit"] = sysClosingDebit;
                newRow["ClosingCredit"] = sysClosingCredit;

                if (!custLookup.TryGetValue(desc, out DataRow custRow))
                {
                    newRow["ATBU_Id"] = 0;
                    newRow["flagStatus"] = "U";

                    newRow["CustOpeningDebit"] = 0;
                    newRow["CustOpeningCredit"] = 0;
                    newRow["CustTrDebit"] = 0;
                    newRow["CustTrCredit"] = 0;
                    newRow["CustClosingDebit"] = 0;
                    newRow["CustClosingCredit"] = 0;

                    newRow["OpeningMatchStatus"] = "Not Found";
                    newRow["TransactionMatchStatus"] = "Not Found";
                    newRow["ClosingMatchStatus"] = "Not Found";
                    newRow["OverallMatchStatus"] = "Not Found";

                    newRow["OpeningDebitVariance"] = sysOpeningDebit;
                    newRow["OpeningCreditVariance"] = sysOpeningCredit;
                    newRow["TrDebitVariance"] = sysTrDebit;
                    newRow["TrCreditVariance"] = sysTrCredit;
                    newRow["ClosingDebitVariance"] = sysClosingDebit;
                    newRow["ClosingCreditVariance"] = sysClosingCredit;
                }
                else
                {
                    decimal custOpeningDebit = GetDecimal(custRow["CustOpeningDebit"]);
                    decimal custOpeningCredit = GetDecimal(custRow["CustOpeningCredit"]);
                    decimal custTrDebit = GetDecimal(custRow["CustTrDebit"]);
                    decimal custTrCredit = GetDecimal(custRow["CustTrCredit"]);
                    decimal custClosingDebit = GetDecimal(custRow["CustClosingDebit"]);
                    decimal custClosingCredit = GetDecimal(custRow["CustClosingCredit"]);

                    newRow["ATBU_Id"] = custRow["ATBU_Id"];
                    newRow["flagStatus"] = custRow["flagStatus"];

                    newRow["CustOpeningDebit"] = custOpeningDebit;
                    newRow["CustOpeningCredit"] = custOpeningCredit;
                    newRow["CustTrDebit"] = custTrDebit;
                    newRow["CustTrCredit"] = custTrCredit;
                    newRow["CustClosingDebit"] = custClosingDebit;
                    newRow["CustClosingCredit"] = custClosingCredit;

                    bool openingMatch = sysOpeningDebit == custOpeningDebit &&
                                        sysOpeningCredit == custOpeningCredit;

                    bool transactionMatch = sysTrDebit == custTrDebit &&
                                             sysTrCredit == custTrCredit;

                    bool closingMatch = sysClosingDebit == custClosingDebit &&
                                        sysClosingCredit == custClosingCredit;

                    newRow["OpeningMatchStatus"] = openingMatch ? "Matched" : "Not Matched";
                    newRow["TransactionMatchStatus"] = transactionMatch ? "Matched" : "Not Matched";
                    newRow["ClosingMatchStatus"] = closingMatch ? "Matched" : "Not Matched";
                    newRow["OverallMatchStatus"] =
                        openingMatch && transactionMatch && closingMatch
                        ? "Fully Matched"
                        : "Partially Matched";

                    newRow["OpeningDebitVariance"] = sysOpeningDebit - custOpeningDebit;
                    newRow["OpeningCreditVariance"] = sysOpeningCredit - custOpeningCredit;
                    newRow["TrDebitVariance"] = sysTrDebit - custTrDebit;
                    newRow["TrCreditVariance"] = sysTrCredit - custTrCredit;
                    newRow["ClosingDebitVariance"] = sysClosingDebit - custClosingDebit;
                    newRow["ClosingCreditVariance"] = sysClosingCredit - custClosingCredit;
                }

                resultTable.Rows.Add(newRow);
            }

            AddUnmatchedCustomerRows(resultTable, custDt, systemDt);

            ds.Tables.Clear();
            ds.Tables.Add(resultTable);

            return ds;
        }

        private static decimal GetDecimal(object value)
        {
            return value == DBNull.Value || value == null
                ? 0
                : Convert.ToDecimal(value);
        }

        // Helper method to add customer rows that don't exist in system
        private void AddUnmatchedCustomerRows(DataTable resultTable, DataTable custDt, DataTable systemDt)
        {
            var systemDescriptions = systemDt.AsEnumerable()
                .Select(row => row.Field<string>("DescriptionCode"))
                .ToList();

            foreach (DataRow custRow in custDt.Rows)
            {
                string desc = custRow["DescriptionCode"]?.ToString();

                if (!systemDescriptions.Contains(desc))
                {
                    DataRow newRow = resultTable.NewRow();

                    // System data is empty
                    newRow["ATBU_Id"] = DBNull.Value;
                    newRow["DescriptionCode"] = desc;
                    newRow["OpeningDebit"] = 0;
                    newRow["OpeningCredit"] = 0;
                    newRow["TrDebit"] = 0;
                    newRow["TrCredit"] = 0;
                    newRow["ClosingDebit"] = 0;
                    newRow["ClosingCredit"] = 0;

                    // Customer data
                    newRow["CustOpeningDebit"] = custRow["CustOpeningDebit"];
                    newRow["CustOpeningCredit"] = custRow["CustOpeningCredit"];
                    newRow["CustTrDebit"] = custRow["CustTrDebit"];
                    newRow["CustTrCredit"] = custRow["CustTrCredit"];
                    newRow["CustClosingDebit"] = custRow["CustClosingDebit"];
                    newRow["CustClosingCredit"] = custRow["CustClosingCredit"];

                    // Status
                    newRow["OpeningMatchStatus"] = "System Missing";
                    newRow["TransactionMatchStatus"] = "System Missing";
                    newRow["ClosingMatchStatus"] = "System Missing";
                    newRow["OverallMatchStatus"] = "System Missing";
                    newRow["flagStatus"] = custRow["flagStatus"];


                    // Variances (customer values since system is 0)
                    newRow["OpeningDebitVariance"] = Convert.ToDecimal(custRow["CustOpeningDebit"]);
                    newRow["OpeningCreditVariance"] = Convert.ToDecimal(custRow["CustOpeningCredit"]);
                    newRow["TrDebitVariance"] = Convert.ToDecimal(custRow["CustTrDebit"]);
                    newRow["TrCreditVariance"] = Convert.ToDecimal(custRow["CustTrCredit"]);
                    newRow["ClosingDebitVariance"] = Convert.ToDecimal(custRow["CustClosingDebit"]);
                    newRow["ClosingCreditVariance"] = Convert.ToDecimal(custRow["CustClosingCredit"]);

                    resultTable.Rows.Add(newRow);
                }
            }
        }


        //UpdateCustomerTBDelFlg
        public async Task<int> UpdateCustomerTrailBalanceStatusAsync(List<UpdateCustomerTrailBalanceStatusDto> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
                return 0; // Nothing to update

            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Build parameterized CASE statement
                var caseStatements = dtoList
                    .Select((d, i) => $"WHEN @Id{i} THEN @Status{i}")
                    .ToList();

                var sql = $@"
          UPDATE Acc_TrailBalance_CustomerUpload
          SET ATBCU_DELFLG = CASE ATBCU_ID
              {string.Join(" ", caseStatements)}
          END
          WHERE ATBCU_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"Status{i}", dtoList[i].Status);
                }

                var updatedCount = await connection.ExecuteAsync(sql, parameters, transaction);

                transaction.Commit();
                return updatedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //public Task<List<GetCustomerTBGridDto>> GetCustomerTBGridAsync(int CompId, int custId, int yearId, int branchId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

