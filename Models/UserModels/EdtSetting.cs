using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_Settings")]
public partial class EdtSetting
{
    [Column("SET_ID")]
    public int? SetId { get; set; }

    [Column("SET_CODE")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SetCode { get; set; }

    [Column("SET_Value")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SetValue { get; set; }

    [Column("SAD_UpdatedBy")]
    public int? SadUpdatedBy { get; set; }

    [Column("SAD_UpdatedOn", TypeName = "datetime")]
    public DateTime? SadUpdatedOn { get; set; }

    [Column("SET_Operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? SetOperation { get; set; }

    [Column("SET_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SetIpaddress { get; set; }

    [Column("SET_CompID")]
    public int? SetCompId { get; set; }
}
