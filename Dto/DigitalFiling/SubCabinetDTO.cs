using System.Text.Json.Serialization;

namespace TracePca.Dto.DigitalFiling
{
    public class SubCabinetDTO
    {
        public int CBN_ID { get; set; }
        public string CBN_Name { get; set; }
        public int CBN_Parent { get; set; }
        public string? CBN_Note { get; set; }
        public int CBN_UserID { get; set; }
        public int CBN_Department { get; set; }
        [JsonIgnore]
        public string? CBN_DepartmentName { get; set; }
        public int CBN_SubCabCount { get; set; }
        public int CBN_FolderCount { get; set; }
        public int CBN_CreatedBy { get; set; }
        [JsonIgnore]
        public string? CBN_CreatedByName { get; set; }
        public DateTime CBN_CreatedOn { get; set; }
        public int? CBN_UpdatedBy { get; set; }
        [JsonIgnore]
        public string? CBN_UpdatedByName { get; set; }
        public DateTime? CBN_UpdatedOn { get; set; }
        public int? CBN_ApprovedBy { get; set; }
        [JsonIgnore]
        public string? CBN_ApprovedByName { get; set; }
        public DateTime? CBN_ApprovedOn { get; set; }
        public int? CBN_DeletedBy { get; set; }
        [JsonIgnore]
        public string? CBN_DeletedByName { get; set; }
        public DateTime? CBN_DeletedOn { get; set; }
        public int? CBN_RecalledBy { get; set; }
        [JsonIgnore]
        public string? CBN_RecalledByName { get; set; }
        public DateTime? CBN_RecalledOn { get; set; }
        public string CBN_Status { get; set; }
        public string CBN_DelFlag { get; set; }
        public int? CBN_CompID { get; set; }
        public string? CBN_Retention { get; set; }
        public SubCabinetPermissionDTO SubCabinetPermissionDetails { get; set; } = new SubCabinetPermissionDTO();
    }

    public class SubCabinetPermissionDTO
    {
        public int CBP_ID { get; set; }
        public string CBP_PermissionType { get; set; }
        public int? CBP_Cabinet { get; set; }
        public int? CBP_User { get; set; }
        public int? CBP_Department { get; set; }
        public int? CBP_View { get; set; }
        public int? CBP_Create { get; set; }
        public int? CBP_Modify { get; set; }
        public int? CBP_Delete { get; set; }
        public int? CBP_Search { get; set; }
        public int? CBP_Index { get; set; }
        public int? CBP_Other { get; set; }
        public int? CBP_CreateFolder { get; set; }
        public int? Entire_File_Plan { get; set; }
    }

    public class UpdateSubCabinetStatusRequestDTO
    {
        public int CompId { get; set; }
        public int UserId { get; set; }
        public int CabinetId { get; set; }
        public int SubCabinetId { get; set; }
        public string StatusCode { get; set; }
    }



	public class SubCabinetDetailsDTO
	{
		public int CBN_ID { get; set; }
		public string CBN_Name { get; set; }
		public int CBN_Parent { get; set; }
		public string? CBN_Note { get; set; }
		public int CBN_UserID { get; set; }
		public int CBN_Department { get; set; }
		[JsonIgnore]
		public string? CBN_DepartmentName { get; set; }
		public int CBN_SubCabCount { get; set; }
		public int CBN_FolderCount { get; set; }
		public string? CBN_CreatedBy { get; set; }
		 
		public string? CBN_CreatedByName { get; set; }
		public DateTime CBN_CreatedOn { get; set; }
		public string? CBN_UpdatedBy { get; set; }
		 
		public string? CBN_UpdatedByName { get; set; }
		public DateTime? CBN_UpdatedOn { get; set; }
		public int? CBN_ApprovedBy { get; set; }
		[JsonIgnore]
		public string? CBN_ApprovedByName { get; set; }
		public DateTime? CBN_ApprovedOn { get; set; }
		public int? CBN_DeletedBy { get; set; }
		[JsonIgnore]
		public string? CBN_DeletedByName { get; set; }
		public DateTime? CBN_DeletedOn { get; set; }
		public int? CBN_RecalledBy { get; set; }
		[JsonIgnore]
		public string? CBN_RecalledByName { get; set; }
		public DateTime? CBN_RecalledOn { get; set; }
		public string CBN_Status { get; set; }
		public string CBN_DelFlag { get; set; }
		public int? CBN_CompID { get; set; }
		public string? CBN_Retention { get; set; }
		public SubCabinetPermissionDTO SubCabinetPermissionDetails { get; set; } = new SubCabinetPermissionDTO();
	}
}
