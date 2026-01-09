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

        //SaveOrUpdateContentMateriality
        Task<string> SaveOrUpdateContentForMTAsync(int? id, int compId, string description, string remarks, string Category);

        //GetMaterialityId
        Task<IEnumerable<GetMaterialityIdDto>> GetMaterialityIdAsync(int CompId, int Id);

        //DeleteMaterialityById
        Task<int> DeleteMaterialityByIdAsync(int Id);

        //LoadDescription
        Task<IEnumerable<LoadDescriptionDto>> LoadDescriptionAsync(int compId, string category);
        //Task<IEnumerable<DescriptionDetailsDto>> GetMaterialityBasisAsync(int compId, int custId, int branchId, int yearId, int typeId);
        Task<MaterialityBasisGridDto> GetMaterialityBasisAsync(int compId, int custId, int branchId, int yearId, int typeId);
    
            Task<List<MaterialityMasterDto>> GetMaterialityAsync(
                int branchId,
                int custId,
                int compId,
                int financialYearId
            );

        Task<int> UpdateMaterialityAsync(LedgerMaterialityUpdateDto dto);

    }
}
