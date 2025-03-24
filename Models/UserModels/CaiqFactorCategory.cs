using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CAIQ_FactorCategory")]
public partial class CaiqFactorCategory
{
    [Column("CFC_PKID")]
    public int? CfcPkid { get; set; }

    [Column("CFC_YearID")]
    public int? CfcYearId { get; set; }

    [Column("CFC_AuditID")]
    public int? CfcAuditId { get; set; }

    [Column("CFC_FactorID")]
    public int? CfcFactorId { get; set; }

    [Column("CFC_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CfcName { get; set; }

    [Column("CFC_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CfcDesc { get; set; }

    [Column("CFC_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CfcFlag { get; set; }

    [Column("CFC_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CfcStatus { get; set; }

    [Column("CFC_CrBy")]
    public int? CfcCrBy { get; set; }

    [Column("CFC_CrOn", TypeName = "datetime")]
    public DateTime? CfcCrOn { get; set; }

    [Column("CFC_UpdatedBy")]
    public int? CfcUpdatedBy { get; set; }

    [Column("CFC_UpdatedOn", TypeName = "datetime")]
    public DateTime? CfcUpdatedOn { get; set; }

    [Column("CFC_ApprovedBy")]
    public int? CfcApprovedBy { get; set; }

    [Column("CFC_ApprovedOn", TypeName = "datetime")]
    public DateTime? CfcApprovedOn { get; set; }

    [Column("CFC_RecallBy")]
    public int? CfcRecallBy { get; set; }

    [Column("CFC_RecallOn", TypeName = "datetime")]
    public DateTime? CfcRecallOn { get; set; }

    [Column("CFC_DeletedBy")]
    public int? CfcDeletedBy { get; set; }

    [Column("CFC_DeletedOn", TypeName = "datetime")]
    public DateTime? CfcDeletedOn { get; set; }

    [Column("CFC_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CfcIpaddress { get; set; }

    [Column("CFC_CompId")]
    public int? CfcCompId { get; set; }
}
