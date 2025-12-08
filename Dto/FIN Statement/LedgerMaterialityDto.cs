namespace TracePca.Dto.FIN_Statement
{
    public class LedgerMaterialityDto
    {
        //GetContentManagement
        public class ContentManagementDto
        {
            public string? cmm_ID { get; set; }
            public string? cmm_Desc { get; set; }
            public int lm_LevelOfRisk { get; set; }
            public int lm_Weightage { get; set; }
        }

        //SaveOrUpdateLedgerMaterialityMaster
        public class LedgerMaterialityMasterDto
        {
            public int lm_ID { get; set; }
            public int lm_MaterialityId { get; set; }
            public int lm_CustId { get; set; }
            public int lm_FinancialYearId { get; set; }
            public int lm_Branch { get; set; }
            public int lm_LevelOfRisk { get; set; }
            public int lm_Weightage { get; set; }
            public string lm_Delflag { get; set; } = "N";   // default N (not deleted)
            public string lm_Status { get; set; } = "A";    // default A (active)
            public int lm_UpdatedBy { get; set; }
            public DateTime? lm_UpdatedOn { get; set; }
            public int lm_ApprovedBy { get; set; }
            public DateTime? lm_ApprovedOn { get; set; }
            public int lm_DeletedBy { get; set; }
            public DateTime? lm_DeletedOn { get; set; }
            public int? lm_RecallBy { get; set; }
            public DateTime? lm_RecallOn { get; set; }
            public string lm_IPAddress { get; set; } = string.Empty;
            public int lm_CompID { get; set; }
            public int lm_CrBy { get; set; }
            public DateTime? lm_CrOn { get; set; }
        }

        //SaveOrUpdateContentMateriality
        public class CreateMTContentRequestDto
        {
            public int? cmm_ID { get; set; }
            public int CMM_CompID { get; set; }
            public string? cmm_Desc { get; set; }
            public string? cms_Remarks { get; set; }
            public string? cmm_Category { get; set; }
        }

        //GetMatrialityId
        public class GetMaterialityIdDto
        {
            public int cmm_ID { get; set; }
            public string cmm_Desc { get; set; }
            public string cms_Remarks { get; set; }
            public string cmm_Code { get; set; }
        }


        //DeleteMaterialityById
        public class DeleteMaterialityIdDto
        {
            public int cmm_ID { get; set; }
        }

        //LoadDescription
        public class LoadDescriptionDto
        {
            public int cmm_ID { get; set; }
            public string cmm_Code { get; set; }
            public string cmm_Desc { get; set; }
            public string cms_Remarks { get; set; }
        }

        public class DescriptionDto
        {
            public int HeadingId { get; set; }
            public string HeadingName { get; set; }
            public string Status { get; set; }
            public decimal CYamt { get; set; }
            public decimal PYamt { get; set; }
            public decimal Difference_Amt { get; set; }
            public decimal Difference_Avg { get; set; }
            public string RiskFactor { get; set; }
            public string Materiality { get; set; }
            public decimal cyCr { get; set; }
            public decimal cyDb { get; set; }
            public decimal pyCr { get; set; }
            public decimal pyDb { get; set; }
        }
    }
}
