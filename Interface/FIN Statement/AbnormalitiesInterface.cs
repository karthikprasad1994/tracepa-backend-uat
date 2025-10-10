using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface AbnormalitiesInterface
    {

        //GetAbnormalTransactions
        //Task<IEnumerable<AbnormalTransactionsDto>> GetAbnormalTransactionsAsync(int iCustId, int iBranchId, int iYearID, int iAbnormalType, decimal dAmount);

        //Type1
        Task<IEnumerable<AbnormalTransactions1Dto>> GetAllAbnormalTransactions1Async(int iCustId, int iBranchId, int iYearID, decimal dAmount);

        //Type2
        Task<IEnumerable<AbnormalTransactions2Dto>> GetAbnormalTransactions2Async(int iCustId, int iBranchId, int iYearID, decimal dAmount);

        //Type3
        Task<IEnumerable<AbnormalTransactions3Dto>> GetAbnormalTransactions3Async(int iCustId, int iBranchId, int iYearID, int iAbnormalType, decimal dAmount);
    }
}
