using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface LedgerMaterialityInterface
    {
        //GetMaterialityDescription
        Task<IEnumerable<ContentManagementDto>> GetMaterialityDescriptionAsync(int CompId, string cmm_Category);

        //SaveOrUpdateLedgerMaterialityMaster
        Task<int[]> SaveOrUpdateLedgerMaterialityAsync(LedgerMaterialityMasterDto dto);

        //GetLedgerMaterialityMaster
        Task<IEnumerable<GetLedgerMaterialityMasterDto>> GetLedgerMaterialityAsync(int compId, int lm_ID);
    }
}
