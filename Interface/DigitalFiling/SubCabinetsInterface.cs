using TracePca.Dto.DigitalFiling;

namespace TracePca.Interface.DigitalFiling
{
    public interface SubCabinetsInterface
    {
        Task<DigitalFilingDropDownListDataDTO> LoadAllDDLDataAsync();

		Task<List<SubCabinetDetailsDTO>> GetAllSubCabinetsDetailsByCabinetAndUserIdAsync(int userId, int cabinetId, string statusCode);
		Task<List<DFDropDownListData>> LoadCabinetsByUserIdDDLAsync(int compId, int userId);
        Task<DigitalFilingDropDownListDataDTO> LoadPermissionUsersByDeptIdDDLAsync(int deptID);
        Task<List<SubCabinetDTO>> GetAllSubCabinetsByCabinetAndUserIdAsync(int userId, int cabinetId, string statusCode);
        Task<string> UpdateSubCabinetStatusAsync(UpdateSubCabinetStatusRequestDTO request);
        Task<int> CheckExtraPermissionsToCabinetAsync(int userId, int cabinetId, string permTypes);
        Task<SubCabinetPermissionDTO> GetSubCabinetPermissionByLevelIdAsync(int compId, int departmentId, int userId, int subCabinetId);
        Task<int> SaveOrUpdateSubCabinetAsync(SubCabinetDTO dto);
    }
}
