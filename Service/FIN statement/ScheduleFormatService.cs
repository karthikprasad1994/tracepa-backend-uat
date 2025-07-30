using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleFormatService : ScheduleFormatInterface
    {
        private readonly IConfiguration _configuration;
        public ScheduleFormatService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //GetScheduleHeading
        public async Task<IEnumerable<ScheduleHeadingDto>> GetScheduleFormatHeadingAsync(int CompId, int ScheduleId, int CustId, int AccHead)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ASH_ID AS HeadingId, 
            ASH_Name AS HeadingName
        FROM ACC_ScheduleHeading
        WHERE 
            Ash_scheduletype = @ScheduleId AND
            Ash_Orgtype = @CustId AND 
            ASH_Notes = @AccHead
        ORDER BY ASH_ID";

            await connection.OpenAsync();

            var result = await connection.QueryAsync<ScheduleHeadingDto>(query, new
            {
                CustId = CustId,
                ScheduleId = ScheduleId,
                AccHead = AccHead
            });
            return result;
        }

        //GetScheduleSubHeading
        public async Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleFormatSubHeadingAsync(
        int CompId, int ScheduleId, int CustId, int HeadingId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ASSH_ID AS SubheadingID, 
            ASSH_Name AS SubheadingName, 
            ISNULL(ASSH_Notes, 0) AS Notes
        FROM ACC_ScheduleSubHeading
        WHERE 
            ASSH_scheduletype = @ScheduleId AND
            ASSh_Orgtype = @CustId AND 
            ASSH_HeadingID = @HeadingId
        ORDER BY ASSH_ID";

            await connection.OpenAsync();

            var result = await connection.QueryAsync<ScheduleSubHeadingDto>(query, new
            {
                CustId = CustId,
                ScheduleId = ScheduleId,
                HeadingId = HeadingId
            });

            return result;
        }

        //GetScheduleItem
        public async Task<IEnumerable<ScheduleItemDto>> GetScheduleFormatItemsAsync(
        int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ASI_ID AS ItemsId, 
            ASI_Name AS ItemsName
        FROM ACC_ScheduleItems
        WHERE 
            ASI_Orgtype = @CustId AND 
            ASI_scheduletype = @ScheduleId AND 
            ASI_SubHeadingID = @SubHeadId
        ORDER BY ASI_ID";

            await connection.OpenAsync();

            var result = await connection.QueryAsync<ScheduleItemDto>(query, new
            {
                CustId = CustId,
                ScheduleId = ScheduleId,
                SubHeadId = SubHeadId
            });

            return result;
        }

        //GetScheduleSubItem
        public async Task<IEnumerable<ScheduleSubItemDto>> GetScheduleFormatSubItemsAsync(
        int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId, int ItemId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ASSI_ID AS SubitemsId, 
            ASSI_Name AS SubitemsName
        FROM ACC_ScheduleSubItems
        WHERE 
            ASSI_scheduletype = @ScheduleId AND
            ASSI_Orgtype = @CustId AND 
            ASSI_ItemsID = @ItemId
        ORDER BY ASSI_ID";

            await connection.OpenAsync();

            var result = await connection.QueryAsync<ScheduleSubItemDto>(query, new
            {
                CustId = CustId,
                ScheduleId = ScheduleId,
                ItemId = ItemId
            });
            return result;
        }

        //GetScheduleTemplate
        public async Task<IEnumerable<ScheduleFormatTemplateDto>> GetScheduleTemplateAsync(
       int CompId, int ScheduleId, int CustId, int AccHead)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var baseQuery = @"
