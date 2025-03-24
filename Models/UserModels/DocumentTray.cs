using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("DocumentTray")]
public partial class DocumentTray
{
    public int? PkId { get; set; }

    [Column("DocID")]
    public int? DocId { get; set; }

    public int? DocParent { get; set; }

    [Unicode(false)]
    public string? Title { get; set; }

    [Unicode(false)]
    public string? DocFormat { get; set; }

    public string? DocBytes { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Delflag { get; set; }

    public int? CrBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CrOn { get; set; }

    public int? Compid { get; set; }
}
