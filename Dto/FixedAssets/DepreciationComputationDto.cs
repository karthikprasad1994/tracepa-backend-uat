using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
namespace TracePca.Dto.FixedAssets
{
    public class DepreciationComputationDto
    {

        //DepreciationBasis


        //MethodofDepreciation

        //SaveDepreciation


        public class DepreciationComputationnDto
        {
            public int ADep_ID { get; set; }
            public int AssetId { get; set; }
            public string Item { get; set; }

            public decimal RateOfDep { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal DepreciationForFY { get; set; }
            public decimal WDV { get; set; }

            public string DelFlag { get; set; }     // Y / N
            public string Status { get; set; }      // A / I

            public int LocationId { get; set; }
            public int DivisionId { get; set; }
            public int DepartmentId { get; set; }
            public int BayId { get; set; }

            public int TransType { get; set; }            // ADD / DEL
            public string Operation { get; set; }   // SAVE / UPDATE
        }
        public class DepreciationITActDto
        {
            public int Id { get; set; }
            public int AssetClassId { get; set; }

            public decimal Rate { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal Depreciation { get; set; }
            public decimal WDV { get; set; }

            public decimal BeforeQuarterAmount { get; set; }
            public decimal BeforeQuarterDep { get; set; }
            public decimal AfterQuarterAmount { get; set; }
            public decimal AfterQuarterDep { get; set; }

            public decimal DeletionAmount { get; set; }
            public decimal InitAmount { get; set; }

            public string DelFlag { get; set; }
            public string Status { get; set; }
            public string Operation { get; set; }
        }
        public class AuditLoggDto
        {
            public int UserId { get; set; }
            public string Module { get; set; }
            public string Form { get; set; }
            public string Event { get; set; }

            public int MasterId { get; set; }
            public string MasterName { get; set; }

            public int SubMasterId { get; set; }
            public string SubMasterName { get; set; }

            public string IPAddress { get; set; }
        }
        public class SaveDepreciationRequestDto
        {
            public int DepBasis { get; set; }
            public int YearId { get; set; }
            public int CompId { get; set; }
            public int CustId { get; set; }
            public int Method { get; set; }
            public int UserId { get; set; }
            public string IpAddress { get; set; }

            public List<DepreciationComputationnDto> NormalList { get; set; }
            public List<DepreciationITActDto> ItActList { get; set; }

            public AuditLoggDto Audit { get; set; }
        }


        //Report(DownloadExcel)
        public class AssetDepreciationResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //Go
        //itcorrect
        public class DepreciationnITActDto
        {
            public int AssetClassID { get; set; }
            public string ClassofAsset { get; set; } = string.Empty;
            public double RateofDep { get; set; }
            public double WDVOpeningValue { get; set; }
            public double WDVOpeningDepreciation { get; set; }
            public double BfrQtrAmount { get; set; }
            public double BfrQtrDep { get; set; }
            public double AftQtrAmount { get; set; }
            public double AftQtrDep { get; set; }
            public double DelAmount { get; set; }
            public double InitDepAmt { get; set; }
            public double PrevInitDepAmt { get; set; }
            public double Depfortheperiod { get; set; }
            public double AdditionDuringtheYear { get; set; }
            public double WDVClosingValue { get; set; }
            public double NextYrCarry { get; set; }
        }

        public class AssetMasterDto
        {
            public int AM_ID { get; set; }
            public string AM_Description { get; set; } = string.Empty;
            public double AM_WDVITAct { get; set; }
            public double AM_ITRate { get; set; }
        }

        public class FixedAssetAdditionDto
        {
            public int FAAD_ItemType { get; set; }
            public DateTime AFAM_CommissionDate { get; set; }
            public double FAAD_AssetValue { get; set; }
            public int FAAD_InitDep { get; set; }
        }

        //--------------


