using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;

namespace TracePca.Interface.AssetMaserInterface
{
    public interface LocationSetUpInterface
    {

        Task<IEnumerable<DropDownDto>> GetDivisionsAsync(int companyId, int customerId, int parentId);
        Task<IEnumerable<DropDownDto>> GetDepartmentsAsync(int companyId, int customerId, int parentId = 0);
        Task<IEnumerable<DropDownDto>> GetHeadersAsync(int companyId, int customerId, int parentId = 0);
        Task<IEnumerable<DropDownDto>> GetSubHeadersAsync(int companyId, int customerId , int parentId = 0);
        Task<IEnumerable<DropDownDto>> GetLocationsAsync(int companyId, int customerId, int parentId = 0);
        Task<IEnumerable<DropDownDto>> GetBayiAsync(int companyId, int customerId, string parentId = "0");
        Task<IEnumerable<DropDownDto>> GetAssetsAsync(int companyId, int customerId, string parentId = "0");
        Task<int> SaveLocationAsync(AddLocationDto locationDto);
        Task<int> SaveDivisionAsync(AddDivisionDto divisionDto);
        Task<IEnumerable<DropDownDto>> GetInsertedLocationsAsync(int companyId, int customerId, int topRecords = 10);

        Task<int> SaveDepartmentAsync(AddDepartmentDto departmentDto);
        Task<int> SaveBayAsync(AddBayDto bayDto);
        Task<int> SaveHeadingAsync(AddHeadingDto headingDto);

        Task<int> SaveSubHeadingAsync(AddSubHeadingDto subHeadingDto);

        Task<int> SaveAssetAsync(AddAssetClassDto assetDto);
        Task<AssetMasterDetailsDto> GetItemDetailsByAmIdAsync(int amId, int compId, int custId);


    }
}
