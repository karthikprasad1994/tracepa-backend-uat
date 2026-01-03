namespace TracePca.Dto.FixedAssets
{
    public class FixedAssetReportDto
    {
        //Report(Ok)
        //public class FixedAssetReportRequestDto
        //{
        //    public int ReportType { get; set; }      // 1 = Company Act, 2 = IT Act
        //    public int CompId { get; set; }
        //    public int CustId { get; set; }
        //    public int YearId { get; set; }
        //    public string FinancialYear { get; set; }
        //    public string LocationIds { get; set; }
        //    public int MethodType { get; set; }
        //}
        //public class FixedAssetReportRowDto
        //{
        //    public string AssetClass { get; set; }
        //    public decimal OpeningWDV { get; set; }
        //    public decimal AdditionsGT180 { get; set; }
        //    public decimal AdditionsLT180 { get; set; }
        //    public decimal Deletions { get; set; }
        //    public decimal Total { get; set; }
        //    public decimal DepRate { get; set; }
        //    public decimal DepForPeriod { get; set; }
        //    public decimal ClosingWDV { get; set; }
        //    public bool IsSubTotal { get; set; }
        //}

        //-----------


        public class FixedAssetReportRequestDto
        {
            public int ReportType { get; set; }      // 1 = Company Act, 2 = IT Act
            public int CompId { get; set; }
            public int CustId { get; set; }
            public int YearId { get; set; }
            public string FinancialYear { get; set; }
            public int LocationIds { get; set; }
            public int MethodType { get; set; }
        }
        public class CompanyActReportRowDto
        {
            public string AssetClass { get; set; }

            // COST
            public decimal CostBegYear { get; set; }
            public decimal Additions { get; set; }
            public decimal Deletions { get; set; }
            public decimal TotalCostEndYear { get; set; }

            // DEPRECIATION
            public decimal DepUptoBegYear { get; set; }
            public decimal DepOnOpening { get; set; }
            public decimal DepOnAdditions { get; set; }
            public decimal DepOnDeletions { get; set; }
            public decimal TotalDepForYear { get; set; }
            public decimal TotalDepEndYear { get; set; }

            // WDV
            public decimal WDVBegYear { get; set; }
            public decimal WDVEndYear { get; set; }

            public bool IsSubTotal { get; set; }
        }

        public class FixedAssetReportRowDto
        {
            public string AssetClass { get; set; }
            public decimal OpeningWDV { get; set; }
            public decimal AdditionsGT180 { get; set; }
            public decimal AdditionsLT180 { get; set; }
            public decimal Deletions { get; set; }
            public decimal Total { get; set; }
            public decimal DepRate { get; set; }
            public decimal DepForPeriod { get; set; }
            public decimal ClosingWDV { get; set; }
            public bool IsSubTotal { get; set; }
        }

        //Report(Go)
        //public class DynCompanyDetailedActRequestDto
        //{
        //    public string NameSpace { get; set; }
        //    public int CompId { get; set; }
        //    public int YearId { get; set; }
        //    public int CustId { get; set; }

        //    // Comma-separated IDs: "1,2,3"
        //    public string LocationIds { get; set; }
        //    public string DivisionIds { get; set; }
        //    public string DepartmentIds { get; set; }
        //    public string BayIds { get; set; }

        //    public int AssetClassId { get; set; }
        //    public int TransType { get; set; }
        //    public int InAmount { get; set; }
        //    public int RoundOff { get; set; }
        //}
        //public class DynCompanyDetailedActResponseDto
        //{
        //    public string AssetClass { get; set; }
        //    public string Asset { get; set; }
        //    public string AssetCode { get; set; }

        //    public string Costasat { get; set; }
        //    public string Additions { get; set; }
        //    public string Deletions { get; set; }
        //    public string TotalAmount { get; set; }

        //    public string DepUptoPY { get; set; }
        //    public string DepOnOpengBal { get; set; }
        //    public string DepOnAdditions { get; set; }
        //    public string DepOnDeletions { get; set; }

        //    public string TotalDepFY { get; set; }
        //    public string TotalDepasOn { get; set; }

        //    public string WDVasOn { get; set; }
        //    public string WDVasOnPY { get; set; }
        //}

        //--------------

        //public class AssetDetailDto
        //{
        //    public string AssetClass { get; set; }
        //    public string Asset { get; set; }
        //    public string AssetCode { get; set; }
        //    public double Costasat { get; set; }
        //    public double Additions { get; set; }
        //    public double Deletions { get; set; }
        //    public double TotalAmount { get; set; }
        //    public double DepUptoPY { get; set; }
        //    public double DepOnOpengBal { get; set; }
        //    public double DepOnAdditions { get; set; }
        //    public double DepOnDeletions { get; set; }
        //    public double TotalDepFY { get; set; }
        //   // public double TotalDepasOn { get; set; }
        //    public double WDVasOn { get; set; }
        //    public double WDVasOnPY { get; set; }
        //}

        public class CompanyDetailedActDto
        {
            public string AssetClass { get; set; }
            public string Asset { get; set; }
            public string AssetCode { get; set; }

            public decimal Costasat { get; set; }
            public decimal Additions { get; set; }
            public decimal Deletions { get; set; }
            public decimal TotalAmount { get; set; }

            public decimal DepUptoPY { get; set; }
            public decimal DepOnOpengBal { get; set; }
            public decimal DepOnAdditions { get; set; }
            public decimal DepOnDeletions { get; set; }

            public decimal TotalDepFY { get; set; }
            public decimal TotalDepasOn { get; set; }

            public decimal WDVasOn { get; set; }
            public decimal WDVasOnPY { get; set; }
        }

        //---
        //public class AssetClassDto
        //{
        //    public int AM_ID { get; set; }
        //    public string AM_Description { get; set; }
        //}

        //public class AssetDto
        //{
        //    public int AFAM_ID { get; set; }
        //    public string AFAM_ItemDescription { get; set; }
        //    public string AFAM_AssetCode { get; set; }
        //}

        //public class AssetItemDto
        //{
        //    public double? OrgCost { get; set; }
        //    public double? AddAmt { get; set; }
        //    public double? DelAmt { get; set; }

        //    public int? AFAD_AssetDeletion { get; set; }

        //    public double? AFAA_DepreAmount { get; set; }
        //    public int? ADep_TransType { get; set; }
        //    public double? ADep_DepreciationforFY { get; set; }
        //    public double? AFAD_DelDeprec { get; set; }

        //    public int? AFAA_Location { get; set; }
        //    public int? AFAA_Division { get; set; }
        //    public int? AFAA_Department { get; set; }
        //    public int? AFAA_Bay { get; set; }
        //}

        //public class YearDto
        //{
        //    public DateTime YMSFROMDATE { get; set; }
        //    public DateTime YMSTODATE { get; set; }
        //    public DateTime YMS_TODATE { get; set; }
        //    public int YMSID { get; set; }
        //}


    }
}
