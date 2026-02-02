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

        //Generete
        public class GenerateAssetCodeRequestDto
        {
            public int CompId { get; set; }
            public int CustId { get; set; }

            public int LocationId { get; set; }
            public int DivisionId { get; set; }
            public int DepartmentId { get; set; }
            public int BayId { get; set; }

            public string AssetCode { get; set; }
        }
        public class GenerateAssetCodeResponseDto
        {
            public int StatusCode { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public string Data { get; set; }
        }

        //UploadAssetCreationExcel
        public class UploadAssetCreationRequestDto
        {
            public int CompId { get; set; }
            public int CustId { get; set; }
            public int YearId { get; set; }
            public int UserId { get; set; }
            public IFormFile File { get; set; }
        }


        public class UploadAssetCreationExcelDto
        {
            // 🔹 Mandatory Fields (RED in Excel)
            public string Location { get; set; }
            public string AssetClass { get; set; }
            public string AssetDescription { get; set; }
            public int? Quantity { get; set; }
            public DateTime? PutToUseDate { get; set; }
            public string UnitOfMeasurement { get; set; }
            public decimal? UsefulLife { get; set; }
            public string QuantityRaw { get; set; }
            public string PutToUseDateRaw { get; set; }
            public string UsefulLifeRaw { get; set; }

            // 🔹 Optional Fields (BLACK in Excel)
            public string Division { get; set; }
            public string Department { get; set; }
            public string Bay { get; set; }
            public string AssetCode { get; set; }

            // 🔹 Extra fields required by SP but not in Excel
            public int? AssetTypeId { get; set; }
            public int? UnitId { get; set; }
            public int? LocationId { get; set; }
            public int? DivisionId { get; set; }
            public int? DepartmentId { get; set; }
            public int? BayId { get; set; }

            public DateTime? PurchaseDate { get; set; }
            //  public decimal? PurchaseAmount { get; set; }
            public string Status { get; set; }
            public string DelFlag { get; set; }

            // 🔹 Metadata
            public int? CompId { get; set; }
            public int? CustId { get; set; }
            public int? YearId { get; set; }
            public int? CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            public string IPAddress { get; set; }
            public string Operation { get; set; } = "I"; // I = Insert, U = Update
            public decimal? PurchaseAmount { get; set; }
            public decimal? AssetAge { get; set; }
            public decimal? Amount { get; set; }

            // 🔹 Validation
            public string ErrorMessage { get; set; }
        }


        // Response wrapper
        public class UploadAssetCreationResponseDto
        {
            public UploadAssetCreationExcelDto asset { get; set; }
            public UploadAssetCreationRequestDto request { get; set; }
            public List<UploadAssetCreationExcelDto> validation { get; set; } = new();
            public List<UploadAssetCreationExcelDto> assets { get; set; } = new();

        }


    }
}
