using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_page_details")]
public partial class EdtPageDetail
{
    [Column("EPD_BASEID", TypeName = "numeric(15, 0)")]
    public decimal? EpdBaseid { get; set; }

    [Column("EPD_DOCTYPE", TypeName = "numeric(15, 0)")]
    public decimal? EpdDoctype { get; set; }

    [Column("EPD_DESCID", TypeName = "numeric(10, 0)")]
    public decimal? EpdDescid { get; set; }

    [Column("EPD_KEYWORD")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? EpdKeyword { get; set; }

    [Column("EPD_VALUE")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? EpdValue { get; set; }

    [Column("EPD_CompID")]
    public int? EpdCompId { get; set; }
}
