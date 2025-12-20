using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;
namespace TracePca.Service.FIN_statement
{
    public class LedgerDifferenceService : LedgerDifferenceInterface
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
                        sql = @" Select ATBUD_Description as headingname, ATBUD_Masid as headingId,
                      abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                      abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                      abs(abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) -
                          abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)))   As Difference_Amt,
                      (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) /
                          NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)), 0))   As Difference_Avg,
isnull (d.ATBU_Closing_TotalCredit_Amount,0) As cyCr,isnull (d.ATBU_Closing_TotalDebit_Amount,0) As cydb,
isnull (e.ATBU_Closing_TotalCredit_Amount,0) As pyCr,isnull (e.ATBU_Closing_TotalDebit_Amount,0) As pydb,
                       d.ATBU_STATUS as Status
                  From Acc_TrailBalance_Upload_Details
                      Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                          And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId 
                          And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                          And d.Atbu_Branchid=@BranchId
                      Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                          And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                          And ATBUD_YEARId=@YearId And e.ATBU_Branchid=Atbud_Branchnameid  
                          And e.Atbu_Branchid=@BranchId
                  WHERE ATBUD_CustId = @CustId
                        AND Atbud_Branchnameid = @BranchId
                        AND atbud_yearid = @YearId
                  GROUP BY ATBUD_Description, ATBUD_Masid, d.ATBU_STATUS
,d.ATBU_Closing_TotalCredit_Amount,d.ATBU_Closing_TotalDebit_Amount,
e.ATBU_Closing_TotalCredit_Amount,e.ATBU_Closing_TotalDebit_Amount ";

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


                        sql = @" Select ATBUD_Description as headingname, ATBUD_Masid as headingId,
                      abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                      abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                      abs(abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) -
                          abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)))   As Difference_Amt,
                      (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) /
                          NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)), 0))   As Difference_Avg,
isnull (d.ATBU_Closing_TotalCredit_Amount,0) As cyCr,isnull (d.ATBU_Closing_TotalDebit_Amount,0) As cydb,
isnull (e.ATBU_Closing_TotalCredit_Amount,0) As pyCr,isnull (e.ATBU_Closing_TotalDebit_Amount,0) As pydb,
                       d.ATBU_STATUS as Status
                  From Acc_TrailBalance_Upload_Details
                      Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                          And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId 
                          And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                          And d.Atbu_Branchid=@BranchId
                      Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                          And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                          And ATBUD_YEARId=@YearId And e.ATBU_Branchid=Atbud_Branchnameid  
                          And e.Atbu_Branchid=@BranchId
                  WHERE ATBUD_CustId = @CustId
                        AND Atbud_Branchnameid = @BranchId
                        AND atbud_yearid = @YearId
                        AND ATBUD_Headingid = @iPkId
                  GROUP BY ATBUD_Description, ATBUD_Masid, d.ATBU_STATUS
,d.ATBU_Closing_TotalCredit_Amount,d.ATBU_Closing_TotalDebit_Amount,
e.ATBU_Closing_TotalCredit_Amount,e.ATBU_Closing_TotalDebit_Amount ";
                    }
                    else if (typeId == 3)
                    {
                        sql = @" Select ATBUD_Description as headingname, ATBUD_Masid as headingId,
                      abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                      abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                      abs(abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) -
                          abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)))   As Difference_Amt,
                      (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) /
                          NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)), 0))   As Difference_Avg,
sum(isnull(d.ATBU_Closing_TotalCredit_Amount,0)) As cyCr,sum(isnull(d.ATBU_Closing_TotalDebit_Amount,0)) As cydb,
sum(isnull(e.ATBU_Closing_TotalCredit_Amount,0)) As pyCr,sum(isnull(e.ATBU_Closing_TotalDebit_Amount,0)) As pydb,
                       d.ATBU_STATUS as Status
                  From Acc_TrailBalance_Upload_Details
                      Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                          And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId 
                          And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                          And d.Atbu_Branchid=@BranchId
                      Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                          And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                          And ATBUD_YEARId=@YearId And e.ATBU_Branchid=Atbud_Branchnameid  
                          And e.Atbu_Branchid=@BranchId
                  WHERE ATBUD_CustId = @CustId
                        AND Atbud_Branchnameid = @BranchId
                        AND atbud_yearid = @YearId
                        AND ATBUD_Subheading = @iPkId
                  GROUP BY ATBUD_Description, ATBUD_Masid, d.ATBU_STATUS";
                    }
                    else if (typeId == 4)
                    {
                        sql = @" Select ATBUD_Description as headingname, ATBUD_Masid as headingId,
                      abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                      abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                      abs(abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) -
                          abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)))   As Difference_Amt,
                      (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) /
                          NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)), 0))   As Difference_Avg,
