using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Sad_Company_Master")]
public partial class SadCompanyMaster
{
    [Column("CM_ID")]
    public int? CmId { get; set; }

    [Column("CM_AccessCode")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CmAccessCode { get; set; }

    [Column("CM_CompanyName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? CmCompanyName { get; set; }

    [Column("CM_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmDelFlag { get; set; }

    [Column("CM_CreatedBy")]
    public int? CmCreatedBy { get; set; }

    [Column("CM_CreatedOn", TypeName = "datetime")]
    public DateTime? CmCreatedOn { get; set; }
}
