using System.Threading.Tasks;
using static TracePca.Dto.FIN_Statement.AgingAnalysisDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface AgingAnalysisInterface
    {

        //GetAnalysisBasedOnMonthForTradePayables
        Task<IEnumerable<TradePayablesDto>> GetTradePayablesAsync(int CompId, int CustId, int BranchId, int YearId);

        //GetAnalysisBasedOnMonthForTradeReceivables
        Task<IEnumerable<TradePayablesDto>> GetTradeReceiveablesAsync(int CompId, int CustId, int BranchId, int YearId);


        //GetAnalysisBasedOnMonthForTradePayablesById
        Task<IEnumerable<TradePayablesByIdDto>> GetTradePayablesByIdAsync(int CompId, int CustId, int BranchId, int YearId);

        //GetAnalysisBasedOnMonthForTradeReceivablesById
        Task<IEnumerable<TradeReceiveablesByIdDto>> GetTradeReceiveablesByIdAsync(int CompId, int CustId, int BranchId, int YearId);
    }
}
