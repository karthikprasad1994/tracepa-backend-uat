using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Grace_ExcelUpload")]
public partial class GraceExcelUpload
{
    [Column("GEU_Pk_Id")]
    public int GeuPkId { get; set; }

    [Column("GEU_MasterName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? GeuMasterName { get; set; }

    [Column("GEU_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? GeuStatus { get; set; }

    [Column("GEU_Compid")]
    public int? GeuCompid { get; set; }
}
