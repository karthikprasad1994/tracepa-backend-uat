using static TracePca.Dto.FIN_Statement.FlaggedTransactionDto;
using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface FlaggedTransactionInterface
    {
        //GetDiferenceAmountStatus
        Task<IEnumerable<GetDiferenceAmountStatusDto>> GetDiferenceAmountStatusAsync(int CompId, int CustId, int BranchId, int YearId);

        //GetAbnormalEntriesAEStatus
        Task<IEnumerable<GetGetAbnormalEntriesSeqReferenceNumDto>> GetAbnormalEntriesSeqReferenceNumAsync(int CompId, int CustId, int BranchId, int YearId);

        //GetSelectedPartiesSeqReferenceNum
        Task<IEnumerable<GetSelectedPartiesSeqReferenceNumDto>> GetSelectedPartiesSeqReferenceNumAsync(int CompId, int CustId, int BranchId, int YearId);

        //GetSystemSamplingStatus
        Task<IEnumerable<GetSystemSamplingStatusDto>> GetSystemSamplingStatusAsync(int CompId, int CustId, int BranchId, int YearId);

        //GetSystemSamplingStatus
        Task<IEnumerable<GetStatifiedSampingStatusDto>> GetStatifiedSampingStatusAsync(int CompId, int CustId, int BranchId, int YearId);

        //GetCustomerTBDelFlg
        Task<IEnumerable<GetCustomerTBDelFlgDto>> GetCustomerTBDelFlgAsync(int CompId, int CustId, int BranchId, int YearId);
    }
}
