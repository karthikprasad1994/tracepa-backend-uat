using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("IntacctDDlItemText")]
public partial class IntacctDdlItemText
{
    [Column("Intac_Id")]
    public int? IntacId { get; set; }

    [Column("Intac_ObjName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? IntacObjName { get; set; }

    [Column("Intac_Objval")]
    [StringLength(100)]
    [Unicode(false)]
    public string? IntacObjval { get; set; }
}
