using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface;
using TracePca.Interface.Audit;
using TracePca.Interface.FIN_Statement;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TracePca.Dto.FIN_Statement.ScheduleCustDto;
using static TracePca.Service.FIN_statement.ScheduleMapping;
namespace TracePca.Service.FIN_statement
{
    public class ScheduleMapping : ScheduleMappingInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        public ScheduleMapping(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<List<CustDto>> LoadCustomers(int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
            Select Cust_Id,Cust_Name 
            FROM SAD_CUSTOMER_MASTER
            WHERE cust_Compid  = @CompID";
            var result = await connection.QueryAsync<CustDto>(query, new { CompID = compId });
            return result.ToList();
        }

        public async Task<List<FinancialYearDto>> LoadFinancialYear(int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
            Select YMS_YEARID, YMS_ID from YEAR_MASTER where YMS_FROMDATE < DATEADD(year,+1,GETDATE()) and YMS_CompId=@CompID order by YMS_ID desc";
            var result = await connection.QueryAsync<FinancialYearDto>(query, new { CompID = compId });
            return result.ToList();
        }

        public async Task<List<CustBranchDto>> LoadBranches(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
           select Mas_Id as Branchid,Mas_Description as BranchName from SAD_CUST_LOCATION where Mas_CustID=@custId and Mas_CompID=@compId";
            var result = await connection.QueryAsync<CustBranchDto>(query, new { CompID = compId });
            return result.ToList();
        }

        public async Task<List<CustDurationDto>> LoadDuration(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
           Select isnull(Cust_DurtnId,0) as Cust_DurtnId  from SAD_CUSTOMER_MASTER where cust_id=@custid and Cust_CompID=@compid";
            var result = await connection.QueryAsync<CustDurationDto>(query, new { CompID = compId });
            return result.ToList();
        }


        // Heading
        public async Task<List<ScheduleHeadingDto>> GetScheduleHeadingsAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
            SELECT DISTINCT b.ASH_Name as ASH_Name , b.ASH_ID as ASH_ID
            FROM ACC_ScheduleTemplates a
            LEFT JOIN ACC_ScheduleHeading b ON b.ASH_ID = a.AST_HeadingID AND b.ASH_STATUS <> 'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASH_Name IS NOT NULL AND b.ASH_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }

        //Sub Heading
        public async Task<List<ScheduleHeadingDto>> GetSchedulesubHeadingsAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
            select distinct(b.AsSH_Name),ASsH_ID from ACC_ScheduleTemplates 
                    left join ACC_ScheduleSubHeading b on b.AsSH_ID = AST_SubHeadingID and ASsH_STATUS<>'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASSH_Name IS NOT NULL AND b.ASSH_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }

        //Item
        public async Task<List<ScheduleHeadingDto>> GetScheduleItemAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
        select distinct(b.ASI_Name),ASI_ID from ACC_ScheduleTemplates 
                    left join ACC_ScheduleItems b on b.ASI_ID = AST_ItemID and ASI_STATUS<>'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASI_Name IS NOT NULL AND b.ASI_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }

        //SubItem
        public async Task<List<ScheduleHeadingDto>> GetScheduleSubItemAsync(int compId, int custId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = @"
            select distinct(b.ASSI_Name),ASSI_ID from ACC_ScheduleTemplates 
                    left join ACC_ScheduleSubItems b on b.ASSI_ID = AST_SubItemID and ASSI_STATUS<>'D'
            WHERE a.AST_CompId = @compId
              AND a.AST_Companytype = @custId
              AND a.AST_Schedule_type = @ScheduleTypeId
              AND b.ASSI_Name IS NOT NULL AND b.ASSI_ID IS NOT NULL";
            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompID = compId, custId = custId, ScheduleTypeId = ScheduleTypeId });
            return result.ToList();
        }
        // Save mapping 



        public async Task<int> SaveTrailBalanceExcelUploadAsync(string sAC, TrailBalanceUploadDto dto, int userId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var builder = new SqlConnectionStringBuilder(connectionString);
            sAC = builder.InitialCatalog;

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add input parameters
                command.Parameters.AddWithValue("@ATBU_ID", (object?)dto.ATBU_ID ?? DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                command.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                command.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                command.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                command.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                command.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                command.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                command.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                command.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                command.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                command.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                command.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                command.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                // Output parameters
                var paramUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var paramOper = new SqlParameter("@iOper", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(paramUpdateOrSave);
                command.Parameters.Add(paramOper);

                await command.ExecuteNonQueryAsync();
                transaction.Commit();




                return (int)(paramOper.Value ?? 0);



            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> SaveTrailBalanceDetailAsync(string dbName, TrailBalanceUploadDetailDto dto, int userId)
        {
            int result = 0;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = dbName
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add parameters
            command.Parameters.AddWithValue("@ATBUD_ID", (object?)dto.ATBUD_ID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ATBUD_Masid", dto.ATBUD_Masid);
            command.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? "");
            command.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? "");
            command.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
            command.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
            command.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
            command.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
            command.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
            command.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
            command.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
            command.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
            command.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
            command.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "N");
            command.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
            command.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
            command.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "A");
            command.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "");
            command.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? "127.0.0.1");
            command.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
            command.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

            // Output parameters
            var paramUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };
            var paramOper = new SqlParameter("@iOper", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(paramUpdateOrSave);
            command.Parameters.Add(paramOper);

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(paramOper.Value);

            return result;
        }

        //SaveScheduleFormatHeadingAndTemplate
        public async Task<int[]> SaveScheduleHeadingAndTemplateAsync(int iCompId, SaveScheduleFormatHeadingDto dto)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection3")))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int headingId = dto.ASH_ID;

                        // --- Save Schedule Heading ---
                        using (var headingCommand = new SqlCommand("spACC_ScheduleHeading", connection, transaction))
                        {
                            headingCommand.CommandType = CommandType.StoredProcedure;

                            headingCommand.Parameters.AddWithValue("@ASH_ID", dto.ASH_ID);
                            headingCommand.Parameters.AddWithValue("@ASH_Name", dto.ASH_Name ?? string.Empty);
                            headingCommand.Parameters.AddWithValue("@ASH_DELFLG", dto.ASH_DELFLG ?? string.Empty);
                            headingCommand.Parameters.AddWithValue("@ASH_CRBY", dto.ASH_CRBY);
                            headingCommand.Parameters.AddWithValue("@ASH_STATUS", dto.ASH_STATUS ?? string.Empty);
                            headingCommand.Parameters.AddWithValue("@ASH_UPDATEDBY", dto.ASH_UPDATEDBY);
                            headingCommand.Parameters.AddWithValue("@ASH_IPAddress", dto.ASH_IPAddress ?? string.Empty);
                            headingCommand.Parameters.AddWithValue("@ASH_CompId", dto.ASH_CompId);
                            headingCommand.Parameters.AddWithValue("@ASH_YEARId", dto.ASH_YEARId);
                            headingCommand.Parameters.AddWithValue("@Ash_scheduletype", dto.Ash_scheduletype);
                            headingCommand.Parameters.AddWithValue("@Ash_Orgtype", dto.Ash_Orgtype);
                            headingCommand.Parameters.AddWithValue("@ASH_Notes", dto.ASH_Notes);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };

                            headingCommand.Parameters.Add(updateOrSaveParam);
                            headingCommand.Parameters.Add(operParam);

                            await headingCommand.ExecuteNonQueryAsync();

                            int updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                            int oper = (int)(operParam.Value ?? 0);

                            // Optionally get the new heading ID if it's returned (you may add output parameter for new ID)
                            headingId = dto.ASH_ID; // Assume this is populated on insert or stays the same on update

                            // --- Save Schedule Template (child of heading) ---
                            using (var templateCommand = new SqlCommand("spACC_ScheduleTemplates", connection, transaction))
                            {
                                templateCommand.CommandType = CommandType.StoredProcedure;

                                templateCommand.Parameters.AddWithValue("@AST_ID", dto.AST_ID);
                                templateCommand.Parameters.AddWithValue("@AST_Name", dto.AST_Name ?? string.Empty);
                                templateCommand.Parameters.AddWithValue("@AST_HeadingID", headingId); // FK to Heading
                                templateCommand.Parameters.AddWithValue("@AST_SubHeadingID", dto.AST_SubHeadingID);
                                templateCommand.Parameters.AddWithValue("@AST_ItemID", dto.AST_ItemID);
                                templateCommand.Parameters.AddWithValue("@AST_SubItemID", dto.AST_SubItemID);
                                templateCommand.Parameters.AddWithValue("@AST_AccHeadId", dto.AST_AccHeadId);
                                templateCommand.Parameters.AddWithValue("@AST_DELFLG", dto.AST_DELFLG);
                                templateCommand.Parameters.AddWithValue("@AST_CRBY", dto.AST_CRBY);
                                templateCommand.Parameters.AddWithValue("@AST_STATUS", dto.AST_STATUS ?? string.Empty);
                                templateCommand.Parameters.AddWithValue("@AST_UPDATEDBY", dto.AST_UPDATEDBY);
                                templateCommand.Parameters.AddWithValue("@AST_IPAddress", dto.AST_IPAddress ?? string.Empty);
                                templateCommand.Parameters.AddWithValue("@AST_CompId", iCompId);
                                templateCommand.Parameters.AddWithValue("@AST_YEARId", dto.AST_YEARId);
                                templateCommand.Parameters.AddWithValue("@AST_Schedule_type", dto.AST_Schedule_type);
                                templateCommand.Parameters.AddWithValue("@AST_Companytype", dto.AST_Companytype);
                                templateCommand.Parameters.AddWithValue("@AST_Company_limit", dto.AST_Company_limit);

                                var updateOrSaveParamTemplate = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                var operParamTemplate = new SqlParameter("@iOper", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };

                                templateCommand.Parameters.Add(updateOrSaveParamTemplate);
                                templateCommand.Parameters.Add(operParamTemplate);

                                await templateCommand.ExecuteNonQueryAsync();

                                // You can optionally read these values if needed
                                int templateSaveStatus = (int)(updateOrSaveParamTemplate.Value ?? 0);
                                int templateOper = (int)(operParamTemplate.Value ?? 0);
                            }

                            // Commit both heading and template
                            transaction.Commit();

                            return new int[] { updateOrSave, oper };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Log error or handle accordingly
                        throw;
                    }
                }
            }
        }
    }
}


