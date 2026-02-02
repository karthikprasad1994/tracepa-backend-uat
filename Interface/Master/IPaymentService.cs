using System.Threading.Tasks;
using TracePca.Controllers;
using TracePca.DTOs;

namespace TracePca.Services
{
    public interface IPaymentService
    {
        Task<object> CreateOrderAsync(CreateOrderDto dto);
        bool VerifySignature(string orderId, string paymentId, string signature);

        Task<bool> VerifyAndSavePaymentAsync(VerifyPaymentRequest request);
        Task<string> GetPlanVersionAsync(long databaseId);
        Task<SubscriptionCountdownDto> GetSubscriptionCountdownAsync(string databaseId);

    }
}
