using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CAIQ_Factors")]
public partial class CaiqFactor
{
    [Column("CF_PKID")]
    public int? CfPkid { get; set; }

    [Column("CF_YearID")]
    public int? CfYearId { get; set; }

    [Column("CF_AuditID")]
    public int? CfAuditId { get; set; }

    [Column("CF_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CfName { get; set; }

    [Column("CF_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CfDesc { get; set; }

    [Column("CF_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CfFlag { get; set; }

    [Column("CF_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CfStatus { get; set; }

    [Column("CF_CrBy")]
    public int? CfCrBy { get; set; }

    [Column("CF_CrOn", TypeName = "datetime")]
    public DateTime? CfCrOn { get; set; }

    [Column("CF_UpdatedBy")]
    public int? CfUpdatedBy { get; set; }

    [Column("CF_UpdatedOn", TypeName = "datetime")]
    public DateTime? CfUpdatedOn { get; set; }

    [Column("CF_ApprovedBy")]
    public int? CfApprovedBy { get; set; }

    [Column("CF_ApprovedOn", TypeName = "datetime")]
    public DateTime? CfApprovedOn { get; set; }

    [Column("CF_RecallBy")]
    public int? CfRecallBy { get; set; }

    [Column("CF_RecallOn", TypeName = "datetime")]
    public DateTime? CfRecallOn { get; set; }

    [Column("CF_DeletedBy")]
    public int? CfDeletedBy { get; set; }

    [Column("CF_DeletedOn", TypeName = "datetime")]
    public DateTime? CfDeletedOn { get; set; }

    [Column("CF_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CfIpaddress { get; set; }

    [Column("CF_CompId")]
    public int? CfCompId { get; set; }
}
