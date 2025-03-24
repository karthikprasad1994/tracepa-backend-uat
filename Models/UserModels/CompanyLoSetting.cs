using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("company_lo_settings")]
public partial class CompanyLoSetting
{
    [Column("CLS_ID")]
    public int ClsId { get; set; }

    [Column("CLS_BIGDATA", TypeName = "image")]
    public byte[]? ClsBigdata { get; set; }

    [Column("CLS_SIZE")]
    public double? ClsSize { get; set; }

    [Column("CLS_FileName")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? ClsFileName { get; set; }

    [Column("CLS_Extn")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? ClsExtn { get; set; }

    [Column("CLS_CompID")]
    public int? ClsCompId { get; set; }

    [Column("CLS_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? ClsStatus { get; set; }
}
