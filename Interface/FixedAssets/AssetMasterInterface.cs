using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
//using TracePca.Dto;

//using TracePca.Dto;
using TracePca.Models.UserModels;
using static TracePca.Dto.FixedAssets.AssetMasterdto;
//using static TracePca.Dto.FixedAssets.AssetSetupDto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetMasterInterface
    {
        //LoadCustomer
        Task<IEnumerable<CustDto>> LoadCustomerAsync(int CompId);

        //LoadStatus
        Task<IEnumerable<StatusDto>> LoadStatusAsync(int compId, string Name);

        //FinancialYear
        Task<IEnumerable<YearDto>> GetYearsAsync(int compId);

       // Location
        Task<IEnumerable<LocationDto>> GetLocationAsync(int compId, int CustId);

        //LoadDivision
        Task<IEnumerable<DivisionDto>> LoadDivisionAsync(int compId, int parentId, int custId);

        //LoadDepartment
        Task<IEnumerable<DepartmentDto>> LoadDepartmentAsync(int compId, int parentId, int custId);
        //LoadBay
        Task<IEnumerable<BayDto>> LoadBayAsync(int compId, int parentId, int custId);

        //LoadHeading
        Task<IEnumerable<HeadingDto>> LoadHeadingAsync(int compId, int custId);

        //LoadSubHeading
        Task<IEnumerable<SubHeadingDto>> LoadSubHeadingAsync(int compId, int parentId, int custId);

        //AssetClassUnderSubHeading
        Task<IEnumerable<ItemDto>> LoadItemsAsync(int compId, int parentId, int custId);


       
    }

}
