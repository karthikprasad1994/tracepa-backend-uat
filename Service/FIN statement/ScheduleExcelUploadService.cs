using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Globalization;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
using static TracePca.Service.FIN_statement.ScheduleExcelUploadService;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleExcelUploadService : ScheduleExcelUploadInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public ScheduleExcelUploadService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //DownloadUploadableExcelAndTemplate
        public Dto.FIN_Statement.ScheduleExcelUploadDto.FileDownloadResult GetExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core\\SampleExcels\\SampleTrailBalExcel.xlsx";

            if (!File.Exists(filePath))
                return new Dto.FIN_Statement.ScheduleExcelUploadDto.FileDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "SampleScheduleTempl3teExcel3.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return new Dto.FIN_Statement.ScheduleExcelUploadDto.FileDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //GetCustomersName
        public async Task<IEnumerable<Dto.FIN_Statement.ScheduleExcelUploadDto.CustDto>> GetCustomerNameAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Cust_Id,
            Cust_Name 
        FROM SAD_CUSTOMER_MASTER
        WHERE cust_Compid = @CompID";

            await connection.OpenAsync();

            return await connection.QueryAsync<Dto.FIN_Statement.ScheduleExcelUploadDto.CustDto>(query, new { CompID = CompId });
        }

        //GetFinancialYear
        public async Task<IEnumerable<Dto.FIN_Statement.ScheduleExcelUploadDto.FinancialYearDto>> GetFinancialYearAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            YMS_YEARID AS Year_Id,
            YMS_ID As Id
        FROM YEAR_MASTER 
        WHERE YMS_FROMDATE < DATEADD(year, +1, GETDATE()) 
          AND YMS_CompId = @CompID 
        ORDER BY YMS_ID DESC";

            await connection.OpenAsync();

            return await connection.QueryAsync<Dto.FIN_Statement.ScheduleExcelUploadDto.FinancialYearDto>(query, new { CompID = CompId });
        }

        //GetDuration
        public async Task<int?> GetCustomerDurationIdAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = "SELECT Cust_DurtnId FROM SAD_CUSTOMER_MASTER WHERE CUST_CompID = @CompId AND CUST_ID = @CustId";

            var parameters = new { CompId = compId, CustId = custId };
            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters);

            return result;
        }

        //GetBranchName
        public async Task<IEnumerable<Dto.FIN_Statement.ScheduleExcelUploadDto.CustBranchDto>> GetBranchNameAsync(int CompId, int CustId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<Dto.FIN_Statement.ScheduleExcelUploadDto.CustBranchDto>(query, new { CompId, CustId });
        }

        //SaveScheduleTemplate(P and L)
        public async Task<List<int>> SaveSchedulePandLAsync(List<ScheduleTemplatePandLDto> dtos)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iErrorLine = 1;

                foreach (var dto in dtos)
                {
                    // === A. Map Account Head Name to ID ===
                    string accHeadName = (dto.AST_AccountHeadName ?? string.Empty).Trim().ToUpper();

                    if (accHeadName == "ASSETS" || accHeadName == "INCOME")
                    {
                        dto.AST_AccHeadId = 1;
                    }
                    else if (accHeadName == "CAPITAL AND LIABILITIES" || accHeadName == "EXPENSES")
                    {
                        dto.AST_AccHeadId = 2;
                    }
                    else if (string.IsNullOrWhiteSpace(accHeadName) || accHeadName == "&NBSP;")
                    {
                        dto.AST_AccHeadId = 0;
                    }
                    else
                    {
                        throw new Exception($"Incorrect Account Head at Line No: {iErrorLine}");
                    }

                    iErrorLine++;

                    // === B. Start processing ===
                    int headingId = 0, subHeadingId = 0, itemId = 0, subItemId = 0, templateId = 0;

                    // === 1. Check if Heading exists ===
                    var checkHeadingSql = @"
                SELECT ISNULL(ASH_ID, 0)
                FROM ACC_ScheduleHeading
                WHERE ASH_CompId = @CompId
                  AND ASH_Name = @Name
                  AND Ash_scheduletype = @ScheduleType
                  AND Ash_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkHeadingSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASH_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASH_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.Ash_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.Ash_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        headingId = Convert.ToInt32(result);
                    }

                    if (headingId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleHeading", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASH_ID", dto.ASH_ID);
                            cmd.Parameters.AddWithValue("@ASH_Name", dto.ASH_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_DELFLG", dto.ASH_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_CRBY", dto.ASH_CRBY);
                            cmd.Parameters.AddWithValue("@ASH_STATUS", dto.ASH_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_UPDATEDBY", dto.ASH_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@ASH_IPAddress", dto.ASH_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_CompId", dto.ASH_CompId);
                            cmd.Parameters.AddWithValue("@ASH_YEARId", dto.ASH_YEARId);
                            cmd.Parameters.AddWithValue("@Ash_scheduletype", dto.Ash_scheduletype);
                            cmd.Parameters.AddWithValue("@Ash_Orgtype", dto.Ash_Orgtype);
                            cmd.Parameters.AddWithValue("@ASH_Notes", dto.ASH_Notes);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            headingId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 2. Check SubHeading ===
                    var checkSubHeadingSql = @"
                SELECT ISNULL(ASSH_ID, 0)
                FROM ACC_ScheduleSubHeading
                WHERE ASSH_CompId = @CompId
                  AND ASSH_Name = @Name
                  AND Assh_scheduletype = @ScheduleType
                  AND Assh_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkSubHeadingSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASSH_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASSH_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSH_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSH_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subHeadingId = Convert.ToInt32(result);
                    }

                    if (subHeadingId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleSubHeading", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASSH_ID", dto.ASSH_ID);
                            cmd.Parameters.AddWithValue("@ASSH_Name", dto.ASSH_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@ASSH_DELFLG", dto.ASSH_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_CRBY", dto.ASSH_CRBY);
                            cmd.Parameters.AddWithValue("@ASSH_STATUS", dto.ASSH_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_UPDATEDBY", dto.ASSH_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@ASSH_IPAddress", dto.ASSH_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_CompId", dto.ASSH_CompId);
                            cmd.Parameters.AddWithValue("@ASSH_YEARId", dto.ASSH_YEARId);
                            cmd.Parameters.AddWithValue("@ASSH_Notes", dto.ASSH_Notes);
                            cmd.Parameters.AddWithValue("@ASSH_scheduletype", dto.ASSH_scheduletype);
                            cmd.Parameters.AddWithValue("@ASSH_Orgtype", dto.ASSH_Orgtype);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            subHeadingId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 3. Check Item ===
                    var checkItemSql = @"
                SELECT ISNULL(ASI_ID, 0)
                FROM ACC_ScheduleItems
                WHERE ASI_CompId = @CompId
                  AND ASI_Name = @Name
                  AND Asi_scheduletype = @ScheduleType
                  AND Asi_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkItemSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASI_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASI_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASI_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASI_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        itemId = Convert.ToInt32(result);
                    }

                    if (itemId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleItems", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASI_ID", dto.ASI_ID);
                            cmd.Parameters.AddWithValue("@ASI_Name", dto.ASI_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@ASI_SubHeadingID", subHeadingId);
                            cmd.Parameters.AddWithValue("@ASI_DELFLG", dto.ASI_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_CRBY", dto.ASI_CRBY);
                            cmd.Parameters.AddWithValue("@ASI_STATUS", dto.ASI_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_IPAddress", dto.ASI_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_CompId", dto.ASI_CompId);
                            cmd.Parameters.AddWithValue("@ASI_YEARId", dto.ASI_YEARId);
                            cmd.Parameters.AddWithValue("@ASI_scheduletype", dto.ASI_scheduletype);
                            cmd.Parameters.AddWithValue("@ASI_Orgtype", dto.ASI_Orgtype);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            itemId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 4. Check SubItem ===
                    var checkSubItemSql = @"
                SELECT ISNULL(ASSI_ID, 0)
                FROM ACC_ScheduleSubItems
                WHERE ASSI_CompId = @CompId
                  AND ASSI_Name = @Name
                  AND Assi_scheduletype = @ScheduleType
                  AND Assi_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkSubItemSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASSI_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASSI_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSI_ScheduleType);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSI_OrgType);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subItemId = Convert.ToInt32(result);
                    }

                    if (subItemId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleSubItems", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASSI_ID", dto.ASSI_ID);
                            cmd.Parameters.AddWithValue("@ASSI_Name", dto.ASSI_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@ASSI_subHeadingID", subHeadingId);
                            cmd.Parameters.AddWithValue("@ASSI_ItemsID", itemId);
                            cmd.Parameters.AddWithValue("@ASSI_DELFLG", dto.ASSI_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_CRBY", dto.ASSI_CRBY);
                            cmd.Parameters.AddWithValue("@ASSI_STATUS", dto.ASSI_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_UPDATEDBY", dto.ASSI_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@ASSI_IPAddress", dto.ASSI_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_CompId", dto.ASSI_CompId);
                            cmd.Parameters.AddWithValue("@ASSI_YEARId", dto.ASSI_YEARId);
                            cmd.Parameters.AddWithValue("@ASSI_scheduletype", dto.ASSI_ScheduleType);
                            cmd.Parameters.AddWithValue("@ASSI_Orgtype", dto.ASSI_OrgType);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            subItemId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 5. Check Template ===
                    var checkTemplateSql = @"
                SELECT ISNULL(AST_ID, 0)
                FROM ACC_ScheduleTemplates
                WHERE 
                    AST_CompId = @CompId AND 
                    AST_HeadingID = @HeadingID AND 
                    AST_SubHeadingID = @SubHeadingID AND 
                    AST_ItemID = @ItemID AND 
                    AST_SubItemID = @SubItemID AND 
                    AST_Companytype = @CompanyType AND 
                    AST_Schedule_type = @ScheduleType";

                    using (var checkCmd = new SqlCommand(checkTemplateSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.AST_CompId);
                        checkCmd.Parameters.AddWithValue("@HeadingID", headingId);
                        checkCmd.Parameters.AddWithValue("@SubHeadingID", subHeadingId);
                        checkCmd.Parameters.AddWithValue("@ItemID", itemId);
                        checkCmd.Parameters.AddWithValue("@SubItemID", subItemId);
                        checkCmd.Parameters.AddWithValue("@CompanyType", dto.AST_Companytype);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.AST_Schedule_type);

                        var result = await checkCmd.ExecuteScalarAsync();
                        templateId = Convert.ToInt32(result);
                    }

                    if (templateId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleTemplates", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@AST_ID", dto.AST_ID);
                            cmd.Parameters.AddWithValue("@AST_Name", dto.AST_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@AST_subHeadingID", subHeadingId);
                            cmd.Parameters.AddWithValue("@AST_ItemID", itemId);
                            cmd.Parameters.AddWithValue("@AST_subItemID", subItemId);
                            cmd.Parameters.AddWithValue("@AST_AccHeadId", dto.AST_AccHeadId);
                            cmd.Parameters.AddWithValue("@AST_DELFLG", dto.AST_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_CRBY", dto.AST_CRBY);
                            cmd.Parameters.AddWithValue("@AST_STATUS", dto.AST_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_UPDATEDBY", dto.AST_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@AST_IPAddress", dto.AST_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_CompId", dto.AST_CompId);
                            cmd.Parameters.AddWithValue("@AST_YEARId", dto.AST_YEARId);
                            cmd.Parameters.AddWithValue("@AST_Schedule_type", dto.AST_Schedule_type);
                            cmd.Parameters.AddWithValue("@AST_Companytype", dto.AST_Companytype);
                            cmd.Parameters.AddWithValue("@AST_Company_limit", dto.AST_Company_limit);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            templateId = (int)(operParam.Value ?? 0);
                        }
                    }

                    resultIds.Add(templateId);
                }

                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving Schedule data: " + ex.Message, ex);
            }
        }

        //SaveScheduleTemplate(Balance Sheet)
        public async Task<List<int>> SaveScheduleBalanceSheetAsync(List<ScheduleTemplateBalanceSheetDto> dtos)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iErrorLine = 1;

                foreach (var dto in dtos)
                {
                    // === A. Map Account Head Name to ID ===
                    string accHeadName = (dto.AST_AccountHeadName ?? string.Empty).Trim().ToUpper();

                    if (accHeadName == "ASSETS" || accHeadName == "INCOME")
                    {
                        dto.AST_AccHeadId = 1;
                    }
                    else if (accHeadName == "CAPITAL AND LIABILITIES" || accHeadName == "EXPENSES")
                    {
                        dto.AST_AccHeadId = 2;
                    }
                    else if (string.IsNullOrWhiteSpace(accHeadName) || accHeadName == "&NBSP;")
                    {
                        dto.AST_AccHeadId = 0;
                    }
                    else
                    {
                        throw new Exception($"Incorrect Account Head at Line No: {iErrorLine}");
                    }

                    iErrorLine++;

                    // === B. Start processing ===
                    int headingId = 0, subHeadingId = 0, itemId = 0, subItemId = 0, templateId = 0;

                    // === 1. Check if Heading exists ===
                    var checkHeadingSql = @"
                SELECT ISNULL(ASH_ID, 0)
                FROM ACC_ScheduleHeading
                WHERE ASH_CompId = @CompId
                  AND ASH_Name = @Name
                  AND Ash_scheduletype = @ScheduleType
                  AND Ash_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkHeadingSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASH_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASH_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.Ash_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.Ash_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        headingId = Convert.ToInt32(result);
                    }

                    if (headingId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleHeading", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASH_ID", dto.ASH_ID);
                            cmd.Parameters.AddWithValue("@ASH_Name", dto.ASH_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_DELFLG", dto.ASH_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_CRBY", dto.ASH_CRBY);
                            cmd.Parameters.AddWithValue("@ASH_STATUS", dto.ASH_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_UPDATEDBY", dto.ASH_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@ASH_IPAddress", dto.ASH_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASH_CompId", dto.ASH_CompId);
                            cmd.Parameters.AddWithValue("@ASH_YEARId", dto.ASH_YEARId);
                            cmd.Parameters.AddWithValue("@Ash_scheduletype", dto.Ash_scheduletype);
                            cmd.Parameters.AddWithValue("@Ash_Orgtype", dto.Ash_Orgtype);
                            cmd.Parameters.AddWithValue("@ASH_Notes", dto.ASH_Notes);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            headingId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 2. Check SubHeading ===
                    var checkSubHeadingSql = @"
                SELECT ISNULL(ASSH_ID, 0)
                FROM ACC_ScheduleSubHeading
                WHERE ASSH_CompId = @CompId
                  AND ASSH_Name = @Name
                  AND Assh_scheduletype = @ScheduleType
                  AND Assh_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkSubHeadingSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASSH_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASSH_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSH_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSH_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subHeadingId = Convert.ToInt32(result);
                    }

                    if (subHeadingId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleSubHeading", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASSH_ID", dto.ASSH_ID);
                            cmd.Parameters.AddWithValue("@ASSH_Name", dto.ASSH_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@ASSH_DELFLG", dto.ASSH_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_CRBY", dto.ASSH_CRBY);
                            cmd.Parameters.AddWithValue("@ASSH_STATUS", dto.ASSH_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_UPDATEDBY", dto.ASSH_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@ASSH_IPAddress", dto.ASSH_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSH_CompId", dto.ASSH_CompId);
                            cmd.Parameters.AddWithValue("@ASSH_YEARId", dto.ASSH_YEARId);
                            cmd.Parameters.AddWithValue("@ASSH_Notes", dto.ASSH_Notes);
                            cmd.Parameters.AddWithValue("@ASSH_scheduletype", dto.ASSH_scheduletype);
                            cmd.Parameters.AddWithValue("@ASSH_Orgtype", dto.ASSH_Orgtype);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            subHeadingId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 3. Check Item ===
                    var checkItemSql = @"
                SELECT ISNULL(ASI_ID, 0)
                FROM ACC_ScheduleItems
                WHERE ASI_CompId = @CompId
                  AND ASI_Name = @Name
                  AND Asi_scheduletype = @ScheduleType
                  AND Asi_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkItemSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASI_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASI_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASI_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASI_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        itemId = Convert.ToInt32(result);
                    }

                    if (itemId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleItems", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASI_ID", dto.ASI_ID);
                            cmd.Parameters.AddWithValue("@ASI_Name", dto.ASI_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@ASI_SubHeadingID", subHeadingId);
                            cmd.Parameters.AddWithValue("@ASI_DELFLG", dto.ASI_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_CRBY", dto.ASI_CRBY);
                            cmd.Parameters.AddWithValue("@ASI_STATUS", dto.ASI_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_IPAddress", dto.ASI_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASI_CompId", dto.ASI_CompId);
                            cmd.Parameters.AddWithValue("@ASI_YEARId", dto.ASI_YEARId);
                            cmd.Parameters.AddWithValue("@ASI_scheduletype", dto.ASI_scheduletype);
                            cmd.Parameters.AddWithValue("@ASI_Orgtype", dto.ASI_Orgtype);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            itemId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 4. Check SubItem ===
                    var checkSubItemSql = @"
                SELECT ISNULL(ASSI_ID, 0)
                FROM ACC_ScheduleSubItems
                WHERE ASSI_CompId = @CompId
                  AND ASSI_Name = @Name
                  AND Assi_scheduletype = @ScheduleType
                  AND Assi_Orgtype = @OrgType";

                    using (var checkCmd = new SqlCommand(checkSubItemSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.ASSI_CompId);
                        checkCmd.Parameters.AddWithValue("@Name", dto.ASSI_Name);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSI_ScheduleType);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSI_OrgType);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subItemId = Convert.ToInt32(result);
                    }

                    if (subItemId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleSubItems", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ASSI_ID", dto.ASSI_ID);
                            cmd.Parameters.AddWithValue("@ASSI_Name", dto.ASSI_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@ASSI_subHeadingID", subHeadingId);
                            cmd.Parameters.AddWithValue("@ASSI_ItemsID", itemId);
                            cmd.Parameters.AddWithValue("@ASSI_DELFLG", dto.ASSI_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_CRBY", dto.ASSI_CRBY);
                            cmd.Parameters.AddWithValue("@ASSI_STATUS", dto.ASSI_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_UPDATEDBY", dto.ASSI_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@ASSI_IPAddress", dto.ASSI_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASSI_CompId", dto.ASSI_CompId);
                            cmd.Parameters.AddWithValue("@ASSI_YEARId", dto.ASSI_YEARId);
                            cmd.Parameters.AddWithValue("@ASSI_scheduletype", dto.ASSI_ScheduleType);
                            cmd.Parameters.AddWithValue("@ASSI_Orgtype", dto.ASSI_OrgType);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            subItemId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 5. Check Template ===
                    var checkTemplateSql = @"
                SELECT ISNULL(AST_ID, 0)
                FROM ACC_ScheduleTemplates
                WHERE 
                    AST_CompId = @CompId AND 
                    AST_HeadingID = @HeadingID AND 
                    AST_SubHeadingID = @SubHeadingID AND 
                    AST_ItemID = @ItemID AND 
                    AST_SubItemID = @SubItemID AND 
                    AST_Companytype = @CompanyType AND 
                    AST_Schedule_type = @ScheduleType";

                    using (var checkCmd = new SqlCommand(checkTemplateSql, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@CompId", dto.AST_CompId);
                        checkCmd.Parameters.AddWithValue("@HeadingID", headingId);
                        checkCmd.Parameters.AddWithValue("@SubHeadingID", subHeadingId);
                        checkCmd.Parameters.AddWithValue("@ItemID", itemId);
                        checkCmd.Parameters.AddWithValue("@SubItemID", subItemId);
                        checkCmd.Parameters.AddWithValue("@CompanyType", dto.AST_Companytype);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.AST_Schedule_type);

                        var result = await checkCmd.ExecuteScalarAsync();
                        templateId = Convert.ToInt32(result);
                    }

                    if (templateId == 0)
                    {
                        using (var cmd = new SqlCommand("spACC_ScheduleTemplates", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@AST_ID", dto.AST_ID);
                            cmd.Parameters.AddWithValue("@AST_Name", dto.AST_Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_HeadingID", headingId);
                            cmd.Parameters.AddWithValue("@AST_subHeadingID", subHeadingId);
                            cmd.Parameters.AddWithValue("@AST_ItemID", itemId);
                            cmd.Parameters.AddWithValue("@AST_subItemID", subItemId);
                            cmd.Parameters.AddWithValue("@AST_AccHeadId", dto.AST_AccHeadId);
                            cmd.Parameters.AddWithValue("@AST_DELFLG", dto.AST_DELFLG ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_CRBY", dto.AST_CRBY);
                            cmd.Parameters.AddWithValue("@AST_STATUS", dto.AST_STATUS ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_UPDATEDBY", dto.AST_UPDATEDBY);
                            cmd.Parameters.AddWithValue("@AST_IPAddress", dto.AST_IPAddress ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AST_CompId", dto.AST_CompId);
                            cmd.Parameters.AddWithValue("@AST_YEARId", dto.AST_YEARId);
                            cmd.Parameters.AddWithValue("@AST_Schedule_type", dto.AST_Schedule_type);
                            cmd.Parameters.AddWithValue("@AST_Companytype", dto.AST_Companytype);
                            cmd.Parameters.AddWithValue("@AST_Company_limit", dto.AST_Company_limit);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            templateId = (int)(operParam.Value ?? 0);
                        }
                    }

                    resultIds.Add(templateId);
                }

                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving Schedule data: " + ex.Message, ex);
            }
        }

        //SaveOpeningBalance
        public async Task<List<int>> SaveOpeningBalanceAsync(List<OpeningBalanceDto> dtos)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var dto in dtos)
                {
                    int iPKId = dto.ATBU_ID;
                    int updateOrSave, oper;

                    // --- Save Main Upload (spAcc_TrailBalance_Upload) ---
                    using (var uploadCommand = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        uploadCommand.CommandType = CommandType.StoredProcedure;

                        uploadCommand.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        uploadCommand.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        uploadCommand.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        uploadCommand.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        uploadCommand.Parameters.Add(updateOrSaveParam);
                        uploadCommand.Parameters.Add(operParam);

                        await uploadCommand.ExecuteNonQueryAsync();

                        updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        oper = (int)(operParam.Value ?? 0);
                    }

                    resultIds.Add(oper);

                    if (dto.ATBU_ID == 0)
                    {
                        // --- Save Upload Details (spAcc_TrailBalance_Upload_Details) ---
                        using (var detailsCommand = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                        {
                            detailsCommand.CommandType = CommandType.StoredProcedure;

                            detailsCommand.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Masid", oper);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

                            var detailOutput1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var detailOutput2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            detailsCommand.Parameters.Add(detailOutput1);
                            detailsCommand.Parameters.Add(detailOutput2);

                            await detailsCommand.ExecuteNonQueryAsync();
                        }
                    }
                }

                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving Trail Balance data: " + ex.Message, ex);
            }
        }

        //SaveTrailBalance
        public async Task<List<int>> SaveTrailBalanceAsync(List<TrailBalanceDto> dtos)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var dto in dtos)
                {
                    int iPKId = dto.ATBU_ID;
                    int updateOrSave, oper;

                    // --- Save Main Upload (spAcc_TrailBalance_Upload) ---
                    using (var uploadCommand = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        uploadCommand.CommandType = CommandType.StoredProcedure;

                        uploadCommand.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        uploadCommand.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        uploadCommand.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        uploadCommand.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        uploadCommand.Parameters.Add(updateOrSaveParam);
                        uploadCommand.Parameters.Add(operParam);

                        await uploadCommand.ExecuteNonQueryAsync();

                        updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        oper = (int)(operParam.Value ?? 0);
                    }

                    resultIds.Add(oper);

                    if (dto.ATBU_ID == 0)
                    {
                        // --- Save Upload Details (spAcc_TrailBalance_Upload_Details) ---
                        using (var detailsCommand = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                        {
                            detailsCommand.CommandType = CommandType.StoredProcedure;

                            detailsCommand.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Masid", oper);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

                            var detailOutput1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var detailOutput2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            detailsCommand.Parameters.Add(detailOutput1);
                            detailsCommand.Parameters.Add(detailOutput2);

                            await detailsCommand.ExecuteNonQueryAsync();
                        }
                    }
                }

                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving Trail Balance data: " + ex.Message, ex);
            }
        }

        //SaveClientTrailBalance
        public async Task<List<int>> ClientTrailBalanceAsync(List<ClientTrailBalance> items)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var obj in items)
                {
                    int iUpdateOrSave, iOper;

                    using (var cmd = new SqlCommand("spAcc_TrailBalance_CustomerUpload", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ATBCU_ID", obj.ATBCU_ID);
                        cmd.Parameters.AddWithValue("@ATBCU_CODE", obj.ATBCU_CODE ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBCU_Description", obj.ATBCU_Description ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBCU_CustId", obj.ATBCU_CustId);
                        cmd.Parameters.AddWithValue("@ATBCU_Opening_Debit_Amount", obj.ATBCU_Opening_Debit_Amount);
                        cmd.Parameters.AddWithValue("@ATBCU_Opening_Credit_Amount", obj.ATBCU_Opening_Credit_Amount);
                        cmd.Parameters.AddWithValue("@ATBCU_TR_Debit_Amount", obj.ATBCU_TR_Debit_Amount);
                        cmd.Parameters.AddWithValue("@ATBCU_TR_Credit_Amount", obj.ATBCU_TR_Credit_Amount);
                        cmd.Parameters.AddWithValue("@ATBCU_Closing_Debit_Amount", obj.ATBCU_Closing_Debit_Amount);
                        cmd.Parameters.AddWithValue("@ATBCU_Closing_Credit_Amount", obj.ATBCU_Closing_Credit_Amount);
                        cmd.Parameters.AddWithValue("@ATBCU_DELFLG", obj.ATBCU_DELFLG ?? "A");
                        cmd.Parameters.AddWithValue("@ATBCU_CRBY", obj.ATBCU_CRBY);
                        cmd.Parameters.AddWithValue("@ATBCU_STATUS", obj.ATBCU_STATUS ?? "C");
                        cmd.Parameters.AddWithValue("@ATBCU_UPDATEDBY", obj.ATBCU_UPDATEDBY);
                        cmd.Parameters.AddWithValue("@ATBCU_IPAddress", obj.ATBCU_IPAddress ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBCU_CompId", obj.ATBCU_CompId);
                        cmd.Parameters.AddWithValue("@ATBCU_YEARId", obj.ATBCU_YEARId);
                        cmd.Parameters.AddWithValue("@ATBCU_Branchid", obj.ATBCU_Branchid);
                        cmd.Parameters.AddWithValue("@ATBCU_MasId", obj.ATBCU_Masid);
                        cmd.Parameters.AddWithValue("@ATBCU_QuarterId", obj.ATBCU_QuarterId);

                        var out1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var out2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                        cmd.Parameters.Add(out1);
                        cmd.Parameters.Add(out2);

                        await cmd.ExecuteNonQueryAsync();

                        iUpdateOrSave = (int)(out1.Value ?? 0);
                        iOper = (int)(out2.Value ?? 0);

                        resultIds.Add(iUpdateOrSave); // or add ATBCU_ID if needed
                    }
                }
                await transaction.CommitAsync();
                return resultIds;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while saving Customer Trail Balance data: " + ex.Message, ex);
            }
        }

        //SaveJournalEntry
        public async Task<List<int>> SaveCompleteTrailBalanceAsync(List<TrailBalanceCompositeModel> models)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var model in models)
                {
                    int uploadId = 0, jeMasId = 0;

                    // === 1. Save/Update TrailBalance Upload ===
                    using (var cmd = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dto = model.Upload;

                        cmd.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        cmd.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        cmd.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        cmd.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        cmd.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        cmd.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        cmd.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        cmd.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        cmd.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        cmd.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        cmd.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        cmd.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        cmd.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        cmd.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        cmd.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        cmd.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var out1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var out2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(out1);
                        cmd.Parameters.Add(out2);

                        await cmd.ExecuteNonQueryAsync();
                        uploadId = (int)(out2.Value ?? dto.ATBU_ID);
                    }

                    // === 2. Save/Update TrailBalance Upload Details ===
                    using (var cmd = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dto = model.UploadDetails;

                        cmd.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                        cmd.Parameters.AddWithValue("@ATBUD_Masid", uploadId);
                        cmd.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                        cmd.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                        cmd.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                        cmd.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                        cmd.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                        cmd.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
                        cmd.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
                        cmd.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
                        cmd.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
                        cmd.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                        cmd.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                        cmd.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                        cmd.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                        cmd.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                        cmd.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                        cmd.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

                        cmd.Parameters.Add(new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output });
                        cmd.Parameters.Add(new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output });

                        await cmd.ExecuteNonQueryAsync();
                    }

                    // === 3. Save/Update Journal Entry ===
                    using (var cmd = new SqlCommand("spAcc_JE_Master", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dto = model.JournalEntry;

                        cmd.Parameters.AddWithValue("@Acc_JE_ID", dto.Acc_JE_ID);
                        cmd.Parameters.AddWithValue("@Acc_JE_TransactionNo", dto.Acc_JE_TransactionNo ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_Party", dto.Acc_JE_Party);
                        cmd.Parameters.AddWithValue("@Acc_JE_Location", dto.Acc_JE_Location);
                        cmd.Parameters.AddWithValue("@Acc_JE_BillType", dto.Acc_JE_BillType);
                        cmd.Parameters.AddWithValue("@Acc_JE_BillNo", dto.Acc_JE_BillNo ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BillDate", dto.Acc_JE_BillDate);
                        cmd.Parameters.AddWithValue("@Acc_JE_BillAmount", dto.Acc_JE_BillAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_AdvanceAmount", dto.Acc_JE_AdvanceAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_AdvanceNaration", dto.Acc_JE_AdvanceNaration ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BalanceAmount", dto.Acc_JE_BalanceAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_NetAmount", dto.Acc_JE_NetAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_PaymentNarration", dto.Acc_JE_PaymentNarration ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_ChequeNo", dto.Acc_JE_ChequeNo ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_ChequeDate", dto.Acc_JE_ChequeDate);
                        cmd.Parameters.AddWithValue("@Acc_JE_IFSCCode", dto.Acc_JE_IFSCCode ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BankName", dto.Acc_JE_BankName ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BranchName", dto.Acc_JE_BranchName ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_CreatedBy", dto.Acc_JE_CreatedBy);
                        cmd.Parameters.AddWithValue("@Acc_JE_YearID", dto.Acc_JE_YearID);
                        cmd.Parameters.AddWithValue("@Acc_JE_CompID", dto.Acc_JE_CompID);
                        cmd.Parameters.AddWithValue("@Acc_JE_Status", dto.Acc_JE_Status ?? "C");
                        cmd.Parameters.AddWithValue("@Acc_JE_Operation", dto.Acc_JE_Operation ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_IPAddress", dto.Acc_JE_IPAddress ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BillCreatedDate", dto.Acc_JE_BillCreatedDate);
                        cmd.Parameters.AddWithValue("@acc_JE_BranchId", dto.Acc_JE_BranchId);
                        cmd.Parameters.AddWithValue("@Acc_JE_QuarterId", dto.Acc_JE_QuarterId);
                        cmd.Parameters.AddWithValue("@Acc_JE_Comnments", dto.Acc_JE_Comnments ?? "");

                        var out1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var out2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(out1);
                        cmd.Parameters.Add(out2);

                        await cmd.ExecuteNonQueryAsync();
                        jeMasId = (int)(out2.Value ?? dto.Acc_JE_ID);
                    }

                    // === 4. Save Transactions and Update Trail Balance ===
                    foreach (var t in model.TransactionDetailsList)
                    {
                        using (var cmd = new SqlCommand("spAcc_JETransactions_Details", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@AJTB_ID", t.AJTB_ID);
                            cmd.Parameters.AddWithValue("@AJTB_MasID", jeMasId);
                            cmd.Parameters.AddWithValue("@AJTB_TranscNo", t.AJTB_TranscNo ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_CustId", t.AJTB_CustId);
                            cmd.Parameters.AddWithValue("@AJTB_ScheduleTypeid", t.AJTB_ScheduleTypeid);
                            cmd.Parameters.AddWithValue("@AJTB_Deschead", t.AJTB_Deschead);
                            cmd.Parameters.AddWithValue("@AJTB_Desc", t.AJTB_Desc);
                            cmd.Parameters.AddWithValue("@AJTB_Debit", t.AJTB_Debit);
                            cmd.Parameters.AddWithValue("@AJTB_Credit", t.AJTB_Credit);
                            cmd.Parameters.AddWithValue("@AJTB_CreatedBy", t.AJTB_CreatedBy);
                            cmd.Parameters.AddWithValue("@AJTB_UpdatedBy", t.AJTB_UpdatedBy);
                            cmd.Parameters.AddWithValue("@AJTB_Status", t.AJTB_Status ?? "C");
                            cmd.Parameters.AddWithValue("@AJTB_IPAddress", t.AJTB_IPAddress ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_CompID", t.AJTB_CompID);
                            cmd.Parameters.AddWithValue("@AJTB_YearID", t.AJTB_YearID);
                            cmd.Parameters.AddWithValue("@AJTB_BillType", t.AJTB_BillType);
                            cmd.Parameters.AddWithValue("@AJTB_DescName", t.AJTB_DescName ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_BranchId", t.AJTB_BranchId);
                            cmd.Parameters.AddWithValue("@AJTB_QuarterId", t.AJTB_QuarterId);

                            cmd.Parameters.Add(new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output });
                            cmd.Parameters.Add(new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output });

                            await cmd.ExecuteNonQueryAsync();
                        }

                        // === Update Trail Balance Closing ===
                        var sql = @"
                UPDATE Acc_TrailBalance_Upload
                SET 
                    ATBU_Closing_TotalDebit_Amount = 
                        CASE 
                            WHEN @IsDebit = 1 THEN ISNULL(ATBU_Closing_Debit_Amount, 0) - @Amount
                            ELSE ATBU_Closing_TotalDebit_Amount
                        END,
                    ATBU_Closing_TotalCredit_Amount = 
                        CASE 
                            WHEN @IsDebit = 0 THEN ISNULL(ATBU_Closing_TotalCredit_Amount, 0) - @Amount
                            ELSE ATBU_Closing_TotalCredit_Amount
                        END
                WHERE ATBU_ID = @ATBU_ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId AND ATBU_QuarterId = @QuarterId";

                        var updateParams = new
                        {
                            IsDebit = t.AJTB_Debit > 0 ? 1 : 0,
                            Amount = t.AJTB_Debit > 0 ? t.AJTB_Debit : t.AJTB_Credit,
                            ATBU_ID = t.AJTB_MasID,
                            CustId = t.AJTB_CustId,
                            CompId = t.AJTB_CompID,
                            QuarterId = t.AJTB_QuarterId
                        };

                        await connection.ExecuteAsync(sql, updateParams, transaction);
                    }

                    resultIds.Add(uploadId);
                }

                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving or updating trail balance: " + ex.Message, ex);
            }
        }
    }
}
