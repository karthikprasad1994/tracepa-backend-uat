using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_JE_Master_History")]
public partial class AccJeMasterHistory
{
    [Column("AJEH_PKID")]
    public int? AjehPkid { get; set; }

    [Column("AJEH_AccJEID")]
    public int? AjehAccJeid { get; set; }

    [Column("AJEH_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AjehComments { get; set; }

    [Column("AJEH_UserID")]
    public int? AjehUserId { get; set; }

    [Column("AJEH_Date", TypeName = "datetime")]
    public DateTime? AjehDate { get; set; }

    [Column("AJEH_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AjehStatus { get; set; }

    [Column("AJEH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AjehIpaddress { get; set; }

    [Column("AJEH_CompID")]
    public int? AjehCompId { get; set; }
}
