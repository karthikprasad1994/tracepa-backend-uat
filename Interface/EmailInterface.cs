using TracePca.Dto.Email;

namespace TracePca.Interface
{
    public interface EmailInterface
    {
        Task SendCommonEmailAsync(CommonEmailDto dto);
    }
}
