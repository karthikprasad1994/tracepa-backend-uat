using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_Statutory_DirectorDetails")]
public partial class SadStatutoryDirectorDetail
{
    [Column("SSD_Id")]
    public int? SsdId { get; set; }

    [Column("SSD_CustID")]
    public int? SsdCustId { get; set; }

    [Column("SSD_DirectorName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SsdDirectorName { get; set; }

    [Column("SSD_DOB", TypeName = "datetime")]
    public DateTime? SsdDob { get; set; }

    [Column("SSD_DIN")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SsdDin { get; set; }

    [Column("SSD_MobileNo")]
    [StringLength(15)]
    [Unicode(false)]
    public string? SsdMobileNo { get; set; }

    [Column("SSD_Email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SsdEmail { get; set; }

    [Column("SSD_Remarks")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SsdRemarks { get; set; }

    [Column("SSD_CRON", TypeName = "datetime")]
    public DateTime? SsdCron { get; set; }

    [Column("SSD_CRBY")]
    public int? SsdCrby { get; set; }

    [Column("SSD_UpdatedOn", TypeName = "datetime")]
    public DateTime? SsdUpdatedOn { get; set; }

    [Column("SSD_UpdatedBy")]
    public int? SsdUpdatedBy { get; set; }

    [Column("SSD_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SsdDelFlag { get; set; }

    [Column("SSD_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SsdStatus { get; set; }

    [Column("SSD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SsdIpaddress { get; set; }

    [Column("SSD_CompID")]
    public int? SsdCompId { get; set; }
}
