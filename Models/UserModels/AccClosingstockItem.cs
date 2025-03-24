using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_Closingstock_Items")]
public partial class AccClosingstockItem
{
    [Column("ACSI_id")]
    public int AcsiId { get; set; }

    [Column("ACSI_ItemdescCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string AcsiItemdescCode { get; set; } = null!;

    [Column("ACSI_Itemdesc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AcsiItemdesc { get; set; }

    [Column("ACSI_classification")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AcsiClassification { get; set; }

    [Column("ACSI_Type")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AcsiType { get; set; }

    [Column("ACSI_Custid")]
    public int? AcsiCustid { get; set; }

    [Column("ACSI_Qty")]
    public int? AcsiQty { get; set; }

    [Column("ACSI_Rate", TypeName = "money")]
    public decimal? AcsiRate { get; set; }

    [Column("ACSI_Total", TypeName = "money")]
    public decimal? AcsiTotal { get; set; }

    [Column("ACSI_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AcsiDelflg { get; set; }

    [Column("ACSI_CRON", TypeName = "datetime")]
    public DateTime? AcsiCron { get; set; }

    [Column("ACSI_CRBY")]
    public int? AcsiCrby { get; set; }

    [Column("ACSI_APPROVEDBY")]
    public int? AcsiApprovedby { get; set; }

    [Column("ACSI_APPROVEDON", TypeName = "datetime")]
    public DateTime? AcsiApprovedon { get; set; }

    [Column("ACSI_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AcsiStatus { get; set; }

    [Column("ACSI_UPDATEDBY")]
    public int? AcsiUpdatedby { get; set; }

    [Column("ACSI_UPDATEDON", TypeName = "datetime")]
    public DateTime? AcsiUpdatedon { get; set; }

    [Column("ACSI_DELETEDBY")]
    public int? AcsiDeletedby { get; set; }

    [Column("ACSI_DELETEDON", TypeName = "datetime")]
    public DateTime? AcsiDeletedon { get; set; }

    [Column("ACSI_RECALLBY")]
    public int? AcsiRecallby { get; set; }

    [Column("ACSI_RECALLON", TypeName = "datetime")]
    public DateTime? AcsiRecallon { get; set; }

    [Column("ACSI_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AcsiIpaddress { get; set; }

    [Column("ACSI_CompId")]
    public int? AcsiCompId { get; set; }

    [Column("ACSI_YEARId")]
    public int? AcsiYearid { get; set; }
}
