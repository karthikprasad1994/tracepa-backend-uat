using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TracePca.Dto.Dashboard.DashboardDto;


namespace TracePca.Interface.Dashboard
{
    public interface DashboardInterface
    {

        Task<RemarksSummaryDto> GetRemarksSummaryAsync();
    }
}
