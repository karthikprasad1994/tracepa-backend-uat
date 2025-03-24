using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ScheduleNote_First")]
public partial class ScheduleNoteFirst
{
    [Column("SNF_ID")]
    public int SnfId { get; set; }

    [Column("SNF_CustId")]
    public int? SnfCustId { get; set; }

    [Column("SNF_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SnfDescription { get; set; }

    [Column("SNF_Category")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SnfCategory { get; set; }

    [Column("SNF_CYear_Amount", TypeName = "money")]
    public decimal? SnfCyearAmount { get; set; }

    [Column("SNF_PYear_Amount", TypeName = "money")]
    public decimal? SnfPyearAmount { get; set; }

    [Column("SNF_YEARId")]
    public int? SnfYearid { get; set; }

    [Column("SNF_CompId")]
    public int? SnfCompId { get; set; }

    [Column("SNF_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SnfStatus { get; set; }

    [Column("SNF_DELFLAG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SnfDelflag { get; set; }

    [Column("SNF_CRON", TypeName = "datetime")]
    public DateTime? SnfCron { get; set; }

    [Column("SNF_CRBY")]
    public int? SnfCrby { get; set; }

    [Column("SNF_UPDATEDBY")]
    public int? SnfUpdatedby { get; set; }

    [Column("SNF_UPDATEDON", TypeName = "datetime")]
    public DateTime? SnfUpdatedon { get; set; }

    [Column("SNF_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SnfIpaddress { get; set; }
}