WITH NumberedRows AS (
    SELECT 
        a.AST_ID,
        b.ASH_ID AS HeadingId,
        b.ASH_Name AS HeadingName,
        c.ASSH_ID AS SubheadingID,
        c.ASSH_Name AS SubheadingName,
        d.ASI_ID AS ItemId,
        d.ASI_Name AS ItemName,
        e.ASSI_ID AS SubitemId,
        e.ASSI_Name AS SubitemName,
        a.AST_AccHeadId,
        CASE  
            WHEN a.AST_Schedule_type = 4 AND a.AST_AccHeadId = 1 THEN 'ASSETS'
            WHEN a.AST_Schedule_type = 4 AND a.AST_AccHeadId = 2 THEN 'CAPITAL AND LIABILITIES'
            WHEN a.AST_Schedule_type = 3 AND a.AST_AccHeadId = 1 THEN 'INCOME'
            WHEN a.AST_Schedule_type = 3 AND a.AST_AccHeadId = 2 THEN 'EXPENSES'
        END AS AccHeadName,

        ROW_NUMBER() OVER (PARTITION BY b.ASH_ID ORDER BY a.AST_ID) AS HeadingRow,
        ROW_NUMBER() OVER (PARTITION BY b.ASH_ID, c.ASSH_ID ORDER BY a.AST_ID) AS SubheadingRow,
        ROW_NUMBER() OVER (PARTITION BY b.ASH_ID, c.ASSH_ID, d.ASI_ID ORDER BY a.AST_ID) AS ItemRow,
        ROW_NUMBER() OVER (PARTITION BY b.ASH_ID, c.ASSH_ID, d.ASI_ID, e.ASSI_ID ORDER BY a.AST_ID) AS SubitemRow

    FROM ACC_ScheduleTemplates a
    LEFT JOIN ACC_ScheduleHeading b 
        ON b.ASH_ID = a.AST_HeadingId AND b.ASH_Orgtype = @CustId
    LEFT JOIN ACC_ScheduleSubHeading c 
        ON c.ASSH_ID = a.AST_SubHeadingId AND c.ASSH_Orgtype = @CustId
    LEFT JOIN ACC_ScheduleItems d 
        ON d.ASI_ID = a.AST_ItemId AND d.ASI_Orgtype = @CustId
    LEFT JOIN ACC_ScheduleSubItems e 
        ON e.ASSI_ID = a.AST_SubItemId AND e.ASSI_Orgtype = @CustId
    WHERE a.AST_CompId = @CompId
";

            // Add dynamic filtering
            if (CustId != 0)
                baseQuery += " AND a.AST_Companytype = @CustId";

            if (ScheduleId != 0)
                baseQuery += " AND a.AST_Schedule_type = @ScheduleId";

            if (AccHead != 0)
                baseQuery += " AND a.AST_AccHeadId = @AccHead";

            baseQuery += @"
)
SELECT
    AST_ID,
    HeadingId,
    CASE WHEN HeadingRow = 1 THEN HeadingName ELSE NULL END AS HeadingName,
    SubheadingID,
    CASE WHEN SubheadingRow = 1 THEN SubheadingName ELSE NULL END AS SubheadingName,
    ItemId,
    CASE WHEN ItemRow = 1 THEN ItemName ELSE NULL END AS ItemName,
    SubitemId,
    CASE WHEN SubitemRow = 1 THEN SubitemName ELSE NULL END AS SubitemName,
    AST_AccHeadId,
    AccHeadName