        //public class DepreciationCompSlmDto
        //{
        //    public int AssetClassID { get; set; }
        //    public int AssetID { get; set; }
        //    public string PurchaseDate { get; set; }
        //    public int NoOfDays { get; set; }
        //    public decimal OrignalCost { get; set; }
        //    public decimal Rsdulvalue { get; set; }
        //    public decimal SalvageValue { get; set; }
        //    public decimal OPBForYR { get; set; }
        //    public decimal DepreciationforFY { get; set; }
        //    public decimal WrtnValue { get; set; }
        //    public string TrType { get; set; }
        //}

        //-------------------------new it correct


        public class DepreciationRequesttDto
        {
           
            public string AccessCode { get; set; }
            public int CompId { get; set; }
            public int YearId { get; set; }
            public int CustId { get; set; }
            public int DepBasis { get; set; } // 1=Company, 2=IT
            public int Method { get; set; }   // 1=SLM, 2=WDV
            public int DurationType { get; set; }
            public int HalfYear { get; set; }
            public int Quarter { get; set; }
            public int Month { get; set; }
            public string FinancialYear { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string SNameSpace { get; set; }
            public int NoOfDays { get; internal set; }
            public int TotalNoOfDays { get; internal set; }
            public int Duration { get; internal set; }
        }

        public class DepreciationResultDto
        {
            public string DepreciationBasis { get; set; }
            public List<ITActDepreciationDto> ITActData { get; set; }
            public List<CompanyActDepreciationDto> CompanyActData { get; set; }
            //public List<DepreciationDto> CompanySLMData { get; internal set; }
        }

        public class ITActDepreciationDto
        {
            public int AssetClassID { get; set; }
            public string ClassOfAsset { get; set; }
            public decimal RateOfDep { get; set; }
            public decimal WDVOpeningValue { get; set; }
            public decimal Before180DaysAddition { get; set; }
            public decimal After180DaysAddition { get; set; }
            public decimal TotalAddition { get; set; }
            public decimal DepBefore180Days { get; set; }
            public decimal DepAfter180Days { get; set; }
            public decimal TotalDepreciation { get; set; }
            public decimal WDVClosingValue { get; set; }
        }

        public class CompanyActDepreciationDto
        {
            public DateTime? DateOfPutToUse { get; set; }
            public string AssetCode { get; set; }
            public string AssetName { get; set; }
            public decimal OriginalCost { get; set; }
            public decimal AssetLife { get; set; }
            public decimal ResidualPercent { get; set; }
            public string Location { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public string Bay { get; set; }
            public decimal DepreciationAmount { get; set; }
            public decimal ClosingValue { get; set; }
        }

        //-------wdv
        public class CompanyActWDVRequestDto
        {
            public int CompId { get; set; }
            public int YearId { get; set; }
            public int CustId { get; set; }

            public int NoOfDays { get; set; }
            public int TotalDays { get; set; }
            public int Duration { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public int Method { get; set; }
        }
        public class CompanyActWDVResponseDto
        {
            public int AssetClassID { get; set; }
            public int AssetID { get; set; }
            public string AssetType { get; set; }
            public string AssetCode { get; set; }

            public DateTime? PurchaseDate { get; set; }
            public int AssetAge { get; set; }

            public decimal OriginalCost { get; set; }
            public decimal ResidualPercent { get; set; }
            public decimal SalvageValue { get; set; }

            public decimal DepreciationRate { get; set; }
            public decimal OpeningWDV { get; set; }
            public decimal DepreciationForFY { get; set; }
            public decimal ClosingWDV { get; set; }

            public int NoOfDays { get; set; }
            public int TrType { get; set; }

            public int LocationID { get; set; }
            public int DivisionID { get; set; }
            public int DepartmentID { get; set; }
            public int BayID { get; set; }
        }

        //---------slnwdv
        public class DepreciationRequestDto
        {
            public int CompId { get; set; }
            public int YearId { get; set; }
            public int CustId { get; set; }
            public int NoOfDays { get; set; }
            public int TotalDays { get; set; }
            public int Duration { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Method { get; set; } // 1 = SLM, 2 = WDV
        }

