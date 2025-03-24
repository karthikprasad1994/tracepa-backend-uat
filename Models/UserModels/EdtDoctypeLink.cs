using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DOCTYPE_LINK")]
public partial class EdtDoctypeLink
{
    [Column("EDD_DOCTYPEID")]
    public int? EddDoctypeid { get; set; }

    [Column("EDD_DPTRID")]
    public int? EddDptrid { get; set; }

    [Column("EDD_ISREQUIRED")]
    [StringLength(5)]
    [Unicode(false)]
    public string? EddIsrequired { get; set; }

    [Column("EDD_Size")]
    public int? EddSize { get; set; }

    [Column("EDD_Pk")]
    public int? EddPk { get; set; }

    [Column("EDD_VALUES")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? EddValues { get; set; }

    [Column("EDD_VALIDATE")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EddValidate { get; set; }

    [Column("EDD_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? EddStatus { get; set; }

    [Column("EDD_CRBY")]
    public int? EddCrby { get; set; }

    [Column("EDD_CRON", TypeName = "datetime")]
    public DateTime? EddCron { get; set; }

    [Column("EDD_UPDATEDBY")]
    public int? EddUpdatedby { get; set; }

    [Column("EDD_UPDATEDON", TypeName = "datetime")]
    public DateTime? EddUpdatedon { get; set; }

    [Column("EDD_RECALLBY")]
    public int? EddRecallby { get; set; }

    [Column("EDD_RECALLON", TypeName = "datetime")]
    public DateTime? EddRecallon { get; set; }

    [Column("EDD_DELETEDBY")]
    public int? EddDeletedby { get; set; }

    [Column("EDD_DELETEDON", TypeName = "datetime")]
    public DateTime? EddDeletedon { get; set; }

    [Column("EDD_APPROVEDBY")]
    public int? EddApprovedby { get; set; }

    [Column("EDD_APPROVEDON", TypeName = "datetime")]
    public DateTime? EddApprovedon { get; set; }

    [Column("EDD_CompId")]
    public int? EddCompId { get; set; }

    [Column("EDD_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EddIpaddress { get; set; }
}
