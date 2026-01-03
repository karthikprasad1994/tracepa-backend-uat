using TracePca.Dto.Audit;

namespace TracePca.Dto.FixedAssets
{
    public class AssetTransactionAdditionDto
    {
        ////LoadCustomer
        //public class CustDto
        //{
        //    public int Cust_Id { get; set; }
        //    public string Cust_Name { get; set; }
        //}

        ////LoadStstus
        //public class StatusDto
        //{
        //    public string Status { get; set; }
        //}


        ////FinancialYear
        //public class YearDto
        //{
        //    public int YMS_YEARID { get; set; }
        //    public string YMS_ID { get; set; }   
        //}

        //AddDetails
        //public class AddAssetDetailsRequest
        //{
        //    public int TransactionTypeId { get; set; }
        //    public int AssetClassId { get; set; }
        //    public int AssetId { get; set; }

        //    public string SupplierName { get; set; }
        //    public string Particulars { get; set; }
        //    public string DocNo { get; set; }
        //    public string DocDate { get; set; }
        //    public decimal BasicCost { get; set; }
        //    public decimal TaxAmount { get; set; }
        //    public decimal Total { get; set; }
        //    public decimal AssetValue { get; set; }
        //}

        //public class AssetAdditionDetailsDto
        //{
        //    public int TypeId { get; set; }
        //    public string Type { get; set; }
        //    public int PKID { get; set; }
        //    public int MasID { get; set; }

        //    public string SupplierName { get; set; }
        //    public string Particulars { get; set; }
        //    public string DocNo { get; set; }
        //    public string DocDate { get; set; }

        //    public decimal BasicCost { get; set; }
        //    public decimal TaxAmount { get; set; }
        //    public decimal Total { get; set; }
        //    public decimal AssetValue { get; set; }
        //}

        public class AssettTypeDto
        {
            public int AssetTypeId { get; set; }
            public string AssetTypeName { get; set; }
        }


        //SaveTransactionAddition
        public class ClsAssetTransactionAdditionDto
        {
            public int AFAA_ID { get; set; }
            public int AFAA_AssetTrType { get; set; }
            public int AFAA_CurrencyType { get; set; }
            public decimal AFAA_CurrencyAmnt { get; set; }

            public int AFAA_Location { get; set; }
            public int AFAA_Division { get; set; }
            public int AFAA_Department { get; set; }
            public int AFAA_Bay { get; set; }

            public string AFAA_ActualLocn { get; set; }

            // 🔥 FIXED: INT instead of STRING
            public int AFAA_SupplierName { get; set; }
            public int AFAA_SupplierCode { get; set; }

            public int AFAA_TrType { get; set; }
            public string AFAA_AssetType { get; set; }
            public string AFAA_AssetNo { get; set; }
            public string AFAA_AssetRefNo { get; set; }
            public string AFAA_Description { get; set; }
            public string AFAA_ItemCode { get; set; }
            public string AFAA_ItemDescription { get; set; }

            public int AFAA_Quantity { get; set; }
            public DateTime? AFAA_CommissionDate { get; set; }
            public DateTime? AFAA_PurchaseDate { get; set; }

            public decimal AFAA_AssetAge { get; set; }
            public decimal AFAA_AssetAmount { get; set; }
            public decimal AFAA_FYAmount { get; set; }
            public decimal AFAA_DepreAmount { get; set; }

            public int AFAA_AssetDelID { get; set; }
            public DateTime? AFAA_AssetDelDate { get; set; }
            public DateTime? AFAA_AssetDeletionDate { get; set; }

            public decimal AFAA_Assetvalue { get; set; }
            public string AFAA_AssetDesc { get; set; }

            public int AFAA_CreatedBy { get; set; }
            public DateTime AFAA_CreatedOn { get; set; }

            public int? AFAA_UpdatedBy { get; set; }
            public DateTime? AFAA_UpdatedOn { get; set; }

            public string AFAA_Status { get; set; }      // varchar(25)
            public string AFAA_Delflag { get; set; }     // char(1) → "Y"/"N"

            public int AFAA_YearID { get; set; }
            public int AFAA_CompID { get; set; }

            public string AFAA_Operation { get; set; }   // varchar(1) → "C"/"U"
            public string AFAA_IPAddress { get; set; }

            public string AFAA_AddnType { get; set; }    // varchar(5)
            public string AFAA_DelnType { get; set; }    // varchar(5)

            public decimal AFAA_Depreciation { get; set; }
            public DateTime? AFAA_AddtnDate { get; set; }

            public int? AFAA_ApprovedBy { get; set; }
            public DateTime? AFAA_ApprovedOn { get; set; }

            public int AFAA_ItemType { get; set; }
            public int AFAA_CustId { get; set; }
        }

        public class TransactionAuditDto
        {
            public int UserId { get; set; }
            public string Module { get; set; }
            public string Form { get; set; }
            public string Event { get; set; }
             public int MasterID { get; set; }
            public string MasterName { get; set; }
            public int SubMasterID { get; set; }
            public string SubMasterName { get; set; }
            public string IPAddress { get; set; }
            public int CompID { get; set; }
        }
        public class SaveAssetRequest
        {
            public ClsAssetTransactionAdditionDto Asset { get; set; }
            public TransactionAuditDto Audit { get; set; }
        }

