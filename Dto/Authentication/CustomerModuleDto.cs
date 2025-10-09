using System.ComponentModel.DataAnnotations.Schema;

namespace TracePca.Dto.Authentication
{
    public class CustomerModuleDto
    {
        [Column("MCM_ID")]
        public int? MCM_ID { get; set; }

        [Column("MCM_ModuleID")]
        public int? MCM_ModuleID { get; set; }
    }
}
