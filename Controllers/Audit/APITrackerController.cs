using Microsoft.AspNetCore.Mvc;
using TracePca.Service.Audit;

namespace TracePca.Controllers.Audit
{
	[Route("api/[controller]")]
	[ApiController]
	public class APITrackerController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly ApiPerformanceTracker _performanceTracker;
		//public APITrackerController(IHttpClientFactory httpClientFactory,
		//				 ApiPerformanceTracker performanceTracker)
		//{
		//	_httpClient = httpClientFactory.CreateClient();
		//	_performanceTracker = performanceTracker;
		//}


		public APITrackerController(IHttpClientFactory httpClientFactory,
					 ApiPerformanceTracker performanceTracker)
		{
			// Create HttpClient with custom handler to bypass SSL validation
			var handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback =
					HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};

			_httpClient = new HttpClient(handler);
			_performanceTracker = performanceTracker;
		}

		//[HttpGet "CallExternalApi"]
		[HttpGet("CallExternalApi")]
		public async Task<IActionResult> CallExternalApi()
		{
			try
			{
				 
				string apiUrl = "https://localhost:7090/api/Cabinet/LoadCabinet?deptId=5&userId=1&compID=1";

				_httpClient.DefaultRequestHeaders.Add("CustomerCode", "");

				var response = await _performanceTracker.TrackPerformanceAsync(
					async () => await _httpClient.GetAsync(apiUrl),
					apiUrl,
					"GET"
				);

				if (response is ObjectResult objectResult)
				{
					return objectResult;
				}

				return BadRequest("Unexpected response type");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}

