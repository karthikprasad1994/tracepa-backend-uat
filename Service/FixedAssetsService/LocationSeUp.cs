using TracePca.Data.CustomerRegistration;
using TracePca.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TracePca.Interface;
using TracePca.Interface.AssetMaserInterface;
using Microsoft.Data.SqlClient;
using TracePca.Dto;
using Dapper;

namespace TracePca.Service.AssetService
{
    public class LocationSeUp : LocationSetUpInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly CustomerRegistrationContext _customerRegistrationContext;
        private readonly IConfiguration _configuration;

        public LocationSeUp(Trdmyus1Context dbContext, CustomerRegistrationContext customerDbContext, IConfiguration configuration)
        {
            _dbcontext = dbContext;
            _customerRegistrationContext = customerDbContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<DropDownDto>> GetLocationsAsync(int companyId, int customerId, int parentId = 0)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = parentId == 0
                ? "SELECT LS_ID AS Id, LS_Description AS Name, LS_LevelCode AS LevelCode, LS_DescCode AS Code  FROM Acc_AssetLocationSetup WHERE LS_LevelCode = 0 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId"
                : "SELECT LS_ID AS Id, LS_Description AS Name, LS_LevelCode AS LevelCode FROM Acc_AssetLocationSetup WHERE LS_ParentID IN (@ParentId) AND LS_LevelCode = 0 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId";

            return await connection.QueryAsync<DropDownDto>(query, new { CompanyId = companyId, CustomerId = customerId, ParentId = parentId });
        }


        public async Task<IEnumerable<DropDownDto>> GetInsertedLocationsAsync(int companyId, int customerId, int parentId = 0)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // SQL query to fetch the recently inserted locations ordered by LS_ID descending
            string query = @"
        SELECT  LS_ID AS Id, LS_Description AS Name, 
        FROM Acc_AssetLocationSetup 
        WHERE LS_CompID = @CompanyId 
        AND LS_CustId = @CustomerId 
        ORDER BY LS_ID DESC"; // Sorting by LS_ID to get the latest entries

            // Execute the query and return the results
            var result = await connection.QueryAsync<DropDownDto>(query, new
            {
                CompanyId = companyId,
                CustomerId = customerId,
                
            });

