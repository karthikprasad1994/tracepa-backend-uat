using TracePca.Dto.DigitalFiling;

namespace TracePca.Interface.DigitalFiling
{
    public interface FoldersInterface
    {
        Task<DigitalFilingDropDownListDataDTO> LoadAllDDLDataAsync();
        Task<List<DFDropDownListData>> LoadCabinetsByUserIdDDLAsync(int compId, int userId);
        Task<DigitalFilingDropDownListDataDTO> LoadSubCabinetsByCabinetIdDDLAsync(int compId, int cabinetId);
        Task<List<FolderDTO>> GetAllFoldersBySubCabinetIdAsync(int subCabinetId, string statusCode);
        Task<string> UpdateFolderStatusAsync(UpdateFolderStatusRequestDTO request);
        Task<int> SaveOrUpdateFolderAsync(FolderDTO dto);
    }
}