        public class DepreciationResulttDto
        {
            public int AssetClassID { get; set; }
            public int AssetID { get; set; }
            public string AssetType { get; set; }
            public string AssetCode { get; set; }
            public DateTime PurchaseDate { get; set; }
            public int AssetAge { get; set; }
            public decimal OriginalCost { get; set; }
            public decimal ResidualPercent { get; set; }
            public decimal SalvageValue { get; set; }
            public decimal DepreciationRate { get; set; }
            public decimal OpeningWDV { get; set; }
            public decimal DepreciationForFY { get; set; }
            public decimal ClosingWDV { get; set; }
            public int NoOfDays { get; set; }
            public int TrType { get; set; }
            public int LocationID { get; set; }
            public int DivisionID { get; set; }
            public int DepartmentID { get; set; }
            public int BayID { get; set; }
        }

        //----------final
        public class DepreciatiionRequestDto
        {
            public int DepBasis { get; set; }   // ✅ 1 = Company Act, 2 = IT Act

            public int CompId { get; set; }
            public int YearId { get; set; }
            public int CustId { get; set; }

            // Company Act only
            public int NoOfDays { get; set; }
            public int TotalDays { get; set; }
            public int Duration { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Method { get; set; }     // 1 = SLM, 2 = WDV
        }
        public class DepreciationnResultDto
        {
            public int AssetClassID { get; set; }
            public int AssetID { get; set; }
            public string AssetType { get; set; } = string.Empty;
            public string AssetCode { get; set; } = string.Empty;
            public DateTime PurchaseDate { get; set; }
            public int AssetAge { get; set; }

            public decimal OriginalCost { get; set; }
            public decimal ResidualPercent { get; set; }
            public decimal SalvageValue { get; set; }
            public decimal DepreciationRate { get; set; }

            public decimal OpeningWDV { get; set; }
            public decimal DepreciationForFY { get; set; }
            public decimal ClosingWDV { get; set; }

            public int NoOfDays { get; set; }
        }
        public class DepreciatioITActDto
        {
            public int AssetClassID { get; set; }
            public string ClassofAsset { get; set; } = string.Empty;

            public double RateofDep { get; set; }
            public double WDVOpeningValue { get; set; }
            public double WDVOpeningDepreciation { get; set; }

            public double BfrQtrAmount { get; set; }
            public double BfrQtrDep { get; set; }
            public double AftQtrAmount { get; set; }
            public double AftQtrDep { get; set; }

            public double DelAmount { get; set; }
            public double InitDepAmt { get; set; }
            public double PrevInitDepAmt { get; set; }

            public double Depfortheperiod { get; set; }
            public double AdditionDuringtheYear { get; set; }
            public double WDVClosingValue { get; set; }
            public double NextYrCarry { get; set; }
        }

        //company calculation
        public class ITDepreciationRequestDto
        {
            public string NameSpace { get; set; } // Database schema / namespace
            public int CompId { get; set; }
            public int YearId { get; set; }
            public int CustId { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class ITDepreciationResponseDto
        {
            public int AssetClassID { get; set; }
            public string ClassofAsset { get; set; }
            public double RateofDep { get; set; }
            public double BfrQtrAmount { get; set; }
            public double BfrQtrDep { get; set; }
            public double AftQtrAmount { get; set; }
            public double AftQtrDep { get; set; }
            public double DelAmount { get; set; }
            public double WDVOpeningValue { get; set; }
            public double WDVOpeningDepreciation { get; set; }
            public double AdditionDuringtheYear { get; set; }
            public double AdditionDepreciation { get; set; }
            public double Depfortheperiod { get; set; }
            public double InitDepAmt { get; set; }
            public double PrevInitDepAmt { get; set; }
            public double WDVClosingValue { get; set; }
            public double NextYrCarry { get; set; }
        }









    }
}
