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

          // SAR_AtthachDocId

           // SAR_MasId (iDRLPKID in VB)

        
        public string?  Remarks { get; set; }      // SAR_Remarks

        [Required]
        public string Timeline { get; set; }     // SAR_TimlinetoResOn

        [Required]
        public List<string> EmailId { get; set; } = new();    // SAR_EmailIds

        public string Status { get; set; } = "A"; // Atch_Vstatus (default 'A')

        public List<string> Reporttype { get; set; } = new();


    }
}
