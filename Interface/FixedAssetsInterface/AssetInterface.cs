using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Dto;
//using TracePca.Models;
using TracePca.Models.UserModels;

namespace TracePca.Interface
{
    public interface AssetInterface
    {

        Task<IEnumerable<CustomerDto>> GetCustomersAsync(int companyId);

        Task<IEnumerable<YearDto>> LoadYearsAsync(int companyId);



       IEnumerable<CustomerDto> GetCustomers();

        Task<IEnumerable<FixedAssetTypeDto>> LoadFixedAssetTypesAsync(int companyId, int customerId);

        //Task<string> GetNextAssetCodeAsync();

        Task<string> GenerateTransactionNoAsync(int compId, int yearId, int custId);

        Task<int> InsertAssetAsync(AddAssetDto assetDto);

       Task<IEnumerable<SupplierDto>> LoadExistingSuppliersAsync(int companyId, int supplierId);

        Task<string> GetNextEmployeeCodeAsync();

        Task<IEnumerable<UnitOfMeasureDto>> LoadUnitsOfMeasureAsync(int companyId);

        Task<IEnumerable<CurrencyDto>> LoadCurrencyAsync(string sNameSpace, int iCompID);

        Task<int> SaveSupplierAsync(AddSupplierDto dto);
        Task<string> UploadAndProcessExcel(IFormFile file, string sheetName, int customerId, int financialYearId);

        Task SaveAssetsAsync(List<AssetExcelUploadDto> validAssets);
        

            Task<int> GetBayIdAsync(string bayName);

        Task<int> GetDepartmentIdAsync(string departmentName);
        Task<int> GetDivisionIdAsync(string divisionName);
        Task<int> GetLocationIdAsync(string locationName);
        Task<string> GetLastAssetCodeAsync();
        Task<List<string>> GetSheetNamesAsync(IFormFile file);
        Task<(List<AssetExcelUploadDto>, List<string>)> ValidateExcelFormatAsync(IFormFile file, string sheetName);
        Task<int> GetAssetIdAsync(string bayName);
        Task<int> GetUnitsIdAsync(string Unitsname);









    }

}
