using TracePca.Dto.Master;
namespace TracePca.Interface.Master
{
    public interface CompanyDetailsInterface
    {
        Task<(bool Success, string Message, List<CompanyDetailsListDto> Data)> GetCompanyDetailsListAsync(int compId);
        Task<(bool Success, string Message, CompanyDetailsDto? Data)> GetCompanyDetailsByIdAsync(int id, int compId);
        Task<(bool Success, string Message, int PkId)> SaveOrUpdateCompanyDetailsAsync(CompanyDetailsDto dto);
    }
}
