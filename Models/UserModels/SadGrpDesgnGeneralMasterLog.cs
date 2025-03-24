using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_GrpDesgn_general_master_Log")]
public partial class SadGrpDesgnGeneralMasterLog
{
    [Column("Log_PKID")]
    public long LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(50)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("Mas_Id")]
    public short? MasId { get; set; }

    [Column("Mas_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MasCode { get; set; }

    [Column("nMas_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? NMasCode { get; set; }

    [Column("Mas_Description")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MasDescription { get; set; }

    [Column("nMas_Description")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NMasDescription { get; set; }

    [Column("Mas_Notes")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MasNotes { get; set; }

    [Column("nMas_Notes")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NMasNotes { get; set; }

    [Column("Mas_Compid")]
    [StringLength(500)]
    [Unicode(false)]
    public string? MasCompid { get; set; }

    [Column("Mas_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MasIpaddress { get; set; }
}
