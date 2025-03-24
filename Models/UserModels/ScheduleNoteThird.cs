using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ScheduleNote_Third")]
public partial class ScheduleNoteThird
{
    [Column("SNT_ID")]
    public int SntId { get; set; }

    [Column("SNT_CustId")]
    public int? SntCustId { get; set; }

    [Column("SNT_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SntDescription { get; set; }

    [Column("SNT_Category")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SntCategory { get; set; }

    [Column("SNT_CYear_Shares", TypeName = "money")]
    public decimal? SntCyearShares { get; set; }

    [Column("SNT_CYear_Amount", TypeName = "money")]
    public decimal? SntCyearAmount { get; set; }

    [Column("SNT_PYear_Shares", TypeName = "money")]
    public decimal? SntPyearShares { get; set; }

    [Column("SNT_PYear_Amount", TypeName = "money")]
    public decimal? SntPyearAmount { get; set; }

    [Column("SNT_YEARId")]
    public int? SntYearid { get; set; }

    [Column("SNT_CompId")]
    public int? SntCompId { get; set; }

    [Column("SNT_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SntStatus { get; set; }

    [Column("SNT_DELFLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SntDelflag { get; set; }

    [Column("SNT_CRON", TypeName = "datetime")]
    public DateTime? SntCron { get; set; }

    [Column("SNT_CRBY")]
    public int? SntCrby { get; set; }

    [Column("SNT_UPDATEDBY")]
    public int? SntUpdatedby { get; set; }

    [Column("SNT_UPDATEDON", TypeName = "datetime")]
    public DateTime? SntUpdatedon { get; set; }

    [Column("SNT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SntIpaddress { get; set; }
}
