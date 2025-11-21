using System.Threading.Tasks;
using static TracePca.Dto.ClientPortal.ClientPortalDto;

namespace TracePca.Interface.ClientPortal
{
    public interface IClientPortalInterface
    {
        Task<UserCustomerResponse> GetUserCustomerDetailsAsync(GetUserCustomerIdRequest request);
        Task<List<DRLLogDto>> LoadDRLLogAsync(int companyId, int auditNo, int custId, int yearId);
        Task<List<AttachmentResponseDto>> LoadAttachmentsAsync(AttachmentRequestDto request);


    }
}
