using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Text.Json.Serialization;
using TracePca.Dto.Audit;

namespace TracePca.Dto.DigitalFilling
{
    public class SearchDto
    {
		public string Title { get; set; }
		public string Extension { get; set; }
        public string Cabinet { get; set; }
		public string SubCabinet { get; set; }
        public string Folder { get; set; }
		public string PGE_BASENAME { get; set; }
		public string URLPath { get; set; }
	}
    public class CabinetDto
    {
       
        public int? CBN_ID { get; set; } = 0;
        public string CBN_NAME { get; set; }
        public string CBN_SubCabCount { get; set; }
        public string CBN_FolderCount { get; set; }
        public string CBN_CreatedBy { get; set; }
        public string CBN_CreatedOn { get; set; }
        public string CBN_DelFlag { get; set; }

        public int CBN_Parent { get; set; }
        public string CBN_Note { get; set; }
        public int CBN_UserID { get; set; }
        public int CBN_Department { get; set; }
        public string CBN_Status { get; set; }
        public int CBN_CompID { get; set; }
        public DateTime CBN_DocumentExpiryDate { get; set; }
		public string CBN_ReminderDay { get; set; }

	}

    public class OrgStructureDto
    {
       //public List<OrgStructureDto> OrgStructureDetails { get; set; } = new List<OrgStructureDto>();
        public int Org_Node { get; set; }
        public string Org_Name { get; set; }

        public int SUO_PKID { get; set; }
        public int SUO_UserID { get; set; }

        public int SUO_DeptID { get; set; }
        public int SUO_IsDeptHead { get; set; }
    }



    public class IndexDocumentDto
    {
        public IFormFile File { get; set; }
        public int CabinetID { get; set; }
        public int SubCabinetID { get; set; }
        public int FolderID { get; set; }
        public int DocumentTypeID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
		public string? Keyword { get; set; } = null;
		public int CompID { get; set; }
    }



	public class DescriptorDto
	{
		public int? DES_ID { get; set; } = 0;
		public string DESC_NAME { get; set; }
		public string DESC_NOTE { get; set; }
		public string DESC_DATATYPE { get; set; }
		public string DESC_SIZE { get; set; }
        public string DESC_DefaultValues { get; set; }
		public int DESC_CompId { get; set; }
	}


	public class DocumentTypeDto
	{
		public int? DOT_DOCTYPEID { get; set; } = 0;
		public string DOT_DOCNAME { get; set; }
		public string DOT_NOTE { get; set; }
		public string DOT_PGROUP { get; set; }
		public string DOT_CRBY { get; set; }
		public string DOT_CRON { get; set; }
		public string DOT_STATUS { get; set; }
		public string DOT_Department { get; set; }
	}


	public class ArchiveDetailsDto
	{
		public int? SA_ID { get; set; } = 0;
		public string SA_AuditNo { get; set; }
		public string SA_ScopeOfAudit { get; set; }
		public int SA_CustID { get; set; }
		public int SA_AuditTypeID { get; set; }
		public int SA_PartnerID { get; set; }
		public int SA_ReviewPartnerID { get; set; }
		public int SA_AttachID { get; set; }
		public int SA_CompID { get; set; }
		public DateTime SA_StartDate { get; set; }
		public DateTime SA_ExpCompDate { get; set; }
		public DateTime SA_AuditOpinionDate { get; set; }
		public DateTime SA_ExpiryDate { get; set; }
		public string CUST_NAME { get; set; }
		public string CUST_CODE { get; set; }
		public string cmm_Code { get; set; }
		public string cmm_Desc { get; set; }

		public int AttachmentCount { get; set; }
		public string SA_RetentionPeriod { get; set; }

		public string SA_AttachmentID { get; set; }
	}

	public class ArchivedDocumentFileDto
	{
		public string FileName { get; set; }
		public string URLPath { get; set; }
	}

}