FROM NumberedRows
";

            var parameters = new
            {
                CompId = CompId,
                ScheduleId = ScheduleId,
                CustId = CustId,
                AccHead = AccHead
            };

            var result = await connection.QueryAsync<ScheduleFormatTemplateDto>(baseQuery, parameters);
            return result;
        }


        //DeleteScheduleTemplate(Grid)
        public async Task<bool> DeleteScheduleTemplateAsync(int CompId, int ScheduleType, int CustId, int SelectedValue, int MainId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                if (SelectedValue == 1)
                {
                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_HeadingID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId",
                        new { Id = MainId, Type = ScheduleType, CustId = CustId, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleHeading WHERE ASH_ID = @Id AND Ash_Orgtype = @CustId AND Ash_scheduletype = @Type AND ASH_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubHeading WHERE ASSH_HeadingID = @Id AND Assh_Orgtype = @CustId AND Assh_scheduletype = @Type AND ASSH_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_HeadingID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_HeadingID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);
                }
                else if (SelectedValue == 2)
                {
                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_SubHeadingID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId",
                        new { Id = MainId, Type = ScheduleType, CustId = CustId, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubHeading WHERE ASSH_ID = @Id AND Assh_Orgtype = @CustId AND Assh_scheduletype = @Type AND ASSH_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_SubHeadingID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_SubHeadingID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);
                }
                else if (SelectedValue == 3)
                {
                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_ItemID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId",
                        new { Id = MainId, Type = ScheduleType, CustId = CustId, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_ID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_ItemsID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);
                }
                else if (SelectedValue == 4)
                {
                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_SubItemID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId",
                        new { Id = MainId, Type = ScheduleType, CustId = CustId, CompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_ID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, CustId = CustId, Type = ScheduleType, CompId = CompId }, transaction);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //SaveScheduleHeadingAndTemplate
        public async Task<int[]> SaveScheduleHeadingAndTemplateAsync(int CompId, SaveScheduleHeadingDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iPKId = dto.ASH_ID;

                // --- Save Schedule Heading ---
                int updateOrSave, oper;
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

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    headingCommand.Parameters.Add(updateOrSaveParam);
                    headingCommand.Parameters.Add(operParam);

                    await headingCommand.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0);
                }
                 bool isNewHeading = dto.ASH_ID == 0 || updateOrSave == 1;

                // Check if it's a new heading insert, then insert template
               if (iPKId == 0)
                {
                    using var templateCommand = new SqlCommand("spACC_ScheduleTemplates", connection, transaction);
                    templateCommand.CommandType = CommandType.StoredProcedure;

                    templateCommand.Parameters.AddWithValue("@AST_ID", dto.AST_ID);
                    templateCommand.Parameters.AddWithValue("@AST_Name", dto.AST_Name ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_HeadingID", oper); // Use the returned heading ID
                    templateCommand.Parameters.AddWithValue("@AST_SubHeadingID", dto.AST_SubHeadingID);
                    templateCommand.Parameters.AddWithValue("@AST_ItemID", dto.AST_ItemID);
                    templateCommand.Parameters.AddWithValue("@AST_SubItemID", dto.AST_SubItemID);
                    templateCommand.Parameters.AddWithValue("@AST_AccHeadId", dto.AST_AccHeadId);
                    templateCommand.Parameters.AddWithValue("@AST_DELFLG", dto.AST_DELFLG);
                    templateCommand.Parameters.AddWithValue("@AST_CRBY", dto.AST_CRBY);
                    templateCommand.Parameters.AddWithValue("@AST_STATUS", dto.AST_STATUS ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_UPDATEDBY", dto.AST_UPDATEDBY);
                    templateCommand.Parameters.AddWithValue("@AST_IPAddress", dto.AST_IPAddress ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_CompId", dto.ASH_CompId);
                    templateCommand.Parameters.AddWithValue("@AST_YEARId", dto.AST_YEARId);
                    templateCommand.Parameters.AddWithValue("@AST_Schedule_type", dto.AST_Schedule_type);
                    templateCommand.Parameters.AddWithValue("@AST_Companytype", dto.AST_Companytype);
                    templateCommand.Parameters.AddWithValue("@AST_Company_limit", dto.AST_Company_limit);

                    var updateOrSaveParamTemplate = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParamTemplate = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    templateCommand.Parameters.Add(updateOrSaveParamTemplate);
                    templateCommand.Parameters.Add(operParamTemplate);

                    await templateCommand.ExecuteNonQueryAsync();
                }

                transaction.Commit();
                return new int[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //SaveScheduleSubHeadingAndTemplate
        public async Task<int[]> SaveScheduleSubHeadingAndTemplateAsync(int CompId, SaveScheduleSubHeadingDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iPKId = dto.ASSH_ID;
                int updateOrSave, oper;

                // --- Save SubHeading ---
                using (var subHeadingCommand = new SqlCommand("spACC_ScheduleSubHeading", connection, transaction))
                {
                    subHeadingCommand.CommandType = CommandType.StoredProcedure;

                    subHeadingCommand.Parameters.AddWithValue("@ASSH_ID", dto.ASSH_ID);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_Name", dto.ASSH_Name ?? string.Empty);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_HeadingID", dto.ASSH_HeadingID);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_DELFLG", dto.ASSH_DELFLG ?? string.Empty);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_CRBY", dto.ASSH_CRBY);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_STATUS", dto.ASSH_STATUS ?? string.Empty);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_UPDATEDBY", dto.ASSH_UPDATEDBY);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_IPAddress", dto.ASSH_IPAddress ?? string.Empty);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_CompId", dto.ASSH_CompId);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_YEARId", dto.ASSH_YEARId);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_Notes", dto.ASSH_Notes);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_scheduletype", dto.ASSH_scheduletype);
                    subHeadingCommand.Parameters.AddWithValue("@ASSH_Orgtype", dto.ASSH_Orgtype);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    subHeadingCommand.Parameters.Add(updateOrSaveParam);
                    subHeadingCommand.Parameters.Add(operParam);

                    await subHeadingCommand.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0); // ASSH_ID inserted/updated
                }

                bool isNewSubHeading = dto.ASSH_ID == 0 || updateOrSave == 1;

                if (iPKId == 0)
                {
                    using var templateCommand = new SqlCommand("spACC_ScheduleTemplates", connection, transaction);
                    templateCommand.CommandType = CommandType.StoredProcedure;

                    templateCommand.Parameters.AddWithValue("@AST_ID", dto.AST_ID);
                    templateCommand.Parameters.AddWithValue("@AST_Name", dto.AST_Name ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_HeadingID", dto.AST_HeadingID);
                    templateCommand.Parameters.AddWithValue("@AST_SubHeadingID", oper); // Use returned subheading ID
                    templateCommand.Parameters.AddWithValue("@AST_ItemID", dto.AST_ItemID);
                    templateCommand.Parameters.AddWithValue("@AST_SubItemID", dto.AST_SubItemID);
                    templateCommand.Parameters.AddWithValue("@AST_AccHeadId", dto.AST_AccHeadId);
                    templateCommand.Parameters.AddWithValue("@AST_DELFLG", dto.AST_DELFLG);
                    templateCommand.Parameters.AddWithValue("@AST_CRBY", dto.AST_CRBY);
                    templateCommand.Parameters.AddWithValue("@AST_STATUS", dto.AST_STATUS ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_UPDATEDBY", dto.AST_UPDATEDBY);
                    templateCommand.Parameters.AddWithValue("@AST_IPAddress", dto.AST_IPAddress ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_CompId", dto.AST_CompId);
                    templateCommand.Parameters.AddWithValue("@AST_YEARId", dto.AST_YEARId);
                    templateCommand.Parameters.AddWithValue("@AST_Schedule_type", dto.AST_Schedule_type);
                    templateCommand.Parameters.AddWithValue("@AST_Companytype", dto.AST_Companytype);
                    templateCommand.Parameters.AddWithValue("@AST_Company_limit", dto.AST_Company_limit);

                    var updateOrSaveParamTemplate = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParamTemplate = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    templateCommand.Parameters.Add(updateOrSaveParamTemplate);
                    templateCommand.Parameters.Add(operParamTemplate);

                    await templateCommand.ExecuteNonQueryAsync();

                    // You may read template status if needed
                }

                transaction.Commit();
                return new[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        //SaveScheduleItem
        public async Task<int[]> SaveScheduleItemAndTemplateAsync(int CompId, SaveScheduleItemDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iPKId = dto.ASI_ID;
                int updateOrSave, oper;

                // --- Save Schedule Item ---
                using (var itemCommand = new SqlCommand("spACC_ScheduleItems", connection, transaction))
                {
                    itemCommand.CommandType = CommandType.StoredProcedure;

                    itemCommand.Parameters.AddWithValue("@ASI_ID", dto.ASI_ID);
                    itemCommand.Parameters.AddWithValue("@ASI_Name", dto.ASI_Name ?? string.Empty);
                    itemCommand.Parameters.AddWithValue("@ASI_HeadingID", dto.ASI_HeadingID);
                    itemCommand.Parameters.AddWithValue("@ASI_SubHeadingID", dto.ASI_SubHeadingID);
                    itemCommand.Parameters.AddWithValue("@ASI_DELFLG", dto.ASI_DELFLG ?? string.Empty);
                    itemCommand.Parameters.AddWithValue("@ASI_CRBY", dto.ASI_CRBY);
                    itemCommand.Parameters.AddWithValue("@ASI_STATUS", dto.ASI_STATUS ?? string.Empty);
                    itemCommand.Parameters.AddWithValue("@ASI_IPAddress", dto.ASI_IPAddress ?? string.Empty);
                    itemCommand.Parameters.AddWithValue("@ASI_CompId", dto.ASI_CompId);
                    itemCommand.Parameters.AddWithValue("@ASI_YEARId", dto.ASI_YEARId);
                    itemCommand.Parameters.AddWithValue("@ASI_scheduletype", dto.ASI_scheduletype);
                    itemCommand.Parameters.AddWithValue("@ASI_Orgtype", dto.ASI_Orgtype);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    itemCommand.Parameters.Add(updateOrSaveParam);
                    itemCommand.Parameters.Add(operParam);

                    await itemCommand.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0); // This is the inserted or updated ASI_ID
                }

                bool isNewItem = dto.ASI_ID == 0 || updateOrSave == 1;

                if (iPKId == 0)
                {
                    using var templateCommand = new SqlCommand("spACC_ScheduleTemplates", connection, transaction);
                    templateCommand.CommandType = CommandType.StoredProcedure;

                    templateCommand.Parameters.AddWithValue("@AST_ID", dto.AST_ID);
                    templateCommand.Parameters.AddWithValue("@AST_Name", dto.AST_Name ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_HeadingID", dto.AST_HeadingID);
                    templateCommand.Parameters.AddWithValue("@AST_SubHeadingID", dto.AST_SubHeadingID);
                    templateCommand.Parameters.AddWithValue("@AST_ItemID", oper); // Link new item ID to template
                    templateCommand.Parameters.AddWithValue("@AST_SubItemID", dto.AST_SubItemID);
                    templateCommand.Parameters.AddWithValue("@AST_AccHeadId", dto.AST_AccHeadId);
                    templateCommand.Parameters.AddWithValue("@AST_DELFLG", dto.AST_DELFLG);
                    templateCommand.Parameters.AddWithValue("@AST_CRBY", dto.AST_CRBY);
                    templateCommand.Parameters.AddWithValue("@AST_STATUS", dto.AST_STATUS ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_UPDATEDBY", dto.AST_UPDATEDBY);
                    templateCommand.Parameters.AddWithValue("@AST_IPAddress", dto.AST_IPAddress ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_CompId", dto.AST_CompId);
                    templateCommand.Parameters.AddWithValue("@AST_YEARId", dto.AST_YEARId);
                    templateCommand.Parameters.AddWithValue("@AST_Schedule_type", dto.AST_Schedule_type);
                    templateCommand.Parameters.AddWithValue("@AST_Companytype", dto.AST_Companytype);
                    templateCommand.Parameters.AddWithValue("@AST_Company_limit", dto.AST_Company_limit);

                    var updateOrSaveParamTemplate = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParamTemplate = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    templateCommand.Parameters.Add(updateOrSaveParamTemplate);
                    templateCommand.Parameters.Add(operParamTemplate);

                    await templateCommand.ExecuteNonQueryAsync();

                    // You can capture templateSaveStatus/templateOper if needed
                }

                transaction.Commit();
                return new int[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //SaveScheduleSubItem
        public async Task<int[]> SaveScheduleSubItemAndTemplateAsync(int CompId, SaveScheduleSubItemDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int iPKId = dto.ASSI_ID;
                int updateOrSave, oper;

                // --- Save Schedule SubItem ---
                using (var subItemCommand = new SqlCommand("spACC_ScheduleSubItems", connection, transaction))
                {
                    subItemCommand.CommandType = CommandType.StoredProcedure;

                    subItemCommand.Parameters.AddWithValue("@ASSI_ID", dto.ASSI_ID);
                    subItemCommand.Parameters.AddWithValue("@ASSI_Name", dto.ASSI_Name ?? string.Empty);
                    subItemCommand.Parameters.AddWithValue("@ASSI_HeadingID", dto.ASSI_HeadingID);
                    subItemCommand.Parameters.AddWithValue("@ASSI_subHeadingID", dto.ASSI_SubHeadingID);
                    subItemCommand.Parameters.AddWithValue("@ASSI_ItemsID", dto.ASSI_ItemsID);
                    subItemCommand.Parameters.AddWithValue("@ASSI_DELFLG", dto.ASSI_DELFLG ?? string.Empty);
                    subItemCommand.Parameters.AddWithValue("@ASSI_CRBY", dto.ASSI_CRBY);
                    subItemCommand.Parameters.AddWithValue("@ASSI_STATUS", dto.ASSI_STATUS ?? string.Empty);
                    subItemCommand.Parameters.AddWithValue("@ASSI_UPDATEDBY", dto.ASSI_UPDATEDBY);
                    subItemCommand.Parameters.AddWithValue("@ASSI_IPAddress", dto.ASSI_IPAddress ?? string.Empty);
                    subItemCommand.Parameters.AddWithValue("@ASSI_CompId", dto.ASSI_CompId);
                    subItemCommand.Parameters.AddWithValue("@ASSI_YEARId", dto.ASSI_YEARId);
                    subItemCommand.Parameters.AddWithValue("@ASSi_scheduletype", dto.ASSI_ScheduleType);
                    subItemCommand.Parameters.AddWithValue("@ASSi_Orgtype", dto.ASSI_OrgType);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    subItemCommand.Parameters.Add(updateOrSaveParam);
                    subItemCommand.Parameters.Add(operParam);

                    await subItemCommand.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0);
                }

                // --- Only save template if it's a new sub-item ---
                bool isNewSubItem = dto.ASSI_ID == 0 || updateOrSave == 1;

                if (iPKId == 0)
                {
                    using var templateCommand = new SqlCommand("spACC_ScheduleTemplates", connection, transaction);
                    templateCommand.CommandType = CommandType.StoredProcedure;

                    templateCommand.Parameters.AddWithValue("@AST_ID", dto.AST_ID);
                    templateCommand.Parameters.AddWithValue("@AST_Name", dto.AST_Name ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_HeadingID", dto.AST_HeadingID);
                    templateCommand.Parameters.AddWithValue("@AST_subHeadingID", dto.AST_SubHeadingID);
                    templateCommand.Parameters.AddWithValue("@AST_ItemID", dto.AST_ItemID);
                    templateCommand.Parameters.AddWithValue("@AST_subItemID", oper); // Link to new subitem ID
                    templateCommand.Parameters.AddWithValue("@AST_AccHeadId", dto.AST_AccHeadId);
                    templateCommand.Parameters.AddWithValue("@AST_DELFLG", dto.AST_DELFLG ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_CRBY", dto.AST_CRBY);
                    templateCommand.Parameters.AddWithValue("@AST_STATUS", dto.AST_STATUS ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_UPDATEDBY", dto.AST_UPDATEDBY);
                    templateCommand.Parameters.AddWithValue("@AST_IPAddress", dto.AST_IPAddress ?? string.Empty);
                    templateCommand.Parameters.AddWithValue("@AST_CompId", dto.AST_CompId);
                    templateCommand.Parameters.AddWithValue("@AST_YEARId", dto.AST_YEARId);
                    templateCommand.Parameters.AddWithValue("@AST_Schedule_type", dto.AST_Schedule_type);
                    templateCommand.Parameters.AddWithValue("@AST_Companytype", dto.AST_Companytype);
                    templateCommand.Parameters.AddWithValue("@AST_Company_limit", dto.AST_Company_limit);

                    var updateOrSaveParamTemplate = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParamTemplate = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    templateCommand.Parameters.Add(updateOrSaveParamTemplate);
                    templateCommand.Parameters.Add(operParamTemplate);

                    await templateCommand.ExecuteNonQueryAsync();

                    // Optional: read templateSaveStatus/templateOper if needed
                }

                transaction.Commit();
                return new int[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //DeleteScheduleTemplate
        public async Task<bool> DeleteInformationAsync(int CompId, int ScheduleType, int CustId, int SelectedValue, int MainId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var parameters = new { Id = MainId, Type = ScheduleType, CustId = CustId, CompId = CompId };

                switch (SelectedValue)
                {
                    case 1:
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_HeadingID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleHeading WHERE ASH_ID = @Id AND Ash_Orgtype = @CustId AND Ash_scheduletype = @Type AND ASH_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubHeading WHERE ASSH_HeadingID = @Id AND Assh_Orgtype = @CustId AND Assh_scheduletype = @Type AND ASSH_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_HeadingID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_HeadingID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId", parameters, transaction);
                        break;

                    case 2:
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_SubHeadingID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubHeading WHERE ASSH_ID = @Id AND Assh_Orgtype = @CustId AND Assh_scheduletype = @Type AND ASSH_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_SubHeadingID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_SubHeadingID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId", parameters, transaction);
                        break;

                    case 3:
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_ItemID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_ID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_ItemsID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId", parameters, transaction);
                        break;

                    case 4:
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_SubItemID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId", parameters, transaction);
                        await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_ID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId", parameters, transaction);
                        break;
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //SaveOrUpdateScheduleHeadingAlias
        public async Task<int[]> SaveScheduleHeadingAliasAsync(ScheduleHeadingAliasDto dto)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection1")))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand("spAcc_GroupingAlias", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@AGA_ID", dto.AGA_ID);
                            command.Parameters.AddWithValue("@AGA_Description", dto.AGA_Description ?? string.Empty);
                            command.Parameters.AddWithValue("@AGA_GLID", dto.AGA_GLID);
                            command.Parameters.AddWithValue("@AGA_GLDESC", dto.AGA_GLDESC ?? string.Empty);
                            command.Parameters.AddWithValue("@AGA_GrpLevel", dto.AGA_GrpLevel);
                            command.Parameters.AddWithValue("@AGA_scheduletype", dto.AGA_scheduletype);
                            command.Parameters.AddWithValue("@AGA_Orgtype", dto.AGA_Orgtype);
                            command.Parameters.AddWithValue("@AGA_Compid", dto.AGA_Compid);
                            command.Parameters.AddWithValue("@AGA_Status", dto.AGA_Status ?? string.Empty);
                            command.Parameters.AddWithValue("@AGA_Createdby", dto.AGA_Createdby);
                            command.Parameters.AddWithValue("@AGA_IPaddress", dto.AGA_IPAddress ?? string.Empty);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };

                            command.Parameters.Add(updateOrSaveParam);
                            command.Parameters.Add(operParam);

                            await command.ExecuteNonQueryAsync();

                            int updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                            int oper = (int)(operParam.Value ?? 0);

                            transaction.Commit();
                            return new int[] { updateOrSave, oper };
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        //GetScheduleTemplateCount
        public async Task<IEnumerable<ScheduleTemplateCountDto>> GetScheduleFormatItemsAsync(
        int CustId, int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

           
          var query = @"
         SELECT COUNT(*) AS TemplateCount
         FROM ACC_ScheduleTemplates
         WHERE AST_Companytype = @CustId AND AST_CompId = @CompId";
;

            await connection.OpenAsync();

            var result = await connection.QueryAsync<ScheduleTemplateCountDto>(query, new{CustId, CompId});

            return result;
        }

        //SaveScheduleTemplate
        public async Task<List<int>> SaveScheduleTemplateAsync(int CompId, List<ScheduleTemplate> dtos)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.AST_Schedule_type);
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
                            cmd.Parameters.AddWithValue("@Ash_scheduletype", dto.AST_Schedule_type);
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
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.AST_Schedule_type);
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
                            cmd.Parameters.AddWithValue("@ASSH_scheduletype", dto.AST_Schedule_type);
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
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.AST_Schedule_type);
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
                            cmd.Parameters.AddWithValue("@ASI_scheduletype", dto.AST_Schedule_type);
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
                        checkCmd.Parameters.AddWithValue("@ScheduleType", dto.AST_Schedule_type);
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
                            cmd.Parameters.AddWithValue("@ASSI_scheduletype", dto.AST_Schedule_type);
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
    }
}
