using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CAIQ_Descriptors")]
public partial class CaiqDescriptor
{
    [Column("CD_PKID")]
    public int? CdPkid { get; set; }

    [Column("CD_YearID")]
    public int? CdYearId { get; set; }

    [Column("CD_AuditID")]
    public int? CdAuditId { get; set; }

    [Column("CD_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CdName { get; set; }

    [Column("CD_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CdDesc { get; set; }

    [Column("CD_Range")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CdRange { get; set; }

    [Column("CD_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CdFlag { get; set; }

    [Column("CD_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CdStatus { get; set; }

    [Column("CD_CrBy")]
    public int? CdCrBy { get; set; }

    [Column("CD_CrOn", TypeName = "datetime")]
    public DateTime? CdCrOn { get; set; }

    [Column("CD_UpdatedBy")]
    public int? CdUpdatedBy { get; set; }

    [Column("CD_UpdatedOn", TypeName = "datetime")]
    public DateTime? CdUpdatedOn { get; set; }

    [Column("CD_ApprovedBy")]
    public int? CdApprovedBy { get; set; }

    [Column("CD_ApprovedOn", TypeName = "datetime")]
    public DateTime? CdApprovedOn { get; set; }

    [Column("CD_RecallBy")]
    public int? CdRecallBy { get; set; }

    [Column("CD_RecallOn", TypeName = "datetime")]
    public DateTime? CdRecallOn { get; set; }

    [Column("CD_DeletedBy")]
    public int? CdDeletedBy { get; set; }

    [Column("CD_DeletedOn", TypeName = "datetime")]
    public DateTime? CdDeletedOn { get; set; }

    [Column("CD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CdIpaddress { get; set; }

    [Column("CD_CompId")]
    public int? CdCompId { get; set; }
}
