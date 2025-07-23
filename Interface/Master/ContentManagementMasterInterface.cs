using TracePca.Dto;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Master
{
    public interface ContentManagementMasterInterface
    {
        Task<(int Id, string Message, List<DropDownListData> MasterList)> SaveOrUpdateMasterDataAndGetRecordsAsync(ContentManagementMasterDTO dto);
    }
}
