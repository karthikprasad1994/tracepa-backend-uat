using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_Currency_Master")]
public partial class SadCurrencyMaster
{
    [Column("CUR_ID")]
    public int CurId { get; set; }

    [Column("CUR_CODE")]
    [StringLength(15)]
    [Unicode(false)]
    public string? CurCode { get; set; }

    [Column("CUR_CountryName")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CurCountryName { get; set; }

    [Column("CUR_Status")]
    [StringLength(5)]
    [Unicode(false)]
    public string? CurStatus { get; set; }

    [Column("CUR_CompID")]
    public int? CurCompId { get; set; }
}
