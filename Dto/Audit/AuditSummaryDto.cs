namespace TracePca.Dto.Audit
{
    public class AuditSummaryDto
    {
        public int Cust_Id { get; set; }
        public string Cust_Name { get; set; }

        public int SA_ID { get; set; }
        public string SA_AuditNo { get; set; }
         
    }

    public class AuditDetailsDto
    {
        public int SrNo { get; set; }
        public string AuditNo { get; set; }
        public int CustID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerShortName { get; set; }
        public string AuditType { get; set; }
        public int StatusID { get; set; }
        public string AuditDate { get; set; }
        public string AuditStatus { get; set; }
        public string Partner { get; set; }
        public string Team { get; set; }
    }


    public class DocumentRequestSummaryDto
    {
        public int ADRL_Id { get; set; }
        public string ADRL_ReportType { get; set; }
        public string ADRL_AuditNo { get; set; }
        public string ReportTypeText { get; set; }
        public string ADRL_Comments { get; set; }
        public string ADRL_RequestedOn { get; set; }
        public string usr_FullName { get; set; }
        public string ADRL_ReceivedOn { get; set; }
        public string ADRL_ReceivedComments { get; set; }
        public int ADRL_AttchDocId { get; set; }
       
    }

    public class AuditProgramSummaryDto
    {
        
        public string AuditProgram { get; set; }
        public string TotalCheckpoints { get; set; }
        public string Mandatory { get; set; }
        public string Tested { get; set; }
        public string Annexures { get; set; }
        public string Reviewed { get; set; }
      

    }

    public class WorkspaceSummaryDto
    {
        public int PKID { get; set; }
        public string WorkpaperChecklist { get; set; }
        public string WorkpaperNo { get; set; }
        public string WorkpaperRef { get; set; }
        public string Observation { get; set; }
        public string Conclusion { get; set; }
        public string ReviewerComments { get; set; }
        public string TypeOfTest { get; set; }
        public string Status { get; set; }
        public int AttachID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ReviewedBy { get; set; }
        public string ReviewedOn { get; set; }
    }


    public class CMADto
    {
        public int SrNo { get; set; }
        public int DBpkId { get; set; }
        public string WorkpaperNo { get; set; }
        public string WorkpaperRef { get; set; }
        public string Observation { get; set; }
        public string Conclusion { get; set; }
        public string TypeOfTest { get; set; }
        public string Status { get; set; }
        public string CAM { get; set; }
        public int AttachmentID { get; set; }
        public string ExceededMateriality { get; set; }
        public string DescriptionOrReasonForSelectionAsCAM { get; set; }
        public string AuditProcedureUndertakenToAddressTheCAM { get; set; }
    }

    public class UpdateStandardAuditASCAMdetailsDto
    {
        public int SACAM_PKID { get; set; }
        public int SACAM_SA_ID { get; set; }
        public string? SACAM_DescriptionOrReasonForSelectionAsCAM { get; set; }
        public string? SACAM_AuditProcedureUndertakenToAddressTheCAM { get; set; }
    }
}
