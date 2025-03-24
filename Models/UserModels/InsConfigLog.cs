using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("INS_Config_Log")]
public partial class InsConfigLog
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

    [Column("conf_ConString")]
    [StringLength(500)]
    [Unicode(false)]
    public string? ConfConString { get; set; }

    [Column("nconf_ConString")]
    [StringLength(500)]
    [Unicode(false)]
    public string? NconfConString { get; set; }

    [Column("conf_RDBMS")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ConfRdbms { get; set; }

    [Column("nconf_RDBMS")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NconfRdbms { get; set; }

    [Column("conf_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ConfIpaddress { get; set; }

    [Column("nconf_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NconfIpaddress { get; set; }

    [Column("conf_Port")]
    public int? ConfPort { get; set; }

    [Column("nconf_Port")]
    public int? NconfPort { get; set; }

    [Column("conf_From")]
    [StringLength(200)]
    [Unicode(false)]
    public string? ConfFrom { get; set; }

    [Column("nconf_From")]
    [StringLength(200)]
    [Unicode(false)]
    public string? NconfFrom { get; set; }

    [Column("conf_hh")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ConfHh { get; set; }

    [Column("nconf_hh")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NconfHh { get; set; }

    [Column("conf_mm")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ConfMm { get; set; }

    [Column("nconf_mm")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NconfMm { get; set; }

    [Column("conf_AmPm")]
    [StringLength(2)]
    [Unicode(false)]
    public string? ConfAmPm { get; set; }

    [Column("nconf_AmPm")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NconfAmPm { get; set; }

    [Column("conf_SenderID")]
    [StringLength(15)]
    [Unicode(false)]
    public string? ConfSenderId { get; set; }

    [Column("nconf_SenderID")]
    [StringLength(15)]
    [Unicode(false)]
    public string? NconfSenderId { get; set; }

    [Column("Conf_RunDate", TypeName = "datetime")]
    public DateTime? ConfRunDate { get; set; }

    [Column("Conf_INS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ConfInsIpaddress { get; set; }

    [Column("conf_CompID")]
    [StringLength(500)]
    [Unicode(false)]
    public string? ConfCompId { get; set; }
}
