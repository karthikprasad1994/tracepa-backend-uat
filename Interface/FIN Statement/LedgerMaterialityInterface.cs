using System.Threading.Tasks;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface LedgerMaterialityInterface
    {
        //GetMaterialityDescription
        Task<IEnumerable<ContentManagementDto>> GetMaterialityDescriptionAsync(int CompId, string cmmCategory, int YearId, int CustId);

        //SaveOrUpdateLedgerMaterialityMaster
        Task<List<int[]>> SaveOrUpdateLedgerMaterialityAsync(IEnumerable<LedgerMaterialityMasterDto> dtos);

        //GetLedgerMaterialityMaster
        Task<IEnumerable<GetLedgerMaterialityMasterDto>> GetLedgerMaterialityAsync(int compId, int lm_ID);

        //GenerateIDButtonForContentMaterialityMaster
        Task<string> GenerateAndInsertContentForMTAsync(int compId, string description);
    }
}
