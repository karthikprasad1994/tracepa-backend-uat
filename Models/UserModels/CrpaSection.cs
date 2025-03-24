using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_Section")]
public partial class CrpaSection
{
    [Column("CAS_ID")]
    public int CasId { get; set; }

    [Column("CAS_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string CasCode { get; set; } = null!;

    [Column("CAS_SECTIONNAME")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? CasSectionname { get; set; }

    [Column("CAS_POINTS")]
    public int? CasPoints { get; set; }

    [Column("CAS_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CasDesc { get; set; }

    [Column("CAS_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string CasDelflg { get; set; } = null!;

    [Column("CAS_CRON", TypeName = "datetime")]
    public DateTime? CasCron { get; set; }

    [Column("CAS_CRBY")]
    public int? CasCrby { get; set; }

    [Column("CAS_APPROVEDBY")]
    public int? CasApprovedby { get; set; }

    [Column("CAS_APPROVEDON", TypeName = "datetime")]
    public DateTime? CasApprovedon { get; set; }

    [Column("CAS_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CasStatus { get; set; }

    [Column("CAS_UPDATEDBY")]
    public int? CasUpdatedby { get; set; }

    [Column("CAS_UPDATEDON", TypeName = "datetime")]
    public DateTime? CasUpdatedon { get; set; }

    [Column("CAS_DELETEDBY")]
    public int? CasDeletedby { get; set; }

    [Column("CAS_DELETEDON", TypeName = "datetime")]
    public DateTime? CasDeletedon { get; set; }

    [Column("CAS_RECALLBY")]
    public int? CasRecallby { get; set; }

    [Column("CAS_RECALLON", TypeName = "datetime")]
    public DateTime? CasRecallon { get; set; }

    [Column("CAS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CasIpaddress { get; set; }

    [Column("CAS_CompId")]
    public int? CasCompId { get; set; }

    [Column("CAS_YEARId")]
    public int? CasYearid { get; set; }
}
