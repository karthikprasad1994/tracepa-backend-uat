using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_ValueRating")]
public partial class CrpaValueRating
{
    [Column("CVR_ID")]
    public int? CvrId { get; set; }

    [Column("CVR_YearID")]
    public int? CvrYearId { get; set; }

    [Column("CVR_Point")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CvrPoint { get; set; }

    [Column("CVR_AuditId")]
    public int? CvrAuditId { get; set; }

    [Column("CVR_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CvrName { get; set; }

    [Column("CVR_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CvrDesc { get; set; }

    [Column("CVR_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CvrFlag { get; set; }

    [Column("CVR_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CvrStatus { get; set; }

    [Column("CVR_CrBy")]
    public int? CvrCrBy { get; set; }

    [Column("CVR_CrOn", TypeName = "datetime")]
    public DateTime? CvrCrOn { get; set; }

    [Column("CVR_UpdatedBy")]
    public int? CvrUpdatedBy { get; set; }

    [Column("CVR_UpdatedOn", TypeName = "datetime")]
    public DateTime? CvrUpdatedOn { get; set; }

    [Column("CVR_ApprovedBy")]
    public int? CvrApprovedBy { get; set; }

    [Column("CVR_ApprovedOn", TypeName = "datetime")]
    public DateTime? CvrApprovedOn { get; set; }

    [Column("CVR_RecallBy")]
    public int? CvrRecallBy { get; set; }

    [Column("CVR_RecallOn", TypeName = "datetime")]
    public DateTime? CvrRecallOn { get; set; }

    [Column("CVR_DeletedBy")]
    public int? CvrDeletedBy { get; set; }

    [Column("CVR_DeletedOn", TypeName = "datetime")]
    public DateTime? CvrDeletedOn { get; set; }

    [Column("CVR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CvrIpaddress { get; set; }

    [Column("CVR_CompId")]
    public int? CvrCompId { get; set; }
}
