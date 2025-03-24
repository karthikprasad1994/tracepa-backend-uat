using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Edt_OutlookAttach")]
public partial class EdtOutlookAttach
{
    [Column("Edt_id")]
    public int? EdtId { get; set; }

    [Column("Edt_BaseName")]
    public int? EdtBaseName { get; set; }

    [Column("Edt_OutlookID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? EdtOutlookId { get; set; }

    [Column("Edt_createdBy")]
    public int? EdtCreatedBy { get; set; }

    [Column("Edt_createdOn", TypeName = "datetime")]
    public DateTime? EdtCreatedOn { get; set; }

    [Column("Edt_ReceivedDate", TypeName = "datetime")]
    public DateTime? EdtReceivedDate { get; set; }

    [Column("edt_CompID")]
    public int? EdtCompId { get; set; }
}
