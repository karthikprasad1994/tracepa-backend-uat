using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Gen_ProductDetails")]
public partial class GenProductDetail
{
    [Column("GP_Pkid")]
    public int? GpPkid { get; set; }

    [Column("GP_Accesscode")]
    [Unicode(false)]
    public string? GpAccesscode { get; set; }

    [Column("GP_Productkey")]
    [Unicode(false)]
    public string? GpProductkey { get; set; }

    [Column("GP_Licensetype")]
    public int? GpLicensetype { get; set; }

    [Column("GP_Licenseuser")]
    public int? GpLicenseuser { get; set; }

    [Column("GP_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? GpStatus { get; set; }
}
