using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Helpers;
using TracePca.Interface;
using TracePca.Models;
using TracePca.Models.UserModels;

namespace TracePca.Service
{
    public class Asset : AssetInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;


        private readonly List<CustomerDto> _customers = new()
    {


        new CustomerDto { CustId = 1, CustName = "John Doe" },
        new CustomerDto { CustId   = 2, CustName = "Alice Smith" },
        new CustomerDto { CustId = 3, CustName = "Bob Johnson" },
    };


        public Asset(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }




        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = "SELECT CUST_ID AS CustId, CUST_NAME AS CustName, CUST_CompID AS CompanyId FROM SAD_CUSTOMER_MASTER WHERE CUST_STATUS <> 'D' AND CUST_CompID = @CompanyId";

            return await connection.QueryAsync<CustomerDto>(query, new { CompanyId = companyId });
        }

        public IEnumerable<CustomerDto> GetCustomers()
        {
            return _customers;
        }

        public async Task<IEnumerable<YearDto>> LoadYearsAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT YMS_YEARID,
               RIGHT(CAST(YMS_ID AS VARCHAR(4)), 2) + '-' + RIGHT(CAST(YMS_ID + 1 AS VARCHAR(4)), 2) AS YMS_ID
        FROM YEAR_MASTER
        WHERE YMS_FROMDATE < DATEADD(YEAR, 1, GETDATE())
          AND YMS_CompId = @CompanyId
        ORDER BY YMS_ID DESC";

            return await connection.QueryAsync<YearDto>(query, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<FixedAssetTypeDto>> LoadFixedAssetTypesAsync(int companyId, int customerId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT AM_ID, AM_Description 
        FROM Acc_AssetMaster 
        WHERE 
           AM_DelFlag = 'X' 
          AND AM_CompID = @CompanyId 
          AND AM_CustId = @CustomerId";

            return await connection.QueryAsync<FixedAssetTypeDto>(query, new
            {
                CompanyId = companyId,
                CustomerId = customerId
            });
        }

        public async Task<string> GenerateTransactionNoAsync(int compId, int yearId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            // Count the number of records matching the filters
            int count = await connection.ExecuteScalarAsync<int>(
                @"SELECT ISNULL(COUNT(AFAM_ID) + 1, 1)
          FROM Acc_FixedAssetMaster
          WHERE AFAM_CompID = @CompID AND AFAM_YearID = @YearID AND AFAM_CustID = @CustID",
                new { CompID = compId, YearID = yearId, CustID = custId });

            // Generate the transaction number in format: FAR000-1
            string transactionNo = $"FAR000-{count}";

            return transactionNo;
        }


        public async Task<string> GetNextEmployeeCodeAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            // Get the next ID based on existing max
            int nextId = await connection.ExecuteScalarAsync<int>(
                "SELECT ISNULL(MAX(AFAM_ID), 0) + 1 FROM Acc_FixedAssetMaster");

            // Generate code in format FAR000_1, FAR000_2, ...
            string nextAssetCode = $"FAR000_{nextId}";

            return nextAssetCode;
        }


        public async Task<IEnumerable<UnitOfMeasureDto>> LoadUnitsOfMeasureAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            string query = @"
        SELECT cmm_ID, cmm_Desc 
        FROM Content_Management_Master 
        WHERE CMM_CompID = @CompanyId 
          AND cmm_Category = 'WS' 
        ORDER BY cmm_ID";

            var result = await connection.QueryAsync<UnitOfMeasureDto>(query, new { CompanyId = companyId });
            return result;
        }


        public async Task<IEnumerable<CurrencyDto>> LoadCurrencyAsync(string sNameSpace, int iCompID)
        {
            var query = @"
    SELECT CUR_ID, CUR_CODE + '-' + CUR_CountryName AS Curcode
    FROM SAD_Currency_Master
    WHERE CUR_Status = 'A'
    ORDER BY CUR_CountryName ASC";

            using (var connection = new SqlConnection(_configuration.GetConnectionString(sNameSpace)))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<CurrencyDto>(query);
            }
        }






        public async Task<IEnumerable<SupplierDto>> LoadExistingSuppliersAsync(int companyId, int supplierId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT 
            SUP_ID,
            SUP_Name,
            SUP_ContactPerson AS ContactPerson,
            SUP_Address AS Address,
            SUP_PhoneNo AS PhoneNo,
            SUP_Email AS EmailId,
            SUP_Website AS Website,
            SUP_Fax AS Fax
        FROM SAD_SUPPLIER_MASTER
        WHERE SUP_CompID = @CompanyId
        AND (@SupplierId = 0 OR SUP_ID = @SupplierId)
        ORDER BY SUP_ID";

            return await connection.QueryAsync<SupplierDto>(query, new
            {
                CompanyId = companyId,
                SupplierId = supplierId
            });
        }


        public async Task<int> SaveSupplierAsync(AddSupplierDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                if (dto.SupId.HasValue && dto.SupId.Value > 0)
                {
                    // Update
                    string updateQuery = @"
                UPDATE SAD_SUPPLIER_MASTER
                SET 
                    SUP_Name = @SupName,
                    SUP_ContactPerson = @SupContactPerson,
                    SUP_Address = @SupAddress,
                    SUP_PhoneNo = @SupPhoneNo,
                    SUP_Fax = @SupFax,
                    SUP_Email = @SupEmail,
                    SUP_Website = @SupWebsite,
                    SUP_STATUS = @SupStatus,
                    SUP_CompID = @SupCompId
                    
                    
                WHERE SUP_ID = @SupId";

                    await connection.ExecuteAsync(updateQuery, dto, transaction);
                    await transaction.CommitAsync();
                    return dto.SupId.Value;
                }
                else
                {
                    // Insert
                    string maxIdQuery = "SELECT ISNULL(MAX(SUP_ID), 0) FROM SAD_SUPPLIER_MASTER WITH (UPDLOCK, HOLDLOCK)";
                    int maxId = await connection.ExecuteScalarAsync<int>(maxIdQuery, transaction: transaction);
                    int newId = maxId + 1;

                    string supplierCode = $"sup{newId.ToString().PadLeft(3, '0')}";

                    string insertQuery = @"
                INSERT INTO SAD_SUPPLIER_MASTER (
                    SUP_ID, SUP_Name, SUP_Code, SUP_ContactPerson, SUP_Address, SUP_PhoneNo,
                    SUP_Fax, SUP_Email, SUP_Website, SUP_CRBY, SUP_CRON, SUP_STATUS, SUP_CompID
                )
                VALUES (
                    @NewId, @SupName, @SupCode, @SupContactPerson, @SupAddress, @SupPhoneNo,
                    @SupFax, @SupEmail, @SupWebsite, @SupCrby, GETDATE(), @SupStatus, @SupCompId
                )";

                    await connection.ExecuteAsync(insertQuery, new
                    {
                        NewId = newId,
                        SupCode = supplierCode,
                        dto.SupName,
                        dto.SupContactPerson,
                        dto.SupAddress,
                        dto.SupPhoneNo,
                        dto.SupFax,
                        dto.SupEmail,
                        dto.SupWebsite,
                        dto.SupCrby,
                        dto.SupStatus,
                        dto.SupCompId
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




        public async Task<int> InsertAssetAsync(AddAssetDto assetDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 1: Get the current max AFAM_ID
                string maxIdQuery = "SELECT ISNULL(MAX(AFAM_ID), 0) FROM Acc_FixedAssetMaster WITH (UPDLOCK, HOLDLOCK)";
                int maxId = await connection.ExecuteScalarAsync<int>(maxIdQuery, transaction: transaction);
                int newId = maxId + 1;

                // Step 2: Set default date
                DateTime defaultDate = new DateTime(1900, 1, 1);

                DateTime commissionDate = assetDto.AfamCommissionDate == null || assetDto.AfamCommissionDate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamCommissionDate.Value;

                DateTime purchaseDate = assetDto.AfamPurchaseDate == null || assetDto.AfamPurchaseDate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamPurchaseDate.Value;

                DateTime afamDate = assetDto.AfamDate == null || assetDto.AfamDate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamDate.Value;

                DateTime afamToDate = assetDto.AfamToDate == null || assetDto.AfamToDate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamToDate.Value;

                DateTime afamAmcFromDate = assetDto.AfamAmcfrmDate == null || assetDto.AfamAmcfrmDate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamAmcfrmDate.Value;

                DateTime afamAmcToDate = assetDto.AfamAmcto == null || assetDto.AfamAmcto == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamAmcto.Value;

                DateTime afamDlnDate = assetDto.AfamDlnDate == null || assetDto.AfamDlnDate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamDlnDate.Value;

                DateTime afamDateOfDeletion = assetDto.AfamDateOfDeletion == null || assetDto.AfamDateOfDeletion == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamDateOfDeletion.Value;

                DateTime afamLDate = assetDto.AfamLdate == null || assetDto.AfamLdate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamLdate.Value;

                DateTime afamLExchDate = assetDto.AfamLexchDate == null || assetDto.AfamLexchDate == DateTime.MinValue
                    ? defaultDate
                    : assetDto.AfamLexchDate.Value;

                // Step 3: Create item description
                string itemDesc = $"{assetDto.AfamItemCode}{assetDto.AfamDescription}";

                // Step 4: Insert query
                string insertQuery = @"
INSERT INTO Acc_FixedAssetMaster (
    AFAM_ID, AFAM_AssetType, AFAM_AssetCode, AFAM_ItemCode, AFAM_ItemDescription,
    AFAM_Description, AFAM_Quantity, AFAM_AssetAge,
    AFAM_CommissionDate, AFAM_PurchaseDate, AFAM_Date, AFAM_ToDate,
    AFAM_PurchaseAmount, AFAM_PolicyNo, AfAM_Amount, AFAM_Location,
    AFAM_Division, AFAM_Department, AFAM_Bay, AFAM_EmployeeName, AFAM_EmployeeCode,
  AfAM_BrokerName, AfAM_CompanyName, AFAM_SuplierName, AFAM_ContactPerson,
    AFAM_Address, AFAM_Phone, AFAM_Fax, AFAM_EmailID, AFAM_Code,
    AFAM_Website, AFAM_Unit, AFAM_CreatedOn, AFAM_DelFlag, AFAM_Status,
    AFAM_YearId, AFAM_CompID,  AFAM_IPAddress, AFAM_ContactPrsn,
    AFAM_AMCFrmDate, AFAM_AMCTo, AFAM_Contprsn, AFAM_PhoneNo, AFAM_AMCCompanyName,
    AFAM_AssetDeletion, AFAM_DlnDate, AFAM_DateOfDeletion, AFAM_Value, AFAM_LToWhom,
    AFAM_LAmount, AFAM_LAggriNo, AFAM_LCurrencyType, AFAM_LDate, AFAM_LExchDate,
    AFAM_CustId, AFAM_CreatedBy, AFAM_UpdatedBy, AFAM_WrntyDesc, Attribute1, Attribute2,
    Attribute3
)
VALUES (
    @AfamId, @AfamAssetType, @AfamAssetCode, @AfamItemCode, @AfamItemDesc,

    @AfamDescription, @AfamQuantity, @AfamAssetAge,
    @AfamCommissionDate, @AfamPurchaseDate, @AfamDate, @AfamToDate,
    @AfamPurchaseAmount, @AfamPolicyNo, @AfamAmount, @AfamLocation, @AfamDivision, @AfamDepartment, @AfamBay,
    @AfamEmployeeName, @AfamEmployeeCode, @AfamBrokerName, @AfamCompanyName, @AfamSuplierName, @AfamContactPerson,
    @AfamAddress, @AfamPhone, @AfamFax, @AfamEmailId, @AfamCode,
    @AfamWebsite, @AfamUnit, GETDATE(), 'A', 'A',
    @AfamYearId,  @AfamCompId, 'IPv4', @AfamContactPrsn,
    @AfamAmcfrmDate, @AfamAmcto, @AfamContprsn, @AfamPhoneNo, @AfamAmcCompanyName,
    0, @AfamDlnDate, @AfamDateOfDeletion, @AfamValue, @AfamLtoWhom,
    @AfamLamount, @AfamLaggriNo, 0, @AfamLdate, @AfamLexchDate,
    @AfamCustId, @AfamCreatedBy,  @AfamUpdatedBy, @AfamWrntyDesc, @Attribute1, @Attribute2,
    @Attribute3
);";

                await connection.ExecuteAsync(insertQuery, new
                {
                    AfamId = newId,
                    AfamAssetType = assetDto.AfamAssetType,
                    AfamCode = assetDto.AfamCode,
                    Attribute1 = assetDto.Attribute1,
                    Attribute2 = assetDto.Attribute2,
                    Attribute3 = assetDto.Attribute3,
                    AfamCreatedBy = assetDto.AfamCreatedBy,
                    AfamUpdatedBy = assetDto.AfamUpdatedBy,
                    AfamCompId = assetDto.AfamCompId,
                    AfamWrntyDesc = assetDto.AfamWrntyDesc,
                    AfamPurchaseAmount = assetDto.AfamPurchaseAmount,
                    AfamAssetCode = assetDto.AfamAssetCode,
                    AfamItemCode = assetDto.AfamItemCode,
                    AfamItemDesc = itemDesc,
                    AfamDescription = assetDto.AfamDescription,
                    AfamQuantity = assetDto.AfamQuantity,
                    AfamAssetAge = assetDto.AfamAssetAge,
                    AfamCommissionDate = commissionDate,
                    AfamPurchaseDate = purchaseDate,
                    AfamDate = afamDate,
                    AfamToDate = afamToDate,
                    AfamPolicyNo = assetDto.AfamPolicyNo,
                    AfamAmount = assetDto.AfamAmount,
                    AfamLocation = assetDto.AfamLocation,
                    AfamDivision = assetDto.AfamDivision,
                    AfamDepartment = assetDto.AfamDepartment,
                    AfamBay = assetDto.AfamBay,
                    AfamEmployeeName = assetDto.AfamEmployeeName,
                    AfamEmployeeCode = assetDto.AfamEmployeeCode,
                    AfamBrokerName = assetDto.AfamBrokerName,
                    AfamCompanyName = assetDto.AfamCompanyName,
                    AfamSuplierName = assetDto.AfamSuplierName,
                    AfamContactPerson = assetDto.AfamContactPerson,
                    AfamAddress = assetDto.AfamAddress,
                    AfamPhone = assetDto.AfamPhone,
                    AfamFax = assetDto.AfamFax,
                    AfamEmailId = assetDto.AfamEmailId,
                    AfamWebsite = assetDto.AfamWebsite,
                    AfamUnit = assetDto.AfamUnit,
                    AfamYearId = assetDto.AfamYearId,
                    AfamContactPrsn = assetDto.AfamContactPrsn,
                    AfamAmcfrmDate = afamAmcFromDate,
                    AfamAmcto = afamAmcToDate,
                    AfamContprsn = assetDto.AfamContprsn,
                    AfamPhoneNo = assetDto.AfamPhoneNo,
                    AfamAmcCompanyName = assetDto.AfamAmccompanyName,
                    AfamDlnDate = afamDlnDate,
                    AfamDateOfDeletion = afamDateOfDeletion,
                    AfamValue = assetDto.AfamValue,
                    AfamLtoWhom = assetDto.AfamLtoWhom,
                    AfamLamount = assetDto.AfamLamount,
                    AfamLaggriNo = assetDto.AfamLaggriNo,
                    AfamLdate = afamLDate,
                    AfamLexchDate = afamLExchDate,
                    AfamCustId = assetDto.AfamCustId
                }, transaction);

                // Step 5: Commit
                await transaction.CommitAsync();
                return newId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> GetLastAssetCodeAsync()
        {
            var sql = "SELECT TOP 1 AssetCode FROM Acc_FixedAssetMaster ORDER BY AM_ID DESC";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<string>(sql);
            }
        }

        public async Task<int> GetLocationIdAsync(string locationName)
        {
            if (string.IsNullOrWhiteSpace(locationName)) return 0;

            var normalizedName = locationName.Trim().ToLower();
            var sql = "SELECT LS_ID FROM Acc_AssetLocationSetup WHERE LOWER(LTRIM(RTRIM(LS_Description))) = @LS_Description";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    sql, new { LS_Description = normalizedName });
            }
        }

        public async Task<int> GetDivisionIdAsync(string divisionName)
        {
            if (string.IsNullOrWhiteSpace(divisionName)) return 0;

            var normalizedName = divisionName.Trim().ToLower();
            var sql = "SELECT LS_ID FROM Acc_AssetLocationSetup WHERE LOWER(LTRIM(RTRIM(LS_Description))) = @LS_Description";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    sql, new { LS_Description = normalizedName });
            }
        }

        public async Task<int> GetDepartmentIdAsync(string departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName)) return 0;

            var normalizedName = departmentName.Trim().ToLower();
            var sql = "SELECT LS_ID FROM Acc_AssetLocationSetup WHERE LOWER(LTRIM(RTRIM(LS_Description))) = @LS_Description";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    sql, new { LS_Description = normalizedName });
            }
        }

        public async Task<int> GetBayIdAsync(string bayName)
        {
            if (string.IsNullOrWhiteSpace(bayName)) return 0;

            var normalizedName = bayName.Trim().ToLower();
            var sql = "SELECT LS_ID FROM Acc_AssetLocationSetup WHERE LOWER(LTRIM(RTRIM(LS_Description))) = @LS_Description";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    sql, new { LS_Description = normalizedName });
            }
        }

        public async Task<int> GetAssetIdAsync(string AmDescription)
        {
            if (string.IsNullOrWhiteSpace(AmDescription))
                return 0;

            var normalizedName = AmDescription.Trim();

            var sql = @"
        SELECT AM_ID 
        FROM Acc_AssetMaster 
        WHERE LOWER(LTRIM(RTRIM(AM_Description))) = LOWER(LTRIM(RTRIM(@AM_Description)))
    ";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    sql, new { AM_Description = normalizedName });
            }
        }


        public async Task<int> GetUnitsIdAsync(string unitsName)
        {
            if (string.IsNullOrWhiteSpace(unitsName))
                return 0;

            var normalizedName = unitsName.Trim();

            var sql = @"
        SELECT cmm_ID 
        FROM Content_Management_Master 
        WHERE LOWER(LTRIM(RTRIM(cmm_Desc))) = LOWER(LTRIM(RTRIM(@unitsName)))
    ";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(
                    sql, new { unitsName = normalizedName });
            }
        }

        public static string GenerateAfamItemDesc(AssetExcelUploadDto asset)
        {
            return $"{asset.TransactionNo} - {asset.AmDescription}";
        }



        public async Task SaveAssetsAsync(List<AssetExcelUploadDto> validAssets)
        {
            var sql = @"
    INSERT INTO Acc_FixedAssetMaster 
    (
        AFAM_ID, AFAM_AssetType, AFAM_CustId, AFAM_YearID, AFAM_Description, AFAM_ItemDescription,
        AFAM_AssetCode, AFAM_Location, AFAM_Division, AFAM_Department, AFAM_Bay,
        AFAM_Quantity, AFAM_Unit, AFAM_AssetAge, AFAM_CommissionDate
    )
    VALUES 
    (
        @AmId, @AssetCodeId, @CustomerId, @FinancialYearId, @AmDescription, @AfamItemDesc,
        @TransactionNo, @LocationId, @DivisionId, @DepartmentId, @BayId,
        @Quantity, @UnitsOfMeasurementId, @AssetAge, @CommissionDate
    )";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                var maxIdQuery = "SELECT ISNULL(MAX(AFAM_ID), 0) FROM Acc_FixedAssetMaster";
                var currentMaxId = await connection.ExecuteScalarAsync<int>(maxIdQuery);
                int nextId = currentMaxId + 1;

                // Assign AmId to each asset
                foreach (var asset in validAssets)
                {
                    asset.AmId = nextId++;
                }

                // Prepare parameters
                var parameterList = validAssets.Select(asset => new
                {
                    AmId = asset.AmId,
                    AssetCodeId = asset.AssetCodeId,
                    CustomerId = asset.CustomerId,
                    FinancialYearId = asset.FinancialYearId,
                    AmDescription = asset.AmDescription,
                    AfamItemDesc = asset.AfamItemDesc,
                    TransactionNo = asset.TransactionNo,
                    LocationId = asset.LocationId,
                    DivisionId = asset.DivisionId,
                    DepartmentId = asset.DepartmentId,
                    BayId = asset.BayId,
                    Quantity = asset.Quantity,
                    UnitsOfMeasurementId = asset.UnitsOfMeasurementId,
                    AssetAge = asset.AssetAge,
                    CommissionDate = asset.CommissionDate.ToDateTime(TimeOnly.MinValue)
                }).ToList();

                await connection.ExecuteAsync(sql, parameterList);
            }
        }








        public async Task<string> UploadAndProcessExcel(IFormFile file, string sheetName, int customerId, int financialYearId)
        {
            if (file == null || file.Length == 0)
                return "No file uploaded.";

            try
            {
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[sheetName];
                    if (worksheet == null)
                        return "Sheet with the specified name not found.";

                    var rowCount = worksheet.Dimension.Rows;
                    var validAssets = new List<AssetExcelUploadDto>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var asset = new AssetExcelUploadDto
                        {
                            Location = worksheet.Cells[row, 2].Text.Trim(),
                            Division = worksheet.Cells[row, 3].Text.Trim(),
                            Department = worksheet.Cells[row, 4].Text.Trim(),
                            Bay = worksheet.Cells[row, 5].Text.Trim(),
                            AssetClassname = worksheet.Cells[row, 6].Text.Trim(),
                            AmDescription = worksheet.Cells[row, 8].Text.Trim(),
                            Quantity = int.TryParse(worksheet.Cells[row, 9].Text, out var qty) ? qty : 0,
                            CommissionDate = DateOnly.TryParse(worksheet.Cells[row, 10].Text, out var date)
    ? date
    : DateOnly.FromDateTime(DateTime.Now),
                            UnitsOfMeasurement = worksheet.Cells[row, 11].Text.Trim(),
                            AssetAge = int.TryParse(worksheet.Cells[row, 12].Text, out var age) ? age : 0,
                            
                           
                            
                            // Default to current date if not valid

                        };

                        // Try mapping names to IDs (0 if not found)
                        asset.LocationId = await GetLocationIdAsync(asset.Location);
                        asset.DivisionId = await GetDivisionIdAsync(asset.Division);
                        asset.DepartmentId = await GetDepartmentIdAsync(asset.Department);
                        asset.BayId = await GetBayIdAsync(asset.Bay);
                        asset.AssetCodeId = await GetAssetIdAsync(asset.AssetClassname);
                       
                        asset.UnitsOfMeasurementId = await GetUnitsIdAsync(asset.UnitsOfMeasurement);




                        validAssets.Add(asset);
                    }

                    if (validAssets.Any())
                    {
                        int compId = 1;
                        string transactionNo = await GetNextEmployeeCodeAsync();



                        foreach (var asset in validAssets)
                        {
                            asset.CustomerId = customerId;
                            asset.FinancialYearId = financialYearId;
                            asset.TransactionNo = transactionNo;
                            asset.AfamItemDesc = GenerateAfamItemDesc(asset);
                        }

                        await SaveAssetsAsync(validAssets);
                        return $"{validAssets.Count} assets uploaded successfully.";
                    }

                    return "No rows processed.";
                }
            }
            catch (Exception ex)
            {

                return $"Error processing the file: {ex.Message}";
            }
        }



        public async Task<(List<AssetExcelUploadDto>, List<string>)> ValidateExcelFormatAsync(IFormFile file, string sheetName)
        {
            var validAssets = new List<AssetExcelUploadDto>();
            var errors = new List<string>();

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[sheetName];

                if (worksheet == null)
                {
                    errors.Add($"Sheet '{sheetName}' not found.");
                    return (validAssets, errors);
                }

                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var dto = new AssetExcelUploadDto
                        {
                            Location = worksheet.Cells[row, 2].Text.Trim(),
                            Division = worksheet.Cells[row, 3].Text.Trim(),
                            Department = worksheet.Cells[row, 4].Text.Trim(),
                            Bay = worksheet.Cells[row, 5].Text.Trim(),
                            AssetClassname = worksheet.Cells[row, 6].Text.Trim(),
                            AssetCode = worksheet.Cells[row, 7].Text.Trim(),
                            AfamDescription = worksheet.Cells[row, 8].Text.Trim(),
                            Quantity = GetQuantityFromExcel(worksheet.Cells[row, 9].Value),
                            CommissionDate = GetCommissionDateFromExcel(worksheet.Cells[row, 10].Value),
                            UnitsOfMeasurement = worksheet.Cells[row, 11].Text.Trim(),
                            AssetAge = GetAssetAgeFromExcel(worksheet.Cells[row, 12].Value)
                        };

                     

                        // Optional: resolve IDs here using lookup services
                       

                        validAssets.Add(dto);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row}: Exception - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"General error while reading Excel: {ex.Message}");
            }

            return (validAssets, errors);
        }



        // Helper methods to handle parsing and conversions:

        private int GetQuantityFromExcel(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (double.TryParse(value.ToString(), out double quantity))
            {
                return quantity > 0 ? Convert.ToInt32(quantity) : 0; // Ensure Quantity is positive
            }

            return 0; // Return 0 if parsing fails
        }



        private int GetAssetAgeFromExcel(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (double.TryParse(value.ToString(), out double assetAge))
            {
                return Convert.ToInt32(assetAge); // Ensure valid asset age
            }

            return 0; // Return 0 if parsing fails
        }

        private DateOnly GetCommissionDateFromExcel(object value)
        {
            if (value == null)
            {
                return DateOnly.MinValue; // Use MinValue if the date is not provided
            }

            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime); // Convert to DateOnly if DateTime is valid
            }

            return DateOnly.MinValue; // Return MinValue if parsing fails
        }


        public async Task<List<string>> GetSheetNamesAsync(IFormFile file)
        {
            var sheetNames = new List<string>();

            try
            {
                if (file == null || file.Length == 0)
                    throw new Exception("File is empty or not uploaded.");

                // Read file directly into a MemoryStream (no need to save to disk)
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0; // Reset stream position before reading

                    using (var package = new ExcelPackage(memoryStream))
                    {
                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            sheetNames.Add(worksheet.Name); // ✅ This gets actual sheet names
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading sheet names from the file: {ex.Message}");
            }

            return sheetNames;
        }
    }
}


