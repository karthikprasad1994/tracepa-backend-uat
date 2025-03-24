using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Excel_Upload_Structure")]
public partial class ExcelUploadStructure
{
    [Column("EUS_Id")]
    public int EusId { get; set; }

    [Column("EUS_Name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? EusName { get; set; }

    [Column("EUS_Fields")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? EusFields { get; set; }

    [Column("EUS_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EusDelflag { get; set; }

    [Column("EUS_Values")]
    [StringLength(1500)]
    [Unicode(false)]
    public string? EusValues { get; set; }

    [Column("EUS_Value")]
    public int? EusValue { get; set; }

    [Column("EUS_CompID")]
    public int? EusCompId { get; set; }
}