        //LoadVoucherNo(transactionno)
        public class AssetTransactionDto
        {
            public int AssetId { get; set; }
            public string AssetNo { get; set; }
        }

        //ExcelUpload
        public class AssetAdditionResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //SaveDetails
        public class ClsAssetOpeningBalExcelUpload
        {
            public int AFAA_ID { get; set; }

            public int AFAA_AssetTrType { get; set; }        // kept for completeness
            public int AFAA_CurrencyType { get; set; }
            public decimal AFAA_CurrencyAmnt { get; set; }

            public int AFAA_Location { get; set; }
            public int AFAA_Division { get; set; }
            public int AFAA_Department { get; set; }
            public int AFAA_Bay { get; set; }

            public string AFAA_ActualLocn { get; set; }

            public int AFAA_SupplierName { get; set; }
            public int AFAA_SupplierCode { get; set; }

            public int AFAA_TrType { get; set; }              // 🔑 CONDITION DRIVER

            public int AFAA_AssetType { get; set; }

            public string AFAA_AssetNo { get; set; }
            public string AFAA_AssetRefNo { get; set; }

            public string AFAA_Description { get; set; }

            public int AFAA_ItemType { get; set; }            // used as ItemCode

            public string AFAA_ItemDescription { get; set; }

            public int AFAA_Quantity { get; set; }

            public DateTime AFAA_CommissionDate { get; set; }
            public DateTime AFAA_PurchaseDate { get; set; }

            public decimal AFAA_AssetAge { get; set; }

            public decimal AFAA_AssetAmount { get; set; }
            public decimal AFAA_FYAmount { get; set; }
            public decimal AFAA_DepreAmount { get; set; }

            public int AFAA_AssetDelID { get; set; }
            public DateTime AFAA_AssetDelDate { get; set; }
            public DateTime AFAA_AssetDeletionDate { get; set; }

            public decimal AFAA_Assetvalue { get; set; }

            public string AFAA_AssetDesc { get; set; }

            public int AFAA_CreatedBy { get; set; }
            public DateTime AFAA_CreatedOn { get; set; }

            public int AFAA_UpdatedBy { get; set; }
            public DateTime AFAA_UpdatedOn { get; set; }

            public string AFAA_Status { get; set; }           // A / D
            public string AFAA_Delflag { get; set; }          // A / D

            public int AFAA_YearID { get; set; }
            public int AFAA_CompID { get; set; }

            public string AFAA_Operation { get; set; }        // SAVE / UPDATE
            public string AFAA_IPAddress { get; set; }

            public string AFAA_AddnType { get; set; }
            public string AFAA_DelnType { get; set; }

            public decimal AFAA_Depreciation { get; set; }

            public DateTime AFAA_AddtnDate { get; set; }

            public int AFAA_ApprovedBy { get; set; }
            public DateTime AFAA_ApprovedOn { get; set; }

            public int AFAA_CustId { get; set; }
        }
        public class ClsAssetTransactionAddition
        {
            public int FAAD_PKID { get; set; }
            public int FAAD_MasID { get; set; }
            public int FAAD_Location { get; set; }
            public int FAAD_Division { get; set; }
            public int FAAD_Department { get; set; }
            public int FAAD_Bay { get; set; }
            public string FAAD_Particulars { get; set; }
            public string FAAD_DocNo { get; set; }
            public DateTime FAAD_DocDate { get; set; }
            public bool FAAD_chkCost { get; set; }
            public decimal FAAD_BasicCost { get; set; }
            public decimal FAAD_TaxAmount { get; set; }
            public decimal FAAD_Total { get; set; }
            public decimal FAAD_AssetValue { get; set; }
            public int FAAD_CreatedBy { get; set; }
            public DateTime FAAD_CreatedOn { get; set; }
            public int FAAD_UpdatedBy { get; set; }
            public DateTime FAAD_UpdatedOn { get; set; }
            public string FAAD_IPAddress { get; set; }
            public int FAAD_CompID { get; set; }
            public string FAAD_Status { get; set; }
            public int FAAD_AssetType { get; set; }
            public int FAAD_ItemType { get; set; }
            public int FAAD_SupplierName { get; set; }
            public int FAAD_CustId { get; set; }
            public string sFAAD_Delflag { get; set; }
            public int FAAD_YearID { get; set; }
            public decimal iFAAD_InitDep { get; set; }
            public int iFAAD_OtherTrType { get; set; }
            public decimal sFAAD_OtherTrAmount { get; set; }
        }
        public class AuditLogDto
        {
            public int ALFO_UserID { get; set; }
            public string ALFO_Module { get; set; }
            public string ALFO_Form { get; set; }
            public string ALFO_Event { get; set; }
            public int ALFO_MasterID { get; set; }
            public string ALFO_MasterName { get; set; }
            public int ALFO_SubMasterID { get; set; }
            public string ALFO_SubMasterName { get; set; }
            public string ALFO_IPAddress { get; set; }
            public int ALFO_CompID { get; set; }
        }
        public class SaveFixedAssetRequestDto
        {
            public ClsAssetOpeningBalExcelUpload Header { get; set; }
            public ClsAssetTransactionAddition Details { get; set; }
            public AuditLogDto Audit { get; set; }
        }


    }
}
