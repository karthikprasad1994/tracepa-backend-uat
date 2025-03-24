using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Edt_Attachments")]
public partial class EdtAttachment
{
    [Column("ATCH_ID")]
    public int? AtchId { get; set; }

    [Column("ATCH_DOCID")]
    public int? AtchDocid { get; set; }

    [Column("ATCH_FNAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? AtchFname { get; set; }

    [Column("ATCH_EXT")]
    [StringLength(20)]
    [Unicode(false)]
    public string? AtchExt { get; set; }

    [Column("ATCH_Desc")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? AtchDesc { get; set; }

    [Column("ATCH_OLE")]
    [MaxLength(1)]
    public byte[]? AtchOle { get; set; }

    [Column("ATCH_SIZE")]
    public long? AtchSize { get; set; }

    [Column("ATCH_FLAG")]
    public int? AtchFlag { get; set; }

    [Column("ATCH_AUDScheduleID")]
    public int? AtchAudscheduleId { get; set; }

    [Column("ATCH_AuditID")]
    public int? AtchAuditId { get; set; }

    [Column("ATCH_SubProcessID")]
    public int? AtchSubProcessId { get; set; }

    [Column("ATCH_CREATEDBY")]
    public int? AtchCreatedby { get; set; }

    [Column("ATCH_CREATEDON", TypeName = "datetime")]
    public DateTime? AtchCreatedon { get; set; }

    [Column("ATCH_MODIFIEDBY")]
    public int? AtchModifiedby { get; set; }

    [Column("ATCH_VERSION")]
    public int? AtchVersion { get; set; }

    [Column("ATCH_FROM")]
    [StringLength(20)]
    [Unicode(false)]
    public string? AtchFrom { get; set; }

    [Column("ATCH_Basename")]
    public int? AtchBasename { get; set; }

    [Column("Atch_Vstatus")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AtchVstatus { get; set; }

    [Column("ATCH_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AtchStatus { get; set; }

    [Column("ATCH_CompID")]
    public int? AtchCompId { get; set; }
}
