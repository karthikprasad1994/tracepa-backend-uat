using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DESCRIPTIOS")]
public partial class EdtDescriptio
{
    [Column("DES_ID")]
    public int? DesId { get; set; }

    [Column("DESC_NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DescName { get; set; }

    [Column("DESC_NOTE")]
    [StringLength(200)]
    [Unicode(false)]
    public string? DescNote { get; set; }

    [Column("DESC_DATATYPE")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DescDatatype { get; set; }

    [Column("DESC_SIZE")]
    [StringLength(3)]
    [Unicode(false)]
    public string? DescSize { get; set; }

    [Column("DESC_CRBY")]
    public int? DescCrby { get; set; }

    [Column("DESC_CRON", TypeName = "datetime")]
    public DateTime? DescCron { get; set; }

    [Column("DESC_STATUS")]
    [StringLength(1)]
    [Unicode(false)]
    public string? DescStatus { get; set; }

    [Column("EDD_Pk")]
    public int? EddPk { get; set; }

    [Column("EDD_DefaultValues")]
    [StringLength(250)]
    [Unicode(false)]
    public string? EddDefaultValues { get; set; }

    [Column("DESC_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? DescDelFlag { get; set; }

    [Column("DESC_UPDATEDBY")]
    public int? DescUpdatedby { get; set; }

    [Column("DESC_UPDATEDON", TypeName = "datetime")]
    public DateTime? DescUpdatedon { get; set; }

    [Column("DESC_RECALLBY")]
    public int? DescRecallby { get; set; }

    [Column("DESC_RECALLON", TypeName = "datetime")]
    public DateTime? DescRecallon { get; set; }

    [Column("DESC_DELETEDBY")]
    public int? DescDeletedby { get; set; }

    [Column("DESC_DELETEDON", TypeName = "datetime")]
    public DateTime? DescDeletedon { get; set; }

    [Column("DESC_APPROVEDBY")]
    public int? DescApprovedby { get; set; }

    [Column("DESC_APPROVEDON", TypeName = "datetime")]
    public DateTime? DescApprovedon { get; set; }

    [Column("DESC_CompId")]
    public int? DescCompId { get; set; }

    [Column("DESC_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? DescIpaddress { get; set; }
}
