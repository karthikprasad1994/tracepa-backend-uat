using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Data;
using System.Data.Common;
using System.Globalization;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleExcelUploadService : ScheduleExcelUploadInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScheduleExcelUploadService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
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

        //SaveScheduleTemplate(P and L)
        public async Task<List<int>> SaveSchedulePandLAsync(int CompId, List<ScheduleTemplatePandLDto> dtos)
        {
            var resultIds = new List<int>();


            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iErrorLine = 1;
                string HeadingName = "";
                string subHeadingName = "";
                string itemName = "";
                string subItemName = "";

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

                    if (dto.ASH_Name != "")
                    {
                        HeadingName = dto.ASH_Name.Trim();
                        subHeadingName = "";
                        itemName = "";
                        subItemName = "";
                    }
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
                        checkCmd.Parameters.AddWithValue("@Name", HeadingName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.Ash_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.Ash_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        headingId = Convert.ToInt32(result);
                    }

                    if (headingId == 0 && HeadingName != "")
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
                            cmd.Parameters.AddWithValue("@ASH_Notes", dto.AST_AccHeadId);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            headingId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 2. Check SubHeading ===

                    if (dto.ASSH_Name != "")
                    {
                        subHeadingName = dto.ASSH_Name.Trim();
                        itemName = "";
                        subItemName = "";
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", subHeadingName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSH_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSH_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subHeadingId = Convert.ToInt32(result);
                    }

                    if (subHeadingId == 0 && subHeadingName != "")
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

                    if (dto.ASI_Name != "")
                    {
                        itemName = dto.ASI_Name.Trim();
                        subItemName = "";
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", itemName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASI_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASI_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        itemId = Convert.ToInt32(result);
                    }

                    if (itemId == 0 && itemName != "")
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

                    if (dto.ASSI_Name != "")
                    {
                        subItemName = dto.ASSI_Name.Trim();
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", subItemName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSI_ScheduleType);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSI_OrgType);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subItemId = Convert.ToInt32(result);
                    }

                    if (subItemId == 0 && subItemName != "")
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
        public async Task<List<int>> SaveScheduleBalanceSheetAsync(int CompId, List<ScheduleTemplateBalanceSheetDto> dtos)
        {
            var resultIds = new List<int>();

            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iErrorLine = 1;
                string HeadingName = "";
                string subHeadingName = "";
                string itemName = "";
                string subItemName = "";

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

                    if (dto.ASH_Name != "")
                    {
                        HeadingName = dto.ASH_Name.Trim();
                        subHeadingName = "";
                        itemName = "";
                        subItemName = "";
                    }
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
                        checkCmd.Parameters.AddWithValue("@Name", HeadingName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.Ash_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.Ash_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        headingId = Convert.ToInt32(result);
                    }

                    if (headingId == 0 && HeadingName != "")
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
                            cmd.Parameters.AddWithValue("@ASH_Notes", dto.AST_AccHeadId);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            headingId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 2. Check SubHeading ===

                    if (dto.ASSH_Name != "")
                    {
                        subHeadingName = dto.ASSH_Name.Trim();
                        itemName = "";
                        subItemName = "";
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", subHeadingName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSH_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSH_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subHeadingId = Convert.ToInt32(result);
                    }

                    if (subHeadingId == 0 && subHeadingName != "")
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

                    if (dto.ASI_Name != "")
                    {
                        itemName = dto.ASI_Name.Trim();
                        subItemName = "";
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", itemName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASI_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASI_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        itemId = Convert.ToInt32(result);
                    }

                    if (itemId == 0 && itemName != "")
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

                    if (dto.ASSI_Name != "")
                    {
                        subItemName = dto.ASSI_Name.Trim();
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", subItemName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSI_ScheduleType);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSI_OrgType);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subItemId = Convert.ToInt32(result);
                    }

                    if (subItemId == 0 && subItemName != "")
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

        //SaveScheduleTemplate
        public async Task<List<int>> SaveScheduleTemplateAsync(int CompId, List<ScheduleTemplateDto> dtos)
        {
            var resultIds = new List<int>();


            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iErrorLine = 1;
                string HeadingName = "";
                string subHeadingName = "";
                string itemName = "";
                string subItemName = "";

                foreach (var dto in dtos)
                {
                    // === A. Map Account Head Name to ID ===
                    string accHeadName = (dto.AST_AccountHeadName ?? string.Empty).Trim().ToUpper();

                    if (accHeadName == "INCOME")
                    {
                        dto.AST_AccHeadId = 1;
                        dto.AST_Schedule_type = 3;
                    }
                    else if (accHeadName == "EXPENSES")
                    {
                        dto.AST_AccHeadId = 2;
                        dto.AST_Schedule_type = 3;
                    }
                    else if (accHeadName == "ASSETS")
                    {
                        dto.AST_AccHeadId = 1;
                        dto.AST_Schedule_type = 4;
                    }
                    else if (accHeadName == "CAPITAL AND LIABILITIES")
                    {
                        dto.AST_AccHeadId = 2;
                        dto.AST_Schedule_type = 4;
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

                    if (dto.ASH_Name != "")
                    {
                        HeadingName = dto.ASH_Name.Trim();
                        subHeadingName = "";
                        itemName = "";
                        subItemName = "";
                    }


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
                        checkCmd.Parameters.AddWithValue("@Name", HeadingName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.Ash_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.Ash_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        headingId = Convert.ToInt32(result);
                    }

                    if (headingId == 0 && HeadingName != "")
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
                            cmd.Parameters.AddWithValue("@ASH_Notes", dto.AST_AccHeadId);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            headingId = (int)(operParam.Value ?? 0);
                        }
                    }

                    // === 2. Check SubHeading ===

                    if (dto.ASSH_Name != "")
                    {
                        subHeadingName = dto.ASSH_Name.Trim();
                        itemName = "";
                        subItemName = "";
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", subHeadingName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSH_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSH_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subHeadingId = Convert.ToInt32(result);
                    }

                    if (subHeadingId == 0 && subHeadingName != "")
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

                    if (dto.ASI_Name != "")
                    {
                        itemName = dto.ASI_Name.Trim();
                        subItemName = "";
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", itemName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASI_scheduletype);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASI_Orgtype);

                        var result = await checkCmd.ExecuteScalarAsync();
                        itemId = Convert.ToInt32(result);
                    }

                    if (itemId == 0 && itemName != "")
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

                    if (dto.ASSI_Name != "")
                    {
                        subItemName = dto.ASSI_Name.Trim();
                    }

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
                        checkCmd.Parameters.AddWithValue("@Name", subItemName);
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.ASSI_ScheduleType);
                        checkCmd.Parameters.AddWithValue("@OrgType", dto.ASSI_OrgType);

                        var result = await checkCmd.ExecuteScalarAsync();
                        subItemId = Convert.ToInt32(result);
                    }

                    if (subItemId == 0 && subItemName != "")
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
        public async Task<List<int>> SaveOpeningBalanceAsync(int CompId, List<OpeningBalanceDto> dtos)
        {
            var resultIds = new List<int>();

            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // ✅ Step 1: Check if record already exists (before inserting new data)
                string checkQuery = @"
         SELECT ATBU_ID 
         FROM Acc_TrailBalance_Upload 
         WHERE ATBU_Description = @Description
            AND ATBU_CustId = @CustId 
            AND ATBU_YEARId = @YearId 
            AND ATBU_BranchId = @BranchId 
            AND ISNULL(ATBU_QuarterId, 0) = @DurationId";

                var existingRecord = await connection.QueryFirstOrDefaultAsync<int?>(checkQuery, new
                {
                    Description = dtos[0].ATBU_Description,
                    CustId = dtos[0].ATBU_CustId,
                    YearId = dtos[0].ATBU_YEARId,
                    BranchId = dtos[0].ATBU_Branchid,
                    DurationId = dtos[0].ATBU_QuarterId
                }, transaction);

                // ✅ Step 2: If record exists → delete it before inserting new data
                if (existingRecord.HasValue && dtos[0].FlagUpdate != 0)
                {
                    // Delete details using all relevant parameters
                    string deleteDetailsQuery = @"
             DELETE FROM Acc_TrailBalance_Upload_Details
             where ATBUD_CustId = @CustId
                AND ATBUD_YEARId = @YearId
                AND Atbud_Branchnameid = @BranchId
                AND ATBUd_QuarterId = @DurationId
                AND ATBUD_CompId = @CompId";

                    await connection.ExecuteAsync(deleteDetailsQuery, new
                    {
                        CustId = dtos[0].ATBU_CustId,
                        YearId = dtos[0].ATBU_YEARId,
                        BranchId = dtos[0].ATBU_Branchid,
                        DurationId = dtos[0].ATBU_QuarterId,
                        CompId = dtos[0].ATBU_CompId
                    }, transaction);

                    // Delete master using all relevant parameters
                    string deleteMasterQuery = @"
             DELETE FROM Acc_TrailBalance_Upload
             WHERE ATBU_CustId = @CustId 
                AND ATBU_YEARId = @YearId 
                AND ATBU_BranchId = @BranchId 
                AND ATBU_QuarterId = @DurationId
                AND ATBU_CompId = @CompId";

                    await connection.ExecuteAsync(deleteMasterQuery, new
                    {
                        CustId = dtos[0].ATBU_CustId,
                        YearId = dtos[0].ATBU_YEARId,
                        BranchId = dtos[0].ATBU_Branchid,
                        DurationId = dtos[0].ATBU_QuarterId,
                        CompId = dtos[0].ATBU_CompId
                    }, transaction);
                }


                // ✅ Step 3: Loop through DTOs for Insert/Update logic
                foreach (var dto in dtos)
                {
                    // 🔹 STEP A: Check if record exists (for update vs insert)
                    var existingRecordForUpdate = await connection.QueryFirstOrDefaultAsync<int>(@"
             SELECT ISNULL(ATBU_ID, 0)
             FROM Acc_TrailBalance_Upload
             WHERE ATBU_Custid = @CustId
                AND ATBU_YEARId = @YearId
                AND ISNULL(ATBU_Description, '') = @Description
                AND ATBU_BranchId = @BranchId
                AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
                    new
                    {
                        CustId = dto.ATBU_CustId,
                        YearId = dto.ATBU_YEARId,
                        Description = dto.ATBU_Description ?? string.Empty,
                        BranchId = dto.ATBU_Branchid,
                        QuarterId = dto.ATBU_QuarterId
                    }, transaction);

                    if (existingRecordForUpdate > 0)
                    {
                        dto.ATBU_ID = existingRecordForUpdate;
                    }
                    else
                    {
                        dto.ATBU_ID = 0;
                    }

                    // 🔹 Fetch Master ID (if exists)
                    var AtbuPkId = await connection.ExecuteScalarAsync<int>(@"
             SELECT ISNULL(ATBU_ID, 0)
             FROM Acc_TrailBalance_Upload
             WHERE ATBU_Custid = @CustId
                AND ATBU_YEARId = @YearId
                AND ISNULL(ATBU_Description, '') = @Description
                AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
                    new
                    {
                        CustId = dto.ATBU_CustId,
                        YearId = dto.ATBU_YEARId,
                        Description = dto.ATBU_Description ?? string.Empty,
                        QuarterId = dto.ATBU_QuarterId
                    }, transaction);

                    // 🔹 Fetch Detail ID (if exists)
                    var AtbudPkId = await connection.ExecuteScalarAsync<int>(@"
             SELECT ISNULL(ATBUD_ID, 0)
             FROM Acc_TrailBalance_Upload_Details
             WHERE ATBUD_Custid = @CustId
                AND ATBUD_YEARId = @YearId
                AND ISNULL(ATBUD_Description, '') = @Description
                AND ISNULL(ATBUD_QuarterId, 0) = @QuarterId",
                    new
                    {
                        CustId = dto.ATBUD_CustId,
                        YearId = dto.ATBUD_YEARId,
                        Description = dto.ATBUD_Description ?? string.Empty,
                        QuarterId = dto.ATBUD_QuarterId
                    }, transaction);

                    // 🔹 Continue with your original save logic (unchanged)
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
        public async Task<int[]> SaveTrailBalanceDetailsAsync(int CompId, List<TrailBalanceDto> dtos)
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
            using var transaction = connection.BeginTransaction();

            var insertedIds = new List<int>();

            try
            {
                // ✅ Step 1: Check if record already exists (global pre-check for batch)
                string checkQuery = @"
         SELECT ATBU_ID 
         FROM Acc_TrailBalance_Upload 
         WHERE ATBU_Description = @Description
            AND ATBU_CustId = @CustId 
            AND ATBU_YEARId = @YearId 
            AND ATBU_BranchId = @BranchId 
            AND ISNULL(ATBU_QuarterId, 0) = @DurationId";

                var existingRecord = await connection.QueryFirstOrDefaultAsync<int?>(checkQuery, new
                {
                    Description = dtos[0].ATBU_Description,
                    CustId = dtos[0].ATBU_CustId,
                    YearId = dtos[0].ATBU_YEARId,
                    BranchId = dtos[0].ATBU_Branchid,
                    DurationId = dtos[0].ATBU_QuarterId
                }, transaction);

                // ✅ Step 2: If record exists → delete before inserting new data
                if (existingRecord.HasValue && dtos[0].FlagUpdate != 0)
                {
                    // Delete details using all relevant parameters
                    string deleteDetailsQuery = @"
             DELETE FROM Acc_TrailBalance_Upload_Details
             where ATBUD_CustId = @CustId
                AND ATBUD_YEARId = @YearId
                AND Atbud_Branchnameid = @BranchId
                AND ATBUd_QuarterId = @DurationId
                AND ATBUD_CompId = @CompId";

                    await connection.ExecuteAsync(deleteDetailsQuery, new
                    {
                        CustId = dtos[0].ATBU_CustId,
                        YearId = dtos[0].ATBU_YEARId,
                        BranchId = dtos[0].ATBU_Branchid,
                        DurationId = dtos[0].ATBU_QuarterId,
                        CompId = dtos[0].ATBU_CompId
                    }, transaction);

                    // Delete master using all relevant parameters
                    string deleteMasterQuery = @"
             DELETE FROM Acc_TrailBalance_Upload
             WHERE ATBU_CustId = @CustId 
                AND ATBU_YEARId = @YearId 
                AND ATBU_BranchId = @BranchId 
                AND ATBU_QuarterId = @DurationId
                AND ATBU_CompId = @CompId";

                    await connection.ExecuteAsync(deleteMasterQuery, new
                    {
                        CustId = dtos[0].ATBU_CustId,
                        YearId = dtos[0].ATBU_YEARId,
                        BranchId = dtos[0].ATBU_Branchid,
                        DurationId = dtos[0].ATBU_QuarterId,
                        CompId = dtos[0].ATBU_CompId
                    }, transaction);
                }


                // ✅ Step 3: Process each DTO individually
                foreach (var dto in dtos)
                {
                    // 🔹 STEP A: Check if record exists (for update vs insert)
                    var existingRecordForUpdate = await connection.QueryFirstOrDefaultAsync<int>(@"
             SELECT ISNULL(ATBU_ID, 0)
             FROM Acc_TrailBalance_Upload
             WHERE ATBU_Custid = @CustId
                AND ATBU_YEARId = @YearId
                AND ISNULL(ATBU_Description, '') = @Description
                AND ATBU_BranchId = @BranchId
                AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
                    new
                    {
                        CustId = dto.ATBU_CustId,
                        YearId = dto.ATBU_YEARId,
                        Description = dto.ATBU_Description ?? string.Empty,
                        BranchId = dto.ATBU_Branchid,
                        QuarterId = dto.ATBU_QuarterId
                    }, transaction);

                    if (existingRecordForUpdate > 0)
                        dto.ATBU_ID = existingRecordForUpdate; // record found → update
                    else
                        dto.ATBU_ID = 0; // no record → insert new

                    // 🔹 Optional PK tracking for debug/reference
                    var AtbuPkId = await connection.ExecuteScalarAsync<int>(@"
             SELECT ISNULL(ATBU_ID, 0)
             FROM Acc_TrailBalance_Upload
             WHERE ATBU_Custid = @CustId
                AND ATBU_YEARId = @YearId
                AND ISNULL(ATBU_Description, '') = @Description
                AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
                    new
                    {
                        CustId = dto.ATBU_CustId,
                        YearId = dto.ATBU_YEARId,
                        Description = dto.ATBU_Description ?? string.Empty,
                        QuarterId = dto.ATBU_QuarterId
                    }, transaction);

                    var AtbudPkId = await connection.ExecuteScalarAsync<int>(@"
             SELECT ISNULL(ATBUD_ID, 0)
             FROM Acc_TrailBalance_Upload_Details
             WHERE ATBUD_Custid = @CustId
                AND ATBUD_YEARId = @YearId
                AND ISNULL(ATBUD_Description, '') = @Description
                AND ISNULL(ATBUD_QuarterId, 0) = @QuarterId",
                    new
                    {
                        CustId = dto.ATBUD_CustId,
                        YearId = dto.ATBUD_YEARId,
                        Description = dto.ATBUD_Description ?? string.Empty,
                        QuarterId = dto.ATBUD_QuarterId
                    }, transaction);

                    // ✅ Your existing stored procedure logic (unchanged)
                    int updateOrSave = 0, oper = 0;
                    int subItemId = 0, itemId = 0, subHeadingId = 0, headingId = 0, scheduleType = 0;

                    if (!string.IsNullOrWhiteSpace(dto.Excel_SubItem))
                        subItemId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleSubItems", "ASSI_ID", dto.Excel_SubItem, dto.ATBU_CustId);

                    if (!string.IsNullOrWhiteSpace(dto.Excel_Item))
                        itemId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleItems", "ASI_ID", dto.Excel_Item, dto.ATBU_CustId);

                    if (!string.IsNullOrWhiteSpace(dto.Excel_SubHeading))
                        subHeadingId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleSubHeading", "ASSH_ID", dto.Excel_SubHeading, dto.ATBU_CustId);

                    if (!string.IsNullOrWhiteSpace(dto.Excel_Heading))
                        headingId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleHeading", "ASH_ID", dto.Excel_Heading, dto.ATBU_CustId);

                    scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBUD_Company_Type);

                    // --- Master Insert ---
                    using (var cmdMaster = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        cmdMaster.CommandType = CommandType.StoredProcedure;
                        cmdMaster.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        cmdMaster.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        cmdMaster.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        cmdMaster.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        cmdMaster.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        cmdMaster.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? "127.0.0.1");
                        cmdMaster.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        cmdMaster.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var output1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var output2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdMaster.Parameters.Add(output1);
                        cmdMaster.Parameters.Add(output2);

                        await cmdMaster.ExecuteNonQueryAsync();

                        updateOrSave = (int)(output1.Value ?? 0);
                        oper = (int)(output2.Value ?? 0);
                    }

                    // --- Detail Insert ---
                    using (var cmdDetail = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                    {
                        cmdDetail.CommandType = CommandType.StoredProcedure;

                        cmdDetail.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Masid", oper);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_SChedule_Type", scheduleType);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Headingid", headingId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Subheading", subHeadingId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_itemid", itemId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_SubItemid", subItemId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? "127.0.0.1");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBU_YEARId);

                        var output1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var output2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdDetail.Parameters.Add(output1);
                        cmdDetail.Parameters.Add(output2);

                        await cmdDetail.ExecuteNonQueryAsync();
                        insertedIds.Add((int)(output2.Value ?? 0));
                    }
                }

                transaction.Commit();

                // ✅ Call UpdateNetIncomeAsync once with common values
                var firstDto = dtos.FirstOrDefault();
                if (firstDto != null)
                {
                    await UpdateNetIncomeAsync(
                        firstDto.ATBUD_CompId,
                        firstDto.ATBUD_CustId,
                        firstDto.ATBUD_CRBY,
                        firstDto.ATBUD_YEARId,
                        firstDto.ATBUD_Branchid,
                        firstDto.ATBUD_QuarterId
                    );
                }
                return insertedIds.ToArray();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<int> GetIdFromNameAsync(SqlConnection conn, SqlTransaction tran, string table, string column, string name, int orgType)
        {
            string nameCol = column.Replace("_ID", "_Name");
            string orgCol = column.Replace("_ID", "_Orgtype");

            var query = $"SELECT ISNULL({column}, 0) FROM {table} WHERE {nameCol} = @name AND {orgCol} = @orgType";
            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@name", name.Trim());
            cmd.Parameters.AddWithValue("@orgType", orgType);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }
        private async Task<int> GetScheduleTypeFromTemplateAsync(SqlConnection conn, SqlTransaction tran, int subItemId, int itemId, int subHeadingId, int headingId, int orgType)
        {
            string query = "";

            if (subItemId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_SubItemID = @id AND AST_Companytype = @orgType";
            else if (itemId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_ItemID = @id AND AST_SubItemID = 0 AND AST_Companytype = @orgType";
            else if (subHeadingId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_SubHeadingID = @id AND AST_Companytype = @orgType";
            else if (headingId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_HeadingID = @id AND AST_Companytype = @orgType";
            else
                return 0;

            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", subItemId > 0 ? subItemId : itemId > 0 ? itemId : subHeadingId > 0 ? subHeadingId : headingId);
            cmd.Parameters.AddWithValue("@orgType", orgType);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        //SaveClientTrailBalance
        public async Task<List<int>> ClientTrailBalanceAsync(int CompId, List<ClientTrailBalance> items)
        {
            var resultIds = new List<int>();


            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
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

        //UploadJournalEntry
        public async Task<List<int>> SaveCompleteTrailBalanceAsync(
            int CompId,
            List<TrailBalanceCompositeModel> models,
            IFormFile excelFile,
            string sheetName)
        {
            // ✅ Step 0: If Excel file is provided, parse into models
            if (excelFile != null && excelFile.Length > 0)
            {
                models = new List<TrailBalanceCompositeModel>();

                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // EPPlus license requirement
                using var package = new ExcelPackage(stream);

                var worksheet = package.Workbook.Worksheets[sheetName];
                if (worksheet == null)
                    throw new Exception($"Sheet '{sheetName}' not found in Excel file.");

                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++) // Skip header
                {
                    string GetDecimalCellValue(int col)
                    {
                        var text = worksheet.Cells[row, col].Text?.Trim();
                        return string.IsNullOrEmpty(text) ? "0" : text;
                    }

                    var model = new TrailBalanceCompositeModel
                    {
                        Upload = new TrailBalanceUploadDto
                        {
                            ATBU_ID = 0,
                            ATBU_CODE = "0",
                            ATBU_Description = worksheet.Cells[row, 8].Text,
                            ATBU_CustId = 12,
                            ATBU_Opening_Debit_Amount = decimal.Parse("0"),
                            ATBU_Opening_Credit_Amount = decimal.Parse("0"),
                            ATBU_TR_Debit_Amount = decimal.Parse("0"),
                            ATBU_TR_Credit_Amount = decimal.Parse("0"),
                            ATBU_Closing_Debit_Amount = decimal.Parse("0"),
                            ATBU_Closing_Credit_Amount = decimal.Parse("0"),
                            ATBU_DELFLG = "A",
                            ATBU_CRBY = "0",
                            ATBU_STATUS = "C",
                            ATBU_UPDATEDBY = "0",
                            ATBU_IPAddress = string.Empty,
                            ATBU_CompId = CompId,
                            ATBU_YEARId = 0,
                            ATBU_Branchid = 0,
                            ATBU_QuarterId = 0
                        },
                        UploadDetails = new TrailBalanceUploadDetailsDto
                        {
                            ATBUD_ID = 0,
                            ATBUD_Masid = 0,   // comes from your method (same as VB iATBUD_Masid)
                            ATBUD_CODE = "0",
                            ATBUD_Description = worksheet.Cells[row, 8].Text,
                            ATBUD_CustId = 12,
                            ATBUD_SChedule_Type = 0,
                            ATBUD_Branchid = 0,
                            ATBUD_QuarterId = 0,
                            ATBUD_Company_Type = 0,
                            ATBUD_Headingid = 0,
                            ATBUD_Subheading = 0,
                            ATBUD_itemid = 0,
                            ATBUD_Subitemid = 0,
                            ATBUD_DELFLG = "A",
                            ATBUD_CRBY = "0",             // if userId available, set it
                            ATBUD_UPDATEDBY = "0",        // if update userId available, set it
                            ATBUD_STATUS = "C",
                            ATBUD_Progress = string.Empty,
                            ATBUD_IPAddress = string.Empty,
                            ATBUD_CompId = CompId,
                            ATBUD_YEARId = 0
                        },
                        JournalEntry = new JournalEntryDto
                        {
                            Acc_JE_ID = 0,
                            Acc_JE_TransactionNo = "13",
                            Acc_JE_Party = 12,
                            Acc_JE_Location = 0,
                            Acc_JE_BillType = 0,
                            Acc_JE_BillNo = string.Empty,
                            Acc_JE_BillDate = null,   // use null instead of dummy date
                            Acc_JE_BillAmount = 0m,
                            Acc_JE_AdvanceAmount = 0m,
                            Acc_JE_AdvanceNaration = string.Empty,
                            Acc_JE_BalanceAmount = 0m,
                            Acc_JE_NetAmount = 0m,
                            Acc_JE_PaymentNarration = string.Empty,
                            Acc_JE_ChequeNo = string.Empty,
                            Acc_JE_ChequeDate = null,
                            Acc_JE_IFSCCode = string.Empty,
                            Acc_JE_BankName = string.Empty,
                            Acc_JE_BranchName = string.Empty,
                            Acc_JE_CreatedBy = "0",
                            Acc_JE_UPDATEDBY = "0",
                            Acc_JE_YearID = 0,
                            Acc_JE_CompID = CompId,
                            Acc_JE_Status = "C",
                            Acc_JE_Operation = string.Empty,
                            Acc_JE_IPAddress = "0.0.0.0",
                            Acc_JE_BillCreatedDate = null,
                            Acc_JE_BranchId = 0,
                            Acc_JE_QuarterId = 0,
                            Acc_JE_Comnments = string.Empty
                        },
                        TransactionDetailsList = new List<TransactionDetailsDto>
{
    new TransactionDetailsDto
    {
        AJTB_ID = 0,
        AJTB_MasID = 0,                 // will be set after JE master insert
        AJTB_TranscNo = "13",
        AJTB_CustId = 0,
        AJTB_ScheduleTypeid = 0,
        AJTB_Deschead = string.Empty,
        AJTB_Desc = string.Empty,
        AJTB_Debit = 0m,
        AJTB_Credit = 0m,
        AJTB_CreatedBy = "0",
        AJTB_UpdatedBy = "0",
        AJTB_Status = "C",
        AJTB_IPAddress = "0.0.0.0",
        AJTB_CompID = CompId,
        AJTB_YearID = 0,
        AJTB_BillType = 0,
        AJTB_DescName = string.Empty,
        AJTB_BranchId = 0,
        AJTB_QuarterId = 0
    }
        }
                    };

                    models.Add(model);
                }

            }

            if (models == null || !models.Any())
                throw new Exception("No valid Trail Balance data found to save.");

            var resultIds = new List<int>();

            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Save to DB
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var model in models)
                {
                    int uploadId = 0, jeMasId = 0;

                    // === 1. Save Upload ===
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
                        cmd.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBU_CRBY", 0);
                        cmd.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        cmd.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        cmd.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        cmd.Parameters.AddWithValue("@ATBU_Branchid", "0");
                        cmd.Parameters.AddWithValue("@ATBU_QuarterId", "0");

                        var out1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var out2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(out1);
                        cmd.Parameters.Add(out2);

                        await cmd.ExecuteNonQueryAsync();
                        uploadId = (int)(out2.Value ?? dto.ATBU_ID);
                    }

                    // === 2. Save Upload Details ===
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
                        cmd.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                        cmd.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                        cmd.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                        cmd.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

                        // Output parameters
                        var updateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                        { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(updateOrSave);

                        var oper = new SqlParameter("@iOper", SqlDbType.Int)
                        { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(oper);

                        await cmd.ExecuteNonQueryAsync();

                        // if you want output values
                        var resultUpdateOrSave = (int)(updateOrSave.Value ?? 0);
                        var resultOper = (int)(oper.Value ?? 0);
                    }


                    // === 3. Save Journal Entry ===
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
                        cmd.Parameters.AddWithValue("@Acc_JE_BillDate", (object?)dto.Acc_JE_BillDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Acc_JE_BillAmount", dto.Acc_JE_BillAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_AdvanceAmount", dto.Acc_JE_AdvanceAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_AdvanceNaration", dto.Acc_JE_AdvanceNaration ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BalanceAmount", dto.Acc_JE_BalanceAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_NetAmount", dto.Acc_JE_NetAmount);
                        cmd.Parameters.AddWithValue("@Acc_JE_PaymentNarration", dto.Acc_JE_PaymentNarration ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_ChequeNo", dto.Acc_JE_ChequeNo ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_ChequeDate", (object?)dto.Acc_JE_ChequeDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Acc_JE_IFSCCode", dto.Acc_JE_IFSCCode ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BankName", dto.Acc_JE_BankName ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BranchName", dto.Acc_JE_BranchName ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_CreatedBy", dto.Acc_JE_CreatedBy ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_UPDATEDBY", dto.Acc_JE_UPDATEDBY ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_YearID", dto.Acc_JE_YearID);
                        cmd.Parameters.AddWithValue("@Acc_JE_CompID", dto.Acc_JE_CompID);
                        cmd.Parameters.AddWithValue("@Acc_JE_Status", dto.Acc_JE_Status ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_Operation", dto.Acc_JE_Operation ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_IPAddress", dto.Acc_JE_IPAddress ?? "");
                        cmd.Parameters.AddWithValue("@Acc_JE_BillCreatedDate", (object?)dto.Acc_JE_BillCreatedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Acc_JE_BranchId", dto.Acc_JE_BranchId);
                        cmd.Parameters.AddWithValue("@Acc_JE_QuarterId", dto.Acc_JE_QuarterId);
                        cmd.Parameters.AddWithValue("@Acc_JE_Comnments", dto.Acc_JE_Comnments ?? "");

                        // Output params
                        var updateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var iOper = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(updateOrSave);
                        cmd.Parameters.Add(iOper);

                        await cmd.ExecuteNonQueryAsync();

                        jeMasId = (int)(iOper.Value ?? dto.Acc_JE_ID);
                    }


                    // === 4. Save Transaction Details ===
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
                            cmd.Parameters.AddWithValue("@AJTB_Deschead", t.AJTB_Deschead ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_Desc", t.AJTB_Desc ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_Debit", t.AJTB_Debit);
                            cmd.Parameters.AddWithValue("@AJTB_Credit", t.AJTB_Credit);
                            cmd.Parameters.AddWithValue("@AJTB_CreatedBy", t.AJTB_CreatedBy ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_UpdatedBy", t.AJTB_UpdatedBy ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_Status", t.AJTB_Status ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_IPAddress", t.AJTB_IPAddress ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_CompID", t.AJTB_CompID);
                            cmd.Parameters.AddWithValue("@AJTB_YearID", t.AJTB_YearID);
                            cmd.Parameters.AddWithValue("@AJTB_BillType", t.AJTB_BillType);
                            cmd.Parameters.AddWithValue("@AJTB_DescName", t.AJTB_DescName ?? "");
                            cmd.Parameters.AddWithValue("@AJTB_BranchId", t.AJTB_BranchId);
                            cmd.Parameters.AddWithValue("@AJTB_QuarterId", t.AJTB_QuarterId);

                            // Output params
                            var updateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var iOper = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(updateOrSave);
                            cmd.Parameters.Add(iOper);

                            await cmd.ExecuteNonQueryAsync();
                        }
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


        //UpdateNetIncome
        public async Task<bool> UpdateNetIncomeAsync(int compId, int custId, int userId, int yearId, int branchId, int durationId)
        {
            // Step 1: Get DB name from session
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 3: Check if 'Net income' record already exists
            var checkQuery = @"
      SELECT * 
      FROM Acc_TrailBalance_Upload 
      WHERE ATBU_Description = 'Net income' 
        AND ATBU_CustId = @CustId 
        AND ATBU_YEARId = @YearId 
        AND ATBU_Branchid = @BranchId 
        AND ATBU_QuarterId = @DurationId";

            var existingRecord = await connection.QueryFirstOrDefaultAsync(checkQuery, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });

            if (existingRecord != null)
            {
                // Record already exists; no insert required
                return false;
            }

            // Step 4: Get new ID
            var maxIdQuery = @"
      SELECT ISNULL(MAX(ATBU_ID), 0) + 1 
      FROM Acc_TrailBalance_Upload ";

            int newId = await connection.ExecuteScalarAsync<int>(maxIdQuery, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });

            // Step 5: Insert into Acc_TrailBalance_Upload
            var insertUploadQuery = @"
      INSERT INTO Acc_TrailBalance_Upload
      (
          ATBU_ID, ATBU_Description, ATBU_CODE, ATBU_CustId, ATBU_Branchid, ATBU_QuarterId, ATBU_YEARId,
          ATBU_Opening_Debit_Amount, ATBU_Opening_Credit_Amount, ATBU_TR_Debit_Amount, ATBU_TR_Credit_Amount,
          ATBU_Closing_Debit_Amount, ATBU_Closing_Credit_Amount, ATBU_Closing_TotalDebit_Amount, ATBU_Closing_TotalCredit_Amount,
          ATBU_CRBY, Atbu_CrOn, ATBU_CompId
      )
      VALUES
      (
          @NewId, 'Net Income', @NewId, @CustId, @BranchId, @DurationId, @YearId,
          0, 0, 0, 0, 0, 0, 0, 0,
          @UserId, GETDATE(), @CompId
      );";

            await connection.ExecuteAsync(insertUploadQuery, new
            {
                NewId = newId,
                CustId = custId,
                BranchId = branchId,
                DurationId = durationId,
                YearId = yearId,
                UserId = userId,
                CompId = compId
            });

            maxIdQuery = @"
      SELECT ISNULL(MAX(ATBUD_ID), 0) + 1 
      FROM Acc_TrailBalance_Upload_Details ";

            newId = await connection.ExecuteScalarAsync<int>(maxIdQuery, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });

            // Step 6: Insert into Acc_TrailBalance_Upload_details
            var insertDetailQuery = @"
      INSERT INTO Acc_TrailBalance_Upload_details
      (
          ATBUD_ID, ATBUD_Masid, ATBUD_Description, ATBUD_CODE, ATBUD_CustId, Atbud_Branchnameid,
          ATBUD_QuarterId, ATBUD_YEARId, ATBUD_CRBY, AtbuD_CrOn, ATBUD_CompId
      )
      VALUES
      (
          @NewId, @NewId, 'Net Income', @NewId, @CustId, @BranchId,
          @DurationId, @YearId, @UserId, GETDATE(), @CompId
      );";

            await connection.ExecuteAsync(insertDetailQuery, new
            {
                NewId = newId,
                CustId = custId,
                BranchId = branchId,
                DurationId = durationId,
                YearId = yearId,
                UserId = userId,
                CompId = compId
            });
            return true;
        }

        //UploadCustomerTrialBalance
        public async Task<string> UploadCustomerTrialBalanceExcelAsync(
            IFormFile excelFile,
            int customerId,
            int yearId,
            int branchId,
            int quarterId,
            int companyId,
            int userId)
        {
            // ---------------- BASIC VALIDATIONS ----------------
            if (excelFile == null || excelFile.Length == 0)
                return "Please upload Excel file";

            if (customerId == 0)
                return "Select Client";

            if (yearId == 0)
                return "Select FinancialYear";

            if (branchId == 0)
                return "Select Branch";

            if (quarterId == 0)
                return "Select Duration";

            // ---------------- DB FROM SESSION ----------------
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionString = _configuration.GetConnectionString(dbName);

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"Connection string for '{dbName}' not found.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tran = connection.BeginTransaction();

            try
            {
                using var stream = excelFile.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var sheet = package.Workbook.Worksheets[0];

                if (sheet.Dimension == null)
                    throw new Exception("Excel sheet is empty");

                int startRow = 4; // DATA STARTS FROM ROW 4
                int lastRow = sheet.Dimension.End.Row;
                string ipAddress =
               _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()?? "UNKNOWN";


                for (int row = startRow; row <= lastRow; row++)
                {
                    string description = sheet.Cells[row, 2].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(description))
                        throw new Exception($"Description missing at row {row}");

                    decimal GetDecimal(int col)
                    {
                        var text = sheet.Cells[row, col].Text;
                        return decimal.TryParse(text, out var val) ? val : 0;
                    }

                    // -------- CHECK TRAIL BALANCE EXISTS --------
                    int trailBalCount = await connection.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(1)
                      FROM Acc_TrailBalance_Upload
                      WHERE ATBU_CustId=@CustId
                        AND ATBU_Branchid=@BranchId
                        AND ATBU_CompId=@CompId
                        AND ATBU_YEARId=@YearId
                        AND ATBU_QuarterId=@QuarterId",
                        new
                        {
                            CustId = customerId,
                            BranchId = branchId,
                            CompId = companyId,
                            YearId = yearId,
                            QuarterId = quarterId
                        },
                        tran);

                    if (trailBalCount != 0)
                        throw new Exception("Trail Balance Not exist.");

                    // -------- GET MASTER ID --------
                    int masId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(ATBU_ID,0)
                      FROM Acc_TrailBalance_Upload
                      WHERE ATBU_Description=@Desc
                        AND ATBU_CustId=@CustId
                        AND ATBU_Branchid=@BranchId
                        AND ATBU_CompId=@CompId
                        AND ATBU_YEARId=@YearId
                        AND ATBU_QuarterId=@QuarterId",
                        new
                        {
                            Desc = description,
                            CustId = customerId,
                            BranchId = branchId,
                            CompId = companyId,
                            YearId = yearId,
                            QuarterId = quarterId
                        },
                        tran);

                    // -------- CHECK CUSTOMER UPLOAD --------
                    int customerUploadId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(ATBCU_ID,0)
                      FROM Acc_TrailBalance_CustomerUpload
                      WHERE ATBCU_Description=@Desc
                        AND ATBCU_CustId=@CustId
                        AND ATBCU_Branchid=@BranchId
                        AND ATBCU_CompId=@CompId
                        AND ATBCU_YEARId=@YearId
                        AND ATBCU_QuarterId=@QuarterId",
                        new
                        {
                            Desc = description,
                            CustId = customerId,
                            BranchId = branchId,
                            CompId = companyId,
                            YearId = yearId,
                            QuarterId = quarterId
                        },
                        tran);

                    // -------- MAX COUNT --------
                    int maxCount = await connection.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(*)
                      FROM Acc_TrailBalance_Upload
                      WHERE ATBU_CustId=@CustId
                        AND ATBU_CompId=@CompId
                        AND ATBU_YEARId=@YearId
                        AND ATBU_BranchId=@BranchId
                        AND ATBU_QuarterId=@QuarterId",
                        new
                        {
                            CustId = customerId,
                            CompId = companyId,
                            YearId = yearId,
                            BranchId = branchId,
                            QuarterId = quarterId
                        },
                        tran);

                    // -------- SAVE --------
                    var param = new DynamicParameters();
                    param.Add("@ATBCU_ID", customerUploadId);
                    param.Add("@ATBCU_CODE", $"SCh00{maxCount + 1}");
                    param.Add("@ATBCU_Description", description);
                    param.Add("@ATBCU_CustId", customerId);
                    param.Add("@ATBCU_Opening_Debit_Amount", GetDecimal(3));
                    param.Add("@ATBCU_Opening_Credit_Amount", GetDecimal(4));
                    param.Add("@ATBCU_TR_Debit_Amount", GetDecimal(5));
                    param.Add("@ATBCU_TR_Credit_Amount", GetDecimal(6));
                    param.Add("@ATBCU_Closing_Debit_Amount", GetDecimal(7));
                    param.Add("@ATBCU_Closing_Credit_Amount", GetDecimal(8));
                    param.Add("@ATBCU_DELFLG", "A");
                    param.Add("@ATBCU_CRBY", userId);
                    param.Add("@ATBCU_STATUS", "C");
                    param.Add("@ATBCU_UPDATEDBY", userId);
                    param.Add("@ATBCU_IPAddress", ipAddress);
                    param.Add("@ATBCU_CompId", companyId);
                    param.Add("@ATBCU_YEARId", yearId);
                    param.Add("@ATBCU_Branchid", branchId);
                    param.Add("@ATBCU_MasId", masId);
                    param.Add("@ATBCU_QuarterId", quarterId);
                    param.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    param.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_TrailBalance_CustomerUpload",
                        param,
                        tran,
                        commandType: CommandType.StoredProcedure);
                }

                tran.Commit();
                return "Successfully Upload";
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}

