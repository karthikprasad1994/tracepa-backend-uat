using System.Threading.Tasks;
using static TracePca.Dto.ClientPortal.ClientPortalDto;

namespace TracePca.Interface.ClientPortal
{
    public interface IClientPortalInterface
    {
        Task<UserCustomerResponse> GetUserCustomerDetailsAsync(GetUserCustomerIdRequest request);
    }
}
