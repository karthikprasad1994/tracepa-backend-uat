using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("INS_Config")]
public partial class InsConfig
{
    [Column("conf_ConString")]
    [StringLength(500)]
    [Unicode(false)]
    public string? ConfConString { get; set; }

    [Column("conf_RDBMS")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ConfRdbms { get; set; }

    [Column("conf_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ConfIpaddress { get; set; }

    [Column("conf_Port")]
    public int? ConfPort { get; set; }

    [Column("conf_From")]
    [StringLength(200)]
    [Unicode(false)]
    public string? ConfFrom { get; set; }

    [Column("conf_hh")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ConfHh { get; set; }

    [Column("conf_mm")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ConfMm { get; set; }

    [Column("conf_AmPm")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ConfAmPm { get; set; }

    [Column("conf_SenderID")]
    [StringLength(15)]
    [Unicode(false)]
    public string? ConfSenderId { get; set; }

    [Column("Conf_INS_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ConfInsIpaddress { get; set; }

    [Column("Conf_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ConfStatus { get; set; }

    [Column("Conf_UpdatedBy")]
    public int? ConfUpdatedBy { get; set; }

    [Column("Conf_UpdatedOn", TypeName = "datetime")]
    public DateTime? ConfUpdatedOn { get; set; }

    [Column("Conf_CompID")]
    public int? ConfCompId { get; set; }
}
