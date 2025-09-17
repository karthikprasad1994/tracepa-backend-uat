using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface LedgerMaterialityInterface
    {
        //GetContentManagement
        Task<IEnumerable<ContentManagementDto>> GetContentManagementAsync(int CompId, string cmm_Category);
    } 
}
