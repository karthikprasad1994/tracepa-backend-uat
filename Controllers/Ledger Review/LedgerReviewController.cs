using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.Audit;
using TracePca.Interface.LedgerReview;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.Ledger_Review
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerReviewController : ControllerBase

    {

        private LedgerReviewInterface _LedgerReviewInterface;
        public LedgerReviewController(LedgerReviewInterface LedgerReviewInterface)
        {
            _LedgerReviewInterface = LedgerReviewInterface;

        }
        // GET: api/<LedgerReviewController>

        [HttpGet("AllRules")]
        public async Task<IActionResult> GetAllRules()
        {
            var rules = await _LedgerReviewInterface.GetAllRulesAsync();
            return Ok(rules);
        }
    }
}

