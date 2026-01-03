using System.Data;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using static TracePca.Dto.FixedAssets.DepreciationComputationDto;

namespace TracePca.Interface.FixedAssetsInterface

{
    public interface DepreciationComputationInterface
    {

        //DepreciationBasis


        //MethodofDepreciation


        //SaveDepreciation
        Task<bool> SaveDepreciationAsync(
   int depBasis,
   int yearId,
   int compId,
   int custId,
   int method,
   int userId,
   string ipAddress, List<DepreciationComputationnDto> normalList, List<DepreciationITActDto> itActList, AuditLoggDto audit);

        //Report(DownloadExcel)
        AssetDepreciationResult GetAssetDepreciationExcelTemplate();



        //Go
        //itcorrect
        Task<List<DepreciationnITActDto>> LoadDepreciationITActAsync(
            int compId, int yearId, int custId, DateTime endDate);

        //------------------new it correct
        Task<DepreciationResultDto> CalculateDepreciationAsync(DepreciationRequesttDto request);

        //----wdv
        Task<List<dynamic>> CalculateCompanyActWDVAsync(
 int compId,
 int yearId,
 int custId,
 int noOfDays,
 int totalDays,
 int duration,
 DateTime startDate,
 DateTime endDate,
 int method);

        //----sln&wdv
        Task<List<dynamic>> CalculateCompanyActDepreciationAsync(
    int compId,
    int yearId,
    int custId,
    int noOfDays,
    int totalDays,
    int duration,
    DateTime startDate,
    DateTime endDate,
    int method);


        //------final

        Task<object> CalculateDepreciationAsync(
   int depBasis,          // 1 = Company Act, 2 = IT Act
   int compId,
   int yearId,
   int custId,
   int noOfDays,
   int totalDays,
   int duration,
   DateTime startDate,
   DateTime endDate,
   int method);

        //companycalculation
        Task<List<ITDepreciationResponseDto>> CalculateITActDepreciationAsync(ITDepreciationRequestDto request);
    }
}