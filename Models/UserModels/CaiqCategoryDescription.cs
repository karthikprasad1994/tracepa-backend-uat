using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CAIQ_CategoryDescription")]
public partial class CaiqCategoryDescription
{
    [Column("CCD_PKID")]
    public int? CcdPkid { get; set; }

    [Column("CCD_YearID")]
    public int? CcdYearId { get; set; }

    [Column("CCD_AuditID")]
    public int? CcdAuditId { get; set; }

    [Column("CCD_FactorID")]
    public int? CcdFactorId { get; set; }

    [Column("CCD_CategoryID")]
    public int? CcdCategoryId { get; set; }

    [Column("CCD_DescriptorID")]
    public int? CcdDescriptorId { get; set; }

    [Column("CCD_Name")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CcdName { get; set; }

    [Column("CCD_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CcdDesc { get; set; }

    [Column("CCD_DescValue")]
    public int? CcdDescValue { get; set; }

    [Column("CCD_FLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CcdFlag { get; set; }

    [Column("CCD_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CcdStatus { get; set; }

    [Column("CCD_CrBy")]
    public int? CcdCrBy { get; set; }

    [Column("CCD_CrOn", TypeName = "datetime")]
    public DateTime? CcdCrOn { get; set; }

    [Column("CCD_UpdatedBy")]
    public int? CcdUpdatedBy { get; set; }

    [Column("CCD_UpdatedOn", TypeName = "datetime")]
    public DateTime? CcdUpdatedOn { get; set; }

    [Column("CCD_ApprovedBy")]
    public int? CcdApprovedBy { get; set; }

    [Column("CCD_ApprovedOn", TypeName = "datetime")]
    public DateTime? CcdApprovedOn { get; set; }

    [Column("CCD_RecallBy")]
    public int? CcdRecallBy { get; set; }

    [Column("CCD_RecallOn", TypeName = "datetime")]
    public DateTime? CcdRecallOn { get; set; }

    [Column("CCD_DeletedBy")]
    public int? CcdDeletedBy { get; set; }

    [Column("CCD_DeletedOn", TypeName = "datetime")]
    public DateTime? CcdDeletedOn { get; set; }

    [Column("CCD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CcdIpaddress { get; set; }

    [Column("CCD_CompId")]
    public int? CcdCompId { get; set; }
}
