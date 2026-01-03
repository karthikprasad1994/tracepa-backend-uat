using System.Threading.Tasks;
using static TracePca.Dto.FixedAssets.AssetSetupDto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetSetupInterface
    {
        ////Location
        //Task<IEnumerable<LocationDto>> GetLocationAsync(int compId, int CustId);

        ////LoadDivision
        //Task<IEnumerable<DivisionDto>> LoadDivisionAsync(int compId, int parentId, int custId);

        ////LoadDepartment
        //Task<IEnumerable<DepartmentDto>> LoadDepartmentAsync(int compId, int parentId, int custId);
        ////LoadBay
        //Task<IEnumerable<BayDto>> LoadBayAsync(int compId, int parentId, int custId);

        ////LoadHeading
        //Task<IEnumerable<HeadingDto>> LoadHeadingAsync(int compId, int custId);

        ////LoadSubHeading
        //Task<IEnumerable<SubHeadingDto>> LoadSubHeadingAsync(int compId, int parentId, int custId);

        ////AssetClassUnderSubHeading
        //Task<IEnumerable<ItemDto>> LoadItemsAsync(int compId, int parentId, int custId);

        //SaveAsset
        Task<int[]> SaveAssetAsync(AssetMasterDto asset);

        ////EditLocation
        //Task<IEnumerable<LocationEditDto>> LoadLocationForEditAsync(int compId, int locationId);

        ////EditDivision
        //Task<IEnumerable<DivisionEditDto>> LoadDivisionForEditAsync(int compId, int DivisionId);

        ////EditDepartment
        //Task<IEnumerable<DepartmentEditDto>> LoadDepartmentForEditAsync(int compId, int DepartmentId);

        ////Editbay
        //Task<IEnumerable<BayEditDto>> LoadBayForEditAsync(int compId, int bayId);

        ////EditHeading
        //Task<IEnumerable<HeadingEditDto>> HeadingForEditAsync(int compId, int HeadingId);

        ////EditSubHeading
        //Task<IEnumerable<SubHeadingEditDto>> SubHeadingForEditAsync(int compId, int SubHeadingId);

        ////EditAssetClassUnderSubHeading(LoadItem)
        //Task<IEnumerable<ItemEditDto>> ItemForEditAsync(int compId, int ItemId);

        //SaveLocation
        Task<int[]> SaveLocationSetupAsync(LocationSetupDto dto);

        //SaveDivision
        Task<int[]> SaveDivisionAsync(SaveDivisionDto dto);

        //SaveDepartment
        Task<int[]> SaveDepartmentAsync(SaveDepartmentDto dto);

        //SaveBay
        Task<int[]> SaveBayAsync(SaveBayDto dto);

        //SaveHeading
        Task<int[]> SaveHeadingAsync(SaveHeadingDto dto);

        //SaveSubHeading
        Task<int[]> SaveSubHeadingAsync(SaveSubHeadingDto dto);

        //SaveAssetClassUnderSubHeading
        Task<string[]> SaveAssetClassAsync(SaveAssetClassDto asset);

        ////UpdateLocation
        //Task<int[]> UpdateLocationSetupAsync(UpadteLocationSetupDto dto);

        ////UpdateDivision
        //Task<int[]> UpdateDivisionSetupAsync(UpadteDivisionSetupDto dto);

        ////UpdateDepartment
        //Task<int[]> UpdateDepartmentSetupAsync(UpadteDepartmentSetupDto dto);

        ////UpdateBay
        //Task<int[]> UpdateBaySetupAsync(UpadteBaySetupDto dto);
    }
}
