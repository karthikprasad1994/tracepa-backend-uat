using TracePca.Dto.Middleware;

namespace TracePca.Interface.Middleware
{
    public interface PerformanceInterface
    {
        Task<IEnumerable<ApiSummaryDto>> GetFormPerformanceSummariesAsync(int lookbackMinutes = 10);
        Task<IEnumerable<ApiAlertDto>> GetSlowApisAsync(int lookbackMinutes = 10);

    }
}
