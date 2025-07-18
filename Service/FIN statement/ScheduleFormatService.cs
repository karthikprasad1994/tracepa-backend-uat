using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
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
SELECT 
    AST_ID,
    b.ASH_ID AS HeadingId,
    NULLIF(b.ASH_Name, LAG(b.ASH_Name) OVER (ORDER BY b.ASH_ID ASC)) AS HeadingName,
    c.ASSH_ID AS SubheadingID,
    NULLIF(c.ASSH_Name, LAG(c.ASSH_Name) OVER (ORDER BY c.ASSH_ID)) AS SubheadingName,
    d.ASI_ID AS ItemId,
    NULLIF(d.ASI_Name, LAG(d.ASI_Name) OVER (ORDER BY d.ASI_ID)) AS ItemName,
    e.ASSI_ID AS SubitemId,
    NULLIF(e.ASSI_Name, LAG(e.ASSI_Name) OVER (ORDER BY e.ASSI_ID)) AS SubitemName,
    a.AST_AccHeadId,
    CASE  
        WHEN a.AST_Schedule_type = 4 AND a.AST_AccHeadId = 1 THEN 'ASSETS'
        WHEN a.AST_Schedule_type = 4 AND a.AST_AccHeadId = 2 THEN 'CAPITAL AND LIABILITIES'
        WHEN a.AST_Schedule_type = 3 AND a.AST_AccHeadId = 1 THEN 'INCOME'
        WHEN a.AST_Schedule_type = 3 AND a.AST_AccHeadId = 2 THEN 'EXPENSES'
    END AS AccHeadName
FROM ACC_ScheduleTemplates a
LEFT JOIN ACC_ScheduleHeading b 
    ON b.ASH_ID = a.AST_HeadingId AND b.ASH_Orgtype = @CustId
LEFT JOIN ACC_ScheduleSubHeading c 
    ON c.ASSH_ID = a.AST_SubHeadingId AND c.ASSH_Orgtype = @CustId
LEFT JOIN ACC_ScheduleItems d 
    ON d.ASI_ID = a.AST_ItemId AND d.ASI_Orgtype = @CustId
LEFT JOIN ACC_ScheduleSubItems e 
    ON e.ASSI_ID = a.AST_SubItemId AND e.ASSI_Orgtype = @CustId
WHERE AST_CompId = @CompId
";

            // Dynamically build WHERE conditions based on input
            if (CustId != 0)
                baseQuery += " AND AST_Companytype = @CustId";

            if (ScheduleId != 0)
                baseQuery += " AND AST_Schedule_type = @ScheduleId";

            if (AccHead != 0)
                baseQuery += " AND AST_AccHeadId = @AccHead";

            baseQuery += " ORDER BY AST_ID, ASH_ID ASC";

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
                        new { Id = MainId, Type = ScheduleType, iCustId = CustId, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleHeading WHERE ASH_ID = @Id AND Ash_Orgtype = @CustId AND Ash_scheduletype = @Type AND ASH_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubHeading WHERE ASSH_HeadingID = @Id AND Assh_Orgtype = @CustId AND Assh_scheduletype = @Type AND ASSH_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_HeadingID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_HeadingID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);
                }
                else if (SelectedValue == 2)
                {
                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_SubHeadingID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId",
                        new { Id = MainId, Type = ScheduleType, iCustId = CustId, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubHeading WHERE ASSH_ID = @Id AND Assh_Orgtype = @CustId AND Assh_scheduletype = @Type AND ASSH_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_SubHeadingID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_SubHeadingID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);
                }
                else if (SelectedValue == 3)
                {
                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_ItemID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId",
                        new { Id = MainId, Type = ScheduleType, iCustId = CustId, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleItems WHERE ASI_ID = @Id AND Asi_Orgtype = @CustId AND Asi_scheduletype = @Type AND ASI_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_ItemsID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);
                }
                else if (SelectedValue == 4)
                {
                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleTemplates WHERE AST_SubItemID = @Id AND AST_Schedule_Type = @Type AND AST_Companytype = @CustId AND AST_CompId = @CompId",
                        new { Id = MainId, Type = ScheduleType, iCustId = CustId, iCompId = CompId }, transaction);

                    await connection.ExecuteAsync("DELETE FROM ACC_ScheduleSubItems WHERE ASSI_ID = @Id AND AsSi_Orgtype = @CustId AND AsSi_scheduletype = @Type AND ASSI_CompId = @CompId",
                        new { Id = MainId, iCustId = CustId, Type = ScheduleType, iCompId = CompId }, transaction);
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
                    templateCommand.Parameters.AddWithValue("@AST_CompId", CompId);
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
                    templateCommand.Parameters.AddWithValue("@AST_CompId", CompId);
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
                    subItemCommand.Parameters.AddWithValue("@ASSI_CompId", CompId);
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
                    templateCommand.Parameters.AddWithValue("@AST_CompId", CompId);
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
    }
}