            return result;
        }



        public async Task<IEnumerable<DropDownDto>> GetDivisionsAsync(int companyId, int customerId, int parentId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = parentId == 0
                ? "SELECT LS_ID AS Id, LS_Description AS Name, LS_LevelCode AS LevelCode, LS_DescCode AS Code  FROM Acc_AssetLocationSetup WHERE LS_LevelCode = 1 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId"
                : "SELECT LS_ID AS Id, LS_Description AS Name,  LS_LevelCode AS LevelCode, LS_Code AS Code  FROM Acc_AssetLocationSetup WHERE LS_ParentID IN (@ParentId) AND LS_LevelCode = 1 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId";
            return await connection.QueryAsync<DropDownDto>(query, new { CompanyId = companyId, CustomerId = customerId, ParentId = parentId });
        }

        public async Task<IEnumerable<DropDownDto>> GetDepartmentsAsync(int companyId, int customerId, int parentId = 0)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = parentId == 0
                ? "SELECT LS_ID AS Id, LS_Description AS Name,  LS_LevelCode AS LevelCode, LS_DescCode AS Code  FROM Acc_AssetLocationSetup WHERE LS_LevelCode = 2 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId"
                : "SELECT LS_ID AS Id, LS_Description AS Name,  LS_LevelCode AS LevelCode  FROM Acc_AssetLocationSetup WHERE LS_ParentID IN (@ParentId) AND LS_LevelCode = 2 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId";

            var departments = await connection.QueryAsync<DropDownDto>(query, new { CompanyId = companyId, CustomerId = customerId, ParentId = parentId });
            return departments;
        }

        public async Task<IEnumerable<DropDownDto>> GetHeadersAsync(int companyId, int customerId, int parentId = 0)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = parentId == 0
                ? "SELECT AM_ID AS Id, AM_ID AS AMID, AM_Description AS Name, AM_LevelCode as AmLevelCode, AM_Code as Code  FROM Acc_AssetMaster WHERE AM_LevelCode = 0 AND AM_CompID = @CompanyId AND AM_CustId = @CustomerId"
                : "SELECT AM_ID AS Id,  AM_ID AS AMID, AM_Description AS Name,  AM_LevelCode as AmLevelCode, AM_Code as Code FROM Acc_AssetMaster WHERE AM_ParentID = @ParentId AND AM_LevelCode = 0 AND AM_CompID = @CompanyId AND AM_CustId = @CustomerId";

            return await connection.QueryAsync<DropDownDto>(query, new { CompanyId = companyId, CustomerId = customerId, ParentId = parentId  });
        }


        public async Task<IEnumerable<DropDownDto>> GetSubHeadersAsync(int companyId, int customerId, int parentId = 0)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = parentId == 0
                ? "SELECT AM_ID AS Id, AM_Description AS Name,  AM_LevelCode as AmLevelCode, AM_Code as Code  FROM Acc_AssetMaster WHERE AM_LevelCode = 1 AND AM_CompID = @CompanyId AND AM_CustId = @CustomerId"
                : "SELECT AM_ID AS Id, AM_Description AS Name,  AM_LevelCode as AmLevelCode, AM_Code as Code FROM Acc_AssetMaster WHERE AM_ParentID = @ParentId AND AM_LevelCode = 1 AND AM_CompID = @CompanyId AND AM_CustId = @CustomerId";

            return await connection.QueryAsync<DropDownDto>(query, new { CompanyId = companyId, CustomerId = customerId, ParentId = parentId });
        }


        public async Task<IEnumerable<DropDownDto>> GetBayiAsync(int companyId, int customerId, string parentId = "0")
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = parentId == "0"
                ? "SELECT LS_ID AS Id, LS_Description AS Name,  LS_LevelCode AS LevelCode, LS_DescCode AS Code  FROM Acc_AssetLocationSetup WHERE LS_LevelCode = 3 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId"
                : "SELECT LS_ID AS Id, LS_Description AS Name,  LS_LevelCode AS LevelCode  FROM Acc_AssetLocationSetup WHERE LS_ParentID IN (@ParentId) AND LS_LevelCode = 3 AND LS_CompID = @CompanyId AND LS_CustId = @CustomerId";

            return await connection.QueryAsync<DropDownDto>(query, new { CompanyId = companyId, CustomerId = customerId, ParentId = parentId });
        }

        public async Task<IEnumerable<DropDownDto>> GetAssetsAsync(int companyId, int customerId, string parentId = "0")
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = parentId == "0"
                ? "SELECT AM_ID AS Id, AM_Description AS Name,  AM_LevelCode as AmLevelCode, AM_Code as Code FROM Acc_AssetMaster WHERE AM_LevelCode = 2 AND AM_CompID = @CompanyId AND AM_CustId = @CustomerId"
                : "SELECT AM_ID AS Id, AM_Description AS Name,  AM_LevelCode as AmLevelCode, AM_Code as Code FROM Acc_AssetMaster WHERE AM_ParentID IN (@ParentId) AND AM_LevelCode = 2 AND AM_CompID = @CompanyId AND AM_CustId = @CustomerId";

            return await connection.QueryAsync<DropDownDto>(query, new { CompanyId = companyId, CustomerId = customerId, ParentId = parentId });
        }

        public async Task<int> SaveLocationAsync(AddLocationDto locationDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (locationDto.Id.HasValue && locationDto.Id.Value > 0)
                {
                    string updateQuery = @"
                UPDATE Acc_AssetLocationSetup
                SET LS_Description = @Name, 
                    LS_DescCode = @Code, 
                    LS_UpdatedBy = @CreatedBy, 
                    LS_UpdatedOn = GETDATE()
                WHERE LS_ID = @Id AND LS_CustId = @CustomerId";

                    await connection.ExecuteAsync(updateQuery, locationDto, transaction);
                    await transaction.CommitAsync();
                    return locationDto.Id.Value;
                }
                else
                {
                    // ✅ Global max LS_ID (across all rows)
                    string maxIdQuery = "SELECT ISNULL(MAX(LS_ID), 0) FROM Acc_AssetLocationSetup WITH (UPDLOCK, HOLDLOCK)";
                    int maxId = await connection.ExecuteScalarAsync<int>(maxIdQuery, transaction: transaction);
                    int newId = maxId + 1;

                    string insertQuery = @"
                INSERT INTO Acc_AssetLocationSetup (
                    LS_ID, LS_Description, LS_DescCode, LS_Code, LS_LevelCode, LS_ParentID, 
                    LS_CreatedBy, LS_CreatedOn, LS_UpdatedBy, LS_UpdatedOn, LS_ApprovedBy, LS_ApprovedOn, 
                    LS_DelFlag, LS_Status, LS_YearID, LS_CompID, LS_Opeartion, LS_IPAddress, LS_CustId
                )
                VALUES (
                    @NewId, @Name, @Code, 0, 0, @ParentID, 
                    @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 
                    'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
                )";

                    await connection.ExecuteAsync(insertQuery, new
                    {
                        NewId = newId,
                        locationDto.Name,
                        locationDto.Code,
                        locationDto.CreatedBy,
                        locationDto.IPAddress,
                        locationDto.YearID,
                        locationDto.CompanyId,
                        locationDto.CustomerId,
                        locationDto.ParentID
                    }, transaction);

                    await transaction.CommitAsync();
                    return newId;
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }





        public async Task<int> SaveDivisionAsync(AddDivisionDto divisionDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // Check for duplicate Division (same name and customer)
            var existsQuery = @"
        SELECT COUNT(1) 
        FROM Acc_AssetLocationSetup 
        WHERE LS_Description = @Name 
          AND LS_CustId = @CustomerId 
          AND (@Id IS NULL OR LS_ID <> @Id)";

            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, divisionDto);

            if (exists > 0)
            {
                return -1; // Duplicate found
            }

            // ✅ Update case
            if (divisionDto.Id.HasValue && divisionDto.Id.Value > 0)
            {
                string checkExistingRecordQuery = @"
            SELECT COUNT(1)
            FROM Acc_AssetLocationSetup
            WHERE LS_ID = @Id AND LS_CustId = @CustomerId";

                var recordExists = await connection.ExecuteScalarAsync<int>(checkExistingRecordQuery, new
                {
                    Id = divisionDto.Id,
                    CustomerId = divisionDto.CustomerId
                });

                if (recordExists > 0)
                {
                    // 🔁 Update existing record
                    string updateQuery = @"
                UPDATE Acc_AssetLocationSetup
                SET LS_Description = @Name, 
                    LS_DescCode = @Code, 
                    LS_UpdatedBy = @CreatedBy, 
                    LS_UpdatedOn = GETDATE()
                WHERE LS_ID = @Id AND LS_CustId = @CustomerId";

                    await connection.ExecuteAsync(updateQuery, new
                    {
                        Id = divisionDto.Id,
                        Name = divisionDto.Name,
                        Code = divisionDto.Code,
                        CreatedBy = divisionDto.CreatedBy,
                        CustomerId = divisionDto.CustomerId
                    });

                    return divisionDto.Id.Value;
                }
            }

            // ➕ Insert new record
            string maxIdQuery = "SELECT ISNULL(MAX(LS_ID), 0) FROM Acc_AssetLocationSetup";
            int newId = (await connection.ExecuteScalarAsync<int>(maxIdQuery)) + 1;

            string insertQuery = @"
        INSERT INTO Acc_AssetLocationSetup (
            LS_ID, LS_Description, LS_DescCode, LS_Code, LS_LevelCode, LS_ParentID, 
            LS_CreatedBy, LS_CreatedOn, LS_UpdatedBy, LS_UpdatedOn, LS_ApprovedBy, LS_ApprovedOn, 
            LS_DelFlag, LS_Status, LS_YearID, LS_CompID, LS_Opeartion, LS_IPAddress, LS_CustId
        )
        VALUES (
            @NewId, @Name, @Code, 0, 1, 0,
            @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 
            'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
        )";

            await connection.ExecuteAsync(insertQuery, new
            {
                NewId = newId,
                Name = divisionDto.Name,
                Code = divisionDto.Code,
                CreatedBy = divisionDto.CreatedBy,
                IPAddress = divisionDto.IPAddress,
                YearID = divisionDto.YearID,
                CompanyId = divisionDto.CompanyId,
                CustomerId = divisionDto.CustomerId
            });

            return newId;
        }




        public async Task<int> SaveDepartmentAsync(AddDepartmentDto departmentDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // ✅ Check for duplicate Department within the same Customer & Division
            var existsQuery = @"
        SELECT COUNT(1) 
        FROM Acc_AssetLocationSetup 
        WHERE LS_Description = @Name 
        AND LS_CustId = @CustomerId 
        AND LS_ParentID = @DivisionId";

            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, departmentDto);

            if (exists > 0)
            {
                return -1; // ❌ Duplicate Department Found
            }

            // ✅ Update Case
            if (departmentDto.Id.HasValue && departmentDto.Id.Value > 0)
            {
                string updateQuery = @"
            UPDATE Acc_AssetLocationSetup
            SET LS_Description = @Name, 
                LS_DescCode = @Code, 
                LS_UpdatedBy = @CreatedBy, 
                LS_UpdatedOn = GETDATE()
            WHERE LS_ID = @Id 
              AND LS_CustId = @CustomerId";  // Ensure it's for the correct customer

                await connection.ExecuteAsync(updateQuery, departmentDto);
                return departmentDto.Id.Value;
            }

            // ➕ Insert Case: Get global max LS_ID (not just per customer)
            string maxIdQuery = "SELECT ISNULL(MAX(LS_ID), 0) FROM Acc_AssetLocationSetup";
            int newDepartmentId = (await connection.ExecuteScalarAsync<int>(maxIdQuery)) + 1;

            string insertQuery = @"
        INSERT INTO Acc_AssetLocationSetup (
            LS_ID, LS_Description, LS_DescCode, LS_Code, LS_LevelCode, LS_ParentID, 
            LS_CreatedBy, LS_CreatedOn, LS_UpdatedBy, LS_UpdatedOn, LS_ApprovedBy, LS_ApprovedOn, 
            LS_DelFlag, LS_Status, LS_YearID, LS_CompID, LS_Opeartion, LS_IPAddress, LS_CustId
        )
        VALUES (
            @NewId, @Name, @Code, 0, 2, @DivisionId,  -- Level 2 for Department
            @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 
            'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
        )";

            await connection.ExecuteAsync(insertQuery, new
            {
                NewId = newDepartmentId,
                departmentDto.Name,
                departmentDto.Code,
                departmentDto.CreatedBy,
                departmentDto.YearID,
                departmentDto.CompanyId,
                departmentDto.IPAddress,
                departmentDto.CustomerId,
                departmentDto.DivisionId
            });

            return newDepartmentId;
        }


        public async Task<int> SaveBayAsync(AddBayDto bayDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // ✅ Check for duplicate Bay within the same Customer and Department
            var existsQuery = @"
        SELECT COUNT(1) 
        FROM Acc_AssetLocationSetup 
        WHERE LS_Description = @Name 
        AND LS_CustId = @CustomerId 
        AND LS_ParentID = @DepartmentId";

            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, bayDto);
            if (exists > 0)
            {
                return -1; // ❌ Duplicate found
            }

            // ✅ Update case
            if (bayDto.Id.HasValue && bayDto.Id.Value > 0)
            {
                string updateQuery = @"
            UPDATE Acc_AssetLocationSetup
            SET LS_Description = @Name, 
                LS_DescCode = @Code, 
                LS_UpdatedBy = @CreatedBy, 
                LS_UpdatedOn = GETDATE()
            WHERE LS_ID = @Id 
              AND LS_CustId = @CustomerId";

                await connection.ExecuteAsync(updateQuery, bayDto);
                return bayDto.Id.Value;
            }

            // ➕ Insert case: Get the global max LS_ID across all records
            string getMaxIdQuery = "SELECT ISNULL(MAX(LS_ID), 0) FROM Acc_AssetLocationSetup";
            int newId = (await connection.ExecuteScalarAsync<int>(getMaxIdQuery)) + 1;

            string insertQuery = @"
        INSERT INTO Acc_AssetLocationSetup (
            LS_ID, LS_Description, LS_DescCode, LS_Code, LS_LevelCode, LS_ParentID, 
            LS_CreatedBy, LS_CreatedOn, LS_UpdatedBy, LS_UpdatedOn, LS_ApprovedBy, LS_ApprovedOn, 
            LS_DelFlag, LS_Status, LS_YearID, LS_CompID, LS_Opeartion, LS_IPAddress, LS_CustId
        )
        VALUES (
            @NewId, @Name, @Code, 0, 3, @DepartmentId,  -- ✅ Level 3 for Bay
            @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 
            'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
        )";

            await connection.ExecuteAsync(insertQuery, new
            {
                NewId = newId,
                bayDto.Name,
                bayDto.Code,
                bayDto.DepartmentId,
                bayDto.CreatedBy,
                bayDto.YearID,
                bayDto.CompanyId,
                bayDto.IPAddress,
                bayDto.CustomerId
            });

            return newId;
        }



        public async Task<int> SaveHeadingAsync(AddHeadingDto headingDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // ✅ Check for duplicate Heading within the same CustomerId
            var existsQuery = @"
    SELECT COUNT(1) 
    FROM Acc_AssetMaster 
    WHERE AM_Description = @Name 
    AND AM_CustId = @CustomerId";

            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, headingDto);
            if (exists > 0)
            {
                return -1; // ❌ Duplicate heading
            }

            if (headingDto.Id.HasValue && headingDto.Id.Value > 0) // ✅ Update Case
            {
                string updateQuery = @"
        UPDATE Acc_AssetMaster
        SET AM_Description = @Name, 
            AM_WDVITAct = @WDVITAct, 
            AM_ITRate = @ITRate, 
            AM_ResidualValue = @ResidualValue,
            AM_UpdatedBy = @CreatedBy, 
            AM_UpdatedOn = GETDATE()
        WHERE AM_ID = @Id 
        AND AM_CustId = @CustomerId";

                await connection.ExecuteAsync(updateQuery, headingDto);
                return headingDto.Id.Value;
            }
            else // ✅ Insert Case
            {
                // 🔁 Use globally unique AM_ID
                string getMaxIdQuery = @"SELECT ISNULL(MAX(AM_ID), 0) + 1 FROM Acc_AssetMaster";
                int newId = await connection.ExecuteScalarAsync<int>(getMaxIdQuery);

                string insertQuery = @"
        INSERT INTO Acc_AssetMaster (
            AM_ID, AM_Description, AM_Code, AM_LevelCode, AM_ParentID,
            AM_WDVITAct, AM_ITRate, AM_ResidualValue,
            AM_CreatedBy, AM_CreatedOn, AM_UpdatedBy, AM_UpdatedOn, AM_ApprovedBy, AM_ApprovedOn, 
            AM_DelFlag, AM_Status, AM_YearID, AM_CompID, AM_Opeartion, AM_IPAddress, AM_CustId
        )
        VALUES (
            @NewId, @Name, 0, 0, 0,
            @WDVITAct, @ITRate, @ResidualValue,
            @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 
            'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
        )";

                await connection.ExecuteAsync(insertQuery, new
                {
                    NewId = newId,
                    headingDto.Name,
                    headingDto.WDVITAct,
                    headingDto.ITRate,
                    headingDto.ResidualValue,
                    headingDto.CreatedBy,
                    headingDto.YearID,
                    headingDto.CompanyId,
                    headingDto.IPAddress,
                    headingDto.CustomerId
                });

                return newId;
            }
        }



        public async Task<int> SaveSubHeadingAsync(AddSubHeadingDto subHeadingDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // ✅ Always treat SubHeadings as top-level (no parent)
            subHeadingDto.ParentId = 0;

            // ✅ Check for duplicate SubHeading under same customer with ParentId = 0
            var existsQuery = @"
    SELECT COUNT(1) 
    FROM Acc_AssetMaster 
    WHERE AM_Description = @Name 
    AND AM_CustId = @CustomerId 
    AND AM_ParentID = @ParentId";

            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new
            {
                subHeadingDto.Name,
                subHeadingDto.CustomerId,
                ParentId = subHeadingDto.ParentId
            });

            if (exists > 0)
            {
                return -1; // ❌ Duplicate found
            }

            if (subHeadingDto.Id.HasValue && subHeadingDto.Id.Value > 0) // ✅ Update
            {
                string updateQuery = @"
        UPDATE Acc_AssetMaster
        SET AM_Description = @Name, 
            AM_WDVITAct = @WDVITAct, 
            AM_ITRate = @ITRate, 
            AM_ResidualValue = @ResidualValue,
            AM_UpdatedBy = @CreatedBy, 
            AM_UpdatedOn = GETDATE()
        WHERE AM_ID = @Id";

                await connection.ExecuteAsync(updateQuery, subHeadingDto);
                return subHeadingDto.Id.Value;
            }
            else // ✅ Insert
            {
                // ✅ Get globally unique AM_ID
                string getMaxIdQuery = @"SELECT ISNULL(MAX(AM_ID), 0) + 1 FROM Acc_AssetMaster";
                int newId = await connection.ExecuteScalarAsync<int>(getMaxIdQuery);

                string insertQuery = @"
        INSERT INTO Acc_AssetMaster (
            AM_ID, AM_Description, AM_Code, AM_LevelCode, AM_ParentID,
            AM_WDVITAct, AM_ITRate, AM_ResidualValue,
            AM_CreatedBy, AM_CreatedOn, AM_UpdatedBy, AM_UpdatedOn, AM_ApprovedBy, AM_ApprovedOn, 
            AM_DelFlag, AM_Status, AM_YearID, AM_CompID, AM_Opeartion, AM_IPAddress, AM_CustId
        )
        VALUES (
            @NewId, @Name, 0, 1, @ParentId,
            @WDVITAct, @ITRate, @ResidualValue,
            @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 
            'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
        )";

                await connection.ExecuteAsync(insertQuery, new
                {
                    NewId = newId,
                    subHeadingDto.Name,
                    subHeadingDto.WDVITAct,
                    subHeadingDto.ITRate,
                    subHeadingDto.ResidualValue,
                    subHeadingDto.CreatedBy,
                    subHeadingDto.YearID,
                    subHeadingDto.CompanyId,
                    subHeadingDto.IPAddress,
                    subHeadingDto.CustomerId,
                    ParentId = subHeadingDto.ParentId
                });

                return newId;
            }
        }



        public async Task<int> SaveAssetAsync(AddAssetClassDto assetDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            assetDto.ParentId = 0; // Asset class has no parent

            // ✅ Exclude current record in duplicate check if updating
            var existsQuery = @"
SELECT COUNT(1) 
FROM Acc_AssetMaster 
WHERE AM_Description = @Name 
AND AM_CustId = @CustomerId 
AND AM_ParentID = @ParentId
" + (assetDto.Id.HasValue && assetDto.Id.Value > 0 ? "AND AM_ID != @Id" : "");

            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new
            {
                assetDto.Name,
                assetDto.CustomerId,
                assetDto.Id,
                ParentId = assetDto.ParentId
            });

            if (exists > 0)
            {
                return -1; // ❌ Duplicate found
            }

            if (assetDto.Id.HasValue && assetDto.Id.Value > 0) // ✅ Update
            {
                string updateQuery = @"
UPDATE Acc_AssetMaster
SET AM_Description = @Name, 
    AM_WDVITAct = @WDVITAct, 
    AM_ITRate = @ITRate, 
    AM_ResidualValue = @ResidualValue,
    AM_OriginalCost = @OriginalCost,
    AM_UpdatedBy = @CreatedBy, 
    AM_UpdatedOn = GETDATE()
WHERE AM_ID = @Id";

                await connection.ExecuteAsync(updateQuery, assetDto);
                return assetDto.Id.Value;
            }
            else // ✅ Insert
            {
                string getMaxIdQuery = @"SELECT ISNULL(MAX(AM_ID), 0) + 1 FROM Acc_AssetMaster";
                int newId = await connection.ExecuteScalarAsync<int>(getMaxIdQuery);

                string insertQuery = @"
INSERT INTO Acc_AssetMaster (
    AM_ID, AM_Description, AM_Code, AM_LevelCode, AM_ParentID,
    AM_WDVITAct, AM_ITRate, AM_ResidualValue, AM_OriginalCost,
    AM_CreatedBy, AM_CreatedOn, AM_UpdatedBy, AM_UpdatedOn, AM_ApprovedBy, AM_ApprovedOn, 
    AM_DelFlag, AM_Status, AM_YearID, AM_CompID, AM_Opeartion, AM_IPAddress, AM_CustId
)
VALUES (
    @NewId, @Name, 0, 2, @ParentId,
    @WDVITAct, @ITRate, @ResidualValue, @OriginalCost,
    @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 
    'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
)";

                await connection.ExecuteAsync(insertQuery, new
                {
                    NewId = newId,
                    assetDto.Name,
                    assetDto.WDVITAct,
                    assetDto.ITRate,
                    assetDto.ResidualValue,
                    assetDto.OriginalCost,
                    assetDto.CreatedBy,
                    assetDto.YearID,
                    assetDto.CompanyId,
                    assetDto.IPAddress,
                    assetDto.CustomerId,
                    ParentId = assetDto.ParentId
                });

                return newId;
            }
        }



        public async Task<AssetMasterDetailsDto> GetItemDetailsByAmIdAsync(int amId, int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"SELECT AM_ID, AM_WDVITAct, AM_ITRate, AM_ResidualValue, AM_OriginalCost 
                  FROM Acc_AssetMaster 
                  WHERE AM_ID = @AmId AND AM_CompID = @CompId AND AM_CustId = @CustId";

            return await connection.QueryFirstOrDefaultAsync<AssetMasterDetailsDto>(query, new
            {
                AmId = amId,
                CompId = compId,
                CustId = custId
            });
        }






    }

}
