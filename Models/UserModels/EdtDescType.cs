using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DESC_TYPE")]
public partial class EdtDescType
{
    [Column("DT_ID")]
    public int? DtId { get; set; }

    [Column("DT_Name")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DtName { get; set; }

    [Column("DT_DataType")]
    [StringLength(20)]
    [Unicode(false)]
    public string? DtDataType { get; set; }

    [Column("DT_Value")]
    [StringLength(20)]
    [Unicode(false)]
    public string? DtValue { get; set; }

    [Column("DT_Size")]
    public int? DtSize { get; set; }

    [Column("DT_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? DtStatus { get; set; }
}
