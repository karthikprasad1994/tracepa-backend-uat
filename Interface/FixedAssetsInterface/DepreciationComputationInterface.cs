using static TracePca.Dto.FixedAssets.DepreciationComputationDto;
namespace TracePca.Interface.FixedAssetsInterface

{
    public interface DepreciationComputationInterface
    {
        //MethodofDepreciation
        Task<List<DepreciationDto>> LoadDepreciationCompSLMAsync(
   string sNameSpace,
   int compId,
   int yearId,
   int noOfDays,
   int tNoOfDays,
   int duration,
   DateTime startDate,
   DateTime endDate,
   int custId,
   int method);
    }
}
