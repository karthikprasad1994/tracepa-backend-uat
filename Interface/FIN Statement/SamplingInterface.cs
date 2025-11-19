using static TracePca.Dto.FIN_Statement.SamplingDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface SamplingInterface
    {
        //GetSystemSampling
        Task<IEnumerable<SystemstemSamplingDTO>> GetSystemSamplingAsync(int compId, int custId, int branchId, int yearId, int nthPosition, int fromrow, int toRow, int sampleSize);

        //GetStatifiedSamping
        Task<IEnumerable<StratifiedSamplingDTO>> GetStratifiedSamplingAsync(int compId, int custId, int branchId, int yearId, decimal percentage);

        //UpdateSystemSamplingStatus
        Task<int> UpdateSystemSamplingStatusAsync(List<UpdateSystemSamplingStatusDto> dtoList);

        //UpdateStatifiedSampingStatus
        Task<int> UpdateStatifiedSampingStatusAsync(List<UpdateStatifiedSampingStatusDto> dtoList);
    }
}
