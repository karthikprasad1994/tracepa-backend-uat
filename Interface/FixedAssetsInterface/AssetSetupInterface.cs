using System.Threading.Tasks;
using static TracePca.Dto.FixedAssets.AssetSetupDto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetSetupInterface
    {
        //Location
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

        //SaveAsset
        Task<int[]> SaveAssetAsync(AssetMasterDto asset);

        //EditLocation
        Task<IEnumerable<LocationEditDto>> LoadLocationForEditAsync(int compId, int locationId);

        //EditDivision
        Task<DivisionEditDto> LoadDivisionForEditAsync(int compId, int DivisionId);

        //EditDepartment
        Task<DepartmentEditDto> LoadDepartmentForEditAsync(int compId, int DepartmentId);


    }
}
