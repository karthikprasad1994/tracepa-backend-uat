using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TracePca.Dto
{
    public class AddSupplierDto
    {



        [Column("SUP_ID")]
        public int? SupId { get; set; }

        [Column("SUP_Name")]
        [StringLength(150)]
        [Unicode(false)]
        public string? SupName { get; set; }

        [Column("SUP_Code")]
        [StringLength(50)]
        [Unicode(false)]
        public string? SupCode { get; set; }

        [Column("SUP_ContactPerson")]
        [StringLength(50)]
        [Unicode(false)]
        public string? SupContactPerson { get; set; }

        [Column("SUP_Address")]
        [StringLength(50)]
        [Unicode(false)]
        public string? SupAddress { get; set; }

        [Column("SUP_PhoneNo")]
        [StringLength(50)]
        [Unicode(false)]
        public string? SupPhoneNo { get; set; }

        [Column("SUP_Fax")]
        [StringLength(50)]
        [Unicode(false)]
        public string? SupFax { get; set; }

        [Column("SUP_Email")]
        [StringLength(50)]
        [Unicode(false)]
        public string? SupEmail { get; set; }

        [Column("SUP_Website")]
        [StringLength(50)]
        [Unicode(false)]
        public string? SupWebsite { get; set; }

        [Column("SUP_CRBY")]
        public int? SupCrby { get; set; }

        [Column("SUP_CRON", TypeName = "datetime")]
        public DateTime? SupCron { get; set; }

        [Column("SUP_STATUS")]
        [StringLength(25)]
        [Unicode(false)]
        public string? SupStatus { get; set; }

        [Column("SUP_IPAddress")]
        [StringLength(25)]
        [Unicode(false)]
        public string? SupIpaddress { get; set; }

        [Column("SUP_CompID")]
        public int? SupCompId { get; set; }
    }
}
