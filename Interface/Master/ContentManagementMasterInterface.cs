using TracePca.Dto;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Master
{
    public interface ContentManagementMasterInterface
    {
        Task<(bool Success, string Message, MasterDropDownListDataDTO? Data)> LoadAllMasterDDLDataAsync(int compId);
        Task<(bool Success, string Message, List<ContentManagementMasterDTO> Data)> GetMasterDataByStatusAsync(string type, string status, int compId);
        Task<(bool Success, string Message, ContentManagementMasterDTO? Data)> GetMasterDataByIdAsync(int id, int compId);
        Task<(int Id, string Message, List<DropDownListData> MasterList)> SaveOrUpdateMasterDataAndGetRecordsAsync(ContentManagementMasterDTO dto);
        Task<(bool Success, string Message)> UpdateRecordsStatusAsync(List<int> ids, string action, int compId, int updatedBy, string ipAddress);
    }
}
