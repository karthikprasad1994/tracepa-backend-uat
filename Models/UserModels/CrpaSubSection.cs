using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_SubSection")]
public partial class CrpaSubSection
{
    [Column("CASU_ID")]
    public int CasuId { get; set; }

    [Column("CASU_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string CasuCode { get; set; } = null!;

    [Column("CASU_SUBSECTIONNAME")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? CasuSubsectionname { get; set; }

    [Column("CASU_SECTIONID")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? CasuSectionid { get; set; }

    [Column("CASU_Points")]
    public int? CasuPoints { get; set; }

    [Column("CASU_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CasuDesc { get; set; }

    [Column("CASU_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string CasuDelflg { get; set; } = null!;

    [Column("CASU_CRON", TypeName = "datetime")]
    public DateTime? CasuCron { get; set; }

    [Column("CASU_CRBY")]
    public int? CasuCrby { get; set; }

    [Column("CASU_APPROVEDBY")]
    public int? CasuApprovedby { get; set; }

    [Column("CASU_APPROVEDON", TypeName = "datetime")]
    public DateTime? CasuApprovedon { get; set; }

    [Column("CASU_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CasuStatus { get; set; }

    [Column("CASU_UPDATEDBY")]
    public int? CasuUpdatedby { get; set; }

    [Column("CASU_UPDATEDON", TypeName = "datetime")]
    public DateTime? CasuUpdatedon { get; set; }

    [Column("CASU_DELETEDBY")]
    public int? CasuDeletedby { get; set; }

    [Column("CASU_DELETEDON", TypeName = "datetime")]
    public DateTime? CasuDeletedon { get; set; }

    [Column("CASU_RECALLBY")]
    public int? CasuRecallby { get; set; }

    [Column("CASU_RECALLON", TypeName = "datetime")]
    public DateTime? CasuRecallon { get; set; }

    [Column("CASU_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CasuIpaddress { get; set; }

    [Column("CASU_CompId")]
    public int? CasuCompId { get; set; }

    [Column("CASU_YEARId")]
    public int? CasuYearid { get; set; }
}
