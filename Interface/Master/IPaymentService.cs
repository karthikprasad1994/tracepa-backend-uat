using System.Threading.Tasks;
using TracePca.DTOs;

namespace TracePca.Services
{
    public interface IPaymentService
    {
        Task<object> CreateOrderAsync(CreateOrderDto dto);
        bool VerifySignature(string orderId, string paymentId, string signature);
    }
}
