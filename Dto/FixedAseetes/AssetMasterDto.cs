namespace TracePca.Dto.FixedAssets
{
    public class AssetMasterdto
    {
        //LoadCustomer
        public class CustDto
        {
            public int Cust_Id { get; set; }
            public string Cust_Name { get; set; }
        }

        //LoadStstus
        public class StatusDto
        {
            public string Status { get; set; }
        }


        //FinancialYear
        public class YearDto
        {
            public int YMS_YEARID { get; set; }
            public string YMS_ID { get; set; }
        }

        //Location
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

        ////SaveAsset
        //public class AssetMasterDto
        //{
        //    public int AM_ID { get; set; }
        //    public string AM_Description { get; set; }
        //    public string AM_Code { get; set; }
        //    public int AM_LevelCode { get; set; }
        //    public int AM_ParentID { get; set; }
        //    public decimal AM_WDVITAct { get; set; }
        //    public string AM_ITRate { get; set; }
        //    public decimal AM_ResidualValue { get; set; }
        //    public int AM_CreatedBy { get; set; }
        //    public DateTime AM_CreatedOn { get; set; }
        //    public int AM_UpdatedBy { get; set; }
        //    public DateTime AM_UpdatedOn { get; set; }
        //    public string AM_DelFlag { get; set; }
        //    public string AM_Status { get; set; }
        //    public int AM_YearID { get; set; }
        //    public int AM_CompID { get; set; }
        //    public int AM_CustId { get; set; }
        //    public int AM_ApprovedBy { get; set; }
        //    public DateTime AM_ApprovedOn { get; set; }
        //    public string AM_Opeartion { get; set; }
        //    public string AM_IPAddress { get; set; }
        //}



        ////public class LocationSetupDto
        ////{
        ////    public int LS_ID { get; set; }
        ////    public string LS_Description { get; set; }
        ////    public string LS_DescCode { get; set; }
        ////    public string LS_Code { get; set; }
        ////    public int LS_LevelCode { get; set; }
        ////    public int LS_ParentID { get; set; }
        ////    public int LS_CreatedBy { get; set; }
        ////    public DateTime LS_CreatedOn { get; set; }
        ////    public int LS_UpdatedBy { get; set; }
        ////    public DateTime LS_UpdatedOn { get; set; }
        ////    public string LS_DelFlag { get; set; }
        ////    public string LS_Status { get; set; }
        ////    public int LS_YearID { get; set; }
        ////    public int LS_CompID { get; set; }
        ////    public int LS_CustId { get; set; }
        ////    public int LS_ApprovedBy { get; set; }
        ////    public DateTime LS_ApprovedOn { get; set; }
        ////    public string LS_Opeartion { get; set; }
        ////    public string LS_IPAddress { get; set; }
        ////}


        ////SaveLocationn
        //public class AddLocationnDto
        //{
        //    public int? Id { get; set; }            // LS_ID (nullable for Insert)
        //    public string Name { get; set; }        // LS_Description
        //    public string Code { get; set; }        // LS_DescCode

        //    public int ParentID { get; set; }       // LS_ParentID
        //    public int YearID { get; set; }         // LS_YearID
        //    public int CompanyId { get; set; }      // LS_CompID
        //    public int CustomerId { get; set; }     // LS_CustId

        //    public int CreatedBy { get; set; }      // LS_CreatedBy / LS_UpdatedBy / LS_ApprovedBy
        //    public string IPAddress { get; set; }   // LS_IPAddress
        //}

        ////SaveDivision
        //public class AddDivisionnDto
        //{
        //    public int? Id { get; set; }                // For update (nullable)
        //    public string Name { get; set; }            // LS_Description
        //    public string Code { get; set; }            // LS_DescCode
        //    public int CreatedBy { get; set; }          // User ID
        //    public string IPAddress { get; set; }       // User IP
        //    public int YearID { get; set; }             // Financial Year
        //    public int CompanyId { get; set; }          // Company ID
        //    public int CustomerId { get; set; }         // Customer ID (mapped to LS_CustId)
        //}

        ////SaveDepartment
        //public class AddDepartmenttDto
        //{
        //    public int? Id { get; set; }                  // Optional for update
        //    public string Name { get; set; }              // Department name
        //    public string Code { get; set; }              // Department code
        //    public int CreatedBy { get; set; }            // User ID who created/updated
        //    public string IPAddress { get; set; }         // IP address of the user
        //    public int YearID { get; set; }               // Year ID
        //    public int CompanyId { get; set; }            // Company ID
        //    public int CustomerId { get; set; }           // Customer ID
        //    public int DivisionId { get; set; }           // Parent Division ID
        //}

        //SaveBay





    }
}
