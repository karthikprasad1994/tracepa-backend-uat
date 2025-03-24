using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ScheduleNote_Second")]
public partial class ScheduleNoteSecond
{
    [Column("SNS_ID")]
    public int SnsId { get; set; }

    [Column("SNS_CustId")]
    public int? SnsCustId { get; set; }

    [Column("SNS_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SnsDescription { get; set; }

    [Column("SNS_Category")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SnsCategory { get; set; }

    [Column("SNS_CYear_BegShares", TypeName = "money")]
    public decimal? SnsCyearBegShares { get; set; }

    [Column("SNS_CYear_BegAmount", TypeName = "money")]
    public decimal? SnsCyearBegAmount { get; set; }

    [Column("SNS_PYear_BegShares", TypeName = "money")]
    public decimal? SnsPyearBegShares { get; set; }

    [Column("SNS_PYear_BegAmount", TypeName = "money")]
    public decimal? SnsPyearBegAmount { get; set; }

    [Column("SNS_CYear_AddShares", TypeName = "money")]
    public decimal? SnsCyearAddShares { get; set; }

    [Column("SNS_CYear_AddAmount", TypeName = "money")]
    public decimal? SnsCyearAddAmount { get; set; }

    [Column("SNS_PYear_AddShares", TypeName = "money")]
    public decimal? SnsPyearAddShares { get; set; }

    [Column("SNS_PYear_AddAmount", TypeName = "money")]
    public decimal? SnsPyearAddAmount { get; set; }

    [Column("SNS_CYear_EndShares", TypeName = "money")]
    public decimal? SnsCyearEndShares { get; set; }

    [Column("SNS_CYear_EndAmount", TypeName = "money")]
    public decimal? SnsCyearEndAmount { get; set; }

    [Column("SNS_PYear_EndShares", TypeName = "money")]
    public decimal? SnsPyearEndShares { get; set; }

    [Column("SNS_PYear_EndAmount", TypeName = "money")]
    public decimal? SnsPyearEndAmount { get; set; }

    [Column("SNS_YEARId")]
    public int? SnsYearid { get; set; }

    [Column("SNS_CompId")]
    public int? SnsCompId { get; set; }

    [Column("SNS_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SnsStatus { get; set; }

    [Column("SNS_DELFLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SnsDelflag { get; set; }

    [Column("SNS_CRON", TypeName = "datetime")]
    public DateTime? SnsCron { get; set; }

    [Column("SNS_CRBY")]
    public int? SnsCrby { get; set; }

    [Column("SNS_UPDATEDBY")]
    public int? SnsUpdatedby { get; set; }

    [Column("SNS_UPDATEDON", TypeName = "datetime")]
    public DateTime? SnsUpdatedon { get; set; }

    [Column("SNS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SnsIpaddress { get; set; }
}
