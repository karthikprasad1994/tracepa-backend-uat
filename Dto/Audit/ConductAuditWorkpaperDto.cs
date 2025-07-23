namespace TracePca.Dto.Audit
{
    public class ConductAuditWorkpaperDto
    {
        public int SSW_ID { get; set; }
        public string SSW_WorkpaperNo { get; set; }
        public string SSW_WorkpaperRef { get; set; }
        public string SSW_Observation { get; set; }
        public string SSW_Conclusion { get; set; }
        public string SSW_TypeOfTest { get; set; }
        public int SSW_WPCheckListID { get; set; }
        public int SSW_DRLID { get; set; }
        public string SSW_Status { get; set; }
        public string SSW_ExceededMateriality { get; set; }
        public decimal? SSW_AuditorHoursSpent { get; set; }
        public string SSW_NotesSteps { get; set; }
        public string SSW_CriticalAuditMatter { get; set; }
        public int SSW_AttachID { get; set; }
    }
}
