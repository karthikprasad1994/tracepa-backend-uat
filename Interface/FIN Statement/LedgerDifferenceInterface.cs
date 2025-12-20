using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface LedgerDifferenceInterface
    {

        //GetDescriptionWiseDetails
        Task<IEnumerable<DescriptionWiseDetailsDto>> GetDescriptionWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId);

        //UpdateDescriptionWiseDetailsStatus
        Task<int> UpdateTrailBalanceStatusAsync(List<UpdateDescriptionWiseDetailsStatusDto> dtoList);

        //GetDescriptionDetails
        Task<IEnumerable<DescriptionDetailsDto>> GetAccountDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId, int pkId);

        //GetVODTotalGrid
        Task<CustCOATrialBalanceResult> GetCustCOAMasterDetailsCustomerAsync(int compId, int custId, int yearId, int scheduleTypeId, int unmapped, int branchId);

        //UpdateCustomerTBDelFlg
        Task<int> UpdateCustomerTrailBalanceStatusAsync(List<UpdateCustomerTrailBalanceStatusDto> dtoList);
    }
}
