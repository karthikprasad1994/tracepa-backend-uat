using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("User_activity_logs")]
public partial class UserActivityLog
{
    [Column("ut_pkid")]
    public int? UtPkid { get; set; }

    [Column("ut_usrid")]
    public int? UtUsrid { get; set; }

    [Column("ut_module")]
    [Unicode(false)]
    public string? UtModule { get; set; }

    [Column("ut_submodule")]
    [Unicode(false)]
    public string? UtSubmodule { get; set; }

    [Column("ut_login_datetime", TypeName = "datetime")]
    public DateTime? UtLoginDatetime { get; set; }

    [Column("ut_asgnmnt_created")]
    public int? UtAsgnmntCreated { get; set; }

    [Column("ut_asgnmnt_inprogress")]
    public int? UtAsgnmntInprogress { get; set; }

    [Column("ut_asgnmnt_completed")]
    public int? UtAsgnmntCompleted { get; set; }

    [Column("ut_compid")]
    public int? UtCompid { get; set; }

    [Column("ut_logindate")]
    public DateOnly? UtLogindate { get; set; }
}
