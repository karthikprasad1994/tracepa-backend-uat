using static TracePca.Dto.Masters.UploadExcelDto;

namespace TracePca.Interface.Master
{
    public interface UploadExcelInterface
    {
        //ValidateClientDetails
        Task<IEnumerable<CustomerValidationResult>> ValidateCustomerExcelAsync(IFormFile file);
    }
}
