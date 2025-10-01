using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface AbnormalitiesInterface
    {

        //GetAbnormalTransactions
        Task<IEnumerable<AbnormalTransactionsDto>> GetAbnormalTransactionsAsync(int iCustId, int iBranchId, int iYearID, int iAbnormalType, string sAmount);
    }
}
