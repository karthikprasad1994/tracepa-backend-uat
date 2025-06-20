using System.ComponentModel.DataAnnotations;

namespace TracePca.Dto.Audit
{
    public class UpdateDrlStatusDto
    {
        [Required]
        public int CustomerId { get; set; }      // SAR_SAC_ID

        [Required]
        public int YearId { get; set; }          // sar_Yearid

        [Required]
        public int CompId { get; set; }          // SAR_CompID

        [Required]
        public int DocId { get; set; }           // SAR_AtthachDocId

        [Required]
        public int DrlPkId { get; set; }        // SAR_MasId (iDRLPKID in VB)

        [Required]
        public string Remarks { get; set; }      // SAR_Remarks

        [Required]
        public string Timeline { get; set; }     // SAR_TimlinetoResOn

        [Required]
        public string EmailIds { get; set; }     // SAR_EmailIds

        public string Status { get; set; } = "A"; // Atch_Vstatus (default 'A')

        public int ADRL_RequestedListID { get; set; }

        public  int AuditId { get; set; }


    }
}
