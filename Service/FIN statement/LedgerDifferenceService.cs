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

        //GetHeadWiseDetails
        public async Task<(IEnumerable<HeadWiseDetailsDto> ScheduleType3, IEnumerable<HeadWiseDetailsDto> ScheduleType4)>
      GetHeadWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int durationId)
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

            var sql = @"
    -- Schedule Type = 3
    Select a.ASH_ID as HeadingId,
           ASH_Name as HeadingName,
           abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) as CYamt,
           abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as PYamt,
           abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) -  
           abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as Difference_Amt,
           (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) /
           NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg
    from Acc_TrailBalance_Upload_Details
         left join ACC_ScheduleHeading a on a.ASH_ID=ATBUD_Headingid
         left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description
              and d.ATBU_YEARId=@YearId and d.ATBU_CustId=@CustId
              and ATBUD_YEARId=@YearId and d.ATBU_Branchid=Atbud_Branchnameid
              and d.Atbu_Branchid=@BranchId and d.ATBU_QuarterId=@DurationId
         left join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description
              and e.ATBU_YEARId=@PrevYearId and e.ATBU_CustId=@CustId
              and ATBUD_YEARId=@PrevYearId and e.ATBU_Branchid=Atbud_Branchnameid
              and e.ATBU_Branchid=@BranchId and e.ATBU_QuarterId=@DurationId
    where ATBUD_SChedule_Type=3 and ATBUD_CustId=@CustId
          and ATBUD_Headingid<>0 and Atbud_Branchnameid=@BranchId
          and ATBUd_QuarterId=@DurationId
    group by ATBUD_Headingid,ASH_Name,a.ASH_ID
    order by ATBUD_Headingid;

    -- Schedule Type = 4
    Select a.ASH_ID as HeadingId,
           ASH_Name as HeadingName,
           abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) as CYamt,
           abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as PYamt,
           abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) -  
           abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) as Difference_Amt,
           (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) /
           NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)),0)) as Difference_Avg
    from Acc_TrailBalance_Upload_Details
         left join ACC_ScheduleHeading a on a.ASH_ID=ATBUD_Headingid
         left join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description
              and d.ATBU_YEARId=@YearId and d.ATBU_CustId=@CustId
              and ATBUD_YEARId=@YearId and d.ATBU_Branchid=Atbud_Branchnameid
              and d.Atbu_Branchid=@BranchId and d.ATBU_QuarterId=@DurationId
         left join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description
              and e.ATBU_YEARId=@PrevYearId and e.ATBU_CustId=@CustId
              and ATBUD_YEARId=@PrevYearId and e.ATBU_Branchid=Atbud_Branchnameid
              and e.ATBU_Branchid=@BranchId and e.ATBU_QuarterId=@DurationId
    where ATBUD_SChedule_Type=4 and ATBUD_CustId=@CustId
          and ATBUD_Headingid<>0 and Atbud_Branchnameid=@BranchId
          and ATBUd_QuarterId=@DurationId
    group by ATBUD_Headingid,ASH_Name,a.ASH_ID
    order by ATBUD_Headingid;";

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                CustId = custId,
                BranchId = branchId,
                YearId = yearId,
                PrevYearId = yearId - 1,
                DurationId = durationId
            });

            var scheduleType3 = await multi.ReadAsync<HeadWiseDetailsDto>();
            var scheduleType4 = await multi.ReadAsync<HeadWiseDetailsDto>();

            return (scheduleType3, scheduleType4);
        }

        //GetAccountWiseDetails
        public async Task<(IEnumerable<AccountWiseDetailsDto> ScheduleType3, IEnumerable<AccountWiseDetailsDto> ScheduleType4)>
       GetAccountWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int durationId)
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

            var sql = @"
        -- Schedule Type = 3 (Income / Expenses)
        Select 
            Case when ASH_Notes = 1 then 'Income' 
                 when ASH_Notes = 2 then 'Expenses' 
            End As HeadingName,
            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) As CYamt,
            a.ASH_Notes,
            abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) As PYamt
        From Acc_TrailBalance_Upload_Details
             Left Join ACC_ScheduleHeading a on a.ASH_ID=ATBUD_Headingid
             Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                  And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId
                  And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                  And d.Atbu_Branchid=@BranchId And d.ATBU_QuarterId=@DurationId
             Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                  And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                  And ATBUD_YEARId=@PrevYearId And e.ATBU_Branchid=Atbud_Branchnameid  
                  And e.Atbu_Branchid=@BranchId And e.ATBU_QuarterId=@DurationId
        where ATBUD_SChedule_Type=3 And ATBUD_CustId=@CustId 
              And ATBUD_Headingid<>0 And Atbud_Branchnameid=@BranchId 
              and ATBUd_QuarterId=@DurationId
        group by ASH_Notes order by ASH_Notes;

        -- Schedule Type = 4 (Liabilities / Assets)
        Select 
            Case when ASH_Notes = 1 then 'Liabilities' 
                 when ASH_Notes = 2 then 'Assets' 
            End As HeadingName,
            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount),0)) As CYamt,
            a.ASH_Notes,
            abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount),0)) As PYamt
        From Acc_TrailBalance_Upload_Details
             Left Join ACC_ScheduleHeading a on a.ASH_ID=ATBUD_Headingid
             Left Join Acc_TrailBalance_Upload d on d.ATBU_Description = ATBUD_Description 
                  And d.ATBU_YEARId=@YearId And d.ATBU_CustId=@CustId
                  And ATBUD_YEARId=@YearId And d.ATBU_Branchid=Atbud_Branchnameid  
                  And d.Atbu_Branchid=@BranchId And d.ATBU_QuarterId=@DurationId
             Left Join Acc_TrailBalance_Upload e on e.ATBU_Description = ATBUD_Description 
                  And e.ATBU_YEARId=@PrevYearId And e.ATBU_CustId=@CustId
                  And ATBUD_YEARId=@PrevYearId And e.ATBU_Branchid=Atbud_Branchnameid  
                  And e.Atbu_Branchid=@BranchId And e.ATBU_QuarterId=@DurationId
        where ATBUD_SChedule_Type=4 And ATBUD_CustId=@CustId 
              And ATBUD_Headingid<>0 And Atbud_Branchnameid=@BranchId 
              and ATBUd_QuarterId=@DurationId
        group by ASH_Notes order by ASH_Notes;";

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                CustId = custId,
                BranchId = branchId,
                YearId = yearId,
                PrevYearId = yearId - 1,
                DurationId = durationId
            });

            var scheduleType3 = await multi.ReadAsync<AccountWiseDetailsDto>();
            var scheduleType4 = await multi.ReadAsync<AccountWiseDetailsDto>();

            return (scheduleType3, scheduleType4);
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
                        sql = @" Select ATBUD_Description as headingname, atbud_id as headingId,
                            abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) As CYamt,
                            abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)) As PYamt,
                            abs(abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) -
                                abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)))   As Difference_Amt,
                            (abs(isnull(sum(d.ATBU_Closing_TotalCredit_Amount - d.ATBU_Closing_TotalDebit_Amount), 0)) /
                                NULLIF(abs(isnull(sum(e.ATBU_Closing_TotalCredit_Amount - e.ATBU_Closing_TotalDebit_Amount), 0)), 0))   As Difference_Avg
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
                        GROUP BY ATBUD_Description, atbud_id";
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
    }
}

