using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_CostSheetDetails")]
public partial class AuditCostSheetDetail
{
    [Column("CSD_ID")]
    public int? CsdId { get; set; }

    [Column("CSD_YearID")]
    public int? CsdYearId { get; set; }

    [Column("CSD_AuditCodeID")]
    public int? CsdAuditCodeId { get; set; }

    [Column("CSD_FunID")]
    public int? CsdFunId { get; set; }

    [Column("CSD_CustID")]
    public int? CsdCustId { get; set; }

    [Column("CSD_DescID")]
    public int? CsdDescId { get; set; }

    [Column("CSD_UserID")]
    public int? CsdUserId { get; set; }

    [Column("CSD_Date")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CsdDate { get; set; }

    [Column("CSD_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CsdComments { get; set; }

    [Column("CSD_KmsTravelled")]
    public int? CsdKmsTravelled { get; set; }

    [Column("CSD_Costs")]
    public int? CsdCosts { get; set; }

    [Column("CSD_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CsdStatus { get; set; }

    [Column("CSD_CrBy")]
    public int? CsdCrBy { get; set; }

    [Column("CSD_CrOn", TypeName = "datetime")]
    public DateTime? CsdCrOn { get; set; }

    [Column("CSD_UpdateBy")]
    public int? CsdUpdateBy { get; set; }

    [Column("CSD_UpdatedOn", TypeName = "datetime")]
    public DateTime? CsdUpdatedOn { get; set; }

    [Column("CSD_ApprovedBy")]
    public int? CsdApprovedBy { get; set; }

    [Column("CSD_ApprovedOn", TypeName = "datetime")]
    public DateTime? CsdApprovedOn { get; set; }

    [Column("CSD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CsdIpaddress { get; set; }

    [Column("CSD_CompID")]
    public int? CsdCompId { get; set; }
}
