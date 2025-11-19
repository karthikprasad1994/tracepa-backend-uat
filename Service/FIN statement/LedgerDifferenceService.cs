using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;
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
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg
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
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg
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
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg
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
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg
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
    }
}

