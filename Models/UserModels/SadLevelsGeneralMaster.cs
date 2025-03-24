using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("sad_Levels_General_Master")]
public partial class SadLevelsGeneralMaster
{
    [Column("mas_Id")]
    public short MasId { get; set; }

    [Column("mas_Code")]
    [StringLength(10)]
    [Unicode(false)]
    public string? MasCode { get; set; }

    [Column("mas_Description")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MasDescription { get; set; }

    [Column("mas_Notes")]
    [StringLength(255)]
    [Unicode(false)]
    public string? MasNotes { get; set; }

    [Column("mas_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? MasDelflag { get; set; }

    [Column("mas_SortOrder")]
    public int? MasSortOrder { get; set; }

    [Column("mas_Type")]
    [StringLength(1)]
    [Unicode(false)]
    public string? MasType { get; set; }

    [Column("Mas_Classify")]
    public short? MasClassify { get; set; }

    [Column("Mas_CompID")]
    public int? MasCompId { get; set; }
}
