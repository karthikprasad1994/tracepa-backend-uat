using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DOCTYPE_LINK_log")]
public partial class EdtDoctypeLinkLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("EDD_Pk")]
    public int? EddPk { get; set; }

    [Column("EDD_DOCTYPEID")]
    public int? EddDoctypeid { get; set; }

    [Column("nEDD_DOCTYPEID")]
    public int? NEddDoctypeid { get; set; }

    [Column("EDD_DPTRID")]
    public int? EddDptrid { get; set; }

    [Column("nEDD_DPTRID")]
    public int? NEddDptrid { get; set; }

    [Column("EDD_ISREQUIRED")]
    [StringLength(5)]
    [Unicode(false)]
    public string? EddIsrequired { get; set; }

    [Column("nEDD_ISREQUIRED")]
    [StringLength(5)]
    [Unicode(false)]
    public string? NEddIsrequired { get; set; }

    [Column("EDD_Size")]
    public int? EddSize { get; set; }

    [Column("nEDD_Size")]
    public int? NEddSize { get; set; }

    [Column("EDD_VALUES")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? EddValues { get; set; }

    [Column("nEDD_VALUES")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NEddValues { get; set; }

    [Column("EDD_VALIDATE")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EddValidate { get; set; }

    [Column("nEDD_VALIDATE")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NEddValidate { get; set; }

    [Column("EDD_CompId")]
    public int? EddCompId { get; set; }

    [Column("EDD_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EddIpaddress { get; set; }
}
