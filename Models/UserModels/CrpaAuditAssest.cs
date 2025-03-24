using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_AuditAssest")]
public partial class CrpaAuditAssest
{
    [Column("CA_PKID")]
    public int? CaPkid { get; set; }

    [Column("CA_AsgNo")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CaAsgNo { get; set; }

    [Column("CA_FinancialYear")]
    public int? CaFinancialYear { get; set; }

    [Column("CA_LOCATIONID")]
    public int? CaLocationid { get; set; }

    [Column("CA_SECTIONID")]
    public int? CaSectionid { get; set; }

    [Column("CA_Date", TypeName = "datetime")]
    public DateTime? CaDate { get; set; }

    [Column("CA_NAME_OF_OPS_HEAD")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CaNameOfOpsHead { get; set; }

    [Column("CA_ADDRESS")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CaAddress { get; set; }

    [Column("CA_NAME_OF_UNIT_PRESIDENT")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CaNameOfUnitPresident { get; set; }

    [Column("CA_AUDITORNAME")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CaAuditorname { get; set; }

    [Column("CA_NetScore")]
    public float? CaNetScore { get; set; }

    [Column("CA_CrBy")]
    public int? CaCrBy { get; set; }

    [Column("CA_CrOn", TypeName = "datetime")]
    public DateTime? CaCrOn { get; set; }

    [Column("CA_UpdatedBy")]
    public int? CaUpdatedBy { get; set; }

    [Column("CA_UpdatedOn", TypeName = "datetime")]
    public DateTime? CaUpdatedOn { get; set; }

    [Column("CA_ASubmittedBy")]
    public int? CaAsubmittedBy { get; set; }

    [Column("CA_ASubmittedOn", TypeName = "datetime")]
    public DateTime? CaAsubmittedOn { get; set; }

    [Column("CA_BSubmittedBy")]
    public int? CaBsubmittedBy { get; set; }

    [Column("CA_BSubmittedOn", TypeName = "datetime")]
    public DateTime? CaBsubmittedOn { get; set; }

    [Column("CA_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CaStatus { get; set; }

    [Column("CA_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CaIpaddress { get; set; }

    [Column("CA_CompID")]
    public int? CaCompId { get; set; }
}
