using TracePca.Dto.Middleware;

namespace TracePca.Interface.Middleware
{
    public interface PerformanceInterface
    {

        // Task<ApiSummaryWithUsersDto> GetFormPerformanceSummariesAsync(int lookbackMinutes = 10);
        // Task<IEnumerable<ApiSummaryDto>> GetFormPerformanceSummariesAsync(int lookbackMinutes = 10);
        //Task<IEnumerable<ApiSummaryWithUsersDto>> GetFormPerformanceSummariesAsync(int lookbackMinutes = 10);
        Task<List<ApiSummaryDto>> GetFormPerformanceSummariesAsync();
        //Task<IEnumerable<ApiAlertDto>> GetSlowApisAsync(int lookbackMinutes = 10);

    }
}
