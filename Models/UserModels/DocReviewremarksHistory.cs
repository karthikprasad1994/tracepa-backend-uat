using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Doc_Reviewremarks_History")]
public partial class DocReviewremarksHistory
{
    [Column("DRH_ID")]
    public int? DrhId { get; set; }

    [Column("DRH_MASid")]
    public int? DrhMasid { get; set; }

    [Column("DRH_Custid")]
    public int? DrhCustid { get; set; }

    [Column("DRH_Loeid")]
    public int? DrhLoeid { get; set; }

    [Column("DRH_RemarksType")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DrhRemarksType { get; set; }

    [Column("DRH_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? DrhRemarks { get; set; }

    [Column("DRH_RemarksBy")]
    public int? DrhRemarksBy { get; set; }

    [Column("DRH_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DrhStatus { get; set; }

    [Column("DRH_Date")]
    [StringLength(20)]
    [Unicode(false)]
    public string? DrhDate { get; set; }

    [Column("DRH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DrhIpaddress { get; set; }

    [Column("DRH_CompID")]
    public int? DrhCompId { get; set; }

    [Column("DRH_Yearid")]
    public int? DrhYearid { get; set; }

    [Column("DRH_attchmentid")]
    public int? DrhAttchmentid { get; set; }

    [Column("DRH_ComplStatus")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DrhComplStatus { get; set; }

    [Column("DRH_DBFlag")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DrhDbflag { get; set; }
}
