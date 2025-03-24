using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_Log_Form_Operations")]
public partial class AuditLogFormOperation
{
    [Column("ALFO_PKID")]
    public int? AlfoPkid { get; set; }

    [Column("ALFO_UserID")]
    public int? AlfoUserId { get; set; }

    [Column("ALFO_Date", TypeName = "datetime")]
    public DateTime? AlfoDate { get; set; }

    [Column("ALFO_Module")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlfoModule { get; set; }

    [Column("ALFO_Form")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AlfoForm { get; set; }

    [Column("ALFO_Event")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AlfoEvent { get; set; }

    [Column("ALFO_MasterID")]
    public int? AlfoMasterId { get; set; }

    [Column("ALFO_MasterName")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AlfoMasterName { get; set; }

    [Column("ALFO_SubMasterID")]
    public int? AlfoSubMasterId { get; set; }

    [Column("ALFO_SubMasterName")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AlfoSubMasterName { get; set; }

    [Column("ALFO_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AlfoIpaddress { get; set; }

    [Column("ALFO_CompID")]
    public int? AlfoCompId { get; set; }
}
