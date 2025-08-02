using TracePca.Dto.Application_Metric;

namespace TracePca.Interface.Middleware
{
    public interface ApplicationMetricInterface
    {
        Task<ApplicationMetricDto> GetApplicationMetricsAsync();

    }
}
