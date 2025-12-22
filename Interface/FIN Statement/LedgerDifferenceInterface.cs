using System.Data;
using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ILedgerDifferenceInterface
    {

        //GetDescriptionWiseDetails
        Task<IEnumerable<DescriptionWiseDetailsDto>> GetDescriptionWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId);

        //UpdateDescriptionWiseDetailsStatus
        Task<int> UpdateTrailBalanceStatusAsync(List<UpdateDescriptionWiseDetailsStatusDto> dtoList);

        //GetDescriptionDetails
        Task<IEnumerable<DescriptionDetailsDto>> GetAccountDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId, int pkId);

        //GetCustomerTBGrid
        //Task<CustCOATrialBalanceResult> GetCustCOAMasterDetailsCustomerAsync(int compId, int custId, int yearId, int scheduleTypeId, int unmapped, int branchId);
        //Task<List<GetCustomerTBGridDto>> GetCustomerTBGridAsync(int CompId, int custId, int yearId, int branchId);
        Task<DataSet> GetCustCOAMasterDetailsCustomerAsync(CustCoaRequestDto request);

        //UpdateCustomerTBDelFlg
        Task<int> UpdateCustomerTrailBalanceStatusAsync(List<UpdateCustomerTrailBalanceStatusDto> dtoList);
       

    }
}
