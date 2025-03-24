using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ScheduleNote_Fourth")]
public partial class ScheduleNoteFourth
{
    [Column("SNFT_ID")]
    public int SnftId { get; set; }

    [Column("SNFT_CustId")]
    public int? SnftCustId { get; set; }

    [Column("SNFT_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SnftDescription { get; set; }

    [Column("SNFT_Category")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SnftCategory { get; set; }

    [Column("SNFT_NumShares", TypeName = "money")]
    public decimal? SnftNumShares { get; set; }

    [Column("SNFT_TotalShares", TypeName = "money")]
    public decimal? SnftTotalShares { get; set; }

    [Column("SNFT_ChangedShares", TypeName = "money")]
    public decimal? SnftChangedShares { get; set; }

    [Column("SNFT_YEARId")]
    public int? SnftYearid { get; set; }

    [Column("SNFT_CompId")]
    public int? SnftCompId { get; set; }

    [Column("SNFT_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SnftStatus { get; set; }

    [Column("SNFT_DELFLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SnftDelflag { get; set; }

    [Column("SNFT_CRON", TypeName = "datetime")]
    public DateTime? SnftCron { get; set; }

    [Column("SNFT_CRBY")]
    public int? SnftCrby { get; set; }

    [Column("SNFT_UPDATEDBY")]
    public int? SnftUpdatedby { get; set; }

    [Column("SNFT_UPDATEDON", TypeName = "datetime")]
    public DateTime? SnftUpdatedon { get; set; }

    [Column("SNFT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SnftIpaddress { get; set; }
}
