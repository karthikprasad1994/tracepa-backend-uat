using TracePca.Dto.LedgerReview;

namespace TracePca.Interface.LedgerReview
{
    public interface LedgerReviewInterface
    {

        Task<List<PcaobRuleDto>> GetAllRulesAsync();

    }
}
