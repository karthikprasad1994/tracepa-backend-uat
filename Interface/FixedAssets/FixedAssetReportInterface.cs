using System.Data;
using System.Threading.Tasks;
using static TracePca.Dto.FixedAssets.FixedAssetReportDto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface FixedAssetReportInterface
    {
        //Task GetFixedAssetReportAsync(FixedAssetReportRequestDto request);

        // Report(Ok)
        //Task<List<FixedAssetReportRowDto>> GetFixedAssetReportAsync(
        //    FixedAssetReportRequestDto request);

        //-----------

        Task<object> GetFixedAssetReportAsync(
     int reportType,
     int compId,
     int custId,
     int yearId,
     int methodType,
     int locationIds,
     string financialYear
 );

        //Report(Go)
        //    Task<DataTable> LoadDynCompanyDetailedActAsync(
        //string nameSpace,
        //int compId,
        //int yearId,
        //int custId,
        //string locationIds,
        //string divisionIds,
        //string departmentIds,
        //string bayIds,
        //int assetClassId,
        //int transType,
        //int inAmount,
        //int roundOff);

        //     Task<IEnumerable<dynamic>> LoadDynComnyDetailedActAsync(
        //string nameSpace,
        //int acId,
        //int yearId,
        //int custId,
        //string locationIds,
        //string divisionIds,
        //string departmentIds,
        //string bayIds,
        //int assetClassId,
        //int transType,
        //int inAmount,
        //int roundOff);


        Task<List<CompanyDetailedActDto>> LoadDynComnyDetailedActAsync(
    string nameSpace,
    int acId,
    int yearId,
    int custId,
    string locationIds,
    string divisionIds,
    string departmentIds,
    string bayIds,
    int assetClassId,
    int transType,
    int inAmount,
    int roundOff);

        //----------
        //Task<DataTable> LoadDynComDetailedReportAsync(
        //     string sNameSpace,
        //     int iACID,
        //     int iYearId,
        //     int iCustId,
        //     string iLocationId,
        //     string iDivId,
        //     string iDeptId,
        //     string iBayId,
        //     int iAsstCls,
        //     int iTransType,
        //     int iInAmt,
        //     int iRoundOff);

    }
}
