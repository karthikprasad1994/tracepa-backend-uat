using System.Text.Json.Serialization;

namespace TracePca.Dto.DigitalFiling
{
    public class FolderDTO
    {
        public int FOL_FolID { get; set; }
        public string FOL_Name { get; set; }
        public string FOL_Note { get; set; }
        public int FOL_Cabinet { get; set; }
        [JsonIgnore]
        public string? FOL_CabinetName { get; set; }
        public int FOL_SubCabinet { get; set; }
        public int FOL_CreatedBy { get; set; }
        [JsonIgnore]
        public string? FOL_CreatedByName { get; set; }
        public DateTime? FOL_CreatedOn { get; set; }
        public int? FOL_UpdatedBy { get; set; }
        [JsonIgnore]
        public string? FOL_UpdatedByName { get; set; }
        public DateTime? FOL_UpdatedOn { get; set; }
        public int? FOL_ApprovedBy { get; set; }
        [JsonIgnore]
        public string? FOL_ApprovedByName { get; set; }
        public DateTime? FOL_ApprovedOn { get; set; }
        public int? FOL_DeletedBy { get; set; }
        [JsonIgnore]
        public string? FOL_DeletedByName { get; set; }
        public DateTime? FOL_DeletedOn { get; set; }
        public int? FOL_RecalledBy { get; set; }
        [JsonIgnore]
        public string? FOL_RecalledByName { get; set; }
        public DateTime? FOL_RecalledOn { get; set; }
        public string FOL_Status { get; set; }
        public string FOL_DelFlag { get; set; }
        public int FOL_CompID { get; set; }
        public FolderPermissionDTO FolderPermissionDTODetails { get; set; } = new FolderPermissionDTO();
    }

    public class FolderPermissionDTO
    {
        public int EFP_ID { get; set; }
        public string EFP_PTYPE { get; set; }
        public int? EFP_GRPID { get; set; }
        public int? EFP_USRID { get; set; }
        public int? EFP_INDEX { get; set; }
        public int? EFP_SEARCH { get; set; }
        public int? EFP_MOD_FOLDER { get; set; }
        public int? EFP_MOD_DOC { get; set; }
        public int? EFP_DEL_FOLDER { get; set; }
        public int? EFP_DEL_DOC { get; set; }
        public int? EFP_EXPORT { get; set; }
        public int? EFP_OTHER { get; set; }
        public int? EFP_CRT_DOC { get; set; }
        public int? EFP_VIEW_Fol { get; set; }
        public int? EFP_FolId { get; set; }
    }

    public class UpdateFolderStatusRequestDTO
    {
        public int CompId { get; set; }
        public int UserId { get; set; }
        public int CabinetId { get; set; }
        public int SubCabinetId { get; set; }
        public int FolderId { get; set; }
        public string StatusCode { get; set; }
    }



	public class FolderDetailDTO
	{
		public int FOL_FolID { get; set; }
		public string FOL_Name { get; set; }
		public string FOL_Note { get; set; }
 
 
		public string? FOL_CreatedBy { get; set; }
		public string FOL_SubCabinet { get; set; }
  
		public DateTime? FOL_CreatedOn { get; set; }
		public int? FOL_UpdatedBy { get; set; }
		[JsonIgnore]
		public string? FOL_UpdatedByName { get; set; }
		public DateTime? FOL_UpdatedOn { get; set; }
		public int? FOL_ApprovedBy { get; set; }
		[JsonIgnore]
		public string? FOL_ApprovedByName { get; set; }
		public DateTime? FOL_ApprovedOn { get; set; }
		public int? FOL_DeletedBy { get; set; }
		[JsonIgnore]
		public string? FOL_DeletedByName { get; set; }
		public DateTime? FOL_DeletedOn { get; set; }
		public int? FOL_RecalledBy { get; set; }
		[JsonIgnore]
		public string? FOL_RecalledByName { get; set; }
		public DateTime? FOL_RecalledOn { get; set; }
		public string FOL_Status { get; set; }
		public string FOL_DelFlag { get; set; }
		public int FOL_CompID { get; set; }

		public int FOL_Documents { get; set; }
        public string DocumentPath { get; set; }

	}
}
