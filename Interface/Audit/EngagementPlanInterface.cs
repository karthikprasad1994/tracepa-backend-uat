using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface EngagementPlanInterface
    {
        Task<LoEDto> GetLoeIdAsync(int customerId, int compId, int serviceId);
        Task<List<AuditTypeDto>> GetAuditTypesAsync(int compId);
         Task<DropDownDataDto> LoadAllDropdownDataAsync(int compId);
        Task<bool> SaveAllLoeDataAsync(AddEngagementDto dto);

    }
}
