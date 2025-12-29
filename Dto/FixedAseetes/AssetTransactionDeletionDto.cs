using TracePca.Dto.Audit;

namespace TracePca.Dto.FixedAssets
{
    public class AssetTransactionDeletionnDto
    {
        //Deletee
        public class AssetDeletionDto
        {
            public int AFAD_ID { get; set; }
            public int AFAD_CustomerName { get; set; }
            public string? AFAD_TransNo { get; set; }
            public int? AFAD_Location { get; set; }
            public int? AFAD_Division { get; set; }
            public int? AFAD_Department { get; set; }
            public int? AFAD_Bay { get; set; }

            public int AFAD_AssetClass { get; set; }
            public int AFAD_Asset { get; set; }
            public int AFAD_AssetDeletion { get; set; }
            public int? AFAD_AssetDeletionType { get; set; }

            public DateTime? AFAD_DeletionDate { get; set; }
            public decimal? AFAD_Amount { get; set; }
            public decimal? AFAD_Quantity { get; set; }
            public int? AFAD_Paymenttype { get; set; }
            public decimal? AFAD_CostofTransport { get; set; }
            public decimal? AFAD_InstallationCost { get; set; }

            public DateTime? AFAD_DateofInitiate { get; set; }
            public DateTime? AFAD_DateofReceived { get; set; }

            public int? AFAD_ToLocation { get; set; }
            public int? AFAD_ToDivision { get; set; }
            public int? AFAD_ToDepartment { get; set; }
            public int? AFAD_ToBay { get; set; }

            public string? AFAD_AssetDelDesc { get; set; }

            public string? AFAD_PorLStatus { get; set; }
            public decimal? AFAD_PorLAmount { get; set; }
            public decimal? AFAD_SalesPrice { get; set; }
            public decimal? AFAD_DelDeprec { get; set; }
            public decimal? AFAD_WDVValue { get; set; }

            public decimal? AFAD_ContAssetValue { get; set; }
            public decimal? AFAD_ContDep { get; set; }
            public decimal? AFAD_ContWDV { get; set; }

            public string? AFAD_InsClaimedNo { get; set; }
            public decimal? AFAD_InsAmtClaimed { get; set; }
            public DateTime? AFAD_InsClaimedDate { get; set; }
            public decimal? AFAD_InsAmtRecvd { get; set; }
            public string? AFAD_InsRefNo { get; set; }
            public DateTime? AFAD_InsRefDate { get; set; }

            public string? AFAD_Remarks { get; set; }

            public int AFAD_CreatedBy { get; set; }
            public DateTime? AFAD_CreatedOn { get; set; }

            public int? AFAD_ApprovedBy { get; set; }
            public DateTime? AFAD_ApprovedOn { get; set; }

            public string? AFAD_Status { get; set; }
            public string? AFAD_Delflag { get; set; }

            public int AFAD_YearID { get; set; }
            public int AFAD_CompID { get; set; }

            public int? AFAD_Deletedby { get; set; }
            public DateTime? AFAD_DeletedOn { get; set; }

            public string? AFAD_IPAddress { get; set; }
        }

        public class AaudittDto
        {
            public int ALFO_UserID { get; set; }
            public string ALFO_Module { get; set; } = string.Empty;
            public string ALFO_Form { get; set; } = string.Empty;
            public string ALFO_Event { get; set; } = string.Empty;
            public int ALFO_MasterID { get; set; }
            public string ALFO_MasterName { get; set; } = string.Empty;
            public int ALFO_SubMasterID { get; set; }
            public string ALFO_SubMasterName { get; set; } = string.Empty;
            public string ALFO_IPAddress { get; set; } = string.Empty;
            public int ALFO_CompID { get; set; }
        }

        public class AssetDeletionnRequest
        {
            public AssetDeletionDto AssetDeletion { get; set; } = new();
            public AaudittDto Audit { get; set; } = new();
        }



    }
}
