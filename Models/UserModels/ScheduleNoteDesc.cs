using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ScheduleNote_Desc")]
public partial class ScheduleNoteDesc
{
    [Column("SND_ID")]
    public int SndId { get; set; }

    [Column("SND_CustId")]
    public int? SndCustId { get; set; }

    [Column("SND_Description")]
    [Unicode(false)]
    public string? SndDescription { get; set; }

    [Column("SND_Category")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SndCategory { get; set; }

    [Column("SND_YEARId")]
    public int? SndYearid { get; set; }

    [Column("SND_CompId")]
    public int? SndCompId { get; set; }

    [Column("SND_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SndStatus { get; set; }

    [Column("SND_DELFLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SndDelflag { get; set; }

    [Column("SND_CRON", TypeName = "datetime")]
    public DateTime? SndCron { get; set; }

    [Column("SND_CRBY")]
    public int? SndCrby { get; set; }

    [Column("SND_UPDATEDBY")]
    public int? SndUpdatedby { get; set; }

    [Column("SND_UPDATEDON", TypeName = "datetime")]
    public DateTime? SndUpdatedon { get; set; }

    [Column("SND_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SndIpaddress { get; set; }
}