sum(isnull(d.ATBU_Closing_TotalCredit_Amount,0)) As cyCr,sum(isnull(d.ATBU_Closing_TotalDebit_Amount,0)) As cydb,
sum(isnull(e.ATBU_Closing_TotalCredit_Amount,0)) As pyCr,sum(isnull(e.ATBU_Closing_TotalDebit_Amount,0)) As pydb,
                       d.ATBU_STATUS as Status
                  From Acc_TrailBalance_Upload_Details
                      Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                          And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId 
                          And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                          And d.Atbu_Branchid=@BranchId
                      Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                          And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                          And ATBUD_YEARId=@YearId And e.ATBU_Branchid=Atbud_Branchnameid  
                          And e.Atbu_Branchid=@BranchId
                  WHERE ATBUD_CustId = @CustId
                        AND Atbud_Branchnameid = @BranchId
                        AND atbud_yearid = @YearId
                        AND ATBUD_itemid = @iPkId
                  GROUP BY ATBUD_Description, ATBUD_Masid, d.ATBU_STATUS";
                    }
                    else if (typeId == 5)
                    {
                        sql = @" Select ATBUD_Description as headingname, ATBUD_Masid as headingId,
                      abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                      abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                      abs(abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) -
                          abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)))   As Difference_Amt,
                      (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) /
                          NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)), 0))   As Difference_Avg,
