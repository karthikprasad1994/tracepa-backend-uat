namespace TracePca.Dto.FixedAssets
{
    public class AssetCreationDto
    {

        //DownloadExcel
        public class AssetMasterResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //LoadAssetClass
        public class AssetTypeDto
        {
            public int AssetTypeId { get; set; }
            public string AssetTypeName { get; set; }
        }

        //New
        public class NewDto
        {
            public string AssetName { get; set; }
            public string AssetCode { get; set; }
            public int CreatedBy { get; set; }
            public int CompId { get; set; }
            public int CustId { get; set; }
        }

        //Search
        public class AssetRegisterRaw
        {
            public int Id { get; set; }
            public int AssetId { get; set; }
            public string? AssetCode { get; set; }
            public string? AssetDescription { get; set; }
            public string? ItemCode { get; set; }
            public string? ItemDescription { get; set; }
            public DateTime? CommissionDate { get; set; }
            public int? Qty { get; set; }
            public int? AssetAge { get; set; }
            public string? CurrentStatus { get; set; }
            public string? TRStatus { get; set; }
            public string? Location { get; set; }
            public string? Division { get; set; }
            public string? Department { get; set; }
            public string? Bay { get; set; }
        }

        public class AssetRegisterDto
        {
            public int SLNo { get; set; }
            public int ID { get; set; }
            public int AssetID { get; set; }
            public string AssetCode { get; set; }
            public string AssetDescription { get; set; }
            public string ItemCode { get; set; }
            public string ItemDescription { get; set; }
            public string DateCommission { get; set; }
            public int Qty { get; set; }
            public int AssetAge { get; set; }
            public string CurrentStatus { get; set; }
            public string TRStatus { get; set; }
            public string Location { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public string Bay { get; set; }
        }

        //LoadUnitsofMeasurement
        public class UnitMeasureDto
        {
            public int Cmm_ID { get; set; }
            public string Cmm_Desc { get; set; }
        }

        //LoadSuplierName
        public class SupplierDto
        {
            public int SUP_ID { get; set; }
            public string SUP_Name { get; set; }
        }

        //EditSuplierName
        public class EditSupplierDetailsDto
        {
            public string SUP_Name { get; set; }
            public string SUP_ContactPerson { get; set; }
            public string SUP_Address { get; set; }
            public string SUP_PhoneNo { get; set; }
            public string SUP_Fax { get; set; }
            public string SUP_Email { get; set; }
            public string SUP_Website { get; set; }
        }

        //SaveSuplierDetails
        public class SaveSupplierDto
        {
            public int SUP_ID { get; set; }
            public string SUP_Name { get; set; }
            public string SUP_Code { get; set; }
            public string SUP_ContactPerson { get; set; }
            public string SUP_Address { get; set; }
            public string SUP_PhoneNo { get; set; }
            public string SUP_Fax { get; set; }
            public string SUP_Email { get; set; }
            public string SUP_Website { get; set; }
            public int SUP_CRBY { get; set; }
            public DateTime SUP_CRON { get; set; }
            public string SUP_STATUS { get; set; }
            public string SUP_IPAddress { get; set; }
            public int SUP_CompID { get; set; }
        }


        //SaveFixedAsset
        // DTO for Fixed Asset
        //public class SaveFixedAssetDto
        //{
        //    public int AFAM_ID { get; set; }
        //    public string AFAM_AssetType { get; set; }
        //    public string AFAM_AssetCode { get; set; }
        //    public string AFAM_Description { get; set; }
        //    public string AFAM_ItemCode { get; set; }
        //    public string AFAM_ItemDescription { get; set; }
        //    public DateTime? AFAM_CommissionDate { get; set; }
        //    public DateTime? AFAM_PurchaseDate { get; set; }
        //    public int AFAM_Quantity { get; set; }
        //    public int AFAM_Unit { get; set; }
        //    public decimal AFAM_AssetAge { get; set; }
        //    public decimal AFAM_PurchaseAmount { get; set; }
        //    public string AFAM_PolicyNo { get; set; }
        //    public decimal AFAM_Amount { get; set; }
        //    public string AFAM_BrokerName { get; set; }
        //    public string AFAM_CompanyName { get; set; }
        //    public DateTime? AFAM_Date { get; set; }
        //    public DateTime? AFAM_ToDate { get; set; }
        //    public int AFAM_Location { get; set; }
        //    public int AFAM_Division { get; set; }
        //    public int AFAM_Department { get; set; }
        //    public int AFAM_Bay { get; set; }
        //    public string AFAM_EmployeeName { get; set; }
        //    public string AFAM_EmployeeCode { get; set; }
        //    public string AFAM_Code { get; set; }
        //    public string AFAM_SuplierName { get; set; }
        //    public string AFAM_ContactPerson { get; set; }
        //    public string AFAM_Address { get; set; }
        //    public string AFAM_Phone { get; set; }
        //    public string AFAM_Fax { get; set; }
        //    public string AFAM_EmailID { get; set; }
        //    public string AFAM_Website { get; set; }
        //    public int AFAM_CreatedBy { get; set; }
        //    public DateTime? AFAM_CreatedOn { get; set; }
        //    public int AFAM_UpdatedBy { get; set; }
        //    public DateTime? AFAM_UpdatedOn { get; set; }
        //    public string AFAM_DelFlag { get; set; }
        //    public string AFAM_Status { get; set; }
        //    public int AFAM_YearID { get; set; }
        //    public int AFAM_CompID { get; set; }
        //    public string AFAM_Opeartion { get; set; }
        //    public string AFAM_IPAddress { get; set; }
        //    public string AFAM_WrntyDesc { get; set; }
        //    public string AFAM_ContactPrsn { get; set; }
        //    public DateTime? AFAM_AMCFrmDate { get; set; }
        //    public DateTime? AFAM_AMCTo { get; set; }
        //    public string AFAM_Contprsn { get; set; }
        //    public string AFAM_PhoneNo { get; set; }
        //    public string AFAM_AMCCompanyName { get; set; }
        //    public int AFAM_AssetDeletion { get; set; }
        //    public DateTime? AFAM_DlnDate { get; set; }
        //    public DateTime? AFAM_DateOfDeletion { get; set; }
        //    public decimal AFAM_Value { get; set; }
        //    public string AFAM_Remark { get; set; }
        //    public string AFAM_EMPCode { get; set; }
        //    public string AFAM_LToWhom { get; set; }
        //    public decimal AFAM_LAmount { get; set; }
        //    public string AFAM_LAggriNo { get; set; }
        //    public DateTime? AFAM_LDate { get; set; }
        //    public int AFAM_LCurrencyType { get; set; }
        //    public DateTime? AFAM_LExchDate { get; set; }
        //    public int AFAM_CustId { get; set; }
        //}


        //// DTO for GRACe Audit Log
        //public class GRACeFormOperationDto
        //{
        //    public int UserId { get; set; }
        //    public string Module { get; set; }
        //    public string Form { get; set; }
        //    public string EventName { get; set; }
        //    public int MasterId { get; set; }
        //    public string MasterName { get; set; }
        //    public int SubMasterId { get; set; }
        //    public string SubMasterName { get; set; }
        //    public string IPAddress { get; set; }
        //    public int CompId { get; set; }
        //}


        //// Wrapper DTO for single POST body
        //public class SaveFixedAssetRequest
        //{
        //    public SaveFixedAssetDto FixedAsset { get; set; }
        //    public GRACeFormOperationDto Audit { get; set; }
        //}


        //-------------------------------

        //public class SaveFixedAssetDto
        //{
        //    public int AFAM_ID { get; set; }
        //    public string AFAM_AssetType { get; set; }
        //    public string AFAM_AssetCode { get; set; }
        //    public string AFAM_Description { get; set; }
        //    public string AFAM_ItemCode { get; set; }
        //    public string AFAM_ItemDescription { get; set; }
        //    public DateTime? AFAM_CommissionDate { get; set; }
        //    public DateTime? AFAM_PurchaseDate { get; set; }
        //    public decimal AFAM_Quantity { get; set; }
        //    public int AFAM_Unit { get; set; }
        //    public int AFAM_AssetAge { get; set; }
        //    public decimal AFAM_PurchaseAmount { get; set; }
        //    public string AFAM_PolicyNo { get; set; }
        //    public decimal AFAM_Amount { get; set; }
        //    public string AFAM_BrokerName { get; set; }
        //    public string AFAM_CompanyName { get; set; }
        //    public DateTime? AFAM_Date { get; set; }
        //    public DateTime? AFAM_ToDate { get; set; }
        //    public int AFAM_Location { get; set; }
        //    public int AFAM_Division { get; set; }
        //    public int AFAM_Department { get; set; }
        //    public int AFAM_Bay { get; set; }
        //    public string AFAM_EmployeeName { get; set; }
        //    public string AFAM_EmployeeCode { get; set; }
        //    public string AFAM_Code { get; set; }
        //    public string AFAM_SuplierName { get; set; }
        //    public string AFAM_ContactPerson { get; set; }
        //    public string AFAM_Address { get; set; }
        //    public string AFAM_Phone { get; set; }
        //    public string AFAM_Fax { get; set; }
        //    public string AFAM_EmailID { get; set; }
        //    public string AFAM_Website { get; set; }
        //    public int AFAM_CreatedBy { get; set; }
        //    public DateTime? AFAM_CreatedOn { get; set; }
        //    public int AFAM_UpdatedBy { get; set; }
        //    public DateTime? AFAM_UpdatedOn { get; set; }
        //    public string AFAM_DelFlag { get; set; }
        //    public string AFAM_Status { get; set; }
        //    public int AFAM_YearID { get; set; }
        //    public int AFAM_CompID { get; set; }
        //    public string AFAM_Opeartion { get; set; }
        //    public string AFAM_IPAddress { get; set; }
        //    public string AFAM_WrntyDesc { get; set; }
        //    public string AFAM_ContactPrsn { get; set; }
        //    public DateTime? AFAM_AMCFrmDate { get; set; }
        //    public DateTime? AFAM_AMCTo { get; set; }
        //    public string AFAM_Contprsn { get; set; }
        //    public string AFAM_PhoneNo { get; set; }
        //    public string AFAM_AMCCompanyName { get; set; }
        //    public int AFAM_AssetDeletion { get; set; }
        //    public DateTime? AFAM_DlnDate { get; set; }
        //    public DateTime? AFAM_DateOfDeletion { get; set; }
        //    public decimal AFAM_Value { get; set; }
        //    public string AFAM_Remark { get; set; }
        //    public string AFAM_EMPCode { get; set; }
        //    public string AFAM_LToWhom { get; set; }
        //    public decimal AFAM_LAmount { get; set; }
        //    public string AFAM_LAggriNo { get; set; }
        //    public DateTime? AFAM_LDate { get; set; }
        //    public int AFAM_LCurrencyType { get; set; }
        //    public DateTime? AFAM_LExchDate { get; set; }
        //    public int AFAM_CustId { get; set; }
        //}

        //public class GRACeFormOperationDto
        //{
        //    public int UserId { get; set; }
        //    public string Module { get; set; }
        //    public string Form { get; set; }
        //    public string EventName { get; set; }
        //    public int MasterId { get; set; }
        //    public string MasterName { get; set; }
        //    public int SubMasterId { get; set; }
        //    public string SubMasterName { get; set; }
        //    public string IPAddress { get; set; }
        //    public int CompId { get; set; }
        //}

        //public class SaveFixedAssetRequestDto
        //{
        //    public SaveFixedAssetDto Asset { get; set; }
        //    public GRACeFormOperationDto Audit { get; set; }
        //}

        //-----------------



        public class FixedAssetDto
        {
            public int AFAM_ID { get; set; }
            public string AFAM_AssetType { get; set; }
            public string AFAM_AssetCode { get; set; }
            public string AFAM_Description { get; set; }
            public string AFAM_ItemCode { get; set; }
            public string AFAM_ItemDescription { get; set; }
            public DateTime? AFAM_CommissionDate { get; set; }
            public DateTime? AFAM_PurchaseDate { get; set; }
            public int AFAM_Quantity { get; set; }
            public int AFAM_Unit { get; set; }
            public decimal AFAM_AssetAge { get; set; }        // money in SQL → decimal
            public decimal AFAM_PurchaseAmount { get; set; }  // money → decimal
            public string AFAM_PolicyNo { get; set; }
            public decimal AFAM_Amount { get; set; }          // money → decimal
            public string AFAM_BrokerName { get; set; }
            public string AFAM_CompanyName { get; set; }
            public DateTime? AFAM_Date { get; set; }
            public DateTime? AFAM_ToDate { get; set; }
            public int AFAM_Location { get; set; }
            public int AFAM_Division { get; set; }
            public int AFAM_Department { get; set; }
            public int AFAM_Bay { get; set; }
            public string AFAM_EmployeeName { get; set; }
            public string AFAM_EmployeeCode { get; set; }
            public string AFAM_Code { get; set; }
            public string AFAM_SuplierName { get; set; }
            public string AFAM_ContactPerson { get; set; }
            public string AFAM_Address { get; set; }
            public string AFAM_Phone { get; set; }
            public string AFAM_Fax { get; set; }
            public string AFAM_EmailID { get; set; }
            public string AFAM_Website { get; set; }
            public int AFAM_CreatedBy { get; set; }
            public DateTime? AFAM_CreatedOn { get; set; }
            public int AFAM_UpdatedBy { get; set; }
            public DateTime? AFAM_UpdatedOn { get; set; }
            public string AFAM_DelFlag { get; set; }
            public string AFAM_Status { get; set; }
            public int AFAM_YearID { get; set; }
            public int AFAM_CompID { get; set; }
            public string AFAM_Opeartion { get; set; }
            public string AFAM_IPAddress { get; set; }
            public string AFAM_WrntyDesc { get; set; }
            public string AFAM_ContactPrsn { get; set; }
            public DateTime? AFAM_AMCFrmDate { get; set; }
            public DateTime? AFAM_AMCTo { get; set; }
            public string AFAM_Contprsn { get; set; }
            public string AFAM_PhoneNo { get; set; }
            public string AFAM_AMCCompanyName { get; set; }
            public int AFAM_AssetDeletion { get; set; }
            public DateTime? AFAM_DlnDate { get; set; }
            public DateTime? AFAM_DateOfDeletion { get; set; }
            public decimal AFAM_Value { get; set; }           // money → decimal
            public string AFAM_Remark { get; set; }
            public string AFAM_EMPCode { get; set; }
            public string AFAM_LToWhom { get; set; }
            public decimal AFAM_LAmount { get; set; }         // money → decimal
            public string AFAM_LAggriNo { get; set; }
            public DateTime? AFAM_LDate { get; set; }
            public int AFAM_LCurrencyType { get; set; }       // int in SQL
            public DateTime? AFAM_LExchDate { get; set; }
            public int AFAM_CustId { get; set; }
            public string AFAM_Attribute1 { get; set; }
            public string AFAM_Attribute2 { get; set; }
            public string AFAM_Attribute3 { get; set; }
        }

        public class AuditDto
        {
            public int UserID { get; set; }
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

        public class FixedAssetRequest
        {
            public FixedAssetDto Asset { get; set; }
            public AuditDto Audit { get; set; }
        }




    }
}
