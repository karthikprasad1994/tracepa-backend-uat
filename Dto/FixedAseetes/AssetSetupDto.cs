namespace TracePca.Dto.FixedAssets
{
    public class AssetSetupDto
    {
        ////Location
        //public class LocationDto
        //{
        //    public int LocationId { get; set; }
        //    public string LocationName { get; set; }
        //}

        ////LoadDivision
        //public class DivisionDto
        //{
        //    public int DivisionId { get; set; }
        //    public string DivisionName { get; set; }
        //}

        ////LoadDepartment
        //public class DepartmentDto
        //{
        //    public int DepartmentId { get; set; }
        //    public string DepartmentName { get; set; }
        //}
        ////LoadBay
        //public class BayDto
        //{
        //    public int BayiId { get; set; }
        //    public string BayiName { get; set; }
        //}

        ////LoadHeading
        //public class HeadingDto
        //{
        //    public int HeadingId { get; set; }
        //    public string HeadingName { get; set; }
        //}


        ////LoadSubHeading
        //public class SubHeadingDto
        //{
        //    public int SubHeadingId { get; set; }
        //    public string SubHeadingName { get; set; }
        //}

        ////AssetClassUnderSubHeading
        //public class ItemDto
        //{
        //    public int ItemId { get; set; }
        //    public string ItemName { get; set; }
        //}

        //SaveAsset
        public class AssetMasterDto
        {
            public int AM_ID { get; set; }
            public string AM_Description { get; set; }
            public string AM_Code { get; set; }
            public int AM_LevelCode { get; set; }
            public int AM_ParentID { get; set; }
            public decimal AM_WDVITAct { get; set; }
            public decimal AM_ITRate { get; set; }
            public decimal AM_ResidualValue { get; set; }
            public int AM_CreatedBy { get; set; }
            public DateTime AM_CreatedOn { get; set; }
            public int AM_UpdatedBy { get; set; }
            public DateTime AM_UpdatedOn { get; set; }
            public string AM_DelFlag { get; set; }
            public string AM_Status { get; set; }
            public int AM_YearID { get; set; }
            public int AM_CompID { get; set; }
            public int AM_CustId { get; set; }
            public int AM_ApprovedBy { get; set; }
            public DateTime AM_ApprovedOn { get; set; }
            public string AM_Opeartion { get; set; }
            public string AM_IPAddress { get; set; }
        }

        // //EditLocation
        // public class LocationEditDto
        // {
        //     public int LocationId { get; set; }
        //     public string LocationName { get; set; }
        //     public string LocationCode { get; set; }
        // }

        // //EditDivision
        // public class DivisionEditDto
        // {
        //     public int DivisionId { get; set; }
        //     public string DivisionName { get; set; }
        //     public string DivisionCode { get; set; }
        // }


        // //EditDepartment
        // public class DepartmentEditDto
        // {
        //     public int DepartmentId { get; set; }
        //     public string DepartmentName { get; set; }
        //     public string DepartmentCode { get; set; }
        // }

        // //EditBay
        // public class BayEditDto
        // {
        //     public int BayId { get; set; }
        //     public string BayName { get; set; }
        //     public string BayCode { get; set; }
        // }

        // //EditHeading
        // public class HeadingEditDto
        // {
        //     public int HeadingId { get; set; }
        //     public string HeadingName { get; set; }
        // }

        // //EditSubHeading
        // public class SubHeadingEditDto
        // {
        //     public int SubHeadingId { get; set; }
        //     public string SubHeadingName { get; set; }
        // }

        // //EditAssetClassUnderSubHeading(LoadItem)
        // public class ItemEditDto
        // {
        //     public int ItemId { get; set; }
        //     public string ItemName { get; set; }
        // }

        //SaveLocation
        public class LocationSetupDto
        {
            public int LS_ID { get; set; }
            public string LS_Description { get; set; }
            public string LS_DescCode { get; set; }
            public string LS_Code { get; set; }
            public int LS_LevelCode { get; set; }
            public int LS_ParentID { get; set; }
            public int LS_CreatedBy { get; set; }
            public DateTime LS_CreatedOn { get; set; }
            public int LS_UpdatedBy { get; set; }
            public DateTime LS_UpdatedOn { get; set; }
            public string LS_DelFlag { get; set; }
            public string LS_Status { get; set; }
            public int LS_YearID { get; set; }
            public int LS_CompID { get; set; }
            public int LS_CustId { get; set; }
            public int LS_ApprovedBy { get; set; }
            public DateTime LS_ApprovedOn { get; set; }
            public string LS_Opeartion { get; set; }
            public string LS_IPAddress { get; set; }
        }

        //SaveDivision
        public class SaveDivisionDto
        {
            public int LS_ID { get; set; }
            public string LS_Description { get; set; }
            public string LS_DescCode { get; set; }
            public string LS_Code { get; set; }
            public int LS_LevelCode { get; set; }
            public int LS_ParentID { get; set; }
            public int LS_CreatedBy { get; set; }
            public DateTime LS_CreatedOn { get; set; }
            public int LS_UpdatedBy { get; set; }
            public DateTime LS_UpdatedOn { get; set; }
            public string LS_DelFlag { get; set; }        // could be 0/1
            public string LS_Status { get; set; }         // could be 0/1
            public int LS_YearID { get; set; }
            public int LS_CompID { get; set; }
            public int LS_CustId { get; set; }
            public int LS_ApprovedBy { get; set; }
            public DateTime LS_ApprovedOn { get; set; }
            public string LS_Opeartion { get; set; }   // "I" for insert, "U" for update
            public string LS_IPAddress { get; set; }
        }

        //SaveDepartment
        public class SaveDepartmentDto
        {
            public int LS_ID { get; set; }
            public string LS_Description { get; set; }
            public string LS_DescCode { get; set; }
            public string LS_Code { get; set; }
            public int LS_LevelCode { get; set; }       // Must match SP type
            public int LS_ParentID { get; set; }
            public int LS_CreatedBy { get; set; }
            public DateTime LS_CreatedOn { get; set; }
            public int LS_UpdatedBy { get; set; }
            public DateTime LS_UpdatedOn { get; set; }
            public string LS_DelFlag { get; set; }
            public string LS_Status { get; set; }
            public int LS_YearID { get; set; }
            public int LS_CompID { get; set; }
            public int LS_CustId { get; set; }
            public int LS_ApprovedBy { get; set; }
            public DateTime LS_ApprovedOn { get; set; }
            public string LS_Opeartion { get; set; }   // "I" for Insert, "U" for Update
            public string LS_IPAddress { get; set; }
        }

        //SaveBay
        public class SaveBayDto
        {
            public int LS_ID { get; set; }
            public string LS_Description { get; set; }
            public string LS_DescCode { get; set; }
            public string LS_Code { get; set; }
            public int LS_LevelCode { get; set; }      // Int as per your example
            public int LS_ParentID { get; set; }
            public int LS_CreatedBy { get; set; }
            public DateTime LS_CreatedOn { get; set; }
            public int LS_UpdatedBy { get; set; }
            public DateTime LS_UpdatedOn { get; set; }
            public string LS_DelFlag { get; set; }
            public string LS_Status { get; set; }
            public int LS_YearID { get; set; }
            public int LS_CompID { get; set; }
            public int LS_CustId { get; set; }
            public int LS_ApprovedBy { get; set; }
            public DateTime LS_ApprovedOn { get; set; }
            public string LS_Opeartion { get; set; }   // "I" for insert, "U" for update
            public string LS_IPAddress { get; set; }
        }

        //SaveHeading
        public class SaveHeadingDto
        {
            public int AM_ID { get; set; }
            public string AM_Description { get; set; }
            public string AM_Code { get; set; }
            public int AM_LevelCode { get; set; }
            public int AM_ParentID { get; set; }
            public decimal AM_WDVITAct { get; set; }
            public decimal AM_ITRate { get; set; }
            public int AM_ResidualValue { get; set; }
            public int AM_CreatedBy { get; set; }
            public DateTime AM_CreatedOn { get; set; }
            public int AM_UpdatedBy { get; set; }
            public DateTime AM_UpdatedOn { get; set; }
            public string AM_DelFlag { get; set; }
            public string AM_Status { get; set; }
            public int AM_YearID { get; set; }
            public int AM_CompID { get; set; }
            public int AM_CustId { get; set; }
            public int AM_ApprovedBy { get; set; }
            public DateTime AM_ApprovedOn { get; set; }
            public string AM_Opeartion { get; set; }
            public string AM_IPAddress { get; set; }
        }

        //SaveSubHeading
        public class SaveSubHeadingDto
        {
            public int AM_ID { get; set; }
            public string AM_Description { get; set; }
            public string AM_Code { get; set; }
            public int AM_LevelCode { get; set; }
            public int AM_ParentID { get; set; }
            public decimal AM_WDVITAct { get; set; }
            public decimal AM_ITRate { get; set; }
            public int AM_ResidualValue { get; set; }
            public int AM_CreatedBy { get; set; }
            public DateTime AM_CreatedOn { get; set; }
            public int AM_UpdatedBy { get; set; }
            public DateTime AM_UpdatedOn { get; set; }
            public string AM_DelFlag { get; set; }
            public string AM_Status { get; set; }
            public int AM_YearID { get; set; }
            public int AM_CompID { get; set; }
            public int AM_CustId { get; set; }
            public int AM_ApprovedBy { get; set; }
            public DateTime AM_ApprovedOn { get; set; }
            public string AM_Opeartion { get; set; }
            public string AM_IPAddress { get; set; }
        }

        //SaveAssetClassUnderSubHeading
        public class SaveAssetClassDto
        {
            public int AM_ID { get; set; }
            public string AM_Description { get; set; }
            public string AM_Code { get; set; }
            public int AM_LevelCode { get; set; }
            public int AM_ParentID { get; set; }
            public decimal AM_WDVITAct { get; set; }
            public decimal AM_ITRate { get; set; }
            public int AM_ResidualValue { get; set; }
            public int AM_CreatedBy { get; set; }
            public DateTime AM_CreatedOn { get; set; }
            public int AM_UpdatedBy { get; set; }
            public DateTime AM_UpdatedOn { get; set; }
            public string AM_DelFlag { get; set; }
            public string AM_Status { get; set; }
            public int AM_YearID { get; set; }
            public int AM_CompID { get; set; }
            public int AM_CustId { get; set; }
            public int AM_ApprovedBy { get; set; }
            public DateTime AM_ApprovedOn { get; set; }
            public string AM_Opeartion { get; set; }
            public string AM_IPAddress { get; set; }
        }

        //UpdateLoaction
        public class UpadteLocationSetupDto
        {
            public int LS_ID { get; set; }
            public string? LS_Description { get; set; }
            public string? LS_DescCode { get; set; }
            public string? LS_Code { get; set; }
            public int LS_LevelCode { get; set; }
            public int LS_ParentID { get; set; }
            public int LS_CreatedBy { get; set; }
            public DateTime LS_CreatedOn { get; set; }
            public int LS_UpdatedBy { get; set; }
            public DateTime LS_UpdatedOn { get; set; }
            public string? LS_DelFlag { get; set; }
            public string? LS_Status { get; set; }
            public int LS_YearID { get; set; }
            public int LS_CompID { get; set; }
            public int LS_CustId { get; set; }
            public int? LS_ApprovedBy { get; set; }
            public DateTime? LS_ApprovedOn { get; set; }
            public int LS_Opeartion { get; set; }   // 1 = Save, 2 = Update (as per SP)
            public string? LS_IPAddress { get; set; }
        }
    }
}