sum(isnull(d.ATBU_Closing_TotalCredit_Amount,0)) As cyCr,sum(isnull(d.ATBU_Closing_TotalDebit_Amount,0)) As cydb,
sum(isnull(e.ATBU_Closing_TotalCredit_Amount,0)) As pyCr,sum(isnull(e.ATBU_Closing_TotalDebit_Amount,0)) As pydb,
                       d.ATBU_STATUS as Status
                  From Acc_TrailBalance_Upload_Details
                      Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                          And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId 
                          And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                          And d.Atbu_Branchid=@BranchId
                      Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                          And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                          And ATBUD_YEARId=@YearId And e.ATBU_Branchid=Atbud_Branchnameid  
                          And e.Atbu_Branchid=@BranchId
                  WHERE ATBUD_CustId = @CustId
                        AND Atbud_Branchnameid = @BranchId
                        AND atbud_yearid = @YearId
                        AND ATBUD_SubItemId = @iPkId
                  GROUP BY ATBUD_Description, ATBUD_Masid, d.ATBU_STATUS ";
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
        public async Task<List<GetCustomerTBGridDto>> GetCustomerTBGridAsync(int CompId, int custId,int yearId,int branchId)
        {
            //1.Get DB from session
                    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode missing in session.");

            // 2. Connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"
    SELECT 
        ROW_NUMBER() OVER (ORDER BY ATBCU_ID ASC) AS SrNo,
        b.ATBUD_ID AS DescID,
        ATBCU_ID AS DescDetailsID,
        ATBCU_CustId,
        ATBCU_DelFlg As DelFlg,
        ATBCU_Description AS DescriptionCode,

        CAST(ATBCU_Opening_Debit_Amount AS DECIMAL(19,2)) AS CustOpeningDebit,
        CAST(ATBCU_Opening_Credit_Amount AS DECIMAL(19,2)) AS CustOpeningCredit,
        CAST(SUM(ATBCU_TR_Debit_Amount) AS DECIMAL(19,2)) AS CustTrDebit,
        CAST(SUM(ATBCU_TR_Credit_Amount) AS DECIMAL(19,2)) AS CustTrCredit,
        ROUND(CAST(ATBCU_Closing_TotalDebit_Amount AS DECIMAL(19,2)),0) AS CustClosingDebit,
        ROUND(CAST(ATBCU_Closing_TotalCredit_Amount AS DECIMAL(19,2)),0) AS CustClosingCredit,

        ISNULL(b.ATBUD_SubItemId,0) AS SubItemID,
        ASSI_Name,

        ISNULL(b.atbud_itemid,0) AS ItemID,
        ASI_Name,

        ISNULL(b.atbud_subheading,0) AS SubHeadingID,
        ASSH_Name,

        ISNULL(b.atbud_headingid,0) AS HeadingID,
        ASH_Name,

        b.atbud_progress AS Status,
        b.Atbud_Company_type AS CompanyType,
        b.ATBUD_SChedule_Type AS ScheduleType,

        CAST(i.ATBU_Opening_Debit_Amount AS DECIMAL(19,2)) AS OpeningDebit,
        CAST(i.ATBU_Opening_Credit_Amount AS DECIMAL(19,2)) AS OpeningCredit,
        CAST(i.ATBU_TR_Debit_Amount AS DECIMAL(19,2)) AS TrDebit,
        CAST(i.ATBU_TR_Credit_Amount AS DECIMAL(19,2)) AS TrCredit,
        CAST(i.ATBU_Closing_Debit_Amount AS DECIMAL(19,2)) AS ClosingDebit,
        CAST(i.ATBU_Closing_Credit_Amount AS DECIMAL(19,2)) AS ClosingCredit

    FROM Acc_TrailBalance_CustomerUpload cu
    LEFT JOIN Acc_TrailBalance_Upload_Details b
        ON b.ATBUD_Description = cu.ATBCU_Description
        AND b.ATBUD_CustId = @CustId
        AND b.ATBUD_YEARId = @YearId

    LEFT JOIN Acc_TrailBalance_Upload i
        ON cu.ATBCU_MasId = i.ATBU_ID
        AND i.ATBU_Branchid = @BranchId
        AND i.ATBU_ID = 0

    LEFT JOIN ACC_ScheduleHeading c ON c.ASH_ID = b.ATBUD_Headingid
    LEFT JOIN ACC_ScheduleSubHeading d ON d.ASSH_ID = b.ATBUD_Subheading
    LEFT JOIN ACC_ScheduleItems e ON e.ASI_ID = b.ATBUD_itemid
    LEFT JOIN ACC_ScheduleSubItems f ON f.ASSI_ID = b.ATBUD_SubItemId

    WHERE cu.ATBCU_MasId = 0
      AND cu.ATBCU_YEARId = @YearId
      AND cu.ATBCU_CustId = @CustId
      AND (@BranchId = 0 OR cu.ATBCU_Branchid = @BranchId)

    GROUP BY 
        ATBCU_ID, ATBCU_CODE, ATBCU_CustId, ATBCU_DelFlg, ATBCU_Description,
        ATBCU_Opening_Debit_Amount, ATBCU_Opening_Credit_Amount,
        ATBCU_TR_Debit_Amount, ATBCU_TR_Credit_Amount,
        ATBCU_Closing_Debit_Amount, ATBCU_Closing_Credit_Amount,
        ATBCU_Closing_TotalDebit_Amount, ATBCU_Closing_TotalCredit_Amount,
        b.ATBUD_ID, b.ATBUD_SubItemId, b.atbud_itemid,
        b.atbud_subheading, b.atbud_headingid,
        ASSI_Name, ASI_Name, ASSH_Name, ASH_Name,
        b.atbud_progress, b.Atbud_Company_type,
        b.ATBUD_SChedule_Type,
        i.ATBU_Opening_Debit_Amount, i.ATBU_Opening_Credit_Amount,
        i.ATBU_TR_Debit_Amount, i.ATBU_TR_Credit_Amount,
        i.ATBU_Closing_Debit_Amount, i.ATBU_Closing_Credit_Amount

    ORDER BY ATBCU_ID ASC;
    ";
            var result = await connection.QueryAsync<GetCustomerTBGridDto>(
                sql,
                new
                {
                    CustId = custId,
                    YearId = yearId,
                    BranchId = branchId
                });

            return result.ToList();
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
    }
}

