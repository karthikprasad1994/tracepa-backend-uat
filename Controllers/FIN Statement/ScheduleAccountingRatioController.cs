using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleAccoutingRatioController : ControllerBase
    {

        private ScheduleAccountingRatioInterface _ScheduleAccountingRatioService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public ScheduleAccoutingRatioController(Trdmyus1Context dbcontext, IConfiguration configuration, ScheduleAccountingRatioInterface ScheduleAccountingRatioInterface, IHttpContextAccessor httpContextAccessor)
        {

            _ScheduleAccountingRatioService = ScheduleAccountingRatioInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;

        }

    }
}
