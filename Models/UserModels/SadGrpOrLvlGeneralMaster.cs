using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_GrpOrLvl_General_Master")]
public partial class SadGrpOrLvlGeneralMaster
{
    [Column("Mas_ID")]
    public int? MasId { get; set; }

    [Column("Mas_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MasCode { get; set; }

    [Column("Mas_Description")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MasDescription { get; set; }

    [Column("Mas_Notes")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MasNotes { get; set; }

    [Column("Mas_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MasDelFlag { get; set; }

    [Column("Mas_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MasStatus { get; set; }

    [Column("Mas_Classify")]
    public int? MasClassify { get; set; }

    [Column("Mas_Createdby")]
    public int? MasCreatedby { get; set; }

    [Column("Mas_Createdon", TypeName = "datetime")]
    public DateTime? MasCreatedon { get; set; }

    [Column("Mas_Updatedby")]
    public int? MasUpdatedby { get; set; }

    [Column("Mas_UpdatedOn", TypeName = "datetime")]
    public DateTime? MasUpdatedOn { get; set; }

    [Column("Mas_ApprovedBy")]
    public int? MasApprovedBy { get; set; }

    [Column("Mas_ApprovedOn", TypeName = "datetime")]
    public DateTime? MasApprovedOn { get; set; }

    [Column("Mas_DeletedBy")]
    public int? MasDeletedBy { get; set; }

    [Column("Mas_DeletedOn", TypeName = "datetime")]
    public DateTime? MasDeletedOn { get; set; }

    [Column("Mas_RecalledBy")]
    public int? MasRecalledBy { get; set; }

    [Column("Mas_RecalledOn", TypeName = "datetime")]
    public DateTime? MasRecalledOn { get; set; }

    [Column("Mas_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MasIpaddress { get; set; }

    [Column("Mas_CompID")]
    public int? MasCompId { get; set; }
}
