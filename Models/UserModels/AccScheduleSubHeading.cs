using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_ScheduleSubHeading")]
public partial class AccScheduleSubHeading
{
    [Column("ASSH_ID")]
    public int AsshId { get; set; }

    [Column("ASSH_Name")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AsshName { get; set; }

    [Column("ASSH_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AsshDelflg { get; set; }

    [Column("ASSH_CRON", TypeName = "datetime")]
    public DateTime? AsshCron { get; set; }

    [Column("ASSH_CRBY")]
    public int? AsshCrby { get; set; }

    [Column("ASSH_APPROVEDBY")]
    public int? AsshApprovedby { get; set; }

    [Column("ASSH_APPROVEDON", TypeName = "datetime")]
    public DateTime? AsshApprovedon { get; set; }

    [Column("ASSH_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AsshStatus { get; set; }

    [Column("ASSH_UPDATEDBY")]
    public int? AsshUpdatedby { get; set; }

    [Column("ASSH_UPDATEDON", TypeName = "datetime")]
    public DateTime? AsshUpdatedon { get; set; }

    [Column("ASSH_DELETEDBY")]
    public int? AsshDeletedby { get; set; }

    [Column("ASSH_DELETEDON", TypeName = "datetime")]
    public DateTime? AsshDeletedon { get; set; }

    [Column("ASSH_RECALLBY")]
    public int? AsshRecallby { get; set; }

    [Column("ASSH_RECALLON", TypeName = "datetime")]
    public DateTime? AsshRecallon { get; set; }

    [Column("ASSH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AsshIpaddress { get; set; }

    [Column("ASSH_CompId")]
    public int? AsshCompId { get; set; }

    [Column("ASSH_YEARId")]
    public int? AsshYearid { get; set; }

    [Column("ASSH_HeadingID")]
    public int? AsshHeadingId { get; set; }

    [Column("ASSH_Code")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AsshCode { get; set; }

    [Column("ASSH_Notes")]
    public int? AsshNotes { get; set; }

    [Column("Assh_scheduletype")]
    public int? AsshScheduletype { get; set; }

    [Column("Assh_Orgtype")]
    public int? AsshOrgtype { get; set; }
}
