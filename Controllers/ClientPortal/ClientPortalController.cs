using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.ClientPortal;
using TracePca.Service.ClientPortal;
using static TracePca.Dto.ClientPortal.ClientPortalDto;

namespace TracePca.Controllers.ClientPortal
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientPortalController : ControllerBase
    {
        private readonly IClientPortalInterface _service;

        public ClientPortalController(IClientPortalInterface service)
        {
            _service = service;
        }

        // GET: api/ClientPortal/GetUserCustomerId?companyId=1&userId=5
        [HttpGet("GetUserCustomerId")]
        public async Task<IActionResult> GetUserCustomerDetails(
            [FromQuery] string companyId,
            [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(companyId) || string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "companyId and userId are required."
                });
            }

            var response = await _service.GetUserCustomerDetailsAsync(new GetUserCustomerIdRequest
            {
                CompanyId = companyId,
                UserId = userId
            });

            return Ok(new
            {
                success = true,
                data = response
            });
        }
    }
}
