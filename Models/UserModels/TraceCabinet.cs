using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("TRACe_Cabinet")]
public partial class TraceCabinet
{
    [Column("TC_PKID")]
    public int? TcPkid { get; set; }

    [Column("TC_Name")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? TcName { get; set; }

    [Column("TC_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? TcRemarks { get; set; }

    [Column("TC_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TcStatus { get; set; }

    [Column("TC_CrOn", TypeName = "datetime")]
    public DateTime? TcCrOn { get; set; }

    [Column("TC_CrBy")]
    public int? TcCrBy { get; set; }

    [Column("TC_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TcIpaddress { get; set; }

    [Column("TC_CompID")]
    public int? TcCompId { get; set; }
}
