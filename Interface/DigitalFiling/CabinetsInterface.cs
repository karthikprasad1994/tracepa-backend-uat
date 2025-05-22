using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFiling;

namespace TracePca.Interface.DigitalFiling
{
    public interface CabinetsInterface
    {
        Task<DigitalFilingDropDownListDataDTO> LoadDepartmentDDLAsync();
        Task<DigitalFilingDropDownListDataDTO> LoadCabinetUserPermissionDDLAsync(int deptID);
        Task<List<DFDropDownListData>> LoadAllUserCabinetDLLAsync(int compId, int userId);
        Task<List<CabinetDto>> GetAllSubCabAsync(string status, int cabId, int userId);
        Task<string> UpdateCabinetStatusAsync(UpdateCabinetStatusRequestDTO request);
    }
}
